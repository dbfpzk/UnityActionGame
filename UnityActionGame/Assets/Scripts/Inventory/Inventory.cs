using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MouseData
{
    public static Inventory mouseOverInventory; //���콺�� �ö� �κ��丮
    public static GameObject slotHoveredOver; //���콺�� �ö� ����
    public static GameObject dragImage; //�巡�׿� �̹���
}

[RequireComponent(typeof(EventTrigger))]
public class Inventory : MonoBehaviour
{
    public GameObject slot; //����

    Vector2 start = new Vector2(-110, 120); //������ġ
    Vector2 size = new Vector2(50, 50); //ũ��
    Vector2 space = new Vector2(5, 5); //����
    int numberOfColumn = 5; //�� ����

    Slot[] slots = new Slot[30]; //�κ��丮 ���� �迭
    Dictionary<GameObject, Slot> slotUIs = new Dictionary<GameObject, Slot>();

    public System.Action<ItemData> OnUseItem; //������ ��� ��������Ʈ

    //�� ���� ����
    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            foreach(Slot slot in slots)
            {
                if(slot.amount == 0)
                {
                    counter++;
                }
            }
            return counter;
        }
    }

    public void OnPostUpdate(Slot slot)
    {
        bool isExist = slot.amount > 0;
        slot.iconImage.sprite = isExist ? slot.data.icon : null;
        slot.iconImage.color = isExist ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0);
        slot.amountText.text = isExist ? slot.amount.ToString("n0") : string.Empty;
        //ToString("n0") : �Ҽ��� ���� ǥ��, 1000�ڸ����� "," ǥ��
    }

    //������ ��ġ ��� �Լ�
    Vector2 CalculatePosition(int i)
    {
        float x = start.x + ((space.x + size.x) * (i % numberOfColumn));
        float y = start.y + (-(space.y + size.y) * (i / numberOfColumn));

        return new Vector3(x, y, 0f);
    }

    //�巡�� �̹��� ����
    GameObject CreateDragImage(GameObject go)
    {
        if(slotUIs[go].data == null)
        {
            return null;
        }
        GameObject drageImage = new GameObject("DragImage");
        RectTransform rectTr = drageImage.AddComponent<RectTransform>();
        rectTr.sizeDelta = new Vector2(40, 40); //�̹��� ũ��
        drageImage.transform.SetParent(transform.parent);
        Image image = drageImage.AddComponent<Image>();

        image.sprite = slotUIs[go].data.icon;
        image.raycastTarget = false; //���콺 ��ġ�� ������ ���� ����
        return drageImage;
    }

    //�̺�Ʈ �߰�
    void AddEvent(GameObject go, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        var trigger = go.GetComponent<EventTrigger>();
        if(!trigger)
        {
            return;
        }
        //EventTrigger.Entry : �����ų �̺�Ʈ�� Ÿ�԰� �Լ��� ��� Ŭ����
        EventTrigger.Entry eventTrigger = new EventTrigger.Entry { eventID = type };
        eventTrigger.callback.AddListener(action); //�Լ��� ���
        trigger.triggers.Add(eventTrigger); //Ʈ���� �߰�
    }

    //���콺�� �κ��丮 �ȿ� ��������
    public void OnEnterInterface(GameObject go)
    {
        MouseData.mouseOverInventory = go.GetComponent<Inventory>();
    }
    //���콺�� �κ��丮�� �������
    public void OnExitInterface(GameObject go)
    {
        MouseData.mouseOverInventory = null;
    }
    //���콺�� ���Կ� ��������
    public void OnEnterSlot(GameObject go)
    {
        MouseData.slotHoveredOver = go;
    }
    //���콺�� �巡�� ����������
    public void OnStartDrag(GameObject go)
    {
        if (slotUIs[go].data == null)
            return;
        MouseData.dragImage = CreateDragImage(go);
    }
    //�巡�� ���϶�
    public void OnDrag(GameObject go)
    {
        if (MouseData.dragImage == null)
            return;
        MouseData.dragImage.GetComponent<RectTransform>().position = Input.mousePosition;
    }
    //�巡�� ��������
    public void OnEndDrag(GameObject go)
    {
        Destroy(MouseData.dragImage); //�巡�� �̹��� ����
        if (slotUIs[go].data == null)
            return;
        if(MouseData.mouseOverInventory == null)
        {
            slotUIs[go].SpawnItem();
        }
        else if(MouseData.slotHoveredOver)
        {
            Slot mouseHorverSlot = MouseData.mouseOverInventory.slotUIs[MouseData.slotHoveredOver];
            SwapItems(slotUIs[go], mouseHorverSlot); //�������� ����
        }
    }

    //���� ���� ����
    public void SwapItems(Slot slotA, Slot slotB)
    {
        if (slotA == slotB)
            return;
        ItemData tempData = slotB.data;
        int tempAmount = slotB.amount;
        slotB.UpdateSlot(slotA.data, slotA.amount);
        slotA.UpdateSlot(tempData, tempAmount);
    }

    public void UseItem(Slot slot)
    {
        if (slot.data == null || slot.amount <= 0)
            return;
        ItemData data = slot.data;
        slot.UpdateSlot(slot.data, slot.amount - 1);
        OnUseItem?.Invoke(data);
    }

    void OnRightClick(Slot slot)
    {
        UseItem(slot);
    }
    void OnLeftClic(Slot slot)
    {
        //TODO : �߰����
    }

    public void OnClick(GameObject go, PointerEventData data)
    {
        Slot slot = slotUIs[go];
        if (slot == null)
            return;
        if(data.button == PointerEventData.InputButton.Left)
        {
            OnLeftClic(slot);
        }
        if(data.button == PointerEventData.InputButton.Right)
        {
            OnRightClick(slot);
        }
    }

    //���� �̸��� ���� ã��
    public Slot FindItemInInventory(Item item)
    {
        //���� �̸��� �ִ��� �˻��Ͽ� �ִٸ� Slot, ���ٸ� null�� ��ȯ
        return slots?.FirstOrDefault(slot => slot.data?.name == item.data?.name);
    }
    //����ִ� ���� ã��
    public Slot GetEmptySlot()
    {
        return slots?.FirstOrDefault(slot => slot.data == null);
    }

    public bool AddItem(Item item, int amount)
    {
        Slot slot = FindItemInInventory(item);
        if(!item.data.isStack || slot == null)
        {
            if(EmptySlotCount <= 0)
            {
                return false;
            }
            GetEmptySlot().UpdateSlot(item.data, amount);
        }
        else
        {
            slot.AddAmount(amount);
        }
        return true;
    }

    void CreateSlot()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            GameObject go = Instantiate(slot, Vector3.zero, Quaternion.identity, transform);

            go.GetComponent<RectTransform>().anchoredPosition = CalculatePosition(i);
            go.AddComponent<EventTrigger>();

            AddEvent(go, EventTriggerType.PointerEnter, delegate { OnEnterSlot(go); });
            AddEvent(go, EventTriggerType.PointerExit, delegate { OnEnterSlot(go); });

            AddEvent(go, EventTriggerType.BeginDrag, delegate { OnStartDrag(go); });
            AddEvent(go, EventTriggerType.EndDrag, delegate { OnEndDrag(go); });

            AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });
            AddEvent(go, EventTriggerType.PointerClick, 
                (data) => { OnClick(go, (PointerEventData)data); });

            slots[i] = go.GetComponent<Slot>();
            slots[i].OnPostUpdate += OnPostUpdate;
            slotUIs.Add(go, slots[i]);
            go.name = "Slot : " + i;
        }
    }

    private void Awake()
    {
        CreateSlot();

        AddEvent(gameObject, EventTriggerType.PointerEnter, 
            (baseEvent) => { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit,
            (baseEvent) => { OnExitInterface(gameObject); });
    }

}
