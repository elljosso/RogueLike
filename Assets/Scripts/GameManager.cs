using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    // Lijst van vijanden
    private List<Actor> enemies = new List<Actor>();

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
        return null;
    }

    // Functie om een vijand toe te voegen aan de lijst met vijanden
    public void AddEnemy(Actor enemy)
    {
        enemies.Add(enemy);
        // Voeg hier eventueel verdere logica toe, zoals het activeren van de vijand in de game wereld.
    }
    public GameObject CreateActor(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        actor.name = name;
        return actor;
    }
}