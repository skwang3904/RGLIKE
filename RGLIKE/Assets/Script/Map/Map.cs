using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	private GameObject[] doors; // 맵의 각 문 // 딱히 없어도될듯?
	private BoxCollider2D[] doorsCollider;
	private SpriteRenderer[] doorSpriteRenderers; // 각 문의 스프라이트렌더러
	private Sprite[,] doorSprites; // 문의 총 스프라이트 => 하나로 만들기
	private int[] doorIndex; // 문의 각 인덱스 0:close, 1:open, 2:closeKey

	public MapState state;
	public int mapNumber;

	private Player player;
	private Monster[] monsters; // 이 맵에 있는 몬스터

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		int i, j;
		doors = new GameObject[4];
		doorsCollider = new BoxCollider2D[4];
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
				doorsCollider[i] = null;
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

			for (j = 0; j < 3; j++)
			{
				Sprite sp = Resources.Load<Sprite>("Sprite/Map/Doors/door_" + i + "_" + j);
				if (sp == null) print("MapDoor Load error");
				
				doorSprites[i, j] = sp;
			}

			SpriteRenderer spr = obj.AddComponent<SpriteRenderer>();
			spr.sprite = doorSprites[i, doorIndex[i]];
			spr.sortingLayerName = "MapDoor";
			if (spr.sortingLayerID == 0)
				print("sortLayerID setting error");


			doors[i] = g;
			doorsCollider[i] = g.GetComponent<BoxCollider2D>();
			doorSpriteRenderers[i] = spr;
		}
	}

	private void Start()
	{
		int i, j = 0;
		player = GameManager.instance.player;

		// 이 맵번호와 같은 맵번호를 가진 몬스터를 검사하여 문을 열거나 닫음
		int mNum = transform.Find("MonsterSpawn").childCount;

		if (mapNumber != player.mapNumber)
		{
			monsters = new Monster[mNum];
			GameObject mons = GameObject.Find("Monsters");
			mNum = mons.transform.childCount;
			for (i = 0; i < mNum; i++)
			{
				Monster m = mons.transform.GetChild(i).GetComponent<Monster>();
				if (m.mapNumber == mapNumber)
					monsters[j++] = m;
			}
		}

		if(state == MapState.boss ||
			state == MapState.shop)
		{
			LevelData ld = LevelData.instance;
			ref bool[] check = ref ld.mapData.maps;
			int sqrt = ld.mapData.mapTotalSqrt;
			int n = mapNumber;
			
			// 보스방 & 상점 검사
			if (n % sqrt == 0		 || check[n - 1] == false) doorLocked(0);
			if (n % sqrt == sqrt - 1 || check[n + 1] == false) doorLocked(1);
			if (n / sqrt == sqrt - 1 || check[n + sqrt] == false) doorLocked(2);
			if (n / sqrt == 0		 || check[n - sqrt] == false) doorLocked(3);
		}
	}

	private void Update()
	{
		if (mapNumber != player.mapNumber)
			return;

		int i;
		if(Input.GetKeyDown(KeyCode.J)) // just test
		{
			for (i = 0; i < 4; i++) 
			{
				if (doors[i] == null) continue;

				doorIndex[i]++;
				if (doorIndex[i] > 2)
					doorIndex[i] = 0;
				doorSpriteRenderers[i].sprite = doorSprites[i, doorIndex[i]];
			}
		}

		bool check = true;
		if (monsters != null)
		{
			for (i = 0; i < monsters.Length; i++)
			{
				if (monsters[i].state != EntityState.dead)
				{
					check = false;
					break;
				}
			}
		}

		if(check)
		{
			for (i = 0; i < 4; i++)
			{
				if (doors[i] == null) continue;

				doorIndex[i] = 1;
				doorSpriteRenderers[i].sprite = doorSprites[i, doorIndex[i]];
			}
		}


		for (i = 0; i < 4; i++) 
		{
			if(doorsCollider[i] != null)
			{
				BoxCollider2D b = doorsCollider[i];

				if (b.IsTouching(player.hitBox))
				{
					if (doorIndex[i] != 1)
						continue;

					GameManager gm = GameManager.instance;
					switch(i)
					{
						case 0: gm.passMapStart(mapNumber - 1);  break;
						case 1: gm.passMapStart(mapNumber + 1); break;
						case 2: gm.passMapStart(mapNumber + LevelData.instance.mapData.mapTotalSqrt); break;
						case 3: gm.passMapStart(mapNumber - LevelData.instance.mapData.mapTotalSqrt); break;
						default: 
							print("door["+ mapNumber +"] index["+ i +"] error"); 
							break;
					}
				}
			}
		}
	}

	private void doorLocked(int dir)
	{
		SpriteRenderer spr = doors[dir].transform.GetChild(0).GetComponent<SpriteRenderer>();
		spr.sprite = null;
		doors[dir] = null;
	}
}
