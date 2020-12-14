using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject_Barrel : MapObject
{
	protected override void Awake()
	{
		base.Awake();

		active += dropItemWhenBroken;
	}

	public override void initialize(int mapNum, Vector2 position)
	{
		base.initialize(mapNum, position);

		type = MapObjectType.broken;

		state = NonEntityState.Idle;
		hp = _hp = 2;
	}

	public override void onDamage(LivingEntity entity)
	{
		base.onDamage(entity);

		animator.SetTrigger("Hurt");
	}

	//--------------------------------------------------

	private void aniBarrelHpDecrease()
	{
		animator.SetInteger("Hp", hp);
	}

	//--------------------------------------------------

	private void dropItemWhenBroken()
	{
		Item.dropItem(mapNumber, transform.position, 
			Item_Type.Gold, 1);
	}


}
