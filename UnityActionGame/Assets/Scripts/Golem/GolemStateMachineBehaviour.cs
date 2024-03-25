using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemStateMachineBehaviour : StateMachineBehaviour
{
    readonly int comboCountHash = Animator.StringToHash(Define.Animations.comboCount);

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //���� �޺� ī��Ʈ���� �����´�.
        int comboCount = animator.GetInteger(comboCountHash);
        if(stateInfo.normalizedTime > 0.9f)
        {
            comboCount = comboCount < 2 ? comboCount + 1 : 0;
            animator.SetInteger(comboCountHash, comboCount);
        }
    }
}
