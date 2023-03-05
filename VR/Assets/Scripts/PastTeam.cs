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

    /*void Start()
    {
        // Attach the OnButtonClick function to the create button's onClick event
        teamDetailButton.onClick.AddListener(OnTeamDetailButtonClick);
    }

    private void OnTeamDetailButtonClick()
    {
        StartCoroutine(MyCoroutine());
    }
    */
   
}
