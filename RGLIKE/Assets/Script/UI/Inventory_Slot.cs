using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Slot : MonoBehaviour
{
	public InventorySlotType type { get; private set; }
	public Inventory_Slot connectSlot { get; private set; }

	public RectTransform rectf { get; private set; }
	public Item item { get; private set; }
	private Image imgSlot;
	public int itemNum { get; private set; }
	private Text textSlotItem;

	private Text textQuickKey;
	private KeyCode key;

	private void Awake()
	{
		connectSlot = null;

		rectf = GetComponent<RectTransform>();

		item = null;

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
		if(type == InventorySlotType.Quick &&
			key != 0)
		{
			if(Input.GetKeyDown(key))
			{
				useItem();
			}
		}		
	}

	//
	public void init(InventorySlotType t, int keyIndex)
	{
		type = t;
		if(type == InventorySlotType.Quick)
		{
			key = KeyCode.Alpha1 + keyIndex;
			textQuickKey = transform.Find("Quick_Num").GetComponent<Text>();
			textQuickKey.text = (keyIndex + 1).ToString();
		}
	}

	public void clear()
	{
		connectSlot = null;

		item = null;
		setImg(null);
		itemNum = 0;
		textSlotItem.text = itemNum.ToString();
	}

	public void addItem(Item it)
	{
		item = it;
		setImg(it.spriteRenderer.sprite);
		increaseNum(1);
	}

	public void addQuickSlot(Inventory_Slot slot)
	{
		foreach(Inventory_Slot s in Inventory.instance.list_quickSlot)
		{
			if (s.item == null)
				continue;

			if(slot.item.type == s.item.type)
			{
				s.clear();
				break;
			}
		}
		connectSlot = slot;
		slot.connectSlot = this;

		item = connectSlot.item;
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
		Inventory_Slot s = connectSlot;
		connectSlot = slot.connectSlot;
		slot.connectSlot = s;

		Item it = item;
		item = slot.item;
		slot.item = it;

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
		if(imgSlot.sprite == null)
		{
			imgSlot.color = IMacro.color_NoneAlpha;
		}
		else
		{
			imgSlot.color = IMacro.color_White;
		}
	}


	/////
	///
	public bool contain(Vector2 pos)
	{
		pos = rectf.InverseTransformPoint(pos);
		if (rectf.rect.Contains(pos))
		{
			return true;
		}
		else
			return false;
	}


#if false
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
#endif
}
