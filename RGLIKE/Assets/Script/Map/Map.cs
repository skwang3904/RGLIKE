using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	private GameObject[] doors; // 맵의 각 문 // 딱히 없어도될듯?
	private BoxCollider2D[] doorsCollider;
	private SpriteRenderer[] doorSpriteRenderers; // 각 문의 스프라이트렌더러
	private Sprite[,] doorSprites; // 문의 총 스프라이트
	private int[] doorIndex; // 문의 각 인덱스 0:close, 1:open, 2:closeKey

	public int mapNumber;

	private Player player;
	private GameObject[] monsters; // 몬스터 만들고 수정

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


			doors[i] = g;
			doorsCollider[i] = g.GetComponent<BoxCollider2D>();
			doorSpriteRenderers[i] = spr;
		}
	}

	private void Start()
	{
		player = GameManager.instance.player;
		int i, j = 0;
#if false
// 몬스터 생성 후 이 맵번호와 같은 맵번호를 가진 몬스터를
// 저장시키고 업데이트에 검사하여 문을 열거나 닫음
		int mNum = transform.Find("MonsterSpawn").GetChildCount();
		monsters = new GameObject[mNum];

		GameObject Monsters = GameObject.Find("Monsters");
		mNum = Monsters.transform.GetChildCount();
		for (i = 0; i < mNum; i++)
		{
			LivingEntity m = Monsters.transform.GetChild(i).GetComponent<LivingEntity>();
			if(m.mapNumber == mapNumber)
			{
				monsters[j++] = m.gameObject;
			}

		}
#endif
	}

	private void Update()
	{
		if (mapNumber != player.mapNumber)
			return;

		if(Input.GetKeyDown(KeyCode.J))
		{
			for (int i = 0; i < 4; i++) 
			{
				if (doorSpriteRenderers[i] == null) continue;

				doorIndex[i]++;
				if (doorIndex[i] > 2)
					doorIndex[i] = 0;
				doorSpriteRenderers[i].sprite = doorSprites[i, doorIndex[i]];
			}
		}

		for (int i = 0; i < 4; i++) 
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
						case 2: gm.passMapStart(mapNumber + LevelData.instance.MAP_TOTAL_SQRT); break;
						case 3: gm.passMapStart(mapNumber - LevelData.instance.MAP_TOTAL_SQRT); break;
						default: 
							print("door["+ mapNumber +"] index["+ i +"] error"); 
							break;
					}
				}
			}
		}
	}
}
