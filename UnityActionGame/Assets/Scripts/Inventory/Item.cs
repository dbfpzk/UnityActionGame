using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData data; //������ ����
    public int amount = 1;

    public Item() { }
    public Item(ItemData itemData)
    {
        data = itemData;
    }

    private void OnTriggerEnter(Collider other)
    {
        var pc = other.GetComponent<PlayerController>();
        if (pc == null) return;
        pc?.PickUpItem(this, amount);
    }

}
