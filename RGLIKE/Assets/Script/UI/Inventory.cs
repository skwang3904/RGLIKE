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

	//------------------------------------------------------------
	// inven slot

	private LinkedList<Inventory_Slot> list_invenSlot;
	private const int slotNum = 10;
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

		//------------------------------------------------------------
		// inven slot
		list_invenSlot = new LinkedList<Inventory_Slot>();
		Inventory_Slot slot;

		Vector2 slotBasePosition = new Vector2(140, 250);
		Vector2 slotDis = new Vector2(140 + 50, 140 + 50);  // slot size + distance

		GameObject slots_go;
		Vector2 v = Vector2.zero;
		for (i = 0; i < slotNum; i++)
		{
			v.Set(slotDis.x * (i % 5),
				slotDis.y - slotDis.y * (i / 5));
			slots_go = Instantiate(Resources.Load("Prefabs/UI/Inventory_Slot"),
				(Vector2)transform.position + slotBasePosition + v,
				Quaternion.identity) as GameObject;
			slots_go.transform.SetParent(transform);

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
			addItem(Item.items[0][0]);
			addItem(Item.items[1][0]);
		}

#endif
		if (!inventoryOpen)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			// 인벤슬롯 검사
			foreach (Inventory_Slot slot in list_invenSlot)
			{
				if (slot.item == null)
					continue;
				Vector2 contain = slot.rectf.InverseTransformPoint(Input.mousePosition);
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
				Vector2 contain = slot.rectf.InverseTransformPoint(Input.mousePosition);
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
				if(mci.clickDt > InvenClickItem._clickDt)
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
		else if(Input.GetMouseButtonUp(0))
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

									if(s.item == mci.clickSlot.item)
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

					if(check)
					{
						foreach(Inventory_Slot slot in list_quickSlot)
						{
							if(slot == mci.clickSlot)
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

	//--------------------------------------------------------------------------------

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

	public void addItem(Item item)
	{
		// 습득시 인벤토리에 추가
		foreach(Inventory_Slot slot in list_invenSlot)
		{
			if (slot.item == null)
				continue;

			if (slot.item.strName == item.strName)
			{
				slot.num++;
				slot.itemNum.text = slot.num.ToString();
				return;
			}
		}

		foreach (Inventory_Slot slot in list_invenSlot)
		{
			if (slot.item == null)
			{
				slot.item = item;
				slot.itemImg.sprite = Resources.Load("Sprite/Item/" + item.strName, typeof(Sprite)) as Sprite;
				slot.itemImg.color = IMacro.color_White;
				slot.num++;
				slot.itemNum.text = slot.num.ToString();
				break;
			}
		}
	}
}
