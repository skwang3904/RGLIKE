using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class Player : LivingEntity, IDamageable, IInitialize
{
    public static Player instance;

    private List<KeyCode> list_operationKey;

    // controller
    private PlayerController pctrl;
    private Vector2 aniMovement;

    //attacks
    private BoxCollider2D attackBox;
    private int currPattenIndex;
    private int nextPattenIndex;
    private bool nextPattern;

    private float knockbackDistance;

    protected override void Awake()
	{
        if (instance == null)
            instance = this;
        else if (instance != null && instance != this)
            Destroy(gameObject);

        base.Awake();

        list_operationKey = new List<KeyCode>();
        addOperationKey();

        pctrl = GetComponent<PlayerController>();
        aniMovement = Vector2.zero;

        attackBox = GetComponents<BoxCollider2D>()[0];
        attackBox.enabled = false;
        currPattenIndex = -1;
        nextPattenIndex = -1;
        nextPattern = false;

        knockbackDistance = 1;
    }

    private void Update()
    {
        if (state == EntityState.dead)
            return;

        setAnimator();

        mainLogic();

        getOperationKeys();

        if(state == EntityState.attack)
		{
            foreach(Monster m in GameManager.instance.monsters)
			{
                if(hitBox.IsTouching(m.hitBox))
				{
                    m.onDamage(this);
                    attackBox.enabled = false;
                    break;
                }
			}
		}
    }

	private void FixedUpdate()
	{
        float dt = livingDeltaTime();

#if true
        rigid.MovePosition((Vector2)transform.position
            + pctrl.movement * moveSpeed * dt);

#else // move test
        rigid.velocity = (pctrl.movement * moveSpeed * dt
            * (pctrl.moving ? 1 : 0));
#endif
    }

#if false
    private void OnTriggerEnter2D(Collider2D collision)
	{
        if (pctrl.attacking)
        {
            if (collision.tag == "Monster")
            {
                // take damage
                attackBox.enabled = false;
                IDamageable go = collision.GetComponent<IDamageable>();
                go.onDamage(this);
            }

            else if (collision.tag == "MapObject")
			{
                attackBox.enabled = false;
                IDamageable go = collision.GetComponent<IDamageable>();
                go.onDamage(this);
            }
        }
    }
#else

#endif

    //---------------------------------------------------
    // Interface
    public void onDamage(LivingEntity entity)
	{
        if (state == EntityState.dead)
            return;

        particle.Play();

        StartCoroutine("crtHurtEffect");

#if true
        if (entity == null)
        {
            hp--;
        }
#else
        hp -= entity.dmg;
#endif
        if (hp < 0)
		{
            hp = 0;
            state = EntityState.dead;
            animator.SetTrigger("Die");

            deadMethod?.Invoke();
        }
	}

    public void initialize(int mapNum)
	{
        state = EntityState.idle;

        PlayerData pd = LevelData.instance.playerData;
        mapNumber = pd.mapNumber;
        hp = pd.hp;
        _hp = pd._hp;
        dmg = pd.dmg;
        _dmg = pd._dmg;
        attackDt = pd.attackDt;
        _attackDt = pd._attackDt;
        moveSpeed = pd.moveSpeed;
    }

    //---------------------------------------------------
    // create -> awake에서 호출할 함수
    public void create(Vector2 position)
	{ 
        // 레벨 시작 시 호출
        transform.position = position;

        initialize(0);
    }

    private void addOperationKey()
	{
        list_operationKey.Add(KeyCode.Escape);
        list_operationKey.Add(KeyCode.Tab);
        list_operationKey.Add(KeyCode.I);
    }

    //---------------------------------------------------
    // Main Logic
    private void setAnimator()
    {
        // move 입력
        aniMovement = pctrl.movement;

        if (aniMovement.y != 0f)
            aniMovement.x = 0;
        animator.SetFloat("moveX", aniMovement.x);
        animator.SetFloat("moveY", aniMovement.y);
        animator.SetBool("moving", pctrl.moving);
    }

    private void mainLogic()
	{
        switch (state)
        {
            case EntityState.idle:
            case EntityState.move:
                {
                    if (pctrl.attacking)
                    {
                        state = EntityState.attack;
                        nextPattenIndex = 0;
                        setAttackPatten();
                    }
                    break;
                }
            case EntityState.attack:
                {
                    if (nextPattern)
                    {
                        if (pctrl.attacking)
                        {
                            nextPattern = false;
                            nextPattenIndex++;
                        }
                    }
                    break;
                }
            default:
                print("pley state setting error");
                break;
        }
    }

    private void getOperationKeys()
    {
        // test
        foreach(KeyCode key in list_operationKey)
		{
            if (Input.GetKeyDown(key))
            {
                switch (key)
                {
                    case KeyCode.Escape:
                        {
                            break;
                        }
                    case KeyCode.Tab:
                        {
                            UIManager.instance.miniMapSizing();
                            break;
                        }
                    case KeyCode.I:
                        {
                            Inventory.instance.inventoryOpenClose();
                            break;
                        }
                    default:
                        print("player key list : not added keys");
                        break;
                }
            }
		}
    }

    //---------------------------------------------------
    // Attack function

	private void nextPattenTrue()
	{
        nextPattern = true;
        attackBox.enabled = true;
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

        attackBox.enabled = false;
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

    public int currAttackPattern()
	{
        return currPattenIndex;
	}

    public Vector2 knockbackMonster(Vector2 v)
	{
        v *= knockbackDistance;
        return v;
	}

    //---------------------------------------------------
    // else function
}
