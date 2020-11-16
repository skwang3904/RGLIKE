using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : LivingEntity, IDamageable, IInitialize
{
    private PlayerController pctrl;
    private BoxCollider2D attackBox;
    private Vector2 attackBoxOffset;
    private Vector2 attackBoxSize;
     
    private float aniX, aniY;
    private int currPattenIndex;
    private int nextPattenIndex;
    private bool nextPattern;

    protected override void Awake()
	{
        base.Awake();

        pctrl = GetComponent<PlayerController>();
        attackBox = GetComponents<BoxCollider2D>()[1];
        attackBox.enabled = false;
        attackBoxOffset = attackBox.offset;
        attackBoxSize = new Vector2(3, 1);

        currPattenIndex = -1;
        nextPattenIndex = -1;
        nextPattern = false;
    }

	void Start()
    {
        
    }

    void Update()
    {
        // move 입력
        aniX = pctrl.movement.x;
        aniY = pctrl.movement.y;
        if (aniY != 0f)
            aniX = 0;
        animator.SetFloat("moveX", aniX);
        animator.SetFloat("moveY", aniY);
        animator.SetBool("moving", pctrl.moving);

        if(aniY != 0)
		{
            if (aniY > 0)   attackBoxOffset.Set(0, 0.5f);
            else            attackBoxOffset.Set(0, -1);
            attackBoxSize.Set(3, 1.5f);
		}
        else if(aniX != 0)
		{
            if (aniX > 0)   attackBoxOffset.Set(1, 0);
            else            attackBoxOffset.Set(-1, 0);
            attackBoxSize.Set(1.5f, 3);
        }

        switch (state)
		{
            case entityState.idle:
            case entityState.move:
                {
                    if (pctrl.attacking)
                    {
                        state = entityState.attack;
                        attackBox.enabled = true;
                        nextPattenIndex = 0;
                        rotateAttackBox();
                        setAttackPatten();
                    }
                    break;
                }
            case entityState.attack:
			    {
                    if(nextPattern)
					{
                        if (pctrl.attacking)
                        {
                            nextPattern = false;
                            nextPattenIndex++;
                            print("next");
                        }
                    }
                    break;
			    }
        }


        // test
        if(Input.GetKeyDown(KeyCode.Escape))
		{
            if (state != entityState.dead)
			{
                onDamage(10000);
			}
		}

    }

	private void FixedUpdate()
	{
        rigid.MovePosition((Vector2)transform.position 
            + (pctrl.movement * moveSpeed * Time.deltaTime));
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(collision.tag == "Monster")
		{
            // take damage
            attackBox.enabled = false;
            IDamageable go = collision.GetComponent<IDamageable>();
            if (go != null)
                go.onDamage(dmg);
            else
                print("player_OnTrigger_onDamage_type error");
        }

    }

    //---------------------------------------------------
    // Interface
    public void onDamage(float damage)
	{
        if (state == entityState.dead)
            return;

        hp -= damage;
        if (hp < 0)
		{
            hp = 0;
            state = entityState.dead;
		}

        animator.SetTrigger("Die");
	}

    public void initialize(int mapNum)
	{
        mapNumber = mapNum;

        hp = _hp = 100;
        dmg = _dmg = 10;
        attackDt = _attackDt = 1;
        moveSpeed = 10;
    }


    //---------------------------------------------------
    // State function
    private void setStateIdle()
	{
        state = entityState.idle;
	}

    //---------------------------------------------------
    // Attack function
    private void rotateAttackBox()
	{
        attackBox.offset = attackBoxOffset;
        attackBox.size = attackBoxSize;
    }

	private void nextPattenTrue()
	{
        nextPattern = true;
    }

    private void setAttackPatten()
	{
        if (currPattenIndex == nextPattenIndex)
		{
            initAttackPattern();
            return;
		}
        if (nextPattenIndex >= 3)
            return;

        attackBox.enabled = true;
        nextPattern = false;
        currPattenIndex = nextPattenIndex;

        animator.SetInteger("AttackPattern", currPattenIndex);
    }

    private void initAttackPattern()
	{
        attackBox.enabled = false;
        nextPattern = false;
        currPattenIndex = -1;
        nextPattenIndex = -1;

        animator.SetInteger("AttackPattern", currPattenIndex);
    }

    //---------------------------------------------------
    // else function
}
