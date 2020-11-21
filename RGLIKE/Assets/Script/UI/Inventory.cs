using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	public static Inventory instance;
	public bool inventoryOpen { get; private set; }

	private Canvas canvasUI;

	private Vector2 slotBasePosition = new Vector2(150, 200);
	private GameObject[] invenSlot;
	private RectTransform[] invenSlotRectTransform;
	private const int slotNum = 10;

	private struct SlotInfo
	{
		public int index;
		public RectTransform rt;
		public Item item;
		public Image img;
		public Button btn;
		public int num;
		public Text text;

		public void changeSlotInfo(ref SlotInfo si)
		{
			//자리만 바꿈
			int tmp = index;
			index = si.index;
			si.index = tmp;

			Vector2 vtmp = rt.anchoredPosition;
			rt.anchoredPosition = si.rt.anchoredPosition;
			si.rt.anchoredPosition = vtmp;
		}
	}
	private SlotInfo[] slotInfo; // 정리되면 리스트로 만들기

	private struct MouseClickItem
	{
		public float mouseClickDt, _mouseClickDt;
		public Vector2 mouseRectPosition;
		public Vector2 mousePosition;
		public int clickNum;
		public Image img;

		public void init()
		{
			mouseClickDt = 0;
			//_mouseClickDt;
			mouseRectPosition.Set(0, 0);
			mousePosition.Set(0, 0);
			clickNum = -1;
			img.sprite = null;
			img.color = IMacro.color_NoneAlpha;
		}
	}
	private MouseClickItem mci;


	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != null && instance != this)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		canvasUI = GameObject.Find("UI Manager").
			transform.Find("Canvas_UI").GetComponent<Canvas>();

		//
		invenSlot = new GameObject[slotNum];
		invenSlotRectTransform = new RectTransform[slotNum];
		slotInfo = new SlotInfo[slotNum];
		Vector2 v = Vector2.zero;
		for (int i = 0; i < slotNum; i++)
		{
			v.Set(150 * (i % 5), 150 - 150 * (i / 5));
			invenSlot[i] = Instantiate(Resources.Load("Prefabs/UI/Inventory_Slot"),
				(Vector2)transform.position + slotBasePosition + v,
				Quaternion.identity) as GameObject;
			invenSlot[i].transform.SetParent(transform);
			invenSlotRectTransform[i] = invenSlot[i].GetComponent<RectTransform>();
			ref SlotInfo si = ref slotInfo[i];
			si.index = i;
			si.rt = invenSlot[i].GetComponent<RectTransform>();

			si.item = null;
			si.img = invenSlot[i].transform.Find("Item_Image").GetComponent<Image>();
			si.img.sprite = null;

			si.btn = invenSlot[i].transform.Find("Item_Image").GetComponent<Button>();
			si.btn.onClick.RemoveAllListeners();

			si.num = 0;
			si.text = invenSlot[i].transform.Find("Item_Num").GetComponent<Text>();
			si.text.text = si.num.ToString();
		}


		mci = new MouseClickItem();
		mci.mouseClickDt = 0;
		mci._mouseClickDt = 0.5f;
		mci.mousePosition = Vector2.zero;
		mci.mouseRectPosition = Vector2.zero;
		mci.clickNum = -1;
		GameObject g = new GameObject();
		g.transform.position = new Vector3(-100, -100, -200); // 마우스 클릭한 아이템 포지션
		g.transform.SetParent(canvasUI.transform);
		g.name = "Invectory Click Item";
		mci.img = g.AddComponent<Image>();
		mci.img.color = IMacro.color_NoneAlpha;
	}

	private void Update()
	{
#if true //test
		inventoryOpen = true;
		if(Input.GetKeyDown(KeyCode.G))
		{
			addItem(Item.items[0, 0]);
			addItem(Item.items[1, 0]);
		}

#endif
		if (!inventoryOpen)
			return;

		mci.mousePosition = Input.mousePosition;
		if (Input.GetMouseButtonDown(0))
		{
			for (int i = 0; i < slotNum; i++)
			{
				mci.mouseRectPosition = slotInfo[i].rt.InverseTransformPoint(mci.mousePosition);
				if (slotInfo[i].rt.rect.Contains(mci.mouseRectPosition))
				{
					mci.clickNum = i;
					print(slotInfo[i].index);
					break;
				}
			}
		}
		else if (Input.GetMouseButton(0))
		{
			if (mci.clickNum != -1)
			{
				mci.mouseClickDt += Time.deltaTime;
				if(mci.mouseClickDt > mci._mouseClickDt)
				{
					if (mci.img.sprite == null)
					{
						mci.img.sprite = slotInfo[mci.clickNum].img.sprite;
						mci.img.color = IMacro.color_White;
						//slotInfo[mci.clickNum].img.sprite = null;
					}
					mci.img.rectTransform.anchoredPosition =
						(mci.mousePosition - new Vector2(Screen.width, Screen.height) / 2)
						/ canvasUI.transform.localScale;
						
				}
			}
		}
		else //if(Input.GetMouseButtonUp(0))
		{
			if (mci.clickNum != -1)
			{
				if (mci.mouseClickDt < mci._mouseClickDt)
				{
					useInvenItem(mci.clickNum);
				}
				else
				{
					for (int i = 0; i < slotNum; i++)
					{
						if (i == mci.clickNum)
							continue;

						mci.mouseRectPosition = slotInfo[i].rt.InverseTransformPoint(mci.mousePosition);
						if (slotInfo[i].rt.rect.Contains(mci.mouseRectPosition))
						{
							changInvenItem(mci.clickNum, i);
							break;
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
	}

	public void addItem(Item item)
	{
		// 습득시 인벤토리에 추가
		int i;
		for (i = 0; i < slotNum; i++) 
		{
			ref SlotInfo si = ref slotInfo[i];
			if (si.item == null)
				continue;

			if(si.item.strName == item.strName)
			{
				addItemNum(i);
				return;
			}
		}

		for (i = 0; i < slotNum; i++)
		{
			ref SlotInfo si = ref slotInfo[i];
			if (si.item == null)
			{
				si.item = item;
				si.img.sprite = Resources.Load("Sprite/Item/" + item.strName, typeof(Sprite)) as Sprite;
				si.img.color = IMacro.color_White;
				//si.btn.onClick.AddListener(() => useInvenItem(i));
				addItemNum(i);
				break;
			}
		}
	}

	private void addItemNum(int i)
	{
		// 인벤토리에 추가할때 기존에 있는 슬롯에 갯수만 추가
		ref SlotInfo si = ref slotInfo[i];
		si.num++;
		si.text.text = si.num.ToString();
	}

	private void useInvenItem(int i)
	{
		// 클릭으로 아이템 사용 or 단축키 사용
		ref SlotInfo si = ref slotInfo[i];
		if (si.item == null)
			return;

		si.item.onUse();
		si.num--;
		si.text.text = si.num.ToString();

		if(si.num <= 0)
		{
			si.item = null;
			si.img.sprite = null;
			si.img.color = IMacro.color_NoneAlpha;
			si.btn.onClick.RemoveAllListeners();
			si.num = 0;
			si.text.text = si.num.ToString();
		}
	}

	private void changInvenItem(int currIndex, int changeIndex)
	{
		ref SlotInfo curr = ref slotInfo[currIndex];
		ref SlotInfo change = ref slotInfo[changeIndex];

		curr.changeSlotInfo(ref change);

		mci.init();
	}
}
