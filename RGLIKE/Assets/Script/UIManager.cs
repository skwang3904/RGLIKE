using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager instance = null;
	private GameManager g;
	private Player player;

	public Canvas canvasUI { get; private set; }
	public CanvasScaler canvasScaler { get; private set; }

	private Text text_PlayTime;
	private Text text_PlayerHp;
	private Text text_Gold;

	private RectTransform rect_MiniMapBG;
	private bool minimapLargeSize;
	private float size_minimap;
	private Vector2 anchored_minimap;
	private Texture2D[] texMinimap;
	private Texture2D[] colorMinimap;


	private Button pauseBtn;
	private GameObject pauseBG;
	private bool isPause;
	private float pauseFadeDt, _pauseFadeDt;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != null && instance != this)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		//
		// PlayTime, PlayerHp, Gold, Menu, Minimap
		//createUItransformData(5);

		canvasUI = GameObject.Find("UI Manager").
			transform.Find("Canvas_UI").GetComponent<Canvas>();
		canvasScaler = canvasUI.GetComponent<CanvasScaler>();

		text_PlayTime = canvasUI.transform.Find("PlayerUI").
			transform.Find("PlayTime").
			GetComponent<Text>();

		text_PlayerHp = canvasUI.transform.Find("PlayerUI").
			transform.Find("PlayerHp").
			GetComponent<Text>();

		text_Gold = canvasUI.transform.Find("PlayerUI").
			transform.Find("Gold").
			GetComponent<Text>();

		//minimap
		rect_MiniMapBG = canvasUI.transform.Find("MinimapBG").GetComponent<RectTransform>();
		size_minimap = Mathf.Min(rect_MiniMapBG.sizeDelta.x, rect_MiniMapBG.sizeDelta.y);
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


		GameObject inven = Instantiate(Resources.Load("Prefabs/UI/Inventory")) as GameObject;
		inven.transform.SetParent(canvasUI.transform);
		inven.transform.SetSiblingIndex(canvasUI.transform.childCount - 2);
		inven.GetComponent<RectTransform>().sizeDelta *= canvasUI.transform.localScale;


		//
		pauseBtn = canvasUI.transform.Find("PauseButton").GetComponent<Button>();
		pauseBtn.onClick.AddListener(() => pauseGame());
		pauseBG = canvasUI.transform.Find("PauseBG").gameObject;
		Transform t = pauseBG.transform.Find("PauseMenu");
		t.transform.Find("Resume").GetComponent<Button>()
			.onClick.AddListener(() => pauseGame());
		t.transform.Find("Exit").GetComponent<Button>()
			.onClick.AddListener(() => exitGame());
		isPause = false;
		pauseFadeDt = 0;
		_pauseFadeDt = 0.2f;
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

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			pauseGame();
		}
	}

	private void OnGUI()
	{
		drawMinimap();
	}

	private void pauseGame()
	{
		isPause = !isPause;
		pauseBG.SetActive(isPause);
		if(isPause)
			StartCoroutine("pauseGameFade");
	}

	private void exitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	private IEnumerator pauseGameFade()
	{
		pauseFadeDt = 0;
		while (true)
		{
			pauseFadeDt += Time.deltaTime;
			if(pauseFadeDt > _pauseFadeDt)
			{
				pauseBG.GetComponent<Image>().color = Color.black;
				break;
			}
			Color c = new Color(0, 0, 0, pauseFadeDt / _pauseFadeDt);
			pauseBG.GetComponent<Image>().color = c;

			yield return null;
		}
	}

	public void miniMapSizing()
	{
		minimapLargeSize = !minimapLargeSize;
		if(minimapLargeSize)
		{
			Vector2 v = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
				float min = Mathf.Min(v.x, v.y);
			rect_MiniMapBG.sizeDelta = new Vector2(min, min);
			rect_MiniMapBG.anchoredPosition = new Vector2(-Screen.width, -Screen.height);
			Time.timeScale = 0f;
		}
		else
		{
			rect_MiniMapBG.sizeDelta = new Vector2(size_minimap, size_minimap);
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
