using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//프로젝트 오른쪽 클릭했을때 메뉴를 생성
[CreateAssetMenu(fileName ="NewItemData", menuName ="Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName; //아이템 이름
    public Define.ItemType itemType; //아이템 타입
    public int price; //가격
    public string description; //설명
    public bool isStack; //쌓일 수 있는지
    public Sprite icon; //아이콘
    public GameObject prefab; //프리팹
}

// * ScriptsableObject
//- 클래스 인스턴스와는 별도로 대량의 데이터를 저장하는데 사용
//- 데이터 컨테이너 이다.(데이터를 저장하는 용도의 클래스)
//- 값의 사본이 생성되는 것을 방지하며, 메모리 사용을 줄임(참조를 한다)
//- 유니티의 생명주기 함수 중 OnEnable, OnDestroy만 사용 가능
//- 게임오브젝트에 AddComponent불가
//- 고유한 하나의 파일로 저장 됨