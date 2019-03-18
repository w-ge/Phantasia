using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerHookshotController : MonoBehaviour
{
    public GameObject hookPrefab;
    public bool hooked;
    private bool release;
    public GameObject player;
    public LayerMask hookLayerMask;
    public Transform hookCheckPoint;
    public bool hookCheck;
    Collider2D lastHook;
    
    // Start is called before the first frame update
    void Start()
    {
        hooked = false;
        release = false;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        hookCheck = Physics2D.OverlapCircle(hookCheckPoint.position, 0.5f, hookLayerMask);

        // When fire is pressed, shoot a hook
        if (hookCheck && Input.GetButtonDown("Fire1") && !hooked)
        {
            Instantiate(hookPrefab, lastHook.gameObject.transform.position, Quaternion.identity);
            hooked = true;
        }

        // Destroy the hook if pressed again
        if (hooked && Input.GetButtonUp("Fire1"))
        {
            release = true;           
        }

        if (hooked && release && Input.GetButtonDown("Fire1"))
        {
            release = false;
            hooked = false;
            player.GetComponent<PlayerMovement>().hooked = false;
            // Turn this on to allow for better player movement on ground: player.GetComponent<PlayerMovement>().hooked = false;
            GameObject[] hook = GameObject.FindGameObjectsWithTag("HookShot");
            for(int i = 0; i < hook.Length; i++)
            {
                Destroy(hook[i]);
            }           
        }
    }


    void OnTriggerStay2D(Collider2D col)
    {
        lastHook = col;
    }
}
