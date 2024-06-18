using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetSceneProgressValue : StateMachineBehaviour
{
    Animator animator;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("sceneprogress", 0);
    }
}
