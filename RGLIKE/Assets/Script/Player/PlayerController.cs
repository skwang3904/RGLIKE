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
        if (player.state == LivingEntity.entityState.dead ||
            player.state == LivingEntity.entityState.hurt)
            return;

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

        moving = movement != Vector2.zero; 

        attacking = Input.GetButton("Fire1");
    }
}
