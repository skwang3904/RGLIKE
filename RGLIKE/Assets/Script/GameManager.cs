﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public bool isGameOver;

    // Score
    public int totalScore;
    public int currStageScore;
    public float totalPlayTime;
    public float currStageTime;
    public int gold;
    private float gameOverDt;
    private const float _gameOverDt = 1f;

    public Map[] maps { get; private set; }
    public Player player { get; private set; }

    //virtual camera
    public CinemachineVirtualCamera vCamera;
    private GameObject vCameraCollider;
    private float passMapDt;
    private const float _passMapDt = 1f;
    private int currMapNumber;
    private Vector2 currMapPos;
    private Vector2 currMapSize;
    private int nextMapNumber;
    private Vector2 nextMapPos;
    private Vector2 nextMapSize;

    // Monster
    public Monster[] monsters { get; private set; } // 총 몬스터
   
    private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
		//DontDestroyOnLoad(gameObject);

        LevelData ld = LevelData.instance;
        if(SaveLoad.isNewGame)
		{
            ld.startLevel();
		}
        else
		{
            ld.loadLevel();
        }

        createMap(); // maps
        createPlayer(); // vCamera
        createMapObject();
        createMonster();
        createItems(); // itemsBase[] 풀메모리
    }

    private void Update()
	{
        if (player.state == EntityState.dead &&
            isGameOver == false)
            StartCoroutine("gameOverFadeOut");

        if (LevelData.instance.isNextLevelLoad)
            StartCoroutine("nextLevelFadeOut");

        totalPlayTime += Time.deltaTime;
        currStageTime += Time.deltaTime;
    }

    public void createNextLevel()
	{
        createMap();
        createPlayer();
        createMonster();
    }

    public void addScore(int hp)
	{
        currStageScore += hp;
        currStageScore += 500 - (int)currStageTime;
        totalScore += currStageScore;
    }

    private IEnumerator nextLevelFadeOut()
	{
        LevelData ld = LevelData.instance;
        ld.isNextLevelLoad = false;
        ld.fadeDt = 0;
        Color c = Color.black;

        while (true)
        {
            ld.fadeDt += Time.deltaTime;
            if (ld.fadeDt > ld._fadeDt)
                ld.fadeDt = ld._fadeDt;

            c.a = Mathf.Lerp(1, 0, ld.fadeDt / ld._fadeDt);
            ld.imgFadeInOut.color = c;

            if (ld.fadeDt == ld._fadeDt)
                break;

            yield return null;
        }
    }

    private IEnumerator gameOverFadeOut()
    {
        isGameOver = true;

        while (true)
        {
            gameOverDt += Time.deltaTime;
            if (gameOverDt > _gameOverDt)
			{
                gameOverDt = _gameOverDt;
                //SceneManager.LoadScene("MainGame"); // 수정
                break;
			}

            yield return null;
        }
    }

    //---------------------------------------------------------------------------
    // createMap
    private void createMap()
	{
        int i;
        if (Map.instance != null)
		{
            for (i = 0; i < Map.instance.Length; i++) 
			{
                if (Map.instance[i] == null)
                    continue;

                Destroy(Map.instance[i].gameObject);
                Map.instance[i].transform.SetParent(null);
                Map.instance[i] = null;
            }
            Map.instance = null;
        }

        MapData md = LevelData.instance.mapData;
        int sqrt = md.mapTotalSqrt;
        int num = md.mapTotalNum;
        string name = "";
        GameObject g = null;
        Sprite sp = null;
        float width = 0;
        float height = 0;
        Vector2 position = Vector2.zero;

        //maps = new Map[num];
        Map.instance = new Map[num];
        maps = Map.instance;

        for (i = 0; i < num; i++)
		{
            if (!md.maps[i])
			{
                maps[i] = null;
                continue;
			}

            name = IMacro.MapName[(int)md.mapName[i]];
            g = Instantiate(Resources.Load("Prefabs/Map/" + name)) as GameObject;
            g.transform.SetParent(GameObject.Find("Maps").transform);

            sp = g.GetComponent<SpriteRenderer>().sprite;
            width = sp.bounds.size.x;
            height = sp.bounds.size.y;
            position.Set(
                (width - 0.5f) * (i % sqrt), 
                (height - 0.5f) * (i / sqrt));
            g.transform.position = position;

            maps[i] = g.GetComponent<Map>();
            maps[i].mapNumber = i;
            maps[i].state = md.mapStates[i];
        }
    }

    //---------------------------------------------------------------------------
    // createPlayer
    private void createPlayer()
	{
        LevelData ld = LevelData.instance;
        int num = ld.playerData.mapNumber;

        if (Player.instance == null)
		{
            Instantiate(Resources.Load("Prefabs/Player/Player"));
            player = Player.instance;
		}

        Player.instance.create(maps[num].transform.Find("PlayerSpawn").position);

        vCameraCollider = GameObject.Find("vCameraCollider");
        vCameraCollider.transform.position = maps[player.mapNumber].transform.position;

        vCamera = GameObject.Find("vCamera").GetComponent<CinemachineVirtualCamera>();
        vCamera.Follow = player.transform;

        currMapNumber = nextMapNumber = player.mapNumber;
        passMapDt = _passMapDt;
    }

    //---------------------------------------------------------------------------
    // createMapObject
    private void createMapObject()
	{
#if true // test
        GameObject g = Instantiate(Resources.Load("Prefabs/Map/MapObject/MapObject_Barrel")) as GameObject;
        Vector3 v = player.transform.position + new Vector3(-2, 2, 0);
        g.GetComponent<MapObject>().initialize(player.mapNumber, v);
        g = Instantiate(Resources.Load("Prefabs/Map/MapObject/MapObject_Lever")) as GameObject;
        v = player.transform.position + new Vector3(2, 2, 0);
        g.GetComponent<MapObject>().initialize(player.mapNumber, v);

        g = Instantiate(Resources.Load("Prefabs/Map/MapObject/MapObject_SpikeTrap")) as GameObject;
        v = player.transform.position + new Vector3(-2, -2, 0);
        g.GetComponent<MapObject>().initialize(player.mapNumber, v);

        g = Instantiate(Resources.Load("Prefabs/Map/MapObject/MapObject_Wall")) as GameObject;
        v = player.transform.position + new Vector3(2, -2, 0);
        g.GetComponent<MapObject>().initialize(player.mapNumber, v);


        for (int i = 0; i < maps.Length; i++) 
		{
            if (maps[i] == null)
                continue;

            if(maps[i].state == MapState.boss)
			{
                g = Instantiate(Resources.Load("Prefabs/Map/MapObject/MapObject_NextDoor")) as GameObject;
                g.GetComponent<MapObject>().initialize(i, maps[i].transform.position);
                break;
            }
		}
#endif
    }

    //---------------------------------------------------------------------------
    // createMonster
    private void createMonster()
	{
        int i, j;
        if (monsters != null) 
		{
            for (i = 0; i < monsters.Length; i++) 
			{
                Destroy(monsters[i].gameObject);
                monsters[i].transform.SetParent(null);
                monsters[i] = null;
            }
            monsters = null;
        }

        LevelData ld = LevelData.instance;
        int total = ld.mapData.mapTotalNum;
        int sum = 0;

        for (i = 0; i < total; i++)
		{
            // 스테이지 총 몬스터 소환 되는 수
            if (maps[i] == null || i == player.mapNumber)
                continue;

            sum += maps[i].transform.Find("MonsterSpawn").childCount;
		}

        monsters = new Monster[sum];

        GameObject g;
        GameObject monsterParent = GameObject.Find("Monsters");
        Monster m;
        Transform t;
        int num, count = 0;
        string str;
        for (i = 0; i < total; i++) 
		{
            if (maps[i] == null || i == player.mapNumber)
                continue;

            t = maps[i].transform.Find("MonsterSpawn");
            if (t == null)
                continue;

            num = t.childCount;
            if(maps[i].state == MapState.boss)
                str = "/Destroyer";
            else
                str = "/Anubis";

            for (j = 0; j < num; j++) 
			{
                g = Instantiate(Resources.Load("Prefabs/Monster" + str),
                    t.GetChild(j).transform.position,
                    Quaternion.identity) as GameObject;
                g.transform.SetParent(monsterParent.transform);

                m = g.GetComponent<Monster>();
                m.initialize(i);
                monsters[count++] = m;

                if (count > sum)
                    print("monster create count over total-Monster-Number");
			}
        }
        
	}

    //---------------------------------------------------------------------------
    // createItems
    private void createItems()
    {
        int i, j;
        Item.createItems();
#if false
        int kinds = (int)IMacro.Item_Type.Max;
        int num = Item.Max_itemNum;

        ref string[] istr = ref IMacro.ItemName;
        GameObject g;
        GameObject parent = GameObject.Find("Items");
        for (i = 0; i < kinds; i++)
        {
            for (j = 0; j < num; j++)
            {
                g = Instantiate(Resources.Load("Prefabs/Item/" + istr[i])) as GameObject;
                //g.GetComponent<SpriteRenderer>().sprite = null;

                g.transform.SetParent(parent.transform);
                g.transform.Translate(-100, -100, 0);
                Item.itemsBase[i].Add(g.GetComponent<Item>());
            }
        }
#endif
    }

    //---------------------------------------------------------------------------
    // passMap
    public bool isPassMap()
	{
        return passMapDt != _passMapDt;
    }

    public void passMapStart(int mapNum)
	{
        if(currMapNumber == mapNum)
		{
            print("Same PassMapNumber error");
            return;
		}

        if (passMapDt != _passMapDt)
            return;

        nextMapNumber = mapNum;

        Map curr = maps[currMapNumber];
        Map next = maps[nextMapNumber];

        currMapPos = curr.transform.position;
        currMapSize = curr.spriteRenderer.sprite.bounds.size;

        nextMapPos = next.transform.position;
        nextMapSize = next.spriteRenderer.sprite.bounds.size;

        // #issue 임의로 만든 값
        player.transform.position = 
            (Vector2)player.transform.position 
            + (nextMapPos - currMapPos) * 0.4f;

        StartCoroutine(passMap_Coroutine());
    }

    private void passMapAnimation()
	{
        passMapDt += Time.deltaTime;
        if(passMapDt >= _passMapDt)
		{
            currMapNumber = nextMapNumber;
            player.mapNumber = currMapNumber;
            passMapDt = _passMapDt;
		}

        float dt = passMapDt / _passMapDt;

        vCameraCollider.transform.position = Vector2.Lerp(
            currMapPos,
            nextMapPos,
            dt);

        BoxCollider2D box = vCameraCollider.GetComponent<BoxCollider2D>();
        box.size = Vector2.Lerp(currMapSize, nextMapSize, dt);
    }

    private IEnumerator passMap_Coroutine()
	{
        passMapDt = 0;

        while (true)
		{
            passMapDt += Time.deltaTime;
            if (passMapDt >= _passMapDt)
                passMapDt = _passMapDt;

            float dt = passMapDt / _passMapDt;

            vCameraCollider.transform.position = Vector2.Lerp(
                currMapPos,
                nextMapPos,
                dt);

            BoxCollider2D box = vCameraCollider.GetComponent<BoxCollider2D>();
            box.size = Vector2.Lerp(currMapSize, nextMapSize, dt);

            if(passMapDt == _passMapDt)
			{
                currMapNumber = nextMapNumber;
                player.mapNumber = currMapNumber;
                break;
            }

            yield return null;
		}
	}
}
