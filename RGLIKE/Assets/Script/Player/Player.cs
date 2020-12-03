using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class Player : LivingEntity, IDamageable, IInitialize
{
    public static Player instance;

    private List<KeyCode> list_operationKey;

    private ParticleSystem particle;
    // controller
    private PlayerController pctrl;
    private Vector2 aniMovement;

    //attacks
    private BoxCollider2D attackBox;
    private Vector2 attackBoxOffset;
    private Vector2 attackBoxSize;
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

        particle = GetComponent<ParticleSystem>();
        pctrl = GetComponent<PlayerController>();
        aniMovement = Vector2.zero;

        attackBox = GetComponents<BoxCollider2D>()[1];
        attackBox.enabled = false;
        attackBoxOffset = attackBox.offset;
        attackBoxSize = new Vector2(3, 1);
        currPattenIndex = -1;
        nextPattenIndex = -1;
        nextPattern = false;
    }

    private void Update()
    {
        if (state == EntityState.dead)
            return;

        setAnimator();

        mainLogic();

        getOperationKeys();
    }

	private void FixedUpdate()
	{
        float dt = livingDeltaTime(timeScale);

#if true
        rigid.MovePosition((Vector2)transform.position
            + pctrl.movement * moveSpeed * dt);

#else // move test
        rigid.velocity = (pctrl.movement * moveSpeed * dt
            * (pctrl.moving ? 1 : 0));
#endif
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if (pctrl.attacking)
        {
            if (collision.tag == "Monster")
            {
                // take damage
                attackBox.enabled = false;
                IDamageable go = collision.GetComponent<IDamageable>();
                go.onDamage(dmg);
            }

            else if (collision.tag == "MapObject")
			{
                attackBox.enabled = false;
                IDamageable go = collision.GetComponent<IDamageable>();
                go.onDamage(dmg);
            }
        }
    }

    //---------------------------------------------------
    // Interface
    public void onDamage(float damage)
	{
        if (state == EntityState.dead)
            return;

        particle.Play();

        hp -= damage;
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
                        rotateAttackBox();
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
    private void rotateAttackBox()
    {
        // #issue 히트박스 회전부분 개선필요
        if (aniMovement.y != 0)
        {
            if (aniMovement.y > 0) attackBoxOffset.Set(0, 0.5f);
            else attackBoxOffset.Set(0, -1);
            attackBoxSize.Set(3, 1.5f);
        }
        else if (aniMovement.x != 0)
        {
            if (aniMovement.x > 0) attackBoxOffset.Set(1, 0);
            else attackBoxOffset.Set(-1, 0);
            attackBoxSize.Set(1.5f, 3);
        }

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
