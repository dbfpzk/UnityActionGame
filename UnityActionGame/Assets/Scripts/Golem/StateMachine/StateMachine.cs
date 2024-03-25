using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T>
{
    protected StateMachine<T> stateMachine;
    protected T context;
    protected int mecanimStateHash;

    public State() { }

    public State(int mecanimStateHash)
    {
        this.mecanimStateHash = mecanimStateHash;
    }

    public State(string mecanimStateName) : 
        this(Animator.StringToHash(mecanimStateName))
    {

    }
    //���� ����
    public void SetMachine(StateMachine<T> stateMachine, T context)
    {
        this.stateMachine = stateMachine;
        this.context = context;

        OnInitialized(); //�ʱ� ����
    }

    public abstract void OnInitialized(); //�ʱ⼳�� �Լ�
    public virtual void OnEnter() { } //�� ���¿� �������� ȣ���ϴ� �Լ�
    public virtual void OnUpdate(float deltaTime) { } //������ ������Ʈ �Լ�
    public virtual void OnFixedUpdate(float deltaTime) { }
    public virtual void OnExit() { } //���°� �������� ȣ���ϴ� �Լ�
}

public class StateMachine<T>
{
    private T context;
    public event System.Action OnChangedState;

    private State<T> currentState; //���� ����
    public State<T> CurrentState => currentState;

    private State<T> previousState; //��������
    public State<T> PreviousState => previousState;

    private float elapsedTime = 0.0f; //������� ��� �ð�
    public float ElapsedTime => elapsedTime;

    private Dictionary<System.Type, State<T>> states = new Dictionary<System.Type, State<T>>(); //���µ��� ������ ��ųʸ�

    public StateMachine(T context, State<T> state)
    {
        this.context = context;
        AddState(state); //���� �߰�
        currentState = state;
        currentState.OnEnter();
    }
    //���� �߰�
    public void AddState(State<T> state)
    {
        state.SetMachine(this, context);
        states[state.GetType()] = state;
    }
    //������Ʈ �Լ�
    public void OnUpdate(float deltaTime)
    {
        elapsedTime += deltaTime;
        currentState.OnUpdate(deltaTime);
    }
    public void OnFixedUpdate(float deltaTime)
    {
        currentState.OnFixedUpdate(deltaTime);
    }
    //���� ��ȯ �Լ�
    public R ChangeState<R>() where R : State<T>
    {
        var newType = typeof(R);
        if(currentState.GetType() == newType)
        {
            return currentState as R;
        }
        if(currentState != null)
        {
            currentState.OnExit();
        }
        //���� ��ȯ
        previousState = currentState; //�������¿� �־���
        currentState = states[newType]; //���ο� ���¸� �־���
        currentState.OnEnter();
        elapsedTime = 0f; //����ð� �ʱ�ȭ

        if(OnChangedState != null)
        {
            OnChangedState?.Invoke();
        }
        return currentState as R;
    }

}
