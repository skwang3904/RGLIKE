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
	public IMacro.Item_Name dropItem;

	protected override void Awake()
	{
		base.Awake();
		player = GameManager.instance.player;
		deadMethod += dropItemWhenDead;
	}

	//---------------------------------------------------------------
	// interface
	public virtual void onDamage(float damage)
	{
		if (state == EntityState.dead)
			return;

		state = EntityState.hurt;
		isHurt = true;
		animator.SetBool("isHurt", isHurt);
		animator.SetTrigger("Hurt");

		hp -= damage;
		if (hp <= 0)
		{
			hp = 0;
			state = EntityState.dead;
			animator.SetTrigger("Die");

			hitBox.enabled = false;
			moveBox.enabled = false;
			rigid.bodyType = RigidbodyType2D.Static;

			deadMethod?.Invoke();
		}
	}

	public void initState() // 패턴관련 
	{
		target = null;
		v2target = Vector2.zero;
		eyeRangeToTarget = 5;
		attackRangeToTarget = 2;

		moveDt = _moveDt = 1f;
		stayDt = _stayDt = 1f;
		v2Random = Vector2.zero;
		flip = false;

		attackPattern = -1;
		isAttack = false;
	}

	public virtual void initialize(int mapNum) // 스텟 수치
	{
		state = EntityState.idle;
		mapNumber = mapNum;
		hp = _hp = 30;
		dmg = _dmg = 5;
		attackDt = _attackDt = 1f;
		moveSpeed = 3f;

		initState();
	}

	//---------------------------------------------------------------
	// move method
	public void randMove()
	{
		randMove(1, 2, 0, 1);
	}
	public void randMove(float moveMin, float moveMax, float stayMin, float stayMax)
	{
		moveDt = 0;
		_moveDt = Random.Range(moveMin, moveMax);
		stayDt = _stayDt = Random.Range(stayMin, stayMax);

		float x = Random.Range(-1f, 1f);
		float y = Random.Range(-1f, 1f);
		v2Random = new Vector2(x, y).normalized;
		animator.SetBool("Walk", true);

		flip = x < 0;
	}

	public void whemTargetNullRandomMove()
	{
		if (moveDt < _moveDt)
		{
			moveDt += Time.deltaTime;
			if (moveDt > _moveDt)
			{
				moveDt = _moveDt;
				stayDt = 0;
				animator.SetBool("Walk", false);
			}
		}

		if (stayDt < _stayDt)
		{
			stayDt += Time.deltaTime;
			if (stayDt > _stayDt)
			{
				randMove();
			}
		}
	}

	//---------------------------------------------------------------
	// attack method
	public void setAttackPattern(int pattern)
	{
		attackPattern = pattern;
		animator.SetInteger("AttackPattern", attackPattern);
	}



	//---------------------------------------------------------------
	// animator callback method
	private void aniAttackToIdle()
	{
		setAttackPattern(-1);
		isAttack = false;
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

	private void stateToWalk()
	{
		state = EntityState.move;
	}

	//---------------------------------------------------------------
	// Drop Method

	private void dropItemWhenDead()
	{
		Item.monsterDropItem(mapNumber, transform.position, IMacro.Item_Name.Potion, 1);
	}
}
