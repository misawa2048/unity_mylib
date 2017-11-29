//#define NOEQUCAM

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QTools {
	[RequireComponent(typeof(Camera))]
	#if !NOEQUCAM
	[RequireComponent(typeof(EquirectangularCamera))]
	#endif
	[DisallowMultipleComponent]
	public class QScreenCaptureTool : MonoBehaviour {
		public bool isSquare;
		public float captureSec=0.1f;
		public string basePath = "imgs/domeTest";
		[SerializeField]
		RenderTexture renderTexture;
		[SerializeField]
		bool captureOneshot; // captureSec==0f

//		private Camera cam;
//		private RenderTexture rt;
		private int cnt = 0;
		private int realCnt = 0;
		private int captureNum;
		#if !NOEQUCAM
		private EquirectangularCamera eqScr;
		#endif

		void Awake(){
			if (renderTexture != null) {
				GetComponent<Camera> ().targetTexture = renderTexture;
			}
		}

		// Use this for initialization
		void Start () {
//			cam = GetComponent<Camera> ();
//			rt = cam.targetTexture;
			#if !NOEQUCAM
			eqScr = GetComponent<EquirectangularCamera> ();
			#endif
			Application.targetFrameRate = 30;
			captureNum = (int)((float)Application.targetFrameRate * captureSec);
			Debug.Log (Application.targetFrameRate+":"+captureNum); 
		}

		// Update is called once per frame
		void Update () {
		}

		void LateUpdate(){
		}
		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			Graphics.Blit(src, dest);
			#if !NOEQUCAM
			if((!eqScr.enabled)||(eqScr.isCaptureImage)){
//				if(eqScr.isCaptureImage){
			#endif
				if (captureSec > 0f) { // continuous capture mode
					if (cnt < captureNum) {
						CaptureAll (src, basePath, cnt, isSquare);
						cnt++;
					} else {
						GetComponent<Camera> ().targetTexture = null;
						this.enabled = false;;
					}
				} else { // oneshot capture mode
					if (captureOneshot) {
						captureOneshot = false;
						CaptureAll (src, basePath, cnt, isSquare);
						cnt++;
					}
				}
			#if !NOEQUCAM
			}
			#endif
			realCnt++;

		}
		/*
		void OnPostRender()
		{
		}
				*/

		public static void CaptureAll(RenderTexture targetTexture, string _path="__test", int _index=0, bool _isSquare=false){
			string str = ("00000" + _index.ToString ());
			str = str.Substring (str.Length - 5, 5);
			string _fileame = _path+"_" + str;
			Debug.Log (str);

//			float whRate = ((float)targetTexture.height / (float)targetTexture.width);
			Rect rect = new Rect (0, 0, targetTexture.width, targetTexture.height);
			if ((_isSquare)&&(rect.width>rect.height)) {
				rect.x = (rect.width - rect.height) * 0.5f;
				rect.width = rect.height;
			}

			//normalize in width
			rect.x /= (float)targetTexture.width;
			rect.y /= (float)targetTexture.width;
			rect.width /= (float)targetTexture.width;
			rect.height /= (float)targetTexture.width;

			CaptureParts (targetTexture, rect, _fileame);
			/*
			rect.width *= 0.25f;
			rect.height *= 0.5f;
			CaptureParts (targetTexture, rect, "__test004");
			rect.x += 0.25f * whRate;
//			rect.y += 0.5f * whRate;
			CaptureParts (targetTexture, rect, "__test005");
*/
			/*
			Texture2D image = new Texture2D((int)rect.width, (int)rect.height);
			image.ReadPixels(new Rect((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height), 0, 0);
			image.Apply();
			byte[] data = image.EncodeToPNG ();
			File.WriteAllBytes (_fileame + ".png", data);
			Debug.Log ("size=" + data.Length);
			*/
		}

		public static void CaptureParts(RenderTexture targetTexture,Rect _captureRate, string _fileame="__test00"){
			// targetTexture.width でnormalizeしているので
			int px = (int)(targetTexture.width * _captureRate.x);
			int py = (int)(targetTexture.width * _captureRate.y);
			int pw = (int)(targetTexture.width * _captureRate.width);
			int ph = (int)(targetTexture.width * _captureRate.height);
			Texture2D image = new Texture2D(pw, ph);
			image.ReadPixels(new Rect(px, py, pw, ph), 0, 0);
			image.Apply();
			byte[] data = image.EncodeToPNG ();
			File.WriteAllBytes (_fileame + ".png", data);
			Destroy (image);
			Debug.Log ("size=" + data.Length);
		}
	}
}
