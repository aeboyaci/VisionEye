using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class PastTeam : MonoBehaviour
{
    public string id;
    public TMPro.TMP_Text name;
    public TMPro.TMP_Text totalScoreGained;
    public TMPro.TMP_Text minutesPlayed;
    public Button teamDetailButton;

    void Start()
    {
        teamDetailButton.onClick.AddListener(OnTeamDetailButtonClick);
    }

    private void OnTeamDetailButtonClick()
    {
        State.ActiveTeamId = id;

        GameObject teamDetailScreen = GameObject.Find("TeamDetailScreen");
        GameObject homeScreen = GameObject.Find("HomeScreen");

        homeScreen.transform.GetChild(0).gameObject.SetActive(false);
        teamDetailScreen.transform.GetChild(0).gameObject.SetActive(true);
    }
}
