using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject_SpikeTrap : MapObject
{
	public bool isTouch;

	protected override void Awake()
	{
		base.Awake();

		type = MapObjectType.trap;
		active += whenPlayerTouched;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			if (isActive == false)
			{
				isActive = true; // 애니메이션용
				animator.SetBool("isActive", true);
			}

			if (isTouch) // isTouch == hit player
				active?.Invoke();
		}
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		isActive = false;
	}

	public override void initialize(int mapNum, Vector2 position)
	{
		base.initialize(mapNum, position);

		state = NonEntityState.Idle;
		value = 5;
	}

	//------------------------------------------------------
	private void spikeTrapActive() { isTouch = true; }
	private void spikeTrapInactive() { isTouch = false;	}
	private void aniIsActiveFalse() { animator.SetBool("isActive", false); }

	private void whenPlayerTouched()
	{
		player.onDamage(value);
		isTouch = false;
	}
}
