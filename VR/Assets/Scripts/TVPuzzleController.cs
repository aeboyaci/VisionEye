using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TVPuzzleController : MonoBehaviour
{
    public Button button;
    public GameObject quizContainer;
    public TMPro.TMP_Text questionText;
    public Toggle toggleA;
    public Toggle toggleB;
    public GameObject table;

    private Animator animator;

    async void Start()
    {
        animator = table.GetComponent<Animator>();
        animator.enabled = false;

        button.onClick.AddListener(onButtonClick);

        toggleA.onValueChanged.AddListener(onToggleAValueChanged);
        toggleB.onValueChanged.AddListener(onToggleBValueChanged);

        toggleA.isOn = false;
        toggleB.isOn = false;

        Question[] quizes = await PublicAPIClient.FetchQuiz();

        for (int i = 0; i < quizes.Length; i++)
        {
            Question quiz = quizes[i];

            if (quiz.question.Split(" ").Length < 13)
            {
                questionText.text = quiz.question;

                toggleA.GetComponentInChildren<Text>().text = quiz.correctAnswer;
                toggleB.GetComponentInChildren<Text>().text = quiz.incorrectAnswers[0];

                break;
            }
        }
    }

    private void onButtonClick()
    {
        quizContainer.gameObject.SetActive(!quizContainer.gameObject.active);
    }

    private void onToggleAValueChanged(bool isOn)
    {
        if (isOn)
        {
            quizContainer.gameObject.SetActive(false);
            animator.enabled = true;

            StartCoroutine(CompleteAchievement_Coroutine());
        }
    }

    private void onToggleBValueChanged(bool isOn)
    {
        if (isOn)
        {
            quizContainer.gameObject.SetActive(false);
        }
    }

    IEnumerator CompleteAchievement_Coroutine()
    {
        string achievementId = "3e9d198e-26a2-45d8-879f-1b9c53fb28d5";

        AchievementBody body = new AchievementBody{ achievementId = achievementId, gameId = ScaryVerseState.ActiveGameId, teamId = ScaryVerseState.ActiveTeamId };

        UnityWebRequest request = Client.PrepareRequest("POST", $"/achievements", body);
        yield return request.SendWebRequest();
    }
}
