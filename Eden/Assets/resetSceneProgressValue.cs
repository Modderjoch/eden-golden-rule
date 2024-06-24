using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetSceneProgressValue : StateMachineBehaviour
{
    Animator animator;

    private bool isGameOver = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("sceneprogress", 0);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(GameManager.Instance.CurrentSceneIndex <= 4)
        {
            if (GameManager.Instance.CurrentSceneIndex < 4)
            {
                isGameOver = false;
            }

            if (isGameOver)
            {
                return;
            }
            if(GameManager.Instance.CurrentSceneIndex == 4)
            {
                isGameOver = true;
            }

            GameManager.Instance.QRScanningUI.SetActive(true);
        }
    }
}
