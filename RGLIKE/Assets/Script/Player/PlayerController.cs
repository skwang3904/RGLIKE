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
        player = GetComponent<Player>();
        
        movement = Vector2.zero;
	}

    void Update()
    {
        if (GameManager.instance.isPassMap())
		{
            allInputZero();
            return;
		}

        if (player.state == LivingEntity.entityState.dead ||
            player.state == LivingEntity.entityState.hurt)
		{
            allInputZero();
            return;
		}

        if(player.state == LivingEntity.entityState.attack)
		{
            movement.Set(0, 0);
		}
        else
        {
            //movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");
        }
        movement.Normalize();
        
        // #issue 방향 입력 유지되는것 수정
        moving = movement != Vector2.zero; 

        attacking = Input.GetButton("Fire1");
    }

    private void allInputZero()
	{
        movement.Set(0, 0);
        attacking = false;
        moving = false;
    }
}
