using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	//#issue 인벤 무작정 구현함 - 추후 수정
	public static Inventory instance;

	private RectTransform rectf;
	private Image img;
	private Text text;

	private bool dragInven;
	private Vector2 prevMousePos;
	private Vector2 invenPosLimit;
	private RectTransform maskRectf;
	private Scrollbar invenScrollbar;

	private List<Item> list_Item;
	public List<Sprite> listItemSprite;

	private Dropdown invenSortMenu;

	//------------------------------------------------------------
	// inven slot

	private List<Inventory_Slot> list_invenSlot;
	private const int slotNum = 20;
	private Vector2[] slotPos;

	private List<Inventory_Slot> list_quickSlot;
	private const int quickSlotNum = 4;
	public bool inventoryOpen { get; private set; }
	private float invenOpenDt, _invenOpenDt;

	//------------------------------------------------------------
	// mouse click

	public float clickDt;
	public const float _clickDt = 0.5f;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != null && instance != this)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		int i;
		rectf = GetComponent<RectTransform>();
		img = GetComponent<Image>();
		text = transform.GetComponent<Text>();

		Vector2 invenPos = Camera.main.ViewportToScreenPoint(new Vector2(0.2f,0.2f));
		GetComponent<RectTransform>().anchoredPosition = invenPos;

		dragInven = false;
		prevMousePos = Vector2.zero;
		invenPosLimit = Vector2.zero;
		maskRectf = transform.Find("Inven_Mask").GetComponent<RectTransform>();
		invenScrollbar = transform.Find("Scrollbar").GetComponent<Scrollbar>();
		invenScrollbar.onValueChanged.AddListener(invenActiveScrollbar);

		list_Item = new List<Item>();
		listItemSprite = new List<Sprite>();
		//for (i = 0; i < (int)IMacro.Item_Type.Max; i++) 
		for (i = 0; i < 2; i++)
		{
			listItemSprite.Add(
				GameObject.Find("ItemImage")
				.transform.GetChild(i)
				.GetComponent<SpriteRenderer>().sprite);
		}


		invenSortMenu = transform.Find("Dropdown").GetComponent<Dropdown>();
		invenSortMenu.onValueChanged.AddListener(delegate 
		{ 
			invenDropdown(invenSortMenu); 
		});
		
		//------------------------------------------------------------
		// inven slot

		list_invenSlot = new List<Inventory_Slot>();
		slotPos = new Vector2[slotNum];
		Inventory_Slot slot;

		Vector2 slotBasePosition = new Vector2(0, 0);
		Vector2 slotDis = new Vector2(140 + 50, 140 + 50);  // slot size + distance

		GameObject slots_go;
		GameObject mask_go = transform.Find("Inven_Mask").gameObject;
		Vector2 v = Vector2.zero;
		for (i = 0; i < slotNum; i++)
		{
			v.Set(50 + slotDis.x * (i % 5),
				 maskRectf.sizeDelta.y - slotDis.y - slotDis.y * (i / 5));
	
			slots_go = Instantiate(Resources.Load("Prefabs/UI/Inventory_Slot")) as GameObject;
			slots_go.transform.SetParent(mask_go.transform);
			slots_go.GetComponent<RectTransform>().anchoredPosition = v;
			slotPos[i] = v;

			GameObject qs = slots_go.transform.Find("Quick_Num").gameObject;
			Destroy(qs);
			qs.transform.SetParent(null);

			slot = slots_go.GetComponent<Inventory_Slot>();
			slot.init(InventorySlotType.Nomal, i);
			list_invenSlot.Add(slot);
		}

		inventoryOpen = true;
		invenOpenDt = 0;
		_invenOpenDt = 0.2f;
		inventoryOpenClose();

		//------------------------------------------------------------
		// quick slot

		list_quickSlot = new List<Inventory_Slot>();
		GameObject quick_go = UIManager.instance.canvasUI.transform.Find("QuickSlots").gameObject;
		//slotBasePosition.Set(-(140 * 2 + 50 * 1.5f), 0);
		slotBasePosition.Set(-(140 * 2), 0);
		for (i = 0; i < quickSlotNum; i++) 
		{
			v.Set((slotDis.x - 50) * (i % 4), 0);
			slots_go = Instantiate(Resources.Load("Prefabs/UI/Inventory_Slot"),
				(Vector2)quick_go.transform.position + slotBasePosition + v,
				Quaternion.identity) as GameObject;
			slots_go.transform.SetParent(quick_go.transform);

			slot = slots_go.GetComponent<Inventory_Slot>();
			slot.init(InventorySlotType.Quick,i);
			list_quickSlot.Add(slot);
		}

		transform.SetParent(UIManager.instance.canvasUI.transform);
		transform.SetAsLastSibling();
		quick_go.transform.SetAsLastSibling();
	}

	private void Update()
	{
#if true //test
		//inventoryOpen = true;
		if (Input.GetKeyDown(KeyCode.G))
		{
			addListItem(Item.items[0][0]);
			addListItem(Item.items[1][0]);
		}

#endif
		if (!inventoryOpen)
			return;

		invenItemClick();
		invenScrollMouse();
	}

	//--------------------------------------------------------------------------------

	public void invenActiveScrollbar(float val)
	{
		float n = maskRectf.sizeDelta.y - 120; // #issue 임의값
		invenPosLimit.y =  n * val;
		if (invenPosLimit.y < 0)
		{
			invenPosLimit.y = 0;
		}
		else if (invenPosLimit.y >= n)
		{
			invenPosLimit.y = n;
		}

		int nn = list_invenSlot.Count;
		for (int i = 0; i < nn; i++) 
		{
			list_invenSlot[i].GetComponent<RectTransform>().anchoredPosition =
				slotPos[i] + invenPosLimit;
		}
	}

	private void invenScrollMouse()
	{
		if (UIMouse.instance.invenSlotClick != null)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			Vector2 contain = maskRectf.InverseTransformPoint(Input.mousePosition);
			if (maskRectf.rect.Contains(contain))
			{
				dragInven = true;
				prevMousePos.Set(0, Input.mousePosition.y);
			}
		}
		else if (Input.GetMouseButton(0))
		{
			if (dragInven)
			{
				invenPosLimit.y += (Input.mousePosition.y - prevMousePos.y) / UIManager.instance.canvasUI.transform.localScale.y;
				float n = maskRectf.sizeDelta.y - 120; // #issue 임의값
				if (invenPosLimit.y < 0)
				{
					invenPosLimit.y = 0;
				}
				else if (invenPosLimit.y >= n)
				{
					invenPosLimit.y = n;
				}

				int nn = list_invenSlot.Count;
				for (int i = 0; i < nn; i++)
				{
					list_invenSlot[i].GetComponent<RectTransform>().anchoredPosition =
						slotPos[i] + invenPosLimit;
				}
				prevMousePos = Input.mousePosition;
			}
		}

		else if (Input.GetMouseButtonUp(0))
		{
			dragInven = false;
		}
	}

	public void inventoryOpenClose()
	{
		StartCoroutine("invenOpen");
	}

	private IEnumerator invenOpen()
	{
		inventoryOpen = !inventoryOpen;
		invenOpenDt = _invenOpenDt - invenOpenDt;

		LivingEntity.EntityTime(false);

		while (true)
		{
			invenOpenDt += Time.deltaTime;
			if (invenOpenDt > _invenOpenDt)
				invenOpenDt = _invenOpenDt;

			if (inventoryOpen)
				transform.localScale = Vector2.Lerp(
				Vector2.zero,
				new Vector2(1, 1),
				invenOpenDt / _invenOpenDt);
			else
				transform.localScale = Vector2.Lerp(
					new Vector2(1, 1), 
					Vector2.zero,
					invenOpenDt / _invenOpenDt);

			if (invenOpenDt == _invenOpenDt)
			{
				if (!inventoryOpen)
					LivingEntity.EntityTime(true);
				break;
			}
			
			yield return null;
		}
	}

	//--------------------------------------------------------------------------------

	private void invenItemClick()
	{
		int i;
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 contain;
			// 인벤슬롯 검사
			int n = list_invenSlot.Count;
			for (i = 0; i < n; i++) 
			{
				Inventory_Slot slot = list_invenSlot[i];
				if (slot.item == null)
					continue;
				contain = slot.rectf.InverseTransformPoint(Input.mousePosition);
				if (slot.rectf.rect.Contains(contain))
				{
					//mci.clickSlot = slot;
					UIMouse.instance.clickInvenItem(slot);
					break;
				}
			}

			// 퀵슬롯 검사
			n = list_quickSlot.Count;
			for (i = 0; i < n; i++) 
			{
				Inventory_Slot slot = list_quickSlot[i];
				if (slot.item == null)
					continue;
				contain = slot.rectf.InverseTransformPoint(Input.mousePosition);
				if (slot.rectf.rect.Contains(contain))
				{
					//mci.clickSlot = slot;
					UIMouse.instance.clickInvenItem(slot);

					break;
				}
			}
		}
		else if (Input.GetMouseButton(0))
		{
			if (UIMouse.instance.invenSlotClick != null)
			{
				clickDt += Time.deltaTime;
				if (clickDt > _clickDt)
				{
					if (UIMouse.instance.imgClick.sprite == null)
					{
						UIMouse.instance.imgClick.sprite = UIMouse.instance.invenSlotClick.itemImg.sprite;
						UIMouse.instance.imgClick.color = IMacro.color_White;
						UIMouse.instance.invenSlotClick.itemImg.color = IMacro.color_White * 0.5f;
					}
				}
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (UIMouse.instance.invenSlotClick != null)
			{
				if (clickDt < _clickDt)
				{
					//mci.clickSlot.useItem();
					UIMouse.instance.invenSlotClick.useItem();

				}
				else
				{
					// 인벤슬롯 검사
					if (UIMouse.instance.invenSlotClick.type == InventorySlotType.Nomal)
					{
						foreach (Inventory_Slot slot in list_invenSlot)
						{
							Vector2 contain = slot.rectf.InverseTransformPoint(Input.mousePosition);
							if (slot.rectf.rect.Contains(contain))
							{
								slot.changeSlot(UIMouse.instance.invenSlotClick);
								break;
							}
						}
					}

					// 퀵슬롯 검사
					bool check = true;
					foreach (Inventory_Slot slot in list_quickSlot)
					{
						Vector2 contain = slot.rectf.InverseTransformPoint(Input.mousePosition);
						if (slot.rectf.rect.Contains(contain))
						{
							if (UIMouse.instance.invenSlotClick.type == InventorySlotType.Nomal)
							{
								foreach (Inventory_Slot s in list_quickSlot)
								{
									if (slot == s)
										continue;

									if (s.item == UIMouse.instance.invenSlotClick.item)
									{
										s.removeItemQuickSlot();
										break;
									}
								}

								slot.addItemQuickSlot(UIMouse.instance.invenSlotClick);
							}
							else if (UIMouse.instance.invenSlotClick.type == InventorySlotType.Quick)
								slot.changeSlot(UIMouse.instance.invenSlotClick);

							check = false;
							break;
						}
					}

					if (check)
					{
						foreach (Inventory_Slot slot in list_quickSlot)
						{
							if (slot == UIMouse.instance.invenSlotClick)
							{
								slot.removeItemQuickSlot();
								break;
							}
						}
					}
				}
			}

			clickDt = 0;
			UIMouse.instance.declickInvenItem();
		}
	}

	public void addListItem(Item item)
	{
		foreach (Inventory_Slot slot in list_invenSlot)
		{
			//#issue 추후 아이템 종류 추가시 수정
			if (slot.item == null)
				continue;

			if (slot.item.type == item.type)
			{
				slot.addItemNum(1);
				if(slot.connectSlot)
				{
					slot.connectSlot.addItemNum(1);
				}
				return;
			}
		}

		foreach (Inventory_Slot slot in list_invenSlot)
		{
			if (slot.item == null)
			{
				list_Item.Add(item);
				slot.addItemInvenSlot(item);
				break;
			}
		}
	}

	public void invenDropdown(Dropdown dropdown)
	{
		int val = dropdown.value;
		
		switch (val)
		{
			case 0: // order by
				{
					break;
				}
			case 1: // name 
				{
					//list_Item sort
					list_Item.Sort(invenListItemSortName);

					/*				 */
					for (int i = 0; i < list_Item.Count; i++)
						print(list_Item[i] + "  /  " + i);
					//list_invenSlot reset
					foreach (Inventory_Slot slot in list_invenSlot)
					{
						if (slot.item)
							slot.clear();
					}

					for (int i = 0; i < list_Item.Count; i++) 
						list_invenSlot[i].addItemInvenSlot(list_Item[i]);
					break;
				}
			case 2: // amount
				{
					break;
				}
		}
	}

	private int invenListItemSortName(Item A, Item B)
	{
		int a = A.strName[0];
		int b = B.strName[0];

		if (a < 65 || a > 122) print("sort error item name out of range [a]");
		if (b < 65 || b > 122) print("sort error item name out of range [b]");

		if (a < 91)	a += 32;
		if (b < 91)	b += 32;

		if (a > b)		return 1;
		else if (a < b)	return -1;
		else			return 0;
	}

	private int invenListItemSortAmount(Item A, Item B)
	{
		if (A.strName[0] > B.strName[0])
		{
			return 1;
		}
		else if (A.strName[0] < B.strName[0])
		{
			return -1;
		}
		else
		{
			return 0;
		}
	}
}
