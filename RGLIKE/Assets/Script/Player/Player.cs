using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : LivingEntity
{
    private PlayerController pctrl;
    private BoxCollider2D attackBox;

    private float aniX, aniY;
    private bool[] attackPatten;
    private int currPattenIndex;
    private int nextPattenIndex;
    private bool nextPattern;

    protected override void Awake()
	{
        base.Awake();

        pctrl = GetComponent<PlayerController>();
        attackBox = GetComponents<BoxCollider2D>()[1];
        attackBox.enabled = false;

        attackPatten = new bool[3];
        currPattenIndex = 0;
        nextPattenIndex = 0;
        nextPattern = false;
    }

	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        aniX = pctrl.movement.x;
        aniY = pctrl.movement.y;
        if (aniY != 0f)
            aniX = 0;
        animator.SetFloat("", aniX);
        animator.SetFloat("", aniY);

		switch (state)
		{
            case entityState.idle:
            case entityState.move:
                {
                    if (pctrl.attacking)
                    {
                        state = entityState.attack;
                        attackBox.enabled = true;
                        currPattenIndex = 1;
                        nextPattenIndex = 1;
                    }
                    break;
                }
            case entityState.attack:
			    {
                    if(nextPattern)
					{
                        nextPattern = false;
                        nextPattenIndex++;
                    }
                    break;
			    }
        }

    }

	private void FixedUpdate()
	{
        rigid.MovePosition((Vector2)transform.position + pctrl.movement);
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(collision.tag == "Monster")
		{
            // take damage
            attackBox.enabled = false;

        }

    }


	private void nextPattenTrue()
	{
        nextPattern = true;
    }
    private void setAttackPatten()
	{
        if (currPattenIndex == nextPattenIndex)
            return;
        if (nextPattenIndex >= 3)
            return;

        attackBox.enabled = true;

        currPattenIndex = nextPattenIndex;

        // 애니메이터 트랜지션
        animator.SetInteger("", currPattenIndex);

    }
    private void initAttackPattern()
	{
        attackBox.enabled = false;
        
        nextPattern = false;
        currPattenIndex = 0;
        nextPattenIndex = 0;
    }

}
