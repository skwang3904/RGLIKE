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

	private bool moveInven;
	private bool dragInven;
	private Vector2 prevMousePos;
	private Vector2 invenPosLimit;
	private RectTransform maskRectf;
	private Scrollbar invenScrollbar;

	private Dropdown invenSortMenu;

	//------------------------------------------------------------
	// inven slot

	public List<Inventory_Slot> list_invenSlot { get; private set; }
	private const int slotNum = 20;
	private Vector2[] slotPos;

	public List<Inventory_Slot> list_quickSlot { get; private set; }
	private const int quickSlotNum = 4;
	public bool inventoryOpen { get; private set; }
	private float invenOpenDt, _invenOpenDt;

	//------------------------------------------------------------
	// mouse click

	public float clickDt;
	public const float _clickDt = 0.1f;

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


		//------------------------------------------------------------
		// dropdown
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
			slot.init(InventorySlotType.Nomal, 0);
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
			slot.init(InventorySlotType.Quick, i);
			list_quickSlot.Add(slot);
		}

		quick_go.transform.SetAsFirstSibling();
		transform.SetParent(UIManager.instance.canvasUI.transform);
		transform.SetAsFirstSibling();
	}

	private void Update()
	{
#if true // inven sort test
		if (Input.GetKeyDown(KeyCode.G))
		{
			print(Item.itemList.Count);
			foreach(Item item in Item.itemList)
			{
				addItemInventory(item);
			}

		}
#endif

		if (!inventoryOpen)
			return;

		invenItemClick();
		invenScrollMouse();

		if(UIMouse.instance.invenSlotClick == null)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (rectf.rect.Contains(rectf.InverseTransformPoint(Input.mousePosition)))
				{
					moveInven = true;
					prevMousePos = Input.mousePosition;
				}
			}
			else if(Input.GetMouseButton(0))
			{
				if(moveInven)
				{
					rectf.position = (Vector2)(rectf.position + Input.mousePosition) - prevMousePos;
					prevMousePos = Input.mousePosition;
				}
			}
			else if(Input.GetMouseButtonUp(0))
			{
				moveInven = false;
			}

		}
	}

#if true // inven sort test
	private void OnGUI()
	{
		if (!inventoryOpen)
			return;

		for (int i = 0; i < list_invenSlot.Count; i++) 
		{
			if (list_invenSlot[i].item == null)
				continue;


			Vector2 pos = rectf.anchoredPosition 
				+ list_invenSlot[i].rectf.anchoredPosition;
			Vector2 size = list_invenSlot[i].rectf.sizeDelta;
			pos *= UIManager.instance.canvasUI.transform.localScale;
			size *= UIManager.instance.canvasUI.transform.localScale;
			pos.x += Screen.width / 9;
			pos.y = Screen.height * 0.8f - pos.y;
			Rect rt = new Rect(pos,	size);

			GUI.Label(rt, list_invenSlot[i].item.strName);

		}
	}
#endif

	//--------------------------------------------------------------------------------

	public void invenActiveScrollbar(float val)
	{
		float n = slotNum / 5 * (list_invenSlot[0].rectf.sizeDelta.y + 50)
			- maskRectf.sizeDelta.y + 50;  // 50 = 간격
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

				float n = slotNum / 5 * (list_invenSlot[0].rectf.sizeDelta.y + 50)
							- maskRectf.sizeDelta.y + 50; // 50 = 간격

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

			float n = maskRectf.sizeDelta.y - 120; // #issue 임의값
			invenScrollbar.value = invenPosLimit.y / n;
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
			Inventory_Slot slot;
			// 인벤슬롯 검사
			int n = list_invenSlot.Count;
			for (i = 0; i < n; i++) 
			{
				slot = list_invenSlot[i];
				if (slot.item == null)
					continue;
				
				if (slot.contain(Input.mousePosition))
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
				slot = list_quickSlot[i];
				if (slot.item == null)
					continue;

				if (slot.contain(Input.mousePosition))
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
						if(UIMouse.instance.invenSlotClick.item != null)
						{
							UIMouse.instance.imgClick.sprite = 
							UIMouse.instance.invenSlotClick.item.spriteRenderer.sprite;

							UIMouse.instance.imgClick.color = IMacro.color_White;
							UIMouse.instance.invenSlotClick.item.spriteRenderer.color = IMacro.color_White * 0.5f;
						}
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
					//UIMouse.instance.invenSlotClick.useItem();
				}
				else
				{
					bool check = true;
					if (UIMouse.instance.invenSlotClick.slotType == InventorySlotType.Nomal)
					{
						// 인벤슬롯 검사
						foreach (Inventory_Slot slot in list_invenSlot)
						{
							if (!maskRectf.rect.Contains(slot.rectf.anchoredPosition))
								continue;

							if (slot.contain(Input.mousePosition))
							{
								slot.change(UIMouse.instance.invenSlotClick);
								check = false;
								break;
							}
						}

						if(check)
						{
							foreach (Inventory_Slot slot in list_quickSlot)
							{
								if (slot.contain(Input.mousePosition))
								{
									slot.addQuickSlot(UIMouse.instance.invenSlotClick);
									break;
								}
							}
						}

					}
					else if (UIMouse.instance.invenSlotClick.slotType == InventorySlotType.Quick)
					{
						// 퀵슬롯 검사
						foreach (Inventory_Slot slot in list_quickSlot)
						{
							if (slot.contain(Input.mousePosition))
							{
								slot.change(UIMouse.instance.invenSlotClick);
								check = false;
								break;
							}
						}
						
						if(check)
						{
							UIMouse.instance.invenSlotClick.clear();
						}
					}
				}
			}

			clickDt = 0;
			UIMouse.instance.declickInvenItem();
		}

		if (Input.GetMouseButtonDown(1))
		{
			foreach (Inventory_Slot slot in list_invenSlot)
			{
				if (!maskRectf.rect.Contains(slot.rectf.anchoredPosition))
					continue;

				if (slot.contain(Input.mousePosition))
				{
					UIMouse.instance.invenSlotClick = slot;
					break;
				}
			}

			if (UIMouse.instance.invenSlotClick == null)
			{
				foreach (Inventory_Slot slot in list_quickSlot)
				{
					if (slot.contain(Input.mousePosition))
					{
						UIMouse.instance.invenSlotClick = slot;
						break;
					}
				}
			}
		}
		else if (Input.GetMouseButtonUp(1))
		{
			if (UIMouse.instance.invenSlotClick != null)
				UIMouse.instance.invenSlotClick.useItem();
		}
	}

	public void addItemInventory(Item it)
	{
#if false // test
		Item item = Instantiate(it);
#else

#endif
		foreach (Inventory_Slot slot in list_invenSlot)
		{
			//#issue 추후 아이템 종류 추가시 수정
			if (slot.item == null)
				continue;

			if (slot.item.type == it.type)
			{
				slot.increaseNum(1);

				slot.connectSlot?.increaseNum(1);
				return;
			}
		}


		//Item item = Instantiate(it);
		//item.type = it.type;
		//item.strName = it.strName;

		foreach (Inventory_Slot slot in list_invenSlot)
		{
			if (slot.item == null)
			{
				slot.addItem(it);
				break;
			}
		}
	}


	//---------------------------------------------------------------
	// Sort Method
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
					list_invenSlot.Sort(invenListItemSortName);
					break;
				}
			case 2: // amount
				{
					list_invenSlot.Sort(invenListItemSortAmount);
					break;
				}
		}

		// sort 후 아이템 사라짐 -> 인벤 클릭하거나 아래 코드 한번 실행하면 보임
		{
			int nn = list_invenSlot.Count;
			for (int i = 0; i < nn; i++)
			{
				list_invenSlot[i].GetComponent<RectTransform>().anchoredPosition =
					slotPos[i] + invenPosLimit;
			}
		}
	}

	private int invenListItemSortName(Inventory_Slot A, Inventory_Slot B)
	{
		if (A.item == null && B.item == null)
			return 0;
		else if (A.item == null)
			return 1;
		else if (B.item == null)
			return -1;

		int a = A.item.strName[0];
		int b = B.item.strName[0];

		if (a < 65 || a > 122) print("sort error item name out of range [a]");
		if (b < 65 || b > 122) print("sort error item name out of range [b]");

		if (a < 91)	a += 32;
		if (b < 91)	b += 32;

		if (a > b)	
			return 1;
		else if (a < b) 
			return -1;
		else 
			return 0;
	}

	private int invenListItemSortAmount(Inventory_Slot A, Inventory_Slot B)
	{
		if (A.item == null && B.item == null)
			return 0;
		else if (A.item == null)
			return 1;
		else if (B.item == null)
			return -1;


		if (A.itemNum > B.itemNum) 
			return 1;
		else if (A.itemNum < B.itemNum) 
			return -1;
		else
			return 0;
	}
}
