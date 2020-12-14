using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storekeeper : NonLivingEntity
{
	private struct SellItems
	{
		public Item item;
		public int price;

		public void set(Item it, int pr)
		{
			item = it;
			price = pr;
		}
	}
	private SellItems[] sells;

	protected override void Awake()
	{
		base.Awake();
	}

	private void Update()
	{
		foreach(SellItems si in sells)
		{
			Item it = si.item;
			if(Player.instance.moveBox.IsTouching(si.item.touchBox))
			{
				if(Input.GetKeyDown(KeyCode.J))
				{
					saleItem(si);
				}
			}
		}
	}

	//-------------------------------------------------------

	public void init(int mapNum)
	{
		mapNumber = mapNum;
		Map m = GameManager.instance.maps[mapNumber];

		Transform tfNpc = m.transform.Find("NpcSpawn");
		transform.position = tfNpc.position;

		Transform objSpawn = m.transform.Find("ObjectSpawn");
		int num = objSpawn.childCount;

		sells = new SellItems[num];
		for(int i=0; i<num;i++)
		{
			sells[i] = new SellItems();
			ref SellItems si = ref sells[i];

			Item it = Instantiate(Item.itemList[Random.Range(0, 2)]);
			it.transform.position = objSpawn.GetChild(i).transform.position;
			it.setItemState(NonEntityState.Appear);
			si.set(it, Random.Range(1, 3));
		}
	}

	private void saleItem(SellItems si)
	{
		if (GameManager.instance.gold < si.price)
		{
			print("not enough money");
			return;
		}

		GameManager.instance.gold -= si.price;
		si.item.setItemState(NonEntityState.Active);
	}
}
