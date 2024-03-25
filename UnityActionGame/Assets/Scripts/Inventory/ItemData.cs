using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������Ʈ ������ Ŭ�������� �޴��� ����
[CreateAssetMenu(fileName ="NewItemData", menuName ="Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName; //������ �̸�
    public Define.ItemType itemType; //������ Ÿ��
    public int price; //����
    public string description; //����
    public bool isStack; //���� �� �ִ���
    public Sprite icon; //������
    public GameObject prefab; //������
}

// * ScriptsableObject
//- Ŭ���� �ν��Ͻ��ʹ� ������ �뷮�� �����͸� �����ϴµ� ���
//- ������ �����̳� �̴�.(�����͸� �����ϴ� �뵵�� Ŭ����)
//- ���� �纻�� �����Ǵ� ���� �����ϸ�, �޸� ����� ����(������ �Ѵ�)
//- ����Ƽ�� �����ֱ� �Լ� �� OnEnable, OnDestroy�� ��� ����
//- ���ӿ�����Ʈ�� AddComponent�Ұ�
//- ������ �ϳ��� ���Ϸ� ���� ��