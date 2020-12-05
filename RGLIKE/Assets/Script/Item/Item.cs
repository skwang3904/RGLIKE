using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : NonLivingEntity, IItem
{
	public static List<Item> itemsBase;
	public static int Max_itemNum = 10;
	public IMacro.Item_Type type;

	public string strName;
	private delegate void onUseMethod(IMacro.Item_Type type);
	private onUseMethod useMethod;

	public static void createItems()
	{
		if (itemsBase != null)
			return;

		itemsBase = new List<Item>();
		UnityEngine.Object[] objs = Resources.LoadAll("Prefabs/Item");
		Item item;
		GameObject g;
		foreach(UnityEngine.Object o in objs)
		{
			g = Instantiate(o) as GameObject;
			item = g.GetComponent<Item>();
			itemsBase.Add(item);
			print("add pool itemsBase : [" + item + "]");
		}

		for (int i = 0; i < 10; i++) 
		{
			// inven sort test
			item = Instantiate(itemsBase[0]);
			item.type = IMacro.Item_Type.A + i;
			item.strName = Convert.ToChar(65 + i).ToString();
			itemsBase.Add(item);
		}
	}


	protected override void Awake()
	{
		base.Awake();
		useMethod += useItem;
	}

	public void onUse()
	{
		useMethod?.Invoke(type);
	}

	public virtual void initialize(int mapNum, IMacro.Item_Type type)
	{
		transform.position = new Vector2(-100, -100);
		state = NonEntityState.NonAppear;
		this.type = IMacro.Item_Type.None;
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
	public void setItemState(NonEntityState state)
	{
		this.state = state;
		animator.SetInteger("ItemState", (int)state);
	}

	public void appearItem(int mapNum, Vector2 position)
	{
		transform.position = position;
		mapNumber = mapNum;
		setItemState(NonEntityState.Appear);
	}

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
					if (touchBox.IsTouching(player.moveBox))
						setItemState(NonEntityState.Active);
					break;
				}
			case NonEntityState.Active:
				{
					if (isActive)
					{
						//onUse();
						Inventory.instance.addItemInventory(this);
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

	//-------------------------------------------------------
	// on use function
	
	public void useItem(IMacro.Item_Type type)
	{
		//#issue 추후 아이템 종류 추가시 수정
		switch (type)
		{
			case IMacro.Item_Type.Gold:
				{
					GameManager.instance.gold += value;
					break;
				}
			case IMacro.Item_Type.Potion:
				{
					player.hp += value;
					break;
				}
			case IMacro.Item_Type.None:
				{
					break;
				}
			default:
				break;
		}
	}

	//-------------------------------------------------------
	// drop function

	public static void dropItem(int mapNum, Vector2 position,
		IMacro.Item_Type index, int num)
	{
		int n = 0;
		foreach (Item it in itemsBase)
		{
			if (it.state == NonEntityState.NonAppear &&
				it.type == index)
			{
				Item item = Instantiate(it);
				item.appearItem(mapNum, position);
				n++;
			}

			if (n >= num)
				break;
		}
	}
}
