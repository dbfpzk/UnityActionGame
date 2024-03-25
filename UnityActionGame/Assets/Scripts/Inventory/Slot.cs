using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class Slot : MonoBehaviour
{
    public ItemData data; //아이템 정보
    public int amount; //갯수
    public System.Action<Slot> OnPostUpdate;

    public Image iconImage;
    public Text amountText;

    public Slot(ItemData data, int amount) => UpdateSlot(data, amount);
    public void AddAmount(int value) => UpdateSlot(data, amount += value);


    // Start is called before the first frame update
    void Start()
    {
        iconImage = transform.GetChild(0).GetComponent<Image>();
        amountText = transform.GetChild(1).GetComponent<Text>();
        iconImage.sprite = null;
        iconImage.color = new Color(0, 0, 0, 0);
        amountText.text = string.Empty;
    }

    //슬롯이 비어있는지
    public bool IsCanPlaceInSlot(Slot slot)
    {
        if(slot.data == null || slot.amount <= 0)
        {
            return true;
        }
        return false;
    }
    //슬롯 정보 업데이트
    public void UpdateSlot(ItemData data, int amout)
    {
        if(amout <= 0)
        {
            data = null;
        }
        this.data = data;
        this.amount = amout;
        OnPostUpdate?.Invoke(this);
    }

    //아이템 생성
    public void SpawnItem()
    {
        PlayerController pc = Transform.FindObjectOfType<PlayerController>();
        Vector3 spawnPos = pc.transform.position + new Vector3(0, 3, 3);
        Item item = Instantiate(data.prefab, spawnPos, Quaternion.identity).GetComponent<Item>();
        item.amount = amount;
        UpdateSlot(null, 0);
    }
}
