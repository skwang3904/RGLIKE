using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
	void onDamage(float damage);
}

public interface IInitialize
{
	void initialize(int mapNum);
}

public enum EntityState
{
	idle = 0,
	move,
	attack,
	hurt,
	dead,
}

public enum MapState
{
	nomal = 0,
	boss,
	shop,
}