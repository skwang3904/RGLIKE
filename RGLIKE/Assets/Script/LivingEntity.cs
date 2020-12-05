using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
	public static bool LivingTime;
	public float timeScale;

	public Rigidbody2D rigid { get; protected set; }
	public Animator animator { get; protected set; }
	public SpriteRenderer spriteRenderer { get; protected set; }
	public BoxCollider2D hitBox { get; protected set; }
	public BoxCollider2D moveBox { get; protected set; }
	public ParticleSystem particle { get; protected set; }

	public EntityState state;
	public int mapNumber;
	public float hp, _hp;
	public float dmg, _dmg;
	public float attackDt, _attackDt; // 공격속도 : animation 속도
	public float moveSpeed;

	//public float criticalChance, _criticalChance;
	//public float evasionChance, _evasionChance;
	public Action deadMethod;

	protected virtual void Awake()
	{
		LivingTime = true;
		timeScale = 1f;

		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		
		BoxCollider2D[] box = GetComponents<BoxCollider2D>();
		hitBox = box[0];
		moveBox = box[1];

		particle = GetComponent<ParticleSystem>();

		StartCoroutine("crtShadowAdd");
	}

	public IEnumerator crtShadowAdd()
	{
		while(true)
		{
			if(Shadow.instance != null)
			{
				Shadow.instance.addShadow(this);
				break;
			}
			yield return null;
		}
	}
	public IEnumerator crtHurtEffect()
	{
		Color c = Color.white;
		Color cc = new Color(0.5f, 0, 0, 1);
		Color color;
		float hurtDt = 0f;
		float _hurtDt = 0.3f;
		float d;
		while (true)
		{
			hurtDt += livingDeltaTime();
			if (hurtDt > _hurtDt)
				hurtDt = _hurtDt;

			d = Mathf.Abs(Mathf.Sin(hurtDt / _hurtDt * 720 * Mathf.Deg2Rad));
			color = Color.Lerp(cc, c, d);
			spriteRenderer.color = color;
			
			if(hurtDt == _hurtDt)
			{
				spriteRenderer.color = Color.white;
				break;
			}

			yield return null;
		}
	}

	//-------------------------------------------------------
	// Time Function
	public float livingDeltaTime()
	{
		return Time.deltaTime * (LivingTime ? 1 : 0) * timeScale;
	}

	public static void EntityTime(bool work)
	{
		LivingTime = work;
	}

	//-------------------------------------------------------
	// State Function
	private void setStateIdle()	{state = EntityState.idle;}
	private void stateToMove() {state = EntityState.move;}
}
