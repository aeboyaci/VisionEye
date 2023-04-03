using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScript : MonoBehaviour
{  
    public void loadScene(string sceneName)
    {
        // TODO: send request to create game if the player is captain

        SceneManager.LoadScene(sceneName);  
    }
}
