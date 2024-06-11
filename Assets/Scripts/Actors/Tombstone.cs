using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tombstone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Voeg deze tombstone toe aan de GameManager
        GameManager.Get.AddTombStone(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}