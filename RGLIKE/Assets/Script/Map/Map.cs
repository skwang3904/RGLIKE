using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	private GameObject[] doors; // 맵의 각 문
	private SpriteRenderer[] doorSpriteRenderers; // 각 문의 스프라이트렌더러
	private Sprite[,] doorSprites; // 문의 총 스프라이트
	private int[] doorIndex; // 문의 각 인덱스

	public int mapNumber;

	private void Awake()
	{
		int i, j;
		doors = new GameObject[4];
		doorSpriteRenderers = new SpriteRenderer[4];
		doorSprites = new Sprite[4,3];
		doorIndex = new int[4];
		for (i = 0; i < doorIndex.Length; i++)
			doorIndex[i] = 0;

		GameObject doorParent = transform.Find("Door").gameObject;
		GameObject g;
		for (i = 0; i < 4; i++) 
		{
			// 문이 없으면
			Transform t = doorParent.transform.Find(IMacro.DoorName[i]);
			if (t == null) 
			{
				doors[i] = null;
				doorSpriteRenderers[i] = null;
				for (j = 0; j < 3; j++)
					doorSprites[i, j] = null;
				continue;
			}
			g = t.gameObject;

			// 문 있으면 생성
			GameObject obj = new GameObject();
			obj.name = IMacro.DoorName[i] + "Sprite";
			obj.transform.SetParent(g.transform);
			obj.transform.localPosition = Vector3.zero;
			if (i < 2)	obj.transform.localScale = new Vector3(1.5f, 3f, 0);
			else		obj.transform.localScale = new Vector3(3f, 1.5f, 0);

			SpriteRenderer spr = obj.AddComponent<SpriteRenderer>();
			for (j = 0; j < 3; j++)
			{
				Sprite sp = Resources.Load<Sprite>("Sprite/Map/Doors/door_" + i + "_" + j);
				if (sp == null) print("MapDoor Load error");
				
				doorSprites[i, j] = sp;
			}
			spr.sprite = doorSprites[i, doorIndex[i]];
			spr.sortingLayerName = "MapDoor";
			if (spr.sortingLayerID == 0)
				print("sortLayerID setting error");


			doorSpriteRenderers[i] = spr;
			doors[i] = g;
		}
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.J))
		{
			for (int i = 0; i < 4; i++) 
			{
				if (doorSpriteRenderers[i] == null) continue;

				doorSpriteRenderers[i].sprite = doorSprites[i, doorIndex[i]++];
				if (doorIndex[i] > 2)
					doorIndex[i] = 0;
			}
		}
	}
}
