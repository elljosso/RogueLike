using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    // Bool to determine if the ladder goes up, can be modified from the editor
    [SerializeField]
    private bool up;

    public bool Up { get => up; set => up = value; }

    // Start is called before the first frame update
    void Start()
    {
        // Add this ladder to the GameManager
        GameManager.Get.AddLadder(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
