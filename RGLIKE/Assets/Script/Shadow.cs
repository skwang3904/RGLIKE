using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
	public static Shadow instance;

	public struct Shadows
	{
		public GameObject gDst;
		public SpriteRenderer sprDst;

		public GameObject gSrc;
		public SpriteRenderer sprSrc;
	}

	public List<Shadows> livings;
	private Vector2 offset;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != null && instance != this)
			Destroy(gameObject);

		livings = new List<Shadows>();
		offset = new Vector2(1, 1);
	}

	private void Update()
	{
		foreach(Shadows s in livings)
		{
			s.gDst.transform.position = s.gSrc.transform.position
				+ new Vector3(0, -1, 0);

			s.sprDst.sprite = s.sprSrc.sprite;
			s.sprDst.flipX = s.sprSrc.flipX;
		}
	}

	public void addShadow(LivingEntity entity)
	{
		Shadows shadow = new Shadows();

		shadow.gSrc = entity.gameObject;
		shadow.sprSrc = entity.GetComponent<SpriteRenderer>();

		shadow.gDst = new GameObject("Shadow"+ livings.Count);
		shadow.gDst.transform.SetParent(GameObject.Find("Shadow").transform);
		shadow.sprDst = shadow.gDst.AddComponent<SpriteRenderer>();

		//sprite
		shadow.sprDst.sprite = shadow.sprSrc.sprite;
		shadow.sprDst.color = Color.black * 0.5f;

		//transform
		//shadow.sprDst.transform.position = new Vector2(0.3f, -0.2f);
		//shadow.sprDst.transform.Rotate(new Vector3(0, 0, -45));
		shadow.sprDst.transform.localScale = new Vector2(1, 0.2f);

		//sort
		shadow.sprDst.sortingLayerID = shadow.sprSrc.sortingLayerID;
		shadow.sprDst.sortingOrder = shadow.sprSrc.sortingOrder - 1;

		livings.Add(shadow);
	}

	public void deleteShadow()
	{
		livings.Clear();
	}
}
