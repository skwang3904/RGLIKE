using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IMacro
{
	public static string[] DoorName = 
		{ "DoorLeft", "DoorRight", "DoorUp", "DoorDown" };

	public enum MAP_NAME
	{
		room_4way = 0,

		room_3way0,
		room_3way1,
		room_3way2,
		room_3way3,

		room_2way0,
		room_2way1,
		room_2way2,
		room_2way3,
		room_2way4,
		room_2way5,

		room_1way0,
		room_1way1,
		room_1way2,
		room_1way3,

		room_boss0,
		room_shop,

		MAX,
	}

	public static string[] MapName =
	{
		"room_4way",

		"room_3way0",
		"room_3way1",
		"room_3way2",
		"room_3way3",

		"room_2way0",
		"room_2way1",
		"room_2way2",
		"room_2way3",
		"room_2way4",
		"room_2way5",

		"room_1way0",
		"room_1way1",
		"room_1way2",
		"room_1way3",

		"room_boss0",
		"room_shop",
	};

	//---------------------------------------------------------------

	public static string[] ItemName =
	{
		"Gold",
		"Potion",
		"Gold",
		"Potion",
		"",
	};

	//---------------------------------------------------------------
	// variable

	public static Color color_White = Color.white;
	public static Color color_NoneAlpha = new Color(1,1,1,0);
	public static Color color_Black = Color.black;

}

//---------------------------------------------------------------
// enum

public enum EntityState
{
	idle = 0,
	move,
	attack,
	hurt,
	dead,
}

public enum NonEntityState
{
	NonAppear = 0,
	Appear,
	Idle,
	Active,
	Disappear,
	dead,

	Hurt,
}

// game object
public enum MapObjectType
{
	wall = 0,
	broken,
	trap,
	onoff,
	trigger,
}

public enum MapState
{
	nomal = 0,
	boss,
	shop,
}

public enum Item_Type
{
	Gold = 0,
	Potion,

	A, B, C, D, E, F, G, H, I, J,
	None,
	Max,
}

// inventory
public enum InventorySlotType
{
	Nomal = 0,
	Quick,
}

