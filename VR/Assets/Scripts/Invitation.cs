using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Invitation : MonoBehaviour
{
    public TMPro.TMP_Text teamName;
    public TMPro.TMP_Text sender;
    public Button acceptButton;
    public Button rejectButton;
    bool isAccepted;

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
        string id = "testId";
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
        Debug.Log("Answering invitatin");
        Debug.Log(isAccepted);
        AnswerInvitation answer = new AnswerInvitation();
        answer.id = id;
        answer.status = status;
        UnityWebRequest request = Client.PrepareRequest("POST", "/invitations", answer);
        yield return request.SendWebRequest();

        if (!(request.result == UnityWebRequest.Result.Success))
            throw new Exception("Answer is invalid!");
    }



}
