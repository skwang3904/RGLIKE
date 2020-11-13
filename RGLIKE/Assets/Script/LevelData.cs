using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
	private static LevelData Instance;
	public static LevelData instance
	{
		get
		{
			if (Instance == null)
				Instance = new LevelData();
			return Instance;
		}
	}

	private LevelData() { }

	public int currStage;
	public int TILE_TOTAL_NUM;
	public int TILE_TOTAL_SQRT;
	public int TILE_CONNECT_NUM;
	public bool[] TILE_DATA;

	//player data

	public int MONSTER_BOSS_INDEX;
	public int MONSTER_KIND_NUM;
	public int[] MONSTER_INDEX;


	public readonly string[] MonsterNames = 
						{
							"monster",

						};

	public readonly string[] MonsterBossNames =
						{
							"boss",

						};

	public void setStage()
	{
		currStage++;

		// 타일데이터 초기화
		TILE_TOTAL_NUM = 16;
		TILE_TOTAL_SQRT = (int)Mathf.Sqrt(TILE_TOTAL_NUM);
		TILE_CONNECT_NUM = TILE_TOTAL_NUM / 2;
		TILE_DATA = new bool[TILE_TOTAL_NUM];

		//player data

		//스테이지에 생성될 몬스터의 데이터
		MONSTER_BOSS_INDEX = 0;
		MONSTER_KIND_NUM = 1;
		MONSTER_INDEX = new int[MONSTER_KIND_NUM];
		for (int i = 0; i < MONSTER_KIND_NUM; i++)
			MONSTER_INDEX[i] = i;
	}

	public void saveLevel()
	{
		// save
	}

	public void loadLevel()
	{
		// load
	}

}
