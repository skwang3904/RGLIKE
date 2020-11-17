using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
	public Rigidbody2D rigid { get; protected set; }
	public Animator animator { get; protected set; }
	public SpriteRenderer spriteRenderer { get; protected set; }
	public BoxCollider2D hitBox { get; protected set; }
	public BoxCollider2D moveBox { get; protected set; }
	//private ParticleSystem particle;

	public EntityState state;
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

		BoxCollider2D[] box = GetComponents<BoxCollider2D>();
		hitBox = box[0];
		moveBox = box[1];
	}
}
