using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor), typeof(AStar))]
public class Enemy : MonoBehaviour
{
    private Actor target;
    private bool isFighting = false;
    private AStar algorithm;
    private int confused = 0;

    private void Start()
    {
        algorithm = GetComponent<AStar>();
    }

    public void MoveAlongPath(Vector3Int targetPosition)
    {
        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(transform.position);
        Vector2 direction = algorithm.Compute((Vector2Int)gridPosition, (Vector2Int)targetPosition);
        Action.MoveOrHit(GetComponent<Actor>(), direction);
    }

    public void RunAI()
    {
        // If target is null, set target to player (from GameManager)
        if (target == null)
        {
            target = GameManager.Get.Player;
        }

        // Convert the position of the target to a gridPosition
        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(target.transform.position);

        // First check if already fighting, because the FieldOfView check costs more CPU
        if (isFighting || GetComponent<Actor>().FieldOfView.Contains(gridPosition))
        {
            // If the enemy was not fighting, it should be fighting now
            isFighting = true;

            // Calculate distance between enemy and target
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance < 1.5f)
            {
                // If distance is less than 1.5, attack the target
                Action.Hit(GetComponent<Actor>(), target.GetComponent<Actor>());
            }
            else
            {
                // Otherwise, move along the path
                MoveAlongPath(gridPosition);
            }
        }
    }
    public void Confuse()
    {
        confused = 8;
    }
}