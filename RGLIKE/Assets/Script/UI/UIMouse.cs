using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMouse : MonoBehaviour
{
	public static UIMouse instance;

	private Sprite[] sprites;
	private Image img;
	private Vector3 pos;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != null && instance != this)
			Destroy(gameObject);

		Object[] obj = Resources.LoadAll("Sprite/UI/mouseCursor");
		sprites = new Sprite[2];
		sprites[0] = obj[9] as Sprite;
		sprites[1] = obj[14] as Sprite;

		img = GetComponent<Image>();
		img.sprite = sprites[0];
	}

	private void Update()
	{
		pos = Input.mousePosition;
		pos.z = -100;
		transform.position = pos;

		//Cursor.visible = false;

		if (Input.GetMouseButton(0))
		{
			img.sprite = sprites[1];
		}
		else if (Input.GetMouseButtonUp(0))
		{
			img.sprite = sprites[0];
		}
	}
}
