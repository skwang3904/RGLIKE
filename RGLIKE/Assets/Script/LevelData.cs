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
	public int MAP_TOTAL_NUM;
	public int MAP_TOTAL_SQRT;
	public int MAP_CONNECT_NUM;
	public bool[] MAP_DATA;

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
		MAP_TOTAL_NUM = 16;
		MAP_TOTAL_SQRT = (int)Mathf.Sqrt(MAP_TOTAL_NUM);
		MAP_CONNECT_NUM = MAP_TOTAL_NUM / 2;
		MAP_DATA = new bool[MAP_TOTAL_NUM];

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
