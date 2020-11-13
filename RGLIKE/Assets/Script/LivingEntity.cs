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

    public int mapNumber { get; set; }
}
