using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : NonLivingEntity, IItem
{
	public static Item[,] items = null;
	public static int Max_itemNum = 10;
	public static void createItems()
	{
		if (items != null)
			return;
		items = new Item[(int)IMacro.Item_Name.Max, Max_itemNum];
	}

	public string strName;
	public Action useMethod;

	protected override void Awake()
	{
		base.Awake();

		initialize(0, Vector2.zero);
	}

	public void onUse()
	{
		useMethod?.Invoke();
	}

	public virtual void initialize(int mapNum, Vector2 position)
	{ 
		transform.position = new Vector2(-100,-100);
		state = NonEntityState.NonAppear;
		//mapNumber = mapNum;
		value = 0;

		isAppear = false;
		isActive = false;
		isDisappear = false;

		//
		strName = "";
	}

	//-------------------------------------------------------
	// item update function

	public void commonFunction()
	{
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
						//onUse();
						Inventory.instance.addItem(this);
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

	public void appearItem(int mapNum, Vector2 position)
	{
		transform.position = position;
		mapNumber = mapNum;
		setItemState(NonEntityState.Appear);
	}

	//-------------------------------------------------------
	// drop function

	public static void dropItem(int mapNum, Vector2 position, 
		IMacro.Item_Name index, int num)
	{
		int i, idx = (int)index;
		int n = 0;

		for (i = 0; i < Max_itemNum; i++) 
		{
			Item it = items[idx, i];
			if (it.state == NonEntityState.NonAppear)
			{
				it.appearItem(mapNum, position);
				n++;
			}

			if(i == Max_itemNum-1)
			{
				print("item num total over");
				break;
			}
			if (n >= num)
				break;
		}
	}
}
