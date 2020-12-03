using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
	void onDamage(LivingEntity entity);
}

public interface IInitialize
{
	void initialize(int mapNum);
}

public interface IItem
{
	void initialize(int mapNum, IMacro.Item_Type type);
	void onUse();
}

public interface IMapObject
{
	void initialize(int mapNum, Vector2 position);
}