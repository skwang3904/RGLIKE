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
		if (animator.GetBool("OnOff") != OnOff)
			return;

		if(touchBox.IsTouching(player.hitBox))
		{
			print("touch");
			if (Input.GetKeyDown(KeyCode.J))
			{
				animator.SetBool("OnOff", !OnOff);
			}
		}
	}

	//------------------------------------------------------

	private void whenLeverOn()
	{
		Item.dropItem(mapNumber, (Vector2)transform.position + Random.insideUnitCircle * 2 ,
			IMacro.Item_Name.Gold, 1);
	}

	private void aniLeverCtrl()
	{
		OnOff = !OnOff;
		if (OnOff)
			active?.Invoke();
	}
}
