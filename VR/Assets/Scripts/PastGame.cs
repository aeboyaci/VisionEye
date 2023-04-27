using UnityEngine;
using UnityEngine.UI;

public class PastGame : MonoBehaviour
{
    public string id;
    public TMPro.TMP_Text roomname;
    public TMPro.TMP_Text score;
    public TMPro.TMP_Text minutes;
    public TMPro.TMP_Text date;
    public Button gameDetailButton;

    void Start()
    {
        gameDetailButton.onClick.AddListener(OnTeamDetailButtonClick);
    }

    private void OnTeamDetailButtonClick()
    {
        ScaryVerseState.ActiveGameId = id;

        GameObject gameDetailScreen = GameObject.Find("GameDetailScreen");
        GameObject teamDetailScreen = GameObject.Find("TeamDetailScreen");

        teamDetailScreen.transform.GetChild(0).gameObject.SetActive(false);
        gameDetailScreen.transform.GetChild(0).gameObject.SetActive(true);
    }

}