using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Anubis : Monster
{
	protected override void Awake()
	{
		base.Awake();
	}

	private void Update()
	{
		if (mapNumber != player.mapNumber)
			return;
		if (player.state == EntityState.dead)
			return;

		distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
		switch (state)
		{
			case EntityState.idle:
			case EntityState.move:
				{
					if (target == null)
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

						if (distanceToPlayer < eyeRangeToTarget)
						{
							target = player;
							break;
						}
					}
					else
					{
						if (distanceToPlayer > eyeRangeToTarget)
						{
							target = null;

							randMove();
							break;
						}
						v2target = (target.transform.position - transform.position).normalized;
						flip = v2target.x < 0;
						if (attackRangeToTarget > distanceToPlayer)
						{
							state = EntityState.attack;
							setAttackPattern(Random.Range(0, 2));
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
		if (player.state == EntityState.dead)
			return;

		switch (state)
		{
			case EntityState.idle:
			case EntityState.move:
				{
					if (target == null)
					{
						if(moveDt < _moveDt)
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
					if (isAttack && distanceToPlayer < 3 * (attackPattern + 1))
					{
						isAttack = false;
						player.onDamage(dmg);
					}
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

	public override void onDamage(float damage)
	{
		base.onDamage(damage);

		if(state == EntityState.dead)
		{
			initState();
			moveBox.isTrigger = true;
		}
	}

	public override void initialize(int mapNum)
	{
		base.initialize(mapNum);

		initState();
	}

	private void initState()
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

	private void randMove()
	{
		moveDt = 0;
		_moveDt = Random.Range(1f, 2f);
		stayDt = _stayDt = Random.Range(0f, 1f);

		float x = Random.Range(-1f, 1f);
		float y = Random.Range(-1f, 1f);
		v2Random = new Vector2(x, y).normalized;
		animator.SetBool("Walk", true);

		flip = x < 0;
	}
}
