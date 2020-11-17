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
		player = GameManager.instance.player;
	}

	private void Update()
	{
		if (mapNumber != player.mapNumber)
			return;

		distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
		switch (state)
		{
			case entityState.idle:
			case entityState.move:
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
							}
						}

						if (stayDt < _stayDt)
						{
							stayDt += Time.deltaTime;
						}

						if (distanceToPlayer < eyeRangeToTarget)
						{
							target = player;
						}
					}
					else
					{
						if (Vector2.Distance(target.transform.position, transform.position) > eyeRangeToTarget)
						{
							target = null;

							randMove();
						}
						v2target = (target.transform.position - transform.position).normalized;
					}


					break;
				}
			case entityState.attack:
				{
					break;
				}
			case entityState.hurt:
				{
					break;
				}
			case entityState.dead:
				{
					break;
				}
		}
		

	}

	private void FixedUpdate()
	{
		switch (state)
		{
			case entityState.idle:
			case entityState.move:
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
						v2target = (target.transform.position - transform.position).normalized;
					}


					break;
				}
			case entityState.attack:
				{
					break;
				}
			case entityState.hurt:
				{
					break;
				}
			case entityState.dead:
				{
					break;
				}
		}
	}

	public override void onDamage(float damage)
	{
		base.onDamage(damage);
	}

	public override void initialize(int mapNum)
	{
		base.initialize(mapNum);

		target = null;
		v2target = Vector2.zero;
		eyeRangeToTarget = 5;
		attackRangeToTarget = 2;

		moveDt = _moveDt = 1f;
		stayDt = _stayDt = 1f;
		v2Random= Vector2.zero;

	}

	private void randMove()
	{
		moveDt = 0;
		_moveDt = Random.Range(1f, 2f);
		stayDt = _stayDt = Random.Range(1f, 2f);
		v2Random.Set(Random.Range(0f, 1f), Random.Range(0f, 1f));
	}
}
