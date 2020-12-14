using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonLivingEntity : MonoBehaviour
{
	public Animator animator { get; protected set; }
	public SpriteRenderer spriteRenderer { get; protected set; }
	public BoxCollider2D touchBox { get; protected set; }

	public NonEntityState state;
	public int mapNumber;
	public int value; // item value <-> object dmg

	public bool isAppear;
	public bool isActive;
	public bool isDisappear;

	protected virtual void Awake()
	{
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		touchBox = GetComponent<BoxCollider2D>();
	}

	private void setAppearTrue() { isAppear = true; }
	private void setActiveTrue() { isActive = true; }
	private void setDisappearTrue() { isDisappear = true; }
}
