using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Gold : Item
{
	protected override void Awake()
	{
		base.Awake();

		useMethod += plusGold;
	}

	private void Update()
	{
		if (state == NonEntityState.NonAppear ||
			state == NonEntityState.dead)
			return;

		switch (state)
		{
			case NonEntityState.NonAppear: break;
			case NonEntityState.Appear:
				{
					if (isAppear)
						setItemState(NonEntityState.Idle);
					break;
				}
			case NonEntityState.Idle:
				{
					// if use 
					// setItemState(NonEntityState.Active);
					if (touchBox.IsTouching(player.hitBox))
						setItemState(NonEntityState.Active);
					break;
				}
			case NonEntityState.Active:
				{
					if (isActive)
					{
						onUse();
						setItemState(NonEntityState.Disappear);
					}
					break;
				}
			case NonEntityState.Disappear:
				{
					// end
					if (isDisappear)
					{
						setItemState(NonEntityState.dead);
						gameObject.SetActive(false);
					}
					break;
				}
		}
	}

	public override void initialize(int mapNum, Vector2 position)
	{
		base.initialize(mapNum, position);

		setItemState(NonEntityState.Appear);
		value = Random.Range(1, 3);
	}

	private void plusGold()
	{
		GameManager.instance.gold += value;
	}
}
