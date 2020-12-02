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


		imgClick = transform.Find("imgClick").GetComponent<Image>();
	}

	private void Update()
	{
		pos = Input.mousePosition;
		pos.z = -100;
		transform.position = pos;

		//Cursor.visible = false;

		if (Input.GetMouseButton(0))
		{
			img.sprite = spritesMouse[1];
		}
		else if (Input.GetMouseButtonUp(0))
		{
			img.sprite = spritesMouse[0];
		}
	}

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


}
