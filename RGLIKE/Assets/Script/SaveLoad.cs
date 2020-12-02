using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public static class SaveLoad
{
	private const string strMapData = "/mapData.sav";
	private const string strPlayerData = "/playerData.sav";
	private const string strMonsterData = "/monsterData.sav";

	public static bool isNewGame = true; // set in intro

	public static void save()
	{
		if (LevelData.instance == null)
		{
			Debug.Log("Save Failed : LevelData is null");
			return;
		}

		saveMaps();
		savePlayer();
		saveMonster();
	}

	public static void load()
	{
		loadMaps();
		loadPlayer();
		loadMonster();
	}

	//--------------------------------------------------------------------
	// SAVE
	private static void saveMaps()
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath + strMapData;
		FileStream stream = new FileStream(path, FileMode.Create);

		MapData md = new MapData(LevelData.instance.mapData);
		formatter.Serialize(stream, md);
		stream.Close();
	}

	private static void savePlayer()
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath + strPlayerData;
		FileStream stream = new FileStream(path, FileMode.Create);

		PlayerData md = new PlayerData(LevelData.instance.playerData);
		formatter.Serialize(stream, md);
		stream.Close();
	}

	private static void saveMonster()
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath + strMonsterData;
		FileStream stream = new FileStream(path, FileMode.Create);

		MonsterData md = new MonsterData(LevelData.instance.monsterData);
		formatter.Serialize(stream, md);
		stream.Close();
	}

	//--------------------------------------------------------------------
	// LOAD
	private static void loadMaps()
	{
		string path = Application.persistentDataPath + strMapData;
		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);

			LevelData.instance.mapData = formatter.Deserialize(stream) as MapData;
			stream.Close();
		}
		else
		{
			Debug.Log(typeof(MapData) + " Save file not found in " + path);
			LevelData.instance.mapData = new MapData();
		}

		saveMaps();
	}

	private static void loadPlayer()
	{
		string path = Application.persistentDataPath + strPlayerData;
		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);

			LevelData.instance.playerData = formatter.Deserialize(stream) as PlayerData;
			stream.Close();
		}
		else
		{
			Debug.Log(typeof(PlayerData) +  " Save file not found in " + path);
			LevelData.instance.playerData = new PlayerData();
		}

		savePlayer();
	}

	private static void loadMonster()
	{
		string path = Application.persistentDataPath + strMonsterData;
		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);

			LevelData.instance.monsterData = formatter.Deserialize(stream) as MonsterData;
			stream.Close();
		}
		else
		{
			Debug.Log(typeof(MonsterData) + " Save file not found in " + path);
			LevelData.instance.monsterData = new MonsterData();
		}

		saveMonster();
	}
}
