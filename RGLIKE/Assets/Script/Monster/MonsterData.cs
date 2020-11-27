using System;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public class MonsterData
{ // index = 종류, kinds_num = 몬스터 종류의 수
	public int boss_index; 
	public int kinds_num;
	public int[] kind_index;

	public MonsterData()
	{
		initMonsterData();
	}

	public MonsterData(MonsterData md)
	{
		boss_index = md.boss_index;
		kinds_num = md.kinds_num;
		kind_index = (int[])md.kind_index.Clone();
	}

	//--------------------------------------------------------------------

	public void nextMonster()
	{
		initMonsterData();
	}

	private void initMonsterData()
	{
		boss_index = 0;
		kinds_num = 1;
		kind_index = new int[kinds_num];
		for (int i = 0; i < kinds_num; i++)
		{
			kind_index[i] = 0;
		}
	}

	//--------------------------------------------------------------------

	public readonly string[] MonsterNames =
	{
			"Anubis",

	};

	public readonly string[] MonsterBossNames =
	{
		"Destroyer",

	};
}
