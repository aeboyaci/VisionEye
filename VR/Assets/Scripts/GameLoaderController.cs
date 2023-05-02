using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoaderController : MonoBehaviour
{
    void Start()
    {
        if (ScaryVerseState.IsAuthenticated)
        {
            GameObject authenticationScreen = GameObject.Find("AuthenticationScreen");
            authenticationScreen.transform.GetChild(0).gameObject.SetActive(false);

            GameObject createNewGameScreen = GameObject.Find("CreateNewGameScreen");
            createNewGameScreen.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
