using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : NonLivingEntity, IItem
{
	public static List<Item> itemList;
	public static int Max_itemNum = 10;
	public Item_Type type;
	
	public string strName;
	public string strInfomation;
	public string strUseEffect;
	private delegate void onUseMethod(Item_Type type);
	private onUseMethod useMethod;

	public static void createItems()
	{
		if (itemList != null)
			return;

		itemList = new List<Item>();
		UnityEngine.Object[] objs = Resources.LoadAll("Prefabs/Item");
		Item item;
		GameObject g;
		foreach(UnityEngine.Object o in objs)
		{
			g = Instantiate(o) as GameObject;
			item = g.GetComponent<Item>();
			itemList.Add(item);
			print("add pool itemList : [" + item + "]");
		}

		for (int i = 0; i < 10; i++) 
		{
			// inven sort test
			item = Instantiate(itemList[0]);
			item.type = Item_Type.A + i;
			item.strName = Convert.ToChar(65 + i).ToString();
			itemList.Add(item);
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

	public virtual void initialize(int mapNum, Item_Type type)
	{
		transform.position = new Vector2(-100, -100);
		state = NonEntityState.NonAppear;
		this.type = Item_Type.None;
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
					if (touchBox.IsTouching(Player.instance.moveBox))
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
	
	public void useItem(Item_Type type)
	{
		//#issue 추후 아이템 종류 추가시 수정
		switch (type)
		{
			case Item_Type.Gold:
				{
					GameManager.instance.gold += value;
					break;
				}
			case Item_Type.Potion:
				{
					Player.instance.hp += value;
					break;
				}
			case Item_Type.None:
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
		Item_Type index, int num)
	{
		int n = 0;
		foreach (Item it in itemList)
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
