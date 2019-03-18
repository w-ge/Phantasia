using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyGet : MonoBehaviour
{
    GameObject door;
    ClosedDoorScript doorScript;
    void Start()
    {
        door = GameObject.FindWithTag("Door");
        doorScript = door.GetComponent<ClosedDoorScript>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            doorScript.keysRemaining--;
            Destroy(gameObject);
        }
    }
}
