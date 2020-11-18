﻿using System.Collections;
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
	};

	//---------------------------------------------------------------

	public enum Item_Name
	{
		Gold = 0,
		Potion,

		Max,
	}

	public static string[] ItemName =
	{
		"Gold",
		"Potion",

	};

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