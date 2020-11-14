using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;

    public Vector2 movement;
    public bool attacking;

	private void Awake()
	{
        player = GetComponent<Player>();

        movement = Vector2.zero;
	}

    void Update()
    {
        if (player.state == LivingEntity.entityState.dead ||
            player.state == LivingEntity.entityState.hurt)
            return;

        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        attacking = Input.GetButton("Fire1");
    }
}
