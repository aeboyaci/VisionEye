using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Invitation : MonoBehaviour
{
    public TMPro.TMP_Text sender;
    public Button acceptButton;
    public Button rejectButton;
    bool isAccepted;
    public string id;
    public string teamId;

    void Start()
    {
        acceptButton.onClick.AddListener(acceptInvitation);
        rejectButton.onClick.AddListener(rejectInvitation);
    }

    private void acceptInvitation()
    {
        isAccepted = true;
        StartCoroutine(AnswerInvitations_Coroutine());
    }

    private void rejectInvitation()
    {
        isAccepted = false;
        StartCoroutine(AnswerInvitations_Coroutine());
    }

    IEnumerator AnswerInvitations_Coroutine()
    {
        string status;
        switch (isAccepted)
        {
            case true:
                status = "ACCEPTED";
                break;
            case false:
                status = "REJECTED";
                break;
        }
        Debug.Log("Answering invitation");
        Debug.Log(isAccepted);
        AnswerInvitation answer = new AnswerInvitation();
        answer.id = id;
        answer.status = status;
        UnityWebRequest request = Client.PrepareRequest("POST", "/invitations", answer);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (isAccepted)
            {
                State.ActiveTeamId = teamId;
                State.IsCaptain = false;

                GameObject createNewGameScreen = GameObject.Find("CreateNewGameScreen");
                GameObject homeScreen = GameObject.Find("HomeScreen");

                createNewGameScreen.transform.GetChild(0).gameObject.SetActive(true);
                homeScreen.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
                Destroy(gameObject);
        }
        else if (request.result != UnityWebRequest.Result.InProgress && request.result == UnityWebRequest.Result.Success)
        {
            Destroy(gameObject);
        }
    }
}
