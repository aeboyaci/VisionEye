using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChestPuzzleController : MonoBehaviour
{
    public GameObject chest;
    public Button button;

    private Animator animator;

    void Start()
    {
        animator = chest.GetComponent<Animator>();
        animator.enabled = false;

        button.onClick.AddListener(onButtonClick);
    }

    private void onButtonClick()
    {
        animator.enabled = true;

        StartCoroutine(CompleteAchievement_Coroutine());
    }

    IEnumerator CompleteAchievement_Coroutine()
    {
        string achievementId = "92d76ec8-c475-4a01-96ab-d1ac6532b457";

        AchievementBody body = new AchievementBody { achievementId = achievementId, gameId = State.ActiveGameId, teamId = State.ActiveTeamId };

        UnityWebRequest request = Client.PrepareRequest("POST", $"/achievements", body);
        yield return request.SendWebRequest();
    }
}
