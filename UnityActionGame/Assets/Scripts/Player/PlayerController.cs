using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Inventory inventory; //�κ��丮

    // * ������Ʈ
    Transform character;
    Animator animator;
    new Rigidbody rigidbody;

    // * ī�޶�
    new Camera camera; //ī�޶�
    Transform camAxis; //ī�޶� �߽���
    float camSpeed = 8f; //ī�޶� �ӵ�
    float mouseX = 0f; //���콺 x��
    float mouseY = 4f; //���콺 y��
    float wheel = -10f; //��
    public Vector3 offset; //������(����, ����)

    //�ִϸ������� parameter�� ���ڿ��� �����ϴ°ͺ��� Hash������ �������� ���ɻ� ������ �ִ�.
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
        character = transform.GetChild(0); //ù��° �ڽ��� ������
        animator = character.GetComponent<Animator>();
        rigidbody = gameObject.GetComponent<Rigidbody>(); //������ٵ� �߰�
        rigidbody.freezeRotation = true;
        
        // * �ݸ���
        var collider = gameObject.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0, 2.95f, 0);
        collider.radius = 1f;
        collider.height = 6f;

        // * ī�޶�
        camera = Camera.main;
        camAxis = new GameObject("CamAxis").transform; //�������Ʈ ����
        camera.transform.parent = camAxis;
        camera.transform.position = new Vector3(0, 5, -10); //ī�޶� �ʱ� ��ġ

        // * �κ��丮
        inventory = FindObjectOfType<Inventory>();
    }
    //Ű�Է�
    void Update()
    {
        Attack();
        ShowInventory();
    }
    //ī�޶� ó��
    private void LateUpdate()
    {
        CameraMove();
        Zoom();
    }
    //���� ó��
    private void FixedUpdate()
    {
        Move();
    }

    void Attack()
    {
        //UI�� Ŭ���ߴ��� ����
        bool isUIClick = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        bool isRightMouseDown = Input.GetMouseButtonDown(0);
        if (!isRightMouseDown || isUIClick) 
            return;
        if(!IsAttacking) //�������� �ƴ϶��
        {
            IsAttacking = true; //������
        }
        else //�������̶��
        {
            IsNextCombo = true; //���� �޺����� ����
        }
    }
    void CameraMove()
    {
        mouseX += Input.GetAxis(Define.Keys.mouseX);
        mouseY -= Input.GetAxis(Define.Keys.mouseY);

        //�� ����
        if (mouseY > 10)
            mouseY = 10;
        if (mouseY < 0)
            mouseY = 0;

        //ī�޶� ȸ��
        camAxis.rotation = Quaternion.Euler(new Vector3(
            camAxis.rotation.x + mouseY,
            camAxis.rotation.y + mouseX, 0) * camSpeed);
    }
    void Zoom()
    {
        //���� ������ ������Ŵ
        wheel += Input.GetAxis(Define.Keys.mouseScroll) * 10;
        //�� ����
        if (wheel >= -10)
            wheel = -10;
        if (wheel <= -20)
            wheel = -20;
        //���Ѹ�ŭ �����ǿ� ����
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

            //TransformDirection : ���ù������� ����
            Vector3 localMovement = transform.TransformDirection(movement);
            rigidbody.velocity = localMovement.normalized * 10;

            //ȸ��(ĳ���͸� ī�޶� ���� �ִ� �������� �ε巴�� ȸ����Ŵ)
            character.transform.localRotation = Quaternion.Slerp(character.transform.localRotation, 
                Quaternion.LookRotation(movement), 5 * Time.deltaTime);
        }
        else
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        }
        Speed = rigidbody.velocity.sqrMagnitude; //�̵��ӵ��� �ִϸ����Ϳ� ����
        camAxis.position = transform.position + new Vector3(0, 4, 0);
        character.eulerAngles = new Vector3(0, character.eulerAngles.y, 0);
    }

    //������ ȹ��
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
