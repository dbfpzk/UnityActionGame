using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemController : MonoBehaviour
{
    float attackRange = 7.0f; //���ݹ���

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

        //���¸ӽ��� ����� ������ �߰�
        stateMachine = new StateMachine<GolemController>(this, new IdleState());
        stateMachine.AddState(new MoveState()); //�̵����� �߰�
        stateMachine.AddState(new AttackState()); //���ݻ��� �߰�
    }

    // Update is called once per frame
    void Update()
    {
        //������Ʈ�ӽ��� ������Ʈ�Լ��� ���������
        //OnUpdate�Լ��� Update�Լ��� ������ ������ �ϰ� ��
        stateMachine.OnUpdate(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        stateMachine.OnFixedUpdate(Time.fixedDeltaTime);
    }

    //������ �������� ����
    public bool IsAvailableAttack
    {
        get
        {
            //Ÿ���� ���ٸ�
            if(!Target)
            {
                return false; //���ݺҰ�
            }
            //�Ÿ��� üũ
            float distance = (Target.position - transform.position).sqrMagnitude;
            //�Ÿ��� �����ٸ� true, �ִٸ� false��ȯ
            return (distance <= (attackRange * attackRange));
        }
    }

    //Ÿ�� �������� ȸ��
    public void RotateToTarget()
    {
        if (!Target)
            return;
        //Ÿ�������� ������ ����
        Vector3 direction = (Target.position - transform.position).normalized;
        //�� ������ x, z�ุ�� �������� ȸ������ ���� (y���� ���Ŵ� ���̸� ����)
        Quaternion lookRotation =
            Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //���麸������ �ڿ������� ȸ��
        transform.rotation = 
            Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }
    //Ÿ������ �̵�
    public void MoveToTarget()
    {
        rigidbody.MovePosition(transform.position + (transform.forward * 3f * Time.deltaTime));
    }
}
