using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    // Score
    private int totalScore = 0;
    private int currStageScore = 0;
    private float totalPlayTime = 0;
    private float currStageTime = 0;

    // Map
    private Map[] maps;

    // Player
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
    private Monster[] monsters;
   
    private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

        
        createMap(); // maps[]
        createPlayer(); // vCamera

        //createMapObject();
        createMonster();
    }

	private void Update()
	{
        totalPlayTime += Time.deltaTime;
        currStageTime += Time.deltaTime;

        passMapAnimation();
    }

    public void addScore(int hp)
	{
        currStageScore += hp;
        currStageScore += 500 - (int)currStageTime;
        totalScore += currStageScore;
    }

    //---------------------------------------------------------------------------
    // createMap
    private void createMap()
	{
        int i;
        if (maps != null)
        {
            for (i = 0; i < maps.Length; i++)
            {
                Destroy(maps[i]);
            }
            maps = null;
        }

        LevelData ld = LevelData.instance;
        ld.setStage();

        int connectNum = ld.MAP_CONNECT_NUM;
        int totalNum = ld.MAP_TOTAL_NUM;
        ref bool[] check = ref ld.MAP_DATA;
        bool[] visit = new bool[totalNum];
        int random;
        int connected = 0;
        int count;

        // MapData 생성, 연결
        while (connected != connectNum)
        {
            connected = 0;
            count = 0;
            check = Enumerable.Repeat(false, check.Length).ToArray();
            visit = Enumerable.Repeat(false, visit.Length).ToArray();

            while (count != connectNum)
            {
                random = Random.Range(0, totalNum);
                if (check[random] == false)
                {
                    check[random] = true;
                    count++;
                }
            }

            for (i = 0; i < totalNum; i++)
            {
                connectMapData(visit, check, i, ref connected);

                if (connected == connectNum)
                    break;
                connected = 0;
                visit = Enumerable.Repeat(false, visit.Length).ToArray();
            }
        }

        // 현재맵에 연결된 맵 체크 후 맵프리팹 생성
        maps = new Map[totalNum];
        for (i = 0; i < totalNum; i++)
            connectCountCheck(check, i);

        for (i = totalNum - 1; i > -1; i--)
		{
            if (maps[i])
			{
                Destroy(maps[i].gameObject);

                loadMapPrefabs(IMacro.MapName[(int)IMacro.MAP_NAME.room_boss0], i);
                maps[i].mapNumber = i;
                maps[i].state = MapState.boss;
                break;
			}
		}
        
        
    }

    private void connectMapData(bool[] visit, bool[] check, int index, ref int connected)
    {
        // 맵이 MAP_CONNECT_NUM 만큼 연결됐는지 확인
        if (index < 0 || index > LevelData.instance.MAP_TOTAL_NUM - 1)
            return;

        if (check[index] == false || visit[index] == true)
            return;

        visit[index] = true;
        connected++;

        int sqrt = LevelData.instance.MAP_TOTAL_SQRT;
        if (index % sqrt > 0)           connectMapData(visit, check, index - 1, ref connected);
        if (index % sqrt < sqrt - 1)    connectMapData(visit, check, index + 1, ref connected);
        if (index / sqrt > 0)           connectMapData(visit, check, index - sqrt, ref connected);
        if (index / sqrt < sqrt - 1)    connectMapData(visit, check, index + sqrt, ref connected);
    }

	private void connectCountCheck(bool[] check, int i)
	{
        if (!check[i])
		{
            Destroy(maps[i]);
            maps[i] = null;
            return;
		}
        
        // 실제 맵 생성
        LevelData ld = LevelData.instance;
        int sqrt = ld.MAP_TOTAL_SQRT;
        bool l = false;
        bool r = false;
        bool u = false;
        bool d = false;

        if (i % sqrt > 0)           l = check[i - 1];
        if (i % sqrt < sqrt - 1)    r = check[i + 1];
        if (i / sqrt < sqrt - 1)    u = check[i + sqrt];
        if (i / sqrt > 0)           d = check[i - sqrt];
        // 위아래 반대로 검사함

        int sum = 0;
        if (l) sum++;
        if (r) sum++;
        if (u) sum++;
        if (d) sum++;

        int n = 0;
        switch (sum)
        {
            case 4:
                {
                    n = (int)IMacro.MAP_NAME.room_4way;
                    break;
                }
            case 3:
                {
                    if      (r && u && d) n = (int)IMacro.MAP_NAME.room_3way0;
                    else if (l && u && d) n = (int)IMacro.MAP_NAME.room_3way1;
                    else if (l && r && d) n = (int)IMacro.MAP_NAME.room_3way2;
                    else if (l && r && u) n = (int)IMacro.MAP_NAME.room_3way3;
                    else
                        print("Map Not Connect : " + i + "번째");
                    break;
                }
            case 2:
                {
                    if      (l && r) n = (int)IMacro.MAP_NAME.room_2way0;
                    else if (u && d) n = (int)IMacro.MAP_NAME.room_2way1;
                    else if (l && u) n = (int)IMacro.MAP_NAME.room_2way2;
                    else if (l && d) n = (int)IMacro.MAP_NAME.room_2way3;
                    else if (r && u) n = (int)IMacro.MAP_NAME.room_2way4;
                    else if (r && d) n = (int)IMacro.MAP_NAME.room_2way5;
                    else
                        print("Map Not Connect : " + i + "번째");
                    break;
                }
            case 1:
                {
                    if      (l) n = (int)IMacro.MAP_NAME.room_1way0;
                    else if (r) n = (int)IMacro.MAP_NAME.room_1way1;
                    else if (u) n = (int)IMacro.MAP_NAME.room_1way2;
                    else if (d) n = (int)IMacro.MAP_NAME.room_1way3;
                    else
                        print("Map Not Connect : " + i + "번째");
                    break;
                }
            case 0:
                {
                    print("Map Not Connect : " + i + "번째");
                    break;
                }
        }

        string name = IMacro.MapName[n];
        loadMapPrefabs(name, i);
    }

    private void loadMapPrefabs(string name, int i)
	{
        LevelData ld = LevelData.instance;
        int sqrt = ld.MAP_TOTAL_SQRT;

        GameObject g = Instantiate(Resources.Load("Prefabs/Map/" + name)) as GameObject;
        g.transform.SetParent(GameObject.Find("Maps").transform);

        Sprite sp = g.GetComponent<SpriteRenderer>().sprite;
        int width = Mathf.FloorToInt(sp.bounds.size.x);
        int height = Mathf.FloorToInt(sp.bounds.size.y);
        Vector3 position = new Vector3(width * (i % sqrt), height * (i / sqrt), 0);
        g.transform.position = position;

        maps[i] = g.GetComponent<Map>();
        maps[i].mapNumber = i;
        maps[i].state = MapState.nomal;
    }

    //---------------------------------------------------------------------------
    // createPlayer
    private void createPlayer()
	{
        int totalNum;
        LevelData ld = LevelData.instance;
        totalNum = ld.MAP_TOTAL_NUM;

        GameObject g = Instantiate(Resources.Load("Prefabs/Player/Player")) as GameObject;
        while(true)
		{
            int random = Random.Range(0, totalNum);
            Map m = maps[random];
            if (m != null && maps[random].state == MapState.nomal)
			{
                g.transform.position = m.transform.Find("PlayerSpawn").position;
                player = g.GetComponent<Player>();
                player.initialize(random);
                break;
			}
        }

        vCameraCollider = GameObject.Find("vCameraCollider");
        vCameraCollider.transform.position = maps[player.mapNumber].transform.position;

        vCamera = GameObject.Find("vCamera").GetComponent<CinemachineVirtualCamera>();
        vCamera.Follow = player.transform;

        currMapNumber = nextMapNumber = player.mapNumber;
        passMapDt = _passMapDt;
}


    //---------------------------------------------------------------------------
    // createMapObject

    //---------------------------------------------------------------------------
    // createMonster

    private void createMonster()
	{
        LevelData ld = LevelData.instance;
        int total = ld.MAP_TOTAL_NUM;
        int i, j, sum = 0;

        for (i = 0; i < total; i++)
		{
            // 스테이지 총 몬스터 소환 되는 수
            if (!maps[i])
                continue;
            if (i == player.mapNumber)
                continue;

            sum += maps[i].transform.Find("MonsterSpawn").childCount;
		}

        monsters = new Monster[sum];

        GameObject g;
        GameObject monsterParent = GameObject.Find("Monsters");
        Monster m;
        Transform t;
        int num;
        for (i = 0; i < total; i++) 
		{
            if (maps[i] == null || i == player.mapNumber)
                continue;

            t = maps[i].transform.Find("MonsterSpawn");
            if (t == null)
                continue;

            num = t.childCount;

            for (j = 0; j < num; j++) 
			{
                g = Instantiate(Resources.Load("Prefabs/Monster/Anubis"),
                    t.GetChild(j).transform.position,
                    Quaternion.identity) as GameObject;
                g.transform.SetParent(monsterParent.transform);

                m = g.GetComponent<Monster>();
                m.initialize(i);
			}
        }
        
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
            print("Same PassMapNumber");
            return;
		}

        if (passMapDt != _passMapDt)
            return;

        nextMapNumber = mapNum;
        passMapDt = 0;

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
    }

    private void passMapAnimation()
	{
        if (currMapNumber == nextMapNumber)
            return;

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
}
