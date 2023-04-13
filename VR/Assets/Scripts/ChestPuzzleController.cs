using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }
}
