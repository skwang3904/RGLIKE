using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : NonLivingEntity, IDamageable, IMapObject
{
	public MapObjectType type;
	public int hp, _hp;

	public Action active;

	protected override void Awake()
	{
		base.Awake();
	}

	public virtual void onDamage(LivingEntity entity)
	{
		if (state == NonEntityState.dead)
			return;
		if (type != MapObjectType.broken)
			return;

		hp--;
		if(hp <= 0)
		{
			hp = 0;
			state = NonEntityState.dead;
			touchBox.enabled = false;
			active?.Invoke();
		}
	}

	public virtual void initialize(int mapNum, Vector2 position)
	{
		transform.position = position;
		state = NonEntityState.NonAppear;
		mapNumber = mapNum;
		value = 0;

		isAppear = false;
		isActive = false;
		isDisappear = false;
	}
}
