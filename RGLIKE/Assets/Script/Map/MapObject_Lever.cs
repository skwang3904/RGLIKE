using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject_Lever : MapObject
{
	public bool OnOff;
	protected override void Awake()
	{
		base.Awake();

		active += whenLeverOn;
	}

	private void FixedUpdate()
	{
		if(touchBox.IsTouching(player.hitBox))
		{
			if (Input.GetKeyDown(KeyCode.J))
			{
				OnOff = !OnOff;
				animator.SetBool("OnOff", OnOff);
				if(OnOff)
					active?.Invoke();
			}
		}
	}

	//------------------------------------------------------

	private void whenLeverOn()
	{
		Item.dropItem(mapNumber,
			(Vector2)transform.position + Random.insideUnitCircle * 3,
			IMacro.Item_Type.Gold, 1);
	}
}
