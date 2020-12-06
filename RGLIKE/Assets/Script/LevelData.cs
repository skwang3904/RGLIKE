using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	public Image imgFadeInOut;
	public float fadeDt = 0f;
	public float _fadeDt = 2.0f;
	public bool isNextLevelLoad;

	public MapData mapData;
    public PlayerData playerData;
	public MonsterData monsterData;

	public void startLevel()
	{
		if(imgFadeInOut == null)
			imgFadeInOut = GameObject.Find("FadeInOut").GetComponent<Image>();

		imgFadeInOut.color = Color.black;

		if (mapData == null)
        { // 첫 시작
            mapData = new MapData();
            playerData = new PlayerData();
            monsterData = new MonsterData();
        }
        else
		{ // 다음 레벨
			mapData.nextMap();
			playerData.nextPlayer(Player.instance);
			monsterData.nextMonster();


			Shadow.instance.deleteShadow();
			GameManager.instance.createNextLevel();
		}
		
        SaveLoad.save();

		isNextLevelLoad = true;
	}

	public void loadLevel()
	{
		// load -> 로드 후 세이브
		SaveLoad.load();
	}

	

}
