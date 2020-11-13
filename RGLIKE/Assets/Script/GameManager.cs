using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

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


        createTileMap();
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

	private void createTileMap()
    {
#if false
        //새로 만들때 기존에 있던것 파괴
        if (rooms != null)
        {
            for (i = 0; i < rooms.Length; i++)
            {
                Destroy(rooms[i]);
            }
            rooms = null;
        }
#endif

        //맵 배치, 불러오기시 실행안함
        createTile(); 

        // 맵 생성
        // 보스, 상점방 재생성
        // 맵 오브젝트 생성

        // 플레이어 생성

        // 몬스터 생성 ( 플레이어 있는곳 제외 )

    }

    private void createTile()
	{
        int i, j;

        LevelData ld = LevelData.instance;
        ld.setStage();

        int connectNum = ld.TILE_CONNECT_NUM;
        int totalNum = ld.TILE_TOTAL_NUM;
        bool[] check = ld.TILE_DATA;
        bool[] visit = new bool[totalNum];
        int random;
        int connected = 0;
        int count;

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
                connectCheck(visit, check, i, ref connected);

                if (connected == connectNum)
                    break;
                connected = 0;
                visit = Enumerable.Repeat(false, visit.Length).ToArray();
            }
        }

    }

    private void connectCheck(bool[] visit, bool[] check, int index, ref int connected)
    {
        // 맵이 TILE_CONNECT_NUM 만큼 연결됐는지 확인
        if (index < 0 || index > LevelData.instance.TILE_TOTAL_NUM - 1)
            return;

        if (check[index] == false || visit[index] == true)
            return;

        visit[index] = true;
        connected++;

        int sqrt = LevelData.instance.TILE_TOTAL_SQRT;
        if (index % sqrt > 0)           connectCheck(visit, check, index - 1, ref connected);
        if (index % sqrt < sqrt - 1)    connectCheck(visit, check, index + 1, ref connected);
        if (index / sqrt > 0)           connectCheck(visit, check, index - sqrt, ref connected);
        if (index / sqrt < sqrt - 1)    connectCheck(visit, check, index + sqrt, ref connected);
    }
}
