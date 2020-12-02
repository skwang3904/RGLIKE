using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Destoyer : Monster
{
	private enum DestoyerPattern
	{
		HP100 = 0,
		HP75,
		HP50,
		HP25,
	}
	private DestoyerPattern pattern;
	private Vector2 attack3_direction;
	private float attack3_chargeSpeed;

	protected override void Awake()
	{
		base.Awake();
	}

	private void Update()
	{
		if (mapNumber != player.mapNumber)
			return;
		if (state == EntityState.dead ||
			player.state == EntityState.dead)
			return;

		
		distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
		switch (state)
		{
			case EntityState.idle:
			case EntityState.move:
				{
					if (target == null)
					{
						whemTargetNullRandomMove();

						if (distanceToPlayer < eyeRangeToTarget)
						{
							target = player;
						}
					}

					if (target)
					{
						if (distanceToPlayer > eyeRangeToTarget)
						{
							target = null;

							randMove(1, 2, 2, 3);
							break;
						}
						v2target = (target.transform.position - transform.position).normalized;
						flip = v2target.x < 0;
						if (attackRangeToTarget > distanceToPlayer)
						{
							state = EntityState.attack;
							setAttackPattern(setDestroyerAttackPattern());
						}
					}

					spriteRenderer.flipX = flip;
					break;
				}
			case EntityState.attack:
				{

					break;
				}
			case EntityState.hurt:
				{
					break;
				}
			case EntityState.dead:
				{
					break;
				}
		}
	}

	private void FixedUpdate()
	{
		if (mapNumber != player.mapNumber)
			return;
		if (state == EntityState.dead ||
			player.state == EntityState.dead)
			return;

		switch (state)
		{
			case EntityState.idle:
			case EntityState.move:
				{
					if (target == null)
					{
						if (moveDt < _moveDt)
						{
							rigid.MovePosition((Vector2)transform.position
								+ v2Random * moveSpeed * Time.deltaTime);
						}
					}
					else
					{
						rigid.MovePosition((Vector2)transform.position
								+ v2target * moveSpeed * Time.deltaTime);
					}

					break;
				}
			case EntityState.attack:
				{
					switch (attackPattern)
					{
						case 0: break;
						case 1: break;
						case 2: break;
						case 3:
							{
								rigid.MovePosition((Vector2)transform.position
									+ attack3_direction * attack3_chargeSpeed * Time.deltaTime);
								break;
							}
					}
					break;
				}
			case EntityState.hurt:
				{
					break;
				}
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (attackPattern == 3)
		{
			if (collision.collider.tag == "Map")
			{
				setAttackPattern(-1);
			}
		}
	}
	private void OnTriggerStay2D(Collider2D collision)
	{
		if(isAttack)
		{
			if(collision.tag == "Player")
			{
				collision.GetComponent<IDamageable>().onDamage(dmg);
				isAttack = false;
			}
		}
	}

	public override void onDamage(float damage)
	{
		base.onDamage(damage);

		if (hp == _hp) pattern = DestoyerPattern.HP100;
		else if (hp > 75) pattern = DestoyerPattern.HP75;
		else if (hp > 50) pattern = DestoyerPattern.HP50;
		else if (hp > 25) pattern = DestoyerPattern.HP25;

	}

	public override void initialize(int mapNum)
	{
		//base.initialize(mapNum);

		state = EntityState.idle;
		mapNumber = mapNum;
		hp = _hp = 100;
		dmg = _dmg = 15;
		attackDt = _attackDt = 1f;
		moveSpeed = 3f;

		pattern = DestoyerPattern.HP100;
		attack3_direction = Vector2.zero;
		attack3_chargeSpeed = 5f;

		//initState();
		target = null;
		v2target = Vector2.zero;
		eyeRangeToTarget = 8;
		attackRangeToTarget = 3;

		moveDt = _moveDt = 1f;
		stayDt = _stayDt = 1f;
		v2Random = Vector2.zero;
		flip = false;

		attackPattern = -1;
		isAttack = false;
	}

	//---------------------------------------------------------------

	private int setDestroyerAttackPattern()
	{
		int n = 0;
		switch (pattern)
		{
			case DestoyerPattern.HP100: n = 0; break;
			case DestoyerPattern.HP75: n = 1; break;
			case DestoyerPattern.HP50: n = 2; break;
			case DestoyerPattern.HP25:
				{
					n = 3;
					attack3_direction = (player.transform.position - transform.position).normalized;
					break;
				}
		}
		return n;
	}

	private void attack3Start()
	{
		animator.SetTrigger("Attack3Start");
		isAttack = true;
	}

	//---------------------------------------------------------------


	//---------------------------------------------------------------

}
