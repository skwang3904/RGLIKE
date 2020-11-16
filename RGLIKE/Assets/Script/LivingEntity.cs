using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
	protected Rigidbody2D rigid;
	protected Animator animator;
	protected SpriteRenderer spriteRenderer;
	protected BoxCollider2D hitBox;
	//private ParticleSystem particle;

	public enum entityState
	{
		idle = 0,
		move,
		attack,
		hurt,
		dead,
	}

	public entityState state;
	public int mapNumber;
	public float hp, _hp;
	public float dmg, _dmg;
	public float attackDt, _attackDt; // 공격속도 : animation 속도
	public float moveSpeed;

	//public float criticalChance, _criticalChance;
	//public float evasionChance, _evasionChance;

	protected virtual void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		hitBox = GetComponent<BoxCollider2D>();
	}

}
