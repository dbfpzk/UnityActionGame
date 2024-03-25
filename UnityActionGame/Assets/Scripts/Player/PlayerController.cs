using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Inventory inventory; //인벤토리

    // * 컴포넌트
    Transform character;
    Animator animator;
    new Rigidbody rigidbody;

    // * 카메라
    new Camera camera; //카메라
    Transform camAxis; //카메라 중심축
    float camSpeed = 8f; //카메라 속도
    float mouseX = 0f; //마우스 x축
    float mouseY = 4f; //마우스 y축
    float wheel = -10f; //휠
    public Vector3 offset; //오프셋(편차, 변위)

    //애니메이터의 parameter를 문자열로 접근하는것보다 Hash값으로 가져오면 성능상 이점이 있다.
    readonly int speedHash = Animator.StringToHash(Define.Animations.speed);
    readonly int comboCountHash = Animator.StringToHash(Define.Animations.comboCount);
    readonly int isAttackingHash = Animator.StringToHash(Define.Animations.isAttacking);
    readonly int isNextComboHash = Animator.StringToHash(Define.Animations.isNextCombo);

    public float Speed
    {
        get { return animator.GetFloat(speedHash); }
        set{ animator.SetFloat(speedHash, value); }
    }
    public int ComboCount
    {
        get { return animator.GetInteger(comboCountHash); }
        set { animator.SetInteger(comboCountHash, value); }
    }
    public bool IsAttacking
    {
        get { return animator.GetBool(isAttackingHash); }
        set { animator.SetBool(isAttackingHash, value); }
    }

    public bool IsNextCombo
    {
        get { return animator.GetBool(isNextComboHash); }
        set { animator.SetBool(isNextComboHash, value); }
    }

    void Start()
    {
        Physics.gravity = new Vector3(0, -9.81f, 0);
        character = transform.GetChild(0); //첫번째 자식을 가져옴
        animator = character.GetComponent<Animator>();
        rigidbody = gameObject.GetComponent<Rigidbody>(); //리지드바디 추가
        rigidbody.freezeRotation = true;
        
        // * 콜리더
        var collider = gameObject.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0, 2.95f, 0);
        collider.radius = 1f;
        collider.height = 6f;

        // * 카메라
        camera = Camera.main;
        camAxis = new GameObject("CamAxis").transform; //빈오브젝트 생성
        camera.transform.parent = camAxis;
        camera.transform.position = new Vector3(0, 5, -10); //카메라 초기 위치

        // * 인벤토리
        inventory = FindObjectOfType<Inventory>();
    }
    //키입력
    void Update()
    {
        Attack();
        ShowInventory();
    }
    //카메라 처리
    private void LateUpdate()
    {
        CameraMove();
        Zoom();
    }
    //물리 처리
    private void FixedUpdate()
    {
        Move();
    }

    void Attack()
    {
        //UI를 클릭했는지 여부
        bool isUIClick = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        bool isRightMouseDown = Input.GetMouseButtonDown(0);
        if (!isRightMouseDown || isUIClick) 
            return;
        if(!IsAttacking) //공격중이 아니라면
        {
            IsAttacking = true; //공격중
        }
        else //공격중이라면
        {
            IsNextCombo = true; //다음 콤보어택 가능
        }
    }
    void CameraMove()
    {
        mouseX += Input.GetAxis(Define.Keys.mouseX);
        mouseY -= Input.GetAxis(Define.Keys.mouseY);

        //축 고정
        if (mouseY > 10)
            mouseY = 10;
        if (mouseY < 0)
            mouseY = 0;

        //카메라 회전
        camAxis.rotation = Quaternion.Euler(new Vector3(
            camAxis.rotation.x + mouseY,
            camAxis.rotation.y + mouseX, 0) * camSpeed);
    }
    void Zoom()
    {
        //휠한 정도를 누적시킴
        wheel += Input.GetAxis(Define.Keys.mouseScroll) * 10;
        //휠 고정
        if (wheel >= -10)
            wheel = -10;
        if (wheel <= -20)
            wheel = -20;
        //휠한만큼 포지션에 적용
        camera.transform.localPosition = new Vector3(0, 0, wheel);
    }

    void Move()
    {
        if(Input.GetButton(Define.Keys.horizontal) 
            || Input.GetButton(Define.Keys.vertical))
        {
            float h = Input.GetAxisRaw(Define.Keys.horizontal);
            float v = Input.GetAxisRaw(Define.Keys.vertical);

            Vector3 movement = new Vector3(h, rigidbody.velocity.y, v);
            transform.rotation = Quaternion.Euler
                (new Vector3(0, camAxis.rotation.y + mouseX, 0) * camSpeed);

            //TransformDirection : 로컬방향으로 변경
            Vector3 localMovement = transform.TransformDirection(movement);
            rigidbody.velocity = localMovement.normalized * 10;

            //회전(캐릭터를 카메라가 보고 있는 방향으로 부드럽게 회전시킴)
            character.transform.localRotation = Quaternion.Slerp(character.transform.localRotation, 
                Quaternion.LookRotation(movement), 5 * Time.deltaTime);
        }
        else
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        }
        Speed = rigidbody.velocity.sqrMagnitude; //이동속도를 애니메이터에 적용
        camAxis.position = transform.position + new Vector3(0, 4, 0);
        character.eulerAngles = new Vector3(0, character.eulerAngles.y, 0);
    }

    //아이템 획득
    public bool PickUpItem(Item item, int amout = -1)
    {
        if (item != null && inventory.AddItem(item, amout))
        {
            Destroy(item.gameObject);
            return true;
        }
        return false;
    }

    void ShowInventory()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
        }
    }
}
