using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{

    Vector2 destiny;
    public float distance;

    public GameObject nodePrefab;

    public GameObject player;

    public GameObject lastNode;
    
    // Start is called before the first frame update
    void Start()
    {
        distance = 1.0f;
        player = GameObject.FindGameObjectWithTag("Player");
        lastNode = transform.gameObject;
        while (Vector2.Distance(player.transform.position, lastNode.transform.position) > distance)
        {
            CreateNode();
        }
        
        lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
        player.GetComponent<PlayerMovement>().hooked = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void CreateNode()
    {
        Vector2 posToCreate = player.transform.position - lastNode.transform.position;
        posToCreate.Normalize();
        posToCreate *= distance;
        posToCreate += (Vector2) lastNode.transform.position;

        GameObject newNode = Instantiate(nodePrefab, posToCreate, Quaternion.identity);

        lastNode.GetComponent<HingeJoint2D>().connectedBody = newNode.GetComponent<Rigidbody2D>();

        lastNode = newNode;
    }
}
