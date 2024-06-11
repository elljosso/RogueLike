using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    private AdamMilVisibility algorithm;
    public List<Vector3Int> FieldOfView = new List<Vector3Int>();
    public int FieldOfViewRange = 8;

    [Header("Powers")]
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int hitPoints;
    [SerializeField] private int defense;
    [SerializeField] private int power;
    [SerializeField] private int level = 1;
    [SerializeField] private int xp = 0;
    [SerializeField] private int xpToNextLevel = 100;

    public int MaxHitPoints { get { return maxHitPoints; } }
    public int HitPoints { get { return hitPoints; } }
    public int Defense { get { return defense; } }
    public int Power { get { return power; } }
    public int Level { get { return level; } }
    public int XP { get { return xp; } }
    public int XPToNextLevel { get { return xpToNextLevel; } }

    private void Start()
    {
        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();

        if (GetComponent<Player>())
        {
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints);
            UIManager.Instance.SetLevel(level);
            UIManager.Instance.SetXP(xp);
        }
    }

    public void Move(Vector3 direction)
    {
        if (MapManager.Get.IsWalkable(transform.position + direction))
        {
            transform.position += direction;
        }
    }
    public void LoadData(PlayerData data)
    {
        maxHitPoints = data.MaxHitPoints;
        hitPoints = data.HitPoints;
        defense = data.Defense;
        power = data.Power;
        level = data.Level;
        xp = data.XP;
        xpToNextLevel = data.XpToNextLevel;
    }
    public void UpdateFieldOfView()
    {
        var pos = MapManager.Get.FloorMap.WorldToCell(transform.position);

        FieldOfView.Clear();
        algorithm.Compute(pos, FieldOfViewRange, FieldOfView);

        if (GetComponent<Player>())
        {
            MapManager.Get.UpdateFogMap(FieldOfView);
        }
    }

    private void Die()
    {
        string message = "";
        Color color = Color.white;

        if (GetComponent<Player>())
        {
            message = "You died!";
            color = Color.red;
        }
        else if (GetComponent<Enemy>())
        {
            string actorName = gameObject.name;
            message = $"{actorName} is dead!";
            color = Color.green;

            Vector3 spawnPosition = transform.position;
            GameManager.Get.CreateActor("Dead", spawnPosition).name = "Remains of " + actorName;
            GameManager.Get.RemoveEnemy(this);
        }

        UIManager.Instance.AddMessage(message, color);
        Destroy(gameObject);
    }

    public void DoDamage(int hp, Actor attacker)
    {
        hitPoints -= hp;
        if (hitPoints < 0)
        {
            hitPoints = 0;
        }

        if (GetComponent<Player>())
        {
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints);
        }

        if (hitPoints == 0)
        {
            if (attacker != null && attacker.GetComponent<Player>())
            {
                attacker.AddXp(xp);
            }
            Die();
        }
    }

    public void Heal(int hp)
    {
        int healedAmount = Mathf.Min(maxHitPoints - hitPoints, hp);
        hitPoints += healedAmount;

        if (GetComponent<Player>())
        {
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints);
            UIManager.Instance.AddMessage($"You have been healed for {healedAmount} hit points!", Color.green);
        }
    }

    public void AddXp(int xp)
    {
        this.xp += xp;
        while (this.xp >= xpToNextLevel)
        {
            LevelUp();
        }
        if (GetComponent<Player>())
        {
            UIManager.Instance.SetXP(this.xp);
        }
    }

    private void LevelUp()
    {
        level++;
        xp -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        maxHitPoints += 10;
        defense += 1;
        power += 1;
        hitPoints = maxHitPoints;

        if (GetComponent<Player>())
        {
            UIManager.Instance.SetLevel(level);
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints);
            UIManager.Instance.AddMessage($"You have reached level {level}!", Color.yellow);
        }
    }

    internal void Confuse(object duration)
    {
        throw new NotImplementedException();
    }
}
