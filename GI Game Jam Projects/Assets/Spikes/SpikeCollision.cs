using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpikeCollision : MonoBehaviour
{
    private Scene scene;
    void Start()
    {
        scene = SceneManager.GetActiveScene();
    }
    void OnCollisionEnter2D(Collision2D col)
    {
       if(col.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(scene.name);
        }
    }
}
