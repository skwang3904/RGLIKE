using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObject_NextDoor : MapObject
{


	protected override void Awake()
	{
		base.Awake();

		type = MapObjectType.trigger;
		active += startNextLevel;

	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Player")
		{
			// nextlevel
			active?.Invoke();
		}
	}

	public override void initialize(int mapNum, Vector2 position)
	{
		base.initialize(mapNum, position);

		state = NonEntityState.Idle;
	}

	private void startNextLevel()
	{
		StartCoroutine("crtStartNextLevel");
	}

	private void crtStartNextLevel()
	{
		Color c = Color.clear;
		LevelData ld =  LevelData.instance;
		ld.fadeDt = 0;
		while (true)
		{
			ld.fadeDt += Time.deltaTime;
			if (ld.fadeDt > ld._fadeDt)
				ld.fadeDt = ld._fadeDt;

			c.a = Mathf.Lerp(0, 1, ld.fadeDt / ld._fadeDt);
			LevelData.instance.imgFadeInOut.color = c;

			if(ld.fadeDt == ld._fadeDt)
			{
				LevelData.instance.startLevel();
				break;
			}
		}
	}
}
