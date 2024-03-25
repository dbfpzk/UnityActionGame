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
        context.Speed = 0; //이동 애니메이션 중지
        context.IsAttacking = false; //공격중 아님
        context.ComboCount = 0; //콤보카운트 0으로 초기화
        rigidbody.velocity = Vector3.zero; //이동중지
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (!context.Target)
            return;
        //공격할 수 없는 상태라면
        if(!context.IsAvailableAttack)
        {
            //이동상태로 전환
            stateMachine.ChangeState<MoveState>();
        }
    }
}
