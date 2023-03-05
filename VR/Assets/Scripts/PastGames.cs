using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PastGames : MonoBehaviour
{

    public PastGame pastgame;
    public GameObject gameContainer;
    public void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            if (pastgame != null)
            {
                var row = Instantiate(pastgame, gameContainer.transform).GetComponent<PastGame>();
                row.roomname.text = "testroom";
                row.score.text = "testscore";
                row.minutes.text = "testminutes";
                row.date.text = "testdate";
            }
        }
    }

}
