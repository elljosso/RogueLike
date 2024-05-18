using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    // Functie om een actor in de opgegeven richting te verplaatsen
    static public void Move(Actor actor, Vector2 direction)
    {
        // Kijk of er een actor is op de doelpositie
        Actor target = GameManager.Get.GetActorAtLocation(actor.transform.position + (Vector3)direction);

        // Als er geen actor is, verplaatsen we de actor
        if (target == null)
        {
            actor.Move(direction);
            actor.UpdateFieldOfView();
        }

        // Beëindig de beurt als dit de speler is
        EndTurn(actor);
    }

    // Functie om de beurt van een actor te beëindigen
    static private void EndTurn(Actor actor)
    {
        // Controleer of de actor een speler is
        if (actor.GetComponent<Player>() != null)
        {
            // Start de beurt van vijanden in GameManager
            GameManager.Get.StartEnemyTurn();
        }
    }
}