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

    public int MaxHitPoints { get { return maxHitPoints; } }
    public int HitPoints { get { return hitPoints; } }
    public int Defense { get { return defense; } }
    public int Power { get { return power; } }

    private void Start()
    {
        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();

        if (GetComponent<Player>())
        {
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints);
        }
    }

    public void Move(Vector3 direction)
    {
        if (MapManager.Get.IsWalkable(transform.position + direction))
        {
            transform.position += direction;
        }
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

    public void DoDamage(int hp)
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

    internal void Confuse(object duration)
    {
        throw new NotImplementedException();
    }
}
