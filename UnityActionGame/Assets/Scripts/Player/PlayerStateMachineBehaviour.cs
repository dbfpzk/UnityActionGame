using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachineBehaviour : StateMachineBehaviour
{
    readonly int comboCountHash = 
        Animator.StringToHash(Define.Animations.comboCount);
    readonly int isNextComboHash = 
        Animator.StringToHash(Define.Animations.isNextCombo);
    readonly int isAttackingHash =
        Animator.StringToHash(Define.Animations.isAttacking);


    //애니메이션 실행되는 동안에 호출
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        float currentTime = stateInfo.normalizedTime; //현재 애니메이션의 진행 시간
        bool isNextCombo = animator.GetBool(isNextComboHash);
        //현재 애니메이션이 70%~90% 사이진행되었고 다음콤보공격이 가능하다면
        if(currentTime < 0.9f && currentTime > 0.7f && isNextCombo)
        {
            int comboCount = animator.GetInteger(comboCountHash);
            //콤보카운트가 2보다 크면 0, 아니면 1 증가
            comboCount = comboCount < 2 ? ++comboCount : 0; 
            animator.SetInteger(comboCountHash, comboCount);
        }
        //90% 진행되었다면
        if(stateInfo.normalizedTime >= 0.9f)
        {
            animator.SetInteger(comboCountHash, 0);
            animator.SetBool(isAttackingHash, false);
            animator.SetBool(isNextComboHash, false);
        }
    }

    //다음 애니메이션으로 바뀌기 직전에 호출
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        //다음 콤보공격 하지 않음
        animator.SetBool(isNextComboHash, false);
    }
}
