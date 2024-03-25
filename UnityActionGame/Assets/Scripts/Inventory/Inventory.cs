using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MouseData
{
    public static Inventory mouseOverInventory; //마우스가 올라간 인벤토리
    public static GameObject slotHoveredOver; //마우스가 올라간 슬롯
    public static GameObject dragImage; //드래그용 이미지
}

[RequireComponent(typeof(EventTrigger))]
public class Inventory : MonoBehaviour
{
    public GameObject slot; //슬롯

    Vector2 start = new Vector2(-110, 120); //시작위치
    Vector2 size = new Vector2(50, 50); //크기
    Vector2 space = new Vector2(5, 5); //여백
    int numberOfColumn = 5; //열 갯수

    Slot[] slots = new Slot[30]; //인벤토리 슬롯 배열
    Dictionary<GameObject, Slot> slotUIs = new Dictionary<GameObject, Slot>();

    public System.Action<ItemData> OnUseItem; //아이템 사용 델리게이트

    //빈 슬롯 갯수
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
        //ToString("n0") : 소수점 없이 표기, 1000자리마다 "," 표기
    }

    //슬롯의 위치 계산 함수
    Vector2 CalculatePosition(int i)
    {
        float x = start.x + ((space.x + size.x) * (i % numberOfColumn));
        float y = start.y + (-(space.y + size.y) * (i / numberOfColumn));

        return new Vector3(x, y, 0f);
    }

    //드래그 이미지 생성
    GameObject CreateDragImage(GameObject go)
    {
        if(slotUIs[go].data == null)
        {
            return null;
        }
        GameObject drageImage = new GameObject("DragImage");
        RectTransform rectTr = drageImage.AddComponent<RectTransform>();
        rectTr.sizeDelta = new Vector2(40, 40); //이미지 크기
        drageImage.transform.SetParent(transform.parent);
        Image image = drageImage.AddComponent<Image>();

        image.sprite = slotUIs[go].data.icon;
        image.raycastTarget = false; //마우스 터치의 영향을 받지 않음
        return drageImage;
    }

    //이벤트 추가
    void AddEvent(GameObject go, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        var trigger = go.GetComponent<EventTrigger>();
        if(!trigger)
        {
            return;
        }
        //EventTrigger.Entry : 실행시킬 이벤트의 타입과 함수를 담는 클래스
        EventTrigger.Entry eventTrigger = new EventTrigger.Entry { eventID = type };
        eventTrigger.callback.AddListener(action); //함수를 등록
        trigger.triggers.Add(eventTrigger); //트리거 추가
    }

    //마우스가 인벤토리 안에 들어왔을때
    public void OnEnterInterface(GameObject go)
    {
        MouseData.mouseOverInventory = go.GetComponent<Inventory>();
    }
    //마우스가 인벤토리를 벗어났을때
    public void OnExitInterface(GameObject go)
    {
        MouseData.mouseOverInventory = null;
    }
    //마우스가 슬롯에 들어왔을때
    public void OnEnterSlot(GameObject go)
    {
        MouseData.slotHoveredOver = go;
    }
    //마우스가 드래그 시작했을때
    public void OnStartDrag(GameObject go)
    {
        if (slotUIs[go].data == null)
            return;
        MouseData.dragImage = CreateDragImage(go);
    }
    //드래그 중일때
    public void OnDrag(GameObject go)
    {
        if (MouseData.dragImage == null)
            return;
        MouseData.dragImage.GetComponent<RectTransform>().position = Input.mousePosition;
    }
    //드래그 끝났을때
    public void OnEndDrag(GameObject go)
    {
        Destroy(MouseData.dragImage); //드래그 이미지 제거
        if (slotUIs[go].data == null)
            return;
        if(MouseData.mouseOverInventory == null)
        {
            slotUIs[go].SpawnItem();
        }
        else if(MouseData.slotHoveredOver)
        {
            Slot mouseHorverSlot = MouseData.mouseOverInventory.slotUIs[MouseData.slotHoveredOver];
            SwapItems(slotUIs[go], mouseHorverSlot); //슬롯정보 스왑
        }
    }

    //슬롯 정보 스왑
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
        //TODO : 추가기능
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

    //같은 이름의 슬롯 찾기
    public Slot FindItemInInventory(Item item)
    {
        //같은 이름이 있는지 검사하여 있다면 Slot, 없다면 null을 반환
        return slots?.FirstOrDefault(slot => slot.data?.name == item.data?.name);
    }
    //비어있는 슬롯 찾기
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
