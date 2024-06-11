using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    // Functie om een actor in de opgegeven richting te verplaatsen of aan te vallen
    static public void MoveOrHit(Actor actor, Vector2 direction)
    {
        // Kijk of er een actor is op de doelpositie
        Actor target = GameManager.Get.GetActorAtLocation(actor.transform.position + (Vector3)direction);

        // Als er geen actor is, verplaatsen we de actor
        if (target == null)
        {
            Move(actor, direction);
        }
        else
        {
            Hit(actor, target);
        }

        // Beëindig de beurt als dit de speler is
        EndTurn(actor);
    }

    // Functie om een actor in de opgegeven richting te verplaatsen
    static private void Move(Actor actor, Vector2 direction)
    {
        actor.Move(direction);
        actor.UpdateFieldOfView();
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

    // Functie om een actor aan te vallen
    static public void Hit(Actor actor, Actor target)
    {
        // Bereken de schade (damage)
        int damage = actor.Power - target.Defense;

        // Als de schade positief is, verminder de hitpoints van het target
        if (damage > 0)
        {
            target.DoDamage(damage, actor); // Pass the attacker as an argument
            UIManager.Instance.AddMessage($"{actor.name} hits {target.name} for {damage} damage.", actor.GetComponent<Player>() ? Color.white : Color.red);
        }
        else
        {
            // Geen schade, toon een bericht
            UIManager.Instance.AddMessage($"{actor.name} attacks {target.name} but does no damage.", actor.GetComponent<Player>() ? Color.white : Color.red);
        }
    }
}