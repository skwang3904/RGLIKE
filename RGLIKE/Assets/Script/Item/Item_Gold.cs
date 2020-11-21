using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Gold : Item
{
	protected override void Awake()
	{
		base.Awake();

		strName = IMacro.ItemName[(int)IMacro.Item_Name.Gold];
		useMethod += plusGold;
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

		setItemState(NonEntityState.Appear);
		value = Random.Range(1, 3);
	}

	private void plusGold()
	{
		GameManager.instance.gold += value;
	}
}
