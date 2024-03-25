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


    //�ִϸ��̼� ����Ǵ� ���ȿ� ȣ��
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        float currentTime = stateInfo.normalizedTime; //���� �ִϸ��̼��� ���� �ð�
        bool isNextCombo = animator.GetBool(isNextComboHash);
        //���� �ִϸ��̼��� 70%~90% ��������Ǿ��� �����޺������� �����ϴٸ�
        if(currentTime < 0.9f && currentTime > 0.7f && isNextCombo)
        {
            int comboCount = animator.GetInteger(comboCountHash);
            //�޺�ī��Ʈ�� 2���� ũ�� 0, �ƴϸ� 1 ����
            comboCount = comboCount < 2 ? ++comboCount : 0; 
            animator.SetInteger(comboCountHash, comboCount);
        }
        //90% ����Ǿ��ٸ�
        if(stateInfo.normalizedTime >= 0.9f)
        {
            animator.SetInteger(comboCountHash, 0);
            animator.SetBool(isAttackingHash, false);
            animator.SetBool(isNextComboHash, false);
        }
    }

    //���� �ִϸ��̼����� �ٲ�� ������ ȣ��
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        //���� �޺����� ���� ����
        animator.SetBool(isNextComboHash, false);
    }
}
