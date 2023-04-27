using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class slidingPuzzle : MonoBehaviour
{
    int x;
    int y;
    int z;
    Vector3 position;
    private Animator mAnimator;
    public GameObject animatorObject;
    // Start is called before the first frame update
    void Start()
    {
        mAnimator = animatorObject.GetComponent<Animator>();
        position = transform.position;
         x = (int)position.x;
         y = (int)position.y;
         z = (int)position.z;


    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        int a = (int)position.x;
        int b = (int)position.y;
        int c = (int)position.z;
        if((a!=x) || (b != y) || (c != z))
        {

           // StartCoroutine(Bekle(1f));
            mAnimator.SetBool("isMoved", true);
            // print(x+" "+y+" "+z);
            // print(position.x + " " + position.y + " " + position.z);

            StartCoroutine(CompleteAchievement_Coroutine());
        }
    }

    IEnumerator Bekle(float saniye)
    {
        yield return new WaitForSeconds(saniye);

        Debug.Log("Beklendi");
    }

    IEnumerator CompleteAchievement_Coroutine()
    {
        string achievementId = "513a7b25-13e7-484d-91ef-485fc09cd9ff";

        AchievementBody body = new AchievementBody { achievementId = achievementId, gameId = ScaryVerseState.ActiveGameId, teamId = ScaryVerseState.ActiveTeamId };

        UnityWebRequest request = Client.PrepareRequest("POST", $"/achievements", body);
        yield return request.SendWebRequest();
    }
}
