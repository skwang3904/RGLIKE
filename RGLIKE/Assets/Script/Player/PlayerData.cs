using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
	public int mapNumber;
	public float hp, _hp;
	public float dmg, _dmg;
	public float attackDt, _attackDt; // 공격속도 : animation 속도
	public float moveSpeed;

	//public float criticalChance, _criticalChance;
	//public float evasionChance, _evasionChance;

	public PlayerData()
	{
		while(true)
		{
			MapData md = LevelData.instance.mapData;
			int ran = UnityEngine.Random.Range(0, md.mapTotalNum);
			if (md.maps[ran] && md.mapStates[ran] == MapState.nomal)
			{
				mapNumber = ran;
				break;
			}
		}
		hp = _hp = 100.0f;
		dmg = _dmg = 10.0f;
		attackDt = _attackDt = 1.0f;
		moveSpeed = 10.0f;
	}

	public PlayerData(PlayerData pd)
	{
		mapNumber = pd.mapNumber;
		hp = pd.hp;
		_hp = pd._hp;
		dmg = pd.dmg;
		_dmg = pd._dmg;
		attackDt = pd.attackDt;
		_attackDt = pd._attackDt;
		moveSpeed = pd.moveSpeed;
	}

	public void nextPlayer(Player p)
	{
		while (true)
		{
			MapData md = LevelData.instance.mapData;
			int ran = UnityEngine.Random.Range(0, md.mapTotalNum);
			if (md.maps[ran] && md.mapStates[ran] == MapState.nomal)
			{
				mapNumber = ran;
				break;
			}
		}
		//mapNumber = p.mapNumber;
		hp = p.hp;
		_hp = p._hp;
		dmg = p.dmg;
		_dmg = p._dmg;
		attackDt = p.attackDt;
		_attackDt = p._attackDt;
		moveSpeed = p.moveSpeed;
	}
}
