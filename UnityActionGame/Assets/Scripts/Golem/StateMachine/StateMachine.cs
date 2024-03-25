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
    //상태 설정
    public void SetMachine(StateMachine<T> stateMachine, T context)
    {
        this.stateMachine = stateMachine;
        this.context = context;

        OnInitialized(); //초기 설정
    }

    public abstract void OnInitialized(); //초기설정 함수
    public virtual void OnEnter() { } //이 상태에 들어왔을때 호출하는 함수
    public virtual void OnUpdate(float deltaTime) { } //상태의 업데이트 함수
    public virtual void OnFixedUpdate(float deltaTime) { }
    public virtual void OnExit() { } //상태가 끝났을때 호출하는 함수
}

public class StateMachine<T>
{
    private T context;
    public event System.Action OnChangedState;

    private State<T> currentState; //현재 상태
    public State<T> CurrentState => currentState;

    private State<T> previousState; //이전상태
    public State<T> PreviousState => previousState;

    private float elapsedTime = 0.0f; //현재상태 경과 시간
    public float ElapsedTime => elapsedTime;

    private Dictionary<System.Type, State<T>> states = new Dictionary<System.Type, State<T>>(); //상태들을 관리할 딕셔너리

    public StateMachine(T context, State<T> state)
    {
        this.context = context;
        AddState(state); //상태 추가
        currentState = state;
        currentState.OnEnter();
    }
    //상태 추가
    public void AddState(State<T> state)
    {
        state.SetMachine(this, context);
        states[state.GetType()] = state;
    }
    //업데이트 함수
    public void OnUpdate(float deltaTime)
    {
        elapsedTime += deltaTime;
        currentState.OnUpdate(deltaTime);
    }
    public void OnFixedUpdate(float deltaTime)
    {
        currentState.OnFixedUpdate(deltaTime);
    }
    //상태 전환 함수
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
        //상태 전환
        previousState = currentState; //이전상태에 넣어줌
        currentState = states[newType]; //새로운 상태를 넣어줌
        currentState.OnEnter();
        elapsedTime = 0f; //경과시간 초기화

        if(OnChangedState != null)
        {
            OnChangedState?.Invoke();
        }
        return currentState as R;
    }

}
