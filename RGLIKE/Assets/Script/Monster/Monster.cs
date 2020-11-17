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
	public bool flip;

	public int attackPattern;
	public bool isAttack;

	public bool isHurt;
	protected override void Awake()
	{
		base.Awake();
		player = GameManager.instance.player;
	}

	public virtual void onDamage(float damage)
	{
		if (state == EntityState.dead)
			return;
		state = EntityState.hurt;
		isHurt = true;
		animator.SetBool("isHurt", isHurt);
		animator.SetTrigger("Hurt");

		hp -= damage;
		if (hp < 0)
		{
			hp = 0;
			state = EntityState.dead;
			animator.SetTrigger("Die");
		}
	}

	public virtual void initialize(int mapNum)
	{
		state = EntityState.idle;
		mapNumber = mapNum;
		hp = _hp = 30;
		dmg = _dmg = 5;
		attackDt = _attackDt = 1f;
		moveSpeed = 3f;
	}

	public void setAttackPattern(int pattern)
	{
		attackPattern = pattern;
		animator.SetInteger("AttackPattern", attackPattern);
	}
	//---------------------------------------------------------------
	private void aniAttackToIdle()
	{
		setAttackPattern(-1);
	}

	private void aniAttacking()
	{
		isAttack = true;
	}

	private void aniHurtStop()
	{
		isHurt = false;
		animator.SetBool("isHurt", isHurt);
	}

	private void stateToIdle()
	{
		state = EntityState.idle;
	}
}
