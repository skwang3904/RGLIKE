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

	public MapData mapData;
    public PlayerData playerData;
	public MonsterData monsterData;

	public void startLevel()
	{
        if (mapData == null)
        { // 첫 시작
            mapData = new MapData();
            playerData = new PlayerData();
            monsterData = new MonsterData();
        }
        else
		{ // 다음 레벨
			mapData.nextMap();
			playerData.nextPlayer(GameManager.instance.player);
			monsterData.nextMonster();
		}

        SaveLoad.save();
    }

    public void loadLevel()
	{
		// load
		SaveLoad.load();
	}



}
