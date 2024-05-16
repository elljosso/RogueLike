using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Enemy : MonoBehaviour
{
    private void Start()
    {
        GameManager.Get.AddEnemy(GetComponent<Actor>());
    }
}