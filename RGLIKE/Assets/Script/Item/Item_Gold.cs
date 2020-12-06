using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Gold : Item
{
	protected override void Awake()
	{
		base.Awake();
		initialize(0, IMacro.Item_Type.Gold);

	}

	private void Update()
	{
		if (state == NonEntityState.NonAppear ||
			state == NonEntityState.dead)
			return;

		commonFunction();
	}

	public override void initialize(int mapNum, IMacro.Item_Type type)
	{
		base.initialize(mapNum, type);

		//setItemState(NonEntityState.Appear);
		this.type = type;
		value = 1;
		strName = IMacro.ItemName[(int)IMacro.Item_Type.Gold];
		strInfomation = "Gold Coin";
		strUseEffect = "Gold +" + value;
	}
}
