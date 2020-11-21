using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
	public static QuickSlot instance;

	private Canvas canvasUI;
	private CanvasScaler canvasScaler;

	//------------------------------------------------------------
	// quick slot

	private struct QuickSlotInfo
	{
		public KeyCode key;
		public int index;
		public RectTransform rt;
		public Item item;
		public Image img;
		public Button btn;
		public int num;
		public Text text;
	}
	private QuickSlotInfo[] quickSlotInfo;
	private const int quickSlotNum = 4;
	private Vector2[] viewPosition;
	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != null && instance != this)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);


		canvasUI = GameObject.Find("UI Manager").
			transform.Find("Canvas_UI").GetComponent<Canvas>();
		canvasScaler = canvasUI.GetComponent<CanvasScaler>();

		int i;
		GameObject[] go = new GameObject[quickSlotNum];
		GameObject quickSlotBase = GameObject.Find("QuickSlots");
		quickSlotInfo = new QuickSlotInfo[quickSlotNum];
		viewPosition = new Vector2[quickSlotNum];
		for (i = 0; i < quickSlotNum; i++)
		{
			go[i] = Instantiate(Resources.Load("Prefabs/UI/QuickSlot")) as GameObject;
			go[i].transform.SetParent(quickSlotBase.transform);
			RectTransform rtt = go[i].GetComponent<RectTransform>();
			rtt.anchoredPosition =  new Vector2(200 * i - 300, 0);
			viewPosition[i] = Camera.main.ScreenToViewportPoint(rtt.anchoredPosition);
			ref QuickSlotInfo qsi = ref quickSlotInfo[i];
		}
	}
}
