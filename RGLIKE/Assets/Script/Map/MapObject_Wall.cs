using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject_Wall : MapObject
{
	protected override void Awake()
	{
		base.Awake();

		type = MapObjectType.trigger;
		active += wallOpen;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.collider.tag == "Player")
		{
			if (GameManager.instance.gold < value)
			{
				return;
			}
			else
			{
				GameManager.instance.gold -= value;
				active?.Invoke();
			}
		}
	}

	public override void initialize(int mapNum, Vector2 position)
	{
		base.initialize(mapNum, position);

		state = NonEntityState.Idle;
		value = 3;
	}
	//------------------------------------------

	private void wallOpen()
	{
		animator.SetTrigger("isOpen");
		touchBox.enabled = false;
	}
}
