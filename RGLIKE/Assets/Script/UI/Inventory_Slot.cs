using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Slot : MonoBehaviour
{
	public RectTransform rectf { get; private set; }
	//private Image img { get; private set; }
	public Image itemImg { get; private set; }
	public Text itemNum { get; private set; }
	public Text quickShortcutKey { get; private set; }

	public InventorySlotType type;
	public Inventory_Slot connectSlot;
	private KeyCode key;

	public Item item;
	private int num;

	private void Awake()
	{
		rectf = GetComponent<RectTransform>();
		itemImg = transform.Find("Item_Image").
			GetComponent<Image>();
		itemNum = transform.transform.Find("Item_Num").
			GetComponent<Text>();

		quickShortcutKey = transform.transform.Find("Quick_Num").
			GetComponent<Text>();

		num = 0;
	}

	private void LateUpdate()
	{
		if (type == InventorySlotType.Nomal ||
			item == null)
			return;

		
		if(Input.GetKeyDown(key))
		{
			useItem();
		}
	}

	public void init(InventorySlotType type, int i)
	{
		this.type = type;
		key = KeyCode.Alpha1 + i;
		item = null;
		itemImg.sprite = null;

		//btn = GetComponent<Button>();

		num = 0;
		itemNum.text = num.ToString();

		if (type == InventorySlotType.Quick)
			quickShortcutKey.text = (i + 1).ToString();
	}

	public void clear()
	{
		item = null;
		itemImg.sprite = null;
		itemImg.color = IMacro.color_NoneAlpha;
		num = 0;
		itemNum.text = num.ToString();
	}

	public void useItem()
	{
		if (item == null)
		{
			print("slotitem use null");
			return;
		}

		item.onUse();
		addItemNum(-1);

		if(connectSlot)
		{
			connectSlot.addItemNum(-1);
			if (connectSlot.num <= 0)
			{
				connectSlot.clear();
			}
		}
	}

	public void addItemNum(int n)
	{
		num += n;
		itemNum.text = num.ToString();
		if (num <= 0)
		{
			clear();
		}

	}

	public void addItemInvenSlot(Item item)
	{
		this.item = item;
		itemImg.sprite = Inventory.instance.listItemSprite[(int)item.type];
		itemImg.color = IMacro.color_White;
		addItemNum(1);
	}


	public void addItemQuickSlot(Inventory_Slot slot)
	{
		connectSlot = slot;
		connectSlot.connectSlot = this;
		item = slot.item;
		itemImg.sprite = slot.itemImg.sprite;
		itemImg.color = IMacro.color_White;
		slot.itemImg.color = IMacro.color_White;
		num = slot.num;
		itemNum.text = num.ToString();
	}

	public void removeItemQuickSlot()
	{
		connectSlot = null;
		item = null;
		itemImg.sprite = null;
		itemImg.color = IMacro.color_NoneAlpha;
		num = 0;
		itemNum.text = num.ToString();
	}

	public void changeSlot(Inventory_Slot slot)
	{
		Item tmpItem = item;
		item = slot.item;
		slot.item = tmpItem;

		Sprite tmpSp = itemImg.sprite;
		itemImg.sprite = slot.itemImg.sprite;
		slot.itemImg.sprite = tmpSp;

		ref Color white = ref IMacro.color_White;
		ref Color none = ref IMacro.color_NoneAlpha;
		itemImg.color = item ? white : none;
		slot.itemImg.color = slot.item ? white : none;

		Inventory_Slot tmpSlot = connectSlot;
		connectSlot = slot.connectSlot;
		slot.connectSlot = tmpSlot;


		int tmpNum = num;
		num = slot.num;
		slot.num = tmpNum;

		itemNum.text = num.ToString();
		slot.itemNum.text = slot.num.ToString();
	}
}
