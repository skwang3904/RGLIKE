using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : NonLivingEntity, IItem
{
	public static int Max_itemNum = 10;
	public Vector2 appearPosition;
	public float appearHeight;

	public Action useMethod;

	protected override void Awake()
	{
		base.Awake();
	}

	public void onUse()
	{
		useMethod?.Invoke();
	}

	public virtual void initialize(int mapNum, Vector2 position)
	{
		transform.position = position;
		state = NonEntityState.NonAppear;
		mapNumber = mapNum;
		value = 0;

		isAppear = false;
		isActive = false;
		isDisappear = false;
	}

	//-------------------------------------------------------

	public static void monsterDropItem(int mapNum, Vector2 position, 
		IMacro.Item_Name index, int num)
	{
		int i, idx = (int)index;
		GameManager g = GameManager.instance;

		int n = 0;

		print(Max_itemNum);
		for (i = 0; i < Max_itemNum; i++) 
		{
			Item it = g.items[idx, i];
			if (it.state == NonEntityState.NonAppear)
			{
				it.initialize(mapNum, position);
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
