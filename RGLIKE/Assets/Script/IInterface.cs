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
