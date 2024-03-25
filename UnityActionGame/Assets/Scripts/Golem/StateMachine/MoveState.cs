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
        //타겟이 없다면
        if(context.Target == null)
        {
            //대기상태 전환
            stateMachine.ChangeState<IdleState>();
        }
        //공격할 수 있다면
        if(context.IsAvailableAttack)
        {
            //공격상태 전환
            stateMachine.ChangeState<AttackState>();
        }
        context.RotateToTarget(); //타겟방향으로 회전
    }

    public override void OnFixedUpdate(float deltaTime)
    {
        base.OnFixedUpdate(deltaTime);
        context.MoveToTarget(); //이동
    }

    public override void OnExit()
    {
        base.OnExit();
        context.Speed = 0;
        rigidbody.velocity = Vector3.zero;
    }
}
