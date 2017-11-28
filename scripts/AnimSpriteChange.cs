using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace animSprite{
	[System.Serializable]
	public class ReplaceInfo{
		public string fromName;
		public string toName;
	}

	[RequireComponent(typeof(SpriteRenderer))]
	public class AnimSpriteChange : MonoBehaviour {
		[SerializeField] string spriteResourcePath="";
		[SerializeField] ReplaceInfo[] replaceInfo;
		SpriteRenderer sr;
		Sprite[] resSprArr;
		Dictionary<string,string> repDic;

		void Start () {
			prepare ();
		}

		void LateUpdate () {
			sprChange ();
		}

		void prepare(){
			Resources.LoadAll<Sprite>(spriteResourcePath);
			sr = GetComponent<SpriteRenderer> ();
			resSprArr = Resources.FindObjectsOfTypeAll<Sprite> ();
			repDic = new Dictionary<string, string> ();
			foreach (ReplaceInfo info in replaceInfo) {
				repDic.Add (info.fromName, info.toName);
			}
		}

		void sprChange(){
			string sprName = sr.sprite.name;
			int pos = sprName.LastIndexOf ('_');
			if (pos >= 0) {
				string pre = sprName.Substring (0, pos);
				if (repDic.ContainsKey (pre)) {
					string post = sprName.Substring (pos);
					Sprite retSpr = resSprArr.FirstOrDefault(e=>e.name.Equals(repDic [pre] + post));
					if (retSpr != null) {
						//					Debug.Log (repDic [pre] + post);
						sr.sprite = retSpr;
					}
				}
			}
		}
	}
} // namespace animSprite
