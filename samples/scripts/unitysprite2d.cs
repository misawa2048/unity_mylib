using UnityEngine;
using System.Collections;

public class unitysprite2d : MonoBehaviour {
	public Sprite[] sprArr;
	void Update () {
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		Sprite spr = sprArr[Random.Range(0,sprArr.Length-1)];
		Rect rc = spr.rect;
		rc.width *= -1.0f;
		Sprite newSpr = Sprite.Create(spr.texture,rc,spr.textureRectOffset,16.0f);
		sr.sprite = newSpr;
	}
}
