using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Items;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public Actor Player { get; set; }
    public List<Actor> Enemies { get; private set; } = new List<Actor>();
    public List<Consumable> Items { get; private set; } = new List<Consumable>();
    public List<Ladder> Ladders { get; private set; } = new List<Ladder>();
    public List<Tombstone> TombStones { get; private set; } = new List<Tombstone>();

    private int totalEnemies;
    public Text enemiesLeftText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        LoadGame();
    }

    public static GameManager Get { get => instance; }

    public Actor GetActorAtLocation(Vector3 location)
    {
        if (Player != null && Player.transform.position == location)
        {
            return Player;
        }
        foreach (var enemy in Enemies)
        {
            if (enemy != null && enemy.transform.position == location)
            {
                return enemy;
            }
        }
        return null;
    }

    public Consumable GetItemAtLocation(Vector3 location)
    {
        foreach (var item in Items)
        {
            if (item != null && item.transform.position == location)
            {
                return item;
            }
        }
        return null;
    }

    public Ladder GetLadderAtLocation(Vector3 location)
    {
        foreach (var ladder in Ladders)
        {
            if (ladder != null && ladder.transform.position == location)
            {
                return ladder;
            }
        }
        return null;
    }

    public GameObject CreateActor(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);

        if (name == "Player")
        {
            Player = actor.GetComponent<Actor>();
            LoadPlayerData();
        }
        else
        {
            AddEnemy(actor.GetComponent<Actor>());
        }

        actor.name = name;
        return actor;
    }

    public GameObject CreateItem(string name, Vector2 position)
    {
        GameObject item = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        AddItem(item.GetComponent<Consumable>());
        item.name = name;
        return item;
    }

    public GameObject CreateLadder(string name, Vector2 position)
    {
        GameObject ladder = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        AddLadder(ladder.GetComponent<Ladder>());
        ladder.name = name;
        return ladder;
    }

    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
        totalEnemies++;
        UpdateEnemiesLeftUI();
    }

    public void AddItem(Consumable item)
    {
        Items.Add(item);
    }

    public void AddLadder(Ladder ladder)
    {
        Ladders.Add(ladder);
    }

    public void AddTombStone(Tombstone stone)
    {
        TombStones.Add(stone);
    }

    public void ClearFloor()
    {
        foreach (var enemy in Enemies)
        {
            Destroy(enemy.gameObject);
        }
        foreach (var item in Items)
        {
            Destroy(item.gameObject);
        }
        foreach (var ladder in Ladders)
        {
            Destroy(ladder.gameObject);
        }
        foreach (var stone in TombStones)
        {
            Destroy(stone.gameObject);
        }

        Enemies.Clear();
        Items.Clear();
        Ladders.Clear();
        TombStones.Clear();
    }

    private void Start()
    {
        Player = GetComponent<Actor>();
    }

    public void StartEnemyTurn()
    {
        foreach (var enemy in GameManager.Get.Enemies)
        {
            if (enemy != null)
            {
                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.RunAI();
                }
            }
        }
    }

    public void RemoveEnemy(Actor enemy)
    {
        Enemies.Remove(enemy);
        totalEnemies--;
        UpdateEnemiesLeftUI();
    }

    public void RemoveItem(Consumable item)
    {
        Items.Remove(item);
        Destroy(item.gameObject);
    }

    public List<Actor> GetNearbyEnemies(Vector3 location)
    {
        List<Actor> nearbyEnemies = new List<Actor>();

        foreach (Actor enemy in Enemies)
        {
            if (enemy != null && Vector3.Distance(enemy.transform.position, location) < 5)
            {
                nearbyEnemies.Add(enemy);
            }
        }

        return nearbyEnemies;
    }

    private void UpdateEnemiesLeftUI()
    {
        if (enemiesLeftText != null)
        {
            enemiesLeftText.text = $"{totalEnemies} enemies left";
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
    

    public void SaveGame()
    {
        if (Player != null)
        {
            PlayerData playerData = new PlayerData()
            {
                MaxHitPoints = Player.MaxHitPoints,
                HitPoints = Player.HitPoints,
                Defense = Player.Defense,
                Power = Player.Power,
                Level = Player.Level,
                XP = Player.XP,
                XpToNextLevel = Player.XPToNextLevel
            };

            string json = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + "/savegame.json", json);
        }
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savegame.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            PlayerDataManager.Instance.SetPlayerData(playerData);
        }
    }

    public void LoadPlayerData()
    {
        PlayerData playerData = PlayerDataManager.Instance.GetPlayerData();
        
        if (playerData != null)
        {
            Player.LoadData(playerData);
        }
    }

    public void DeleteSaveGame()
    {
        string path = Application.persistentDataPath + "/savegame.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public int MaxHitPoints;
    public int HitPoints;
    public int Defense;
    public int Power;
    public int Level;
    public int XP;
    public int XpToNextLevel;
}

public class PlayerDataManager
{
    private static PlayerDataManager instance;
    private PlayerData playerData;

    public static PlayerDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerDataManager();
            }
            return instance;
        }
    }

    public void SetPlayerData(PlayerData data)
    {
        playerData = data;
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }
}