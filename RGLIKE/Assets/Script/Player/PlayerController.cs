using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;

    public Vector2 movement;
    public bool attacking;
    public bool moving;

	private void Awake()
	{
        player = Player.instance;
        movement = Vector2.zero;
	}

    void Update()
    {
        if (!LivingEntity.LivingTime)
            return;

        if (GameManager.instance.isPassMap())
		{
            allInputZero();
            return;
		}

        if (player.state == EntityState.dead ||
            player.state == EntityState.hurt)
		{
            allInputZero();
            return;
		}
        movement.Set(0, 0);
        if (player.state != EntityState.attack)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            movement.Normalize();
        }
        
        // #issue 방향 입력 유지되는것 수정
        moving = movement != Vector2.zero; 

        attacking = Input.GetButton("Fire1");
        if (attacking)
		{
            moving = false;
            movement.Set(0, 0);
        }
        
    }

    private void allInputZero()
	{
        movement.Set(0, 0);
        attacking = false;
        moving = false;
    }
}
