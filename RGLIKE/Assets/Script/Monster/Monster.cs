using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : LivingEntity, IDamageable, IInitialize
{
	// 목표 있을때 목표로 이동, 
	// 목표 없을때 랜덤이동
	protected Player player;
	protected Player target;
	protected Vector2 v2target;
	protected float distanceToPlayer;
	protected float eyeRangeToTarget;
	protected float attackRangeToTarget;

	protected float moveDt, _moveDt;
	protected float stayDt, _stayDt;
	protected Vector2 v2Random;


	protected override void Awake()
	{
		base.Awake();
	}

	public virtual void onDamage(float damage)
	{
		if (state == entityState.dead)
			return;
		state = entityState.hurt;

		hp -= damage;
		if (hp < 0)
		{
			hp = 0;
			state = entityState.dead;
		}

		animator.SetTrigger("Die");
	}

	public virtual void initialize(int mapNum)
	{
		state = entityState.idle;
		mapNumber = mapNum;
		hp = _hp = 30;
		dmg = _dmg = 5;
		attackDt = _attackDt = 1f;
		moveSpeed = 3f;
	}
}
