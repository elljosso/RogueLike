using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public Actor Player { get; set; }
    // List of enemies
    public List<Actor> Enemies { get; private set; } = new List<Actor>();
    // List of items
    public List<Consumable> Items { get; private set; } = new List<Consumable>();

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

    public GameObject CreateActor(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);

        if (name == "Player")
        {
            Player = actor.GetComponent<Actor>();
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

    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
    }

    public void AddItem(Consumable item)
    {
        Items.Add(item);
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
        Destroy(enemy.gameObject); // Remove the GameObject from the scene
    }

    public void RemoveItem(Consumable item)
    {
        Items.Remove(item);
        Destroy(item.gameObject); // Remove the GameObject from the scene
    }
}