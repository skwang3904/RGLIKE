using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMouse : MonoBehaviour
{
	public static UIMouse instance;

	private Sprite[] spritesMouse;
	private Image img;
	private Vector3 pos;

	public Inventory_Slot invenSlotClick;
	public bool isClick;
	public Image imgClick;

	// item info
	private Inventory_Slot selectSlot;
	private Vector2 posInfo;
	private GameObject goInfo;
	private Image imgInfo;
	private Text textInfoName;
	private Text textInfo;
	private Text textInfoUse;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != null && instance != this)
			Destroy(gameObject);

		Object[] obj = Resources.LoadAll("Sprite/UI/mouseCursor");
		spritesMouse = new Sprite[2];
		spritesMouse[0] = obj[9] as Sprite;
		spritesMouse[1] = obj[14] as Sprite;

		img = GetComponent<Image>();
		img.sprite = spritesMouse[0];

		invenSlotClick = null;
		isClick = false;
		imgClick = transform.Find("imgClick").GetComponent<Image>();

		posInfo = Vector2.zero;
		goInfo = GameObject.Find("Mouse_UI").transform.Find("Info_Item").gameObject;
		imgInfo =		goInfo.transform.Find("Img").GetComponent<Image>();
		textInfoName =  goInfo.transform.Find("Name").GetComponent<Text>();
		textInfo =		goInfo.transform.Find("Info").GetComponent<Text>();
		textInfoUse =	goInfo.transform.Find("UseEffect").GetComponent<Text>();
	}

	private void Update()
	{
		pos = Input.mousePosition;
		pos.z = -100;
		transform.position = pos;

		Cursor.visible = false;

		if (Input.GetMouseButton(0))
		{
			img.sprite = spritesMouse[1];
		}
		else if (Input.GetMouseButtonUp(0))
		{
			img.sprite = spritesMouse[0];
		}


		if (goInfo.activeSelf)
		{
			if (!selectSlot.contain(Input.mousePosition))
				hideInfo();
		}
		else
		{
			foreach (Inventory_Slot slot in Inventory.instance.list_invenSlot)
			{
				if (slot.item == null)
					continue;
				
				if (slot.contain(Input.mousePosition))
				{
					showInfo(slot);
					break;
				}
			}
		}
	}

	//--------------------------------------------------------
	//--------------------------------------------------------

	public void clickInvenItem(Inventory_Slot slot)
	{
		invenSlotClick = slot;
		//imgClick.sprite = invenSlotClick.itemImg.sprite;
		//imgClick.color = IMacro.color_White;
	}

	public void declickInvenItem()
	{
		invenSlotClick = null;
		imgClick.sprite = null;
		imgClick.color = IMacro.color_NoneAlpha;
	}

	//--------------------------------------------------------
	//--------------------------------------------------------

	private void showInfo(Inventory_Slot slot)
	{
		selectSlot = slot;

		Item it = slot.item;

		RectTransform parentRtf = goInfo.GetComponentInParent<RectTransform>();// Mouse_UI
		RectTransform rtf = goInfo.GetComponent<RectTransform>();
		posInfo = slot.rectf.position;
		posInfo.x += slot.rectf.sizeDelta.x * parentRtf.localScale.x;
		posInfo.y -= (rtf.sizeDelta.y - slot.rectf.sizeDelta.y) * parentRtf.localScale.y;
		posInfo /= parentRtf.localScale;

		goInfo.GetComponent<RectTransform>().anchoredPosition = posInfo;
		goInfo.SetActive(true);
		imgInfo.sprite = it.spriteRenderer.sprite;
		textInfoName.text = it.strName;
		textInfo.text = it.strInfomation;
		textInfoUse.text = it.strUseEffect;
	}

	private void hideInfo()
	{
		goInfo.SetActive(false);
	}
}
