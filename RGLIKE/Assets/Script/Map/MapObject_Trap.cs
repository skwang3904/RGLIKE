using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject_Trap : MapObject
{
	protected override void Awake()
	{
		base.Awake();

		active += whenPlayerTouched;
	}

	private void FixedUpdate()
	{
		if (state == NonEntityState.dead)
			return; 

		if(touchBox.IsTouching(player.moveBox))
		{
			player.onDamage(value);
			active?.Invoke();
		}
	}

	public override void initialize(int mapNum, Vector2 position)
	{
		base.initialize(mapNum, position);

		state = NonEntityState.Idle;
		value = 5;
	}

	//------------------------------------------------------

	private void whenPlayerTouched()
	{
		state = NonEntityState.dead;
	}
}
