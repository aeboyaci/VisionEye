using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit;

public class cubeColorControl : MonoBehaviour
{

    public static string cubePassForDoor="";
    [SerializeField] public TMPro.TMP_Text textOfChest;
    public string answer = "";
    public GameObject door;
    private void Start()
    {
        for(int i =0; i < 5; i++)
        {
            float randomNumber = Random.Range(0, 3);
            randomNumber = (int)randomNumber;
            if (randomNumber == 0)
            {
                answer = answer + "A";
            }
            if (randomNumber == 1)
            {
                answer = answer + "B";
            }
            if (randomNumber == 2)
            {
                answer = answer + "C";
            }
        }
        textOfChest.text = answer;
    }
    // Update is called once per frame
    void Update()
    {
        if(cubePassForDoor.Length>4)
        {
            cubePassForDoor = cubePassForDoor.Substring(cubePassForDoor.Length - 5);
        }
        if (cubePassForDoor == answer)
        {
            GetComponent<Renderer>().material.color = new Color(0, 204, 102);
            door.GetComponent<XRGrabInteractable>().enabled = true;

            StartCoroutine(CompleteAchievement_Coroutine());
        }
    }

    IEnumerator CompleteAchievement_Coroutine()
    {
        string achievementId = "aaf6c38f-8c35-4d54-944d-3fb76ba4f630";

        AchievementBody body = new AchievementBody { achievementId = achievementId, gameId = ScaryVerseState.ActiveGameId, teamId = ScaryVerseState.ActiveTeamId };

        UnityWebRequest request = Client.PrepareRequest("POST", $"/achievements", body);
        yield return request.SendWebRequest();
    }
}
