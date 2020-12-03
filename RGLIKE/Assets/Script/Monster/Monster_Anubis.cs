using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Anubis : Monster
{
	protected override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		randMove();
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
					
					if(target)
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
							//setAttackPattern(Random.Range(0, 2));
							setAttackPattern(1);
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
		float dt = livingDeltaTime(timeScale);

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
						if(moveDt < _moveDt)
						{
							rigid.MovePosition((Vector2)transform.position
								+ v2Random * moveSpeed * dt);
						}
					}
					else
					{
						rigid.MovePosition((Vector2)transform.position
								+ v2target * moveSpeed * dt);
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
		particle.Play();
		if(state == EntityState.dead)
		{
			initState();
		}
	}

	public override void initialize(int mapNum)
	{
		base.initialize(mapNum);

		initState();
	}


}
