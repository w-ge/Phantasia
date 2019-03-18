using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class OpenDoor : MonoBehaviour
{
    private int currentScene;
    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

   void OnTriggerEnter2D(Collider2D col)
    {
        SceneManager.LoadScene(currentScene + 1);
    }
}
