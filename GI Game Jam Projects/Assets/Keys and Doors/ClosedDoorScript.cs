using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedDoorScript : MonoBehaviour
{

    public int keysRemaining;

    // Start is called before the first frame update
    void Start()
    {
        keysRemaining = GameObject.FindGameObjectsWithTag("Key").Length;
        Debug.Log(keysRemaining);
    }

    // Update is called once per frame
    void Update()
    {
        if(keysRemaining == 0)
        {
            Destroy(gameObject);
        }
    }
}
