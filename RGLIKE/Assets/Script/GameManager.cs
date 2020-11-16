using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private Map[] maps;
    private Player player;

    private int totalScore = 0;
    private int currStageScore = 0;
    private float totalPlayTime = 0;
    private float currStageTime = 0;
    private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

        
        createMap();
        createPlayer();
        //createMapObject();
        //createMonster();
    }

	private void Update()
	{
        totalPlayTime += Time.deltaTime;
        currStageTime += Time.deltaTime;
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
        bool[] check = ld.MAP_DATA;
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
        GameObject g = Instantiate(Resources.Load("Prefabs/Map/" + name)) as GameObject;
        g.transform.SetParent(GameObject.Find("Maps").transform);
        Sprite sp = g.GetComponent<SpriteRenderer>().sprite;
        float width = sp.bounds.size.x;
        float height = sp.bounds.size.y;
        Vector3 position = new Vector3(width * (i % sqrt), height * (i / sqrt), 0);
        g.transform.position = position;
        maps[i] = g.GetComponent<Map>();
        maps[i].mapNumber = i;
	}

    //---------------------------------------------------------------------------
    // createPlayer
    private void createPlayer()
	{
        int i, totalNum;
        LevelData ld = LevelData.instance;
        totalNum = ld.MAP_TOTAL_NUM;

        GameObject g = Instantiate(Resources.Load("Prefabs/Player/Player")) as GameObject;
        while(true)
		{
            int random = Random.Range(0, totalNum);
            Map m = maps[random];
            if (m != null)
			{
                g.transform.position = m.transform.Find("PlayerSpawn").position;
                player = g.GetComponent<Player>();
                player.initialize(random);
                break;
			}
        }
	}


    //---------------------------------------------------------------------------
    // createMapObject

    //---------------------------------------------------------------------------
    // createMonster
}
