using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Potion : Item
{
	protected override void Awake()
	{
		base.Awake();

	}

	private void Update()
	{
		if (state == NonEntityState.NonAppear ||
			state == NonEntityState.dead)
			return;

		commonFunction();
	}

	public override void initialize(int mapNum, Vector2 position)
	{
		base.initialize(mapNum, position);

		//setItemState(NonEntityState.Appear);
		strName = IMacro.ItemName[(int)IMacro.Item_Name.Potion];
		value = 10;
		useMethod += healthPlayer;
	}

	private void healthPlayer()
	{
		player.hp += value;
	}
}
