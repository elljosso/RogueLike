using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public Actor Player { get; set; }
    // Lijst van vijanden
    public List<Actor> Enemies { get; private set; } = new List<Actor>();


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
        // Plaats hier eventuele logica om een Actor op een bepaalde locatie te vinden
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
    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
    }
    private void Start()
    {
        Player = GetComponent<Actor>();
    }
    public void StartEnemyTurn()
    {
        foreach (var enemy in GameManager.Get.Enemies)
        {
            // Controleer of enemy niet null is voordat we GetComponent aanroepen
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
        Destroy(enemy.gameObject); // Verwijder ook het GameObject van de scene
    }

}