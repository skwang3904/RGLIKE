using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Slot : MonoBehaviour
{
	public InventorySlotType slotType { get; private set; }
	public Inventory_Slot connectSlot { get; private set; }

	public RectTransform rectf { get; private set; }
	private Item_Type itemType;
	public Item item
	{ // 슬롯에는 타입만 가지고, item으로 다른곳에서 불러서 사용
		get
		{
			if (itemType == Item_Type.None)
				return null;
			return Item.itemList[(int)itemType];
		}
	}
	private Image imgSlot;
	public int itemNum { get; private set; }
	private Text textSlotItem;

	private Text textQuickKey;
	private KeyCode key;

	private void Awake()
	{
		connectSlot = null;

		rectf = GetComponent<RectTransform>();

		itemType = Item_Type.None;

		GameObject g = transform.Find("Item_Image").gameObject;
		imgSlot = g.GetComponent<Image>();

		itemNum = 0;
		g = transform.Find("Item_Num").gameObject;
		textSlotItem = g.GetComponent<Text>();

		textQuickKey = null;
		key = 0;
	}

	private void LateUpdate()
	{
		if (slotType == InventorySlotType.Quick &&
			key != 0)
		{
			if (Input.GetKeyDown(key))
			{
				useItem();
			}
		}
	}

	//
	public void init(InventorySlotType t, int keyIndex)
	{
		slotType = t;
		if (slotType == InventorySlotType.Quick)
		{
			key = KeyCode.Alpha1 + keyIndex;
			textQuickKey = transform.Find("Quick_Num").GetComponent<Text>();
			textQuickKey.text = (keyIndex + 1).ToString();
		}
	}

	public void clear()
	{
		connectSlot = null;

		itemType = Item_Type.None;
		setImg(null);
		itemNum = 0;
		textSlotItem.text = itemNum.ToString();
		
	}

	public void addItem(Item it)
	{
		itemType = it.type;
		setImg(it.spriteRenderer.sprite);
		//increaseNum(1);
		itemNum = 1;
		textSlotItem.text = itemNum.ToString();
	}

	public void addQuickSlot(Inventory_Slot slot)
	{
		foreach (Inventory_Slot s in Inventory.instance.list_quickSlot)
		{
			if (s.itemType == Item_Type.None)
				continue;
			//slot 새로 들어오는 것
			//s 기존 있던것
			if (slot.itemType == s.itemType)
			{
				s.clear();
				break;
			}
		}
		connectSlot = slot;
		slot.connectSlot = this;

		itemType = connectSlot.itemType;
		setImg(connectSlot.imgSlot.sprite);
		itemNum = connectSlot.itemNum;
		textSlotItem.text = itemNum.ToString();
	}

	// increase decrease
	public void increaseNum(int num)
	{
		itemNum += num;
		textSlotItem.text = itemNum.ToString();
	}

	public void decreaseNum(int num)
	{
		itemNum -= num;
		textSlotItem.text = itemNum.ToString();
		if (itemNum <= 0)
			clear();
	}

	public void change(Inventory_Slot slot)
	{
		// slot 바꾸는 슬롯
		Inventory_Slot s = connectSlot;
		connectSlot = slot.connectSlot;
		slot.connectSlot = s;

		if (connectSlot)
			connectSlot.connectSlot = this;
		if (slot.connectSlot)
			slot.connectSlot.connectSlot = slot;

		Item_Type itty = itemType;
		itemType = slot.itemType;
		slot.itemType = itty;

		Sprite sp = imgSlot.sprite;
		setImg(slot.imgSlot.sprite);
		slot.setImg(sp);

		int n = itemNum;
		itemNum = slot.itemNum;
		slot.itemNum = n;

		textSlotItem.text = itemNum.ToString();
		slot.textSlotItem.text = slot.itemNum.ToString();
	}

	public void useItem()
	{
		if (item == null)
		{
			print("item null");
			return;
		}

		item.onUse();

		connectSlot?.decreaseNum(1);
		decreaseNum(1);
	}


	private void setImg(Sprite sp)
	{
		imgSlot.sprite = sp;
		if (imgSlot.sprite == null)
		{
			imgSlot.color = IMacro.color_NoneAlpha;
		}
		else
		{
			imgSlot.color = IMacro.color_White;
		}
	}


	/////
	public bool contain(Vector2 pos)
	{
		pos = rectf.InverseTransformPoint(pos);
		if (rectf.rect.Contains(pos))
			return true;
		else
			return false;
	}
}