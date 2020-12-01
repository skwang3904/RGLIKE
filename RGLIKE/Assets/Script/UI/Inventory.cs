using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	public static Inventory instance;

	private RectTransform rectf;
	private Image img;
	private Text text;

	private bool dragInven;
	private Vector2 prevMousePos;
	private Vector2 invenLimit;
	private RectTransform maskRectf;
	private Scrollbar invenScrollbar;

	private List<Item> list_Item;
	public Sprite[] imgItems;
	//------------------------------------------------------------
	// inven slot

	private LinkedList<Inventory_Slot> list_invenSlot;
	private const int slotNum = 20;
	private Vector2[] slotPos;

	private LinkedList<Inventory_Slot> list_quickSlot;
	private const int quickSlotNum = 4;
	public bool inventoryOpen { get; private set; }
	private float invenOpenDt, _invenOpenDt;

	//------------------------------------------------------------
	// mouse click
	private struct InvenClickItem
	{
		public Inventory_Slot clickSlot;
		public float clickDt;
		public const float _clickDt = 0.5f;
		public Image clickImg;

		public void init()
		{
/*			if(clickSlot != null)
				clickSlot.itemImg.color = IMacro.color_NoneAlpha;*/
			clickSlot = null;
			clickDt = 0;
			clickImg.sprite = null;
			clickImg.color = IMacro.color_NoneAlpha;
		}
	}
	private InvenClickItem mci;


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
		invenLimit = Vector2.zero;
		maskRectf = transform.Find("Inven_Mask").GetComponent<RectTransform>();
		invenScrollbar = transform.Find("Scrollbar").GetComponent<Scrollbar>();
		invenScrollbar.onValueChanged.AddListener(invenActiveScroll);

		list_Item = new List<Item>();
		imgItems = new Sprite[(int)IMacro.Item_Type.Max];
		for (i = 0; i < imgItems.Length; i++) 
		{
			imgItems[i] = Resources.Load("Sprite/Item/" 
				+ IMacro.ItemName[i]) as Sprite;
		}

		//------------------------------------------------------------
		// inven slot

		list_invenSlot = new LinkedList<Inventory_Slot>();
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
			list_invenSlot.AddLast(slot);
		}

		inventoryOpen = true;
		invenOpenDt = 0;
		_invenOpenDt = 0.2f;
		inventoryOpenClose();

		//------------------------------------------------------------
		// quick slot
		list_quickSlot = new LinkedList<Inventory_Slot>();
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
			list_quickSlot.AddLast(slot);
		}

		//------------------------------------------------------------
		// mouse click

		mci = new InvenClickItem();
		mci.clickDt = 0;
		mci.clickSlot = null;
		GameObject g = new GameObject();
		g.transform.position = new Vector3(-100, -100, -200); // 마우스 클릭한 아이템 포지션
		g.transform.SetParent(UIManager.instance.canvasUI.transform);
		g.name = "InvectoryClickItem";
		mci.clickImg = g.AddComponent<Image>();
		mci.clickImg.color = IMacro.color_NoneAlpha;
		Vector2 vZero = Vector2.zero;
		mci.clickImg.rectTransform.anchorMin = vZero;
		mci.clickImg.rectTransform.anchorMax = vZero;

		transform.SetParent(UIManager.instance.canvasUI.transform);
		transform.SetAsLastSibling();
		quick_go.transform.SetAsLastSibling();
		g.transform.SetAsLastSibling();
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
		invenScroll();
	}

	//--------------------------------------------------------------------------------

	public void invenActiveScroll(float val)
	{
		float n = maskRectf.sizeDelta.y - 120; // #issue 임의값
		invenLimit.y =  n * val;
		if (invenLimit.y < 0)
		{
			invenLimit.y = 0;
		}
		else if (invenLimit.y >= n)
		{
			invenLimit.y = n;
		}

		int nn = 0;
		foreach (Inventory_Slot slot in list_invenSlot)
		{
			slot.GetComponent<RectTransform>().anchoredPosition = 
				slotPos[nn++] + invenLimit;
		}
	}

	public void inventoryOpenClose()
	{
		inventoryOpen = !inventoryOpen;
		if (invenOpenDt == _invenOpenDt)
			invenOpenDt = 0;
		else
			invenOpenDt = _invenOpenDt - invenOpenDt;
		StartCoroutine("invenOpen");
	}

	private IEnumerator invenOpen()
	{
		while(true)
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
				break;
			
			yield return null;
		}
	}

	//--------------------------------------------------------------------------------

	private void invenItemClick()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 contain;
			// 인벤슬롯 검사
			foreach (Inventory_Slot slot in list_invenSlot)
			{
				if (slot.item == null)
					continue;
				contain = slot.rectf.InverseTransformPoint(Input.mousePosition);
				if (slot.rectf.rect.Contains(contain))
				{
					mci.clickSlot = slot;
					break;
				}
			}

			// 퀵슬롯 검사
			foreach (Inventory_Slot slot in list_quickSlot)
			{
				if (slot.item == null)
					continue;
				contain = slot.rectf.InverseTransformPoint(Input.mousePosition);
				if (slot.rectf.rect.Contains(contain))
				{
					mci.clickSlot = slot;
					break;
				}
			}
		}
		else if (Input.GetMouseButton(0))
		{
			if (mci.clickSlot != null)
			{
				mci.clickDt += Time.deltaTime;
				if (mci.clickDt > InvenClickItem._clickDt)
				{
					if (mci.clickImg.sprite == null)
					{
						mci.clickImg.sprite = mci.clickSlot.itemImg.sprite;
						mci.clickImg.color = IMacro.color_White;
						mci.clickSlot.itemImg.color = IMacro.color_White * 0.5f;
					}

					Vector2 view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
					Vector2 mv = view
						* UIManager.instance.canvasScaler.referenceResolution;

					mci.clickImg.rectTransform.anchoredPosition = mv;
				}
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (mci.clickSlot != null)
			{
				if (mci.clickDt < InvenClickItem._clickDt)
				{
					mci.clickSlot.useItem();
				}
				else
				{
					// 인벤슬롯 검사
					if (mci.clickSlot.type == InventorySlotType.Nomal)
					{
						foreach (Inventory_Slot slot in list_invenSlot)
						{
							Vector2 contain = slot.rectf.InverseTransformPoint(Input.mousePosition);
							if (slot.rectf.rect.Contains(contain))
							{
								slot.changeSlot(mci.clickSlot);
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
							if (mci.clickSlot.type == InventorySlotType.Nomal)
							{
								foreach (Inventory_Slot s in list_quickSlot)
								{
									if (slot == s)
										continue;

									if (s.item == mci.clickSlot.item)
									{
										s.removeItemQuickSlot();
										break;
									}
								}

								slot.addItemQuickSlot(mci.clickSlot);
							}
							else if (mci.clickSlot.type == InventorySlotType.Quick)
								slot.changeSlot(mci.clickSlot);

							check = false;
							break;
						}
					}

					if (check)
					{
						foreach (Inventory_Slot slot in list_quickSlot)
						{
							if (slot == mci.clickSlot)
							{
								slot.removeItemQuickSlot();
								break;
							}
						}
					}
				}
			}

			mci.init();
		}
	}

	private void invenScroll()
	{
		if (mci.clickSlot != null)
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
				invenLimit.y += (Input.mousePosition.y - prevMousePos.y) / UIManager.instance.canvasUI.transform.localScale.y;
				float n = maskRectf.sizeDelta.y - 120; // #issue 임의값
				if (invenLimit.y < 0)
				{
					invenLimit.y = 0;
				}
				else if (invenLimit.y >= n)
				{
					invenLimit.y = n;
				}

				int nn = 0;
				foreach (Inventory_Slot slot in list_invenSlot)
				{
					slot.GetComponent<RectTransform>().anchoredPosition = slotPos[nn++] + invenLimit;
				}
				prevMousePos = Input.mousePosition;
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			dragInven = false;
		}
	}

	//

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

	public void invenUpdate()
	{

	}
}
