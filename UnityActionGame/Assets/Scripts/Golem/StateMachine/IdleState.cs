using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State<GolemController>
{
    private Rigidbody rigidbody;

    public override void OnInitialized()
    {
        rigidbody = context?.GetComponent<Rigidbody>();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        context.Speed = 0; //�̵� �ִϸ��̼� ����
        context.IsAttacking = false; //������ �ƴ�
        context.ComboCount = 0; //�޺�ī��Ʈ 0���� �ʱ�ȭ
        rigidbody.velocity = Vector3.zero; //�̵�����
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (!context.Target)
            return;
        //������ �� ���� ���¶��
        if(!context.IsAvailableAttack)
        {
            //�̵����·� ��ȯ
            stateMachine.ChangeState<MoveState>();
        }
    }
}
