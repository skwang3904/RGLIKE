using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager instance = null;
	private GameManager g;
	private Player player;

	private Canvas canvasUI;
	private Text text_PlayTime;
	private Text text_PlayerHp;
	private Text text_Gold;

	private RectTransform rect_MiniMapBG;
	private bool minimapLargeSize;
	private Vector2 size_minimap;
	private Vector2 anchored_minimap;
	private Texture2D[] texMinimap;
	private Texture2D[] colorMinimap;

	private struct QuickSlotData
	{
		public Sprite sp;
		public int num;

		public void changeQuickSlot(QuickSlotData qsd)
		{
			QuickSlotData tmp = this;
			this = qsd;
			qsd = tmp;
		}
	}
	private RectTransform rect_QuickSlot;
	private const int quickSlotNum = 4;
	private Item[] quickItem;
	private Button btn_Menu;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != null && instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		//

		canvasUI = GameObject.Find("UI Manager").
			transform.Find("Canvas_UI").GetComponent<Canvas>();
		text_PlayTime = canvasUI.transform.Find("PlayTime").GetComponent<Text>();
		text_PlayerHp = canvasUI.transform.Find("PlayerHp").GetComponent<Text>();
		text_Gold = canvasUI.transform.Find("Gold").GetComponent<Text>();

		//minimap
		rect_MiniMapBG = canvasUI.transform.Find("MinimapBG").GetComponent<RectTransform>();
		size_minimap = rect_MiniMapBG.sizeDelta;
		anchored_minimap = rect_MiniMapBG.anchoredPosition;

		const int minimapNum = 5;
		texMinimap = new Texture2D[minimapNum];
		colorMinimap = new Texture2D[minimapNum];
		Color[] color = new Color[minimapNum] 
		{ 
			Color.black,
			Color.white, // nomal
			Color.green, // player
			Color.red, // boss
			Color.blue, // shop
		};

		string[] strPath = {
			"",
			"",
			"Sprite/Player/WarriorGirl/Front - Idle/Front - Idle_000",
			"Sprite/UI/skull_north", 
			"Sprite/UI/chest_3.1",
		};

		Sprite[] sp = new Sprite[minimapNum];
		for (int i = 0; i < minimapNum; i++)
		{
			sp[i] = Resources.Load(strPath[i], typeof(Sprite)) as Sprite;
			if(sp[i] == null)
				texMinimap[i] = null;
			else
				texMinimap[i] = sp[i].texture;
			colorMinimap[i] = createTextureFromColor(color[i]);
		}

		//
		rect_QuickSlot = canvasUI.transform.Find("QuickSlotBase").GetComponent<RectTransform>();
		quickItem = new Item[quickSlotNum];

		btn_Menu = canvasUI.transform.Find("Menu").GetComponent<Button>();
	}

	private void Start()
	{
		player = GameManager.instance.player;
		g = GameManager.instance;
	}

	private void Update()
	{
		text_PlayTime.text = "Play Time [ "
			+ (int)(g.totalPlayTime / 60) + " : "
			+ (int)(g.totalPlayTime % 60) + " ]";

		text_PlayerHp.text = "HP : "
			+ player.hp + " / " + player._hp;

		text_Gold.text = "Gold : " + g.gold;
	}

	private void OnGUI()
	{
		drawMinimap();
	}

	public void miniMapSizing()
	{
		minimapLargeSize = !minimapLargeSize;
		if(minimapLargeSize)
		{
			float width = Screen.width;
			float height = Screen.height;
			rect_MiniMapBG.sizeDelta = new Vector2(width, height);
			rect_MiniMapBG.anchoredPosition = new Vector2(-width / 2, -height / 2);
			Time.timeScale = 0f;
		}
		else
		{
			rect_MiniMapBG.sizeDelta = size_minimap;
			rect_MiniMapBG.anchoredPosition = anchored_minimap;
			Time.timeScale = 1f;
		}
	}

	private void drawMinimap()
	{
		int i;
		int total = LevelData.instance.MAP_TOTAL_NUM;
		int sqrt = LevelData.instance.MAP_TOTAL_SQRT;
		Map[] maps = GameManager.instance.maps;


		float rx = canvasUI.transform.localScale.x; // (화면크기 / 캔버스크기)
		float ry = canvasUI.transform.localScale.y;
		float x = Screen.width - (rect_MiniMapBG.sizeDelta.x - rect_MiniMapBG.anchoredPosition.x) * rx;
		float y = (rect_MiniMapBG.sizeDelta.y - rect_MiniMapBG.anchoredPosition.y) * ry;

		float width = rect_MiniMapBG.sizeDelta.x / sqrt * rx;
		float height = rect_MiniMapBG.sizeDelta.y / sqrt * ry;
		float w = width / 10;
		float h = height / 10;
		Rect rt = Rect.zero;
		int n = 0;
		for (i = 0; i < total; i++)
		{
			Map m = maps[i];
			if (m != null)
			{
				rt.Set(x + width * (i % sqrt) - w / 2, y - height - height * (i / sqrt) - h / 2,
					width + w, height + h);


				drawQuad(rt, colorMinimap[0]);

				// texMinimap = 0:bg, 1:nomal, 2:player, 3:boss, 4:shop
				if (i == player.mapNumber)			n = 2;
				else if (m.state == MapState.nomal)	n = 1;
				else if (m.state == MapState.boss)	n = 3;
				else if (m.state == MapState.shop)	n = 4;

				rt.Set(rt.x + w, rt.y + h, rt.width - w * 2, rt.height - h * 2);
				drawQuad(rt, colorMinimap[n]);
				drawQuad(rt, texMinimap[n]);
			}
		}
	}

	private void drawQuad(Rect rt, Texture2D tex)
	{
		if (tex == null)
			return;

		GUI.skin.box.normal.background = tex;
		GUI.Box(rt, GUIContent.none);
	}

	private Texture2D createTextureFromColor(Color color)
	{
		Texture2D tex = new Texture2D(1,1);
		tex.SetPixel(0, 0, color);
		tex.Apply();

		return tex;
	}
}
