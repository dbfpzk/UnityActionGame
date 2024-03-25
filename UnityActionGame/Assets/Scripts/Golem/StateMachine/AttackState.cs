using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<GolemController>
{
    public override void OnInitialized()
    {

    }

    public override void OnEnter()
    {
        base.OnEnter();
        context.ComboCount = 0;
        context.IsAttacking = true;
    }
    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        context.RotateToTarget();
        if(!context.Target)
        {
            stateMachine.ChangeState<IdleState>();
        }
        if(!context.IsAvailableAttack)
        {
            stateMachine.ChangeState<MoveState>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        context.IsAttacking = false;
    }
}
