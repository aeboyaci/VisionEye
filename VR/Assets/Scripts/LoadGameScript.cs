using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScript : MonoBehaviour
{
    // Start is called before the first frame update
   
    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);  
    }
}
