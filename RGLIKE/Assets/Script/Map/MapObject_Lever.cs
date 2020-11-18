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

	private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.tag == "Player")
		{
			if(Input.GetKeyDown(KeyCode.J))
			{
				OnOff = !OnOff;
				animator.SetBool("", OnOff);
				if (OnOff)
					active?.Invoke();
			}
		}
	}
	//------------------------------------------------------

	private void whenLeverOn()
	{
		Item.monsterDropItem(mapNumber, (Vector2)transform.position + Random.insideUnitCircle * 2 ,
			IMacro.Item_Name.Gold, 1);
	}
}
