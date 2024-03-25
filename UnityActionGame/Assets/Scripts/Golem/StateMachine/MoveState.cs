using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State<GolemController>
{
    Rigidbody rigidbody;

    public override void OnInitialized()
    {
        rigidbody = context?.rigidbody;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        rigidbody.velocity = Vector3.zero;
        context.Speed = 3;
        context.IsAttacking = false;
        context.ComboCount = 0;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        //Ÿ���� ���ٸ�
        if(context.Target == null)
        {
            //������ ��ȯ
            stateMachine.ChangeState<IdleState>();
        }
        //������ �� �ִٸ�
        if(context.IsAvailableAttack)
        {
            //���ݻ��� ��ȯ
            stateMachine.ChangeState<AttackState>();
        }
        context.RotateToTarget(); //Ÿ�ٹ������� ȸ��
    }

    public override void OnFixedUpdate(float deltaTime)
    {
        base.OnFixedUpdate(deltaTime);
        context.MoveToTarget(); //�̵�
    }

    public override void OnExit()
    {
        base.OnExit();
        context.Speed = 0;
        rigidbody.velocity = Vector3.zero;
    }
}
