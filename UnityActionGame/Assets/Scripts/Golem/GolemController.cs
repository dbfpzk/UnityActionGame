using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemController : MonoBehaviour
{
    float attackRange = 7.0f; //공격범위

    [HideInInspector]
    public new Rigidbody rigidbody;

    FieldOfView fov;
    public Transform Target => fov?.NearestTarget;

    protected StateMachine<GolemController> stateMachine;

    protected readonly int speedHash = 
        Animator.StringToHash(Define.Animations.speed);
    protected readonly int isAttacking = 
        Animator.StringToHash(Define.Animations.isAttacking);
    protected readonly int comboCountHash =
        Animator.StringToHash(Define.Animations.comboCount);

    Animator animator;

    public bool IsAttacking
    {
        get { return animator.GetBool(isAttacking); }
        set { animator.SetBool(isAttacking, value); }
    }
    public float Speed
    {
        get { return animator.GetFloat(speedHash); }
        set { animator.SetFloat(speedHash, value); }
    }
    public int ComboCount
    {
        get { return animator.GetInteger(comboCountHash); }
        set { animator.SetInteger(comboCountHash, value); }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        fov = GetComponent<FieldOfView>();

        //상태머신을 만들고 대기상태 추가
        stateMachine = new StateMachine<GolemController>(this, new IdleState());
        stateMachine.AddState(new MoveState()); //이동상태 추가
        stateMachine.AddState(new AttackState()); //공격상태 추가
    }

    // Update is called once per frame
    void Update()
    {
        //스테이트머신의 업데이트함수를 실행시켜줌
        //OnUpdate함수가 Update함수와 동일한 역할을 하게 됨
        stateMachine.OnUpdate(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        stateMachine.OnFixedUpdate(Time.fixedDeltaTime);
    }

    //공격이 가능한지 여부
    public bool IsAvailableAttack
    {
        get
        {
            //타겟이 없다면
            if(!Target)
            {
                return false; //공격불가
            }
            //거리를 체크
            float distance = (Target.position - transform.position).sqrMagnitude;
            //거리가 가깝다면 true, 멀다면 false반환
            return (distance <= (attackRange * attackRange));
        }
    }

    //타겟 방향으로 회전
    public void RotateToTarget()
    {
        if (!Target)
            return;
        //타겟으로의 방향을 구함
        Vector3 direction = (Target.position - transform.position).normalized;
        //그 방향을 x, z축만을 기준으로 회전값을 구함 (y축을 뺀거는 높이를 뺏다)
        Quaternion lookRotation =
            Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //구면보간으로 자연스럽게 회전
        transform.rotation = 
            Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }
    //타겟으로 이동
    public void MoveToTarget()
    {
        rigidbody.MovePosition(transform.position + (transform.forward * 3f * Time.deltaTime));
    }
}
