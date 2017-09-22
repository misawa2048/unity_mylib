using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
/*
!!Forked from base script&shader(this script and EquirectangularPlusShader)
!!左右反転を追加しました。
!!シェーダーマルチコンパイル対応しました
!!正方形出力対応しました

EquirectangularCamera: Cameraを中心とした全球を正距円筒図法やドームマスターにレンダリングするスクリプト

【使い方】
・このスクリプトをCameraオブジェクトにアタッチすると、Cameraを中心とした全球の正距円筒図法
　　（モードによってはドームマスター）でレンダリングされます。
・このスクリプトはエディターモードでも動作するので、編集中もリアルタイムに表示を確認することができます。

正距円筒図法での表示：
・生成される画面の中心はCameraの背面（-Z方向）になります。

ドームマスターでの表示：
・Cameraのローカル座標でのXZ平面より上側を、（正距円筒図法と異なり）内側から見た様子がレンダリングされます。
・生成される画面の中心は天頂（Cameraの+Y方向）、下端が正面（Cameraの+Z方向）になります。


【パラメータ】
antiAliasing：　CubeMapに描画する際にアンチエイリアシングをかけるピクセル数を指定します。
mode：　生成するイメージの種類を、正距円筒図法かドームマスターが選択できます。

【注意】
LateUpdate（）の中で全球シーンが撮影されます。
他のスクリプトでもLateUpdate()を使っている場合、呼び出される順番によっては意図通りの結果が得られないことがあります。
その場合は、メニューのEdit > Project Settings > Script Execution Orderで、EquirectangularCameraが呼ばれる順番
を明示的に遅くしてみてください。
*/

namespace QTools {

	/// <summary>
	/// Cameraを中心とした全球を正距円筒図法やドームマスターにレンダリングするスクリプト。
	/// </summary>
	// このスクリプトを貼れるのはCameraオブジェクトに限定する。
	[RequireComponent(typeof(Camera))]
	// シーン制作中もプレビューできるように、エディットモードで実行できるようにする。
	[ExecuteInEditMode]
	// このコンポーネントはひとつしかアタッチできません。
	[DisallowMultipleComponent]
	public class EquirectangularCamera : MonoBehaviour {
		public enum QubeResolution{
			VeryLow = 256,
			Low = 512,
			Middle = 1024,
			High = 2048,
			VeryHigh = 4096,
		}

		[TooltipAttribute("内部で確保するCubeMapの解像度を一辺のピクセル数で指定してください")]
		public QubeResolution resolution = QubeResolution.Middle;
		[HideInInspector]
		public int cubeResolution;

		public enum Antialias {
			None = 1,
			Two = 2,
			Four = 4,
			Eight = 8,
		}

		[TooltipAttribute("レンダリング時にアンチエイリアスをかけるピクセル数")]
		public Antialias antiAliasing = Antialias.None;

		public enum Mode {
			Equirectangular = 0,
			DomeMaster = 1,
		}
		[TooltipAttribute("レンダリングする図法を正距円筒図法かドームマスターから選択してください")]
		public Mode mode;
		[TooltipAttribute("左右反転したい場合はON")]
		public bool hFlip;
		[TooltipAttribute("正方形に投影したい場合はON")]
		public bool isSquare;
		[Range(-179f,90f),TooltipAttribute("変位またはズーム")]
		public float displacement = 0;
		[Range(-0.25f,0.25f),TooltipAttribute("外枠マージン(短辺基準)")]
		public float margin=0.0f;
		[Range(0.5f,4.0f),TooltipAttribute("輝度調整")]
		public float brightness=1.0f;
		public bool forwrdAsCenter=false;

		//Capture tool の　OnRenderImageで使用
		public bool isCaptureImage { get { return !(_on_cube_render || !cubeTexture); } }

		Camera cubeCam;
		RenderTexture cubeTexture;
		Material equirectangularMaterial;
		int _cube_resolution;
		int _antiAliasing; // 1,2,4,8
		bool _on_cube_render = false;
		int _cullingMask;

		bool useRuntimeCamera = false;

		void LateUpdate () {
			
			if (! cubeCam) {
				if (Application.isPlaying && useRuntimeCamera) {
					var copied = Instantiate(gameObject, transform.position, transform.rotation, transform) as GameObject;
					copied.name = "cube camera for runtime only";
					copied.transform.localScale = Vector3.one;
					cubeCam = copied.GetComponent<Camera>();
					copied.SetActive(false);
				} else {
					cubeCam = GetComponent<Camera>();
				}
				cubeCam.orthographic = false;
			}

			// CubeMapのサイズを2の累乗に合わせる（最小256, 最大4096）
			var cuveRes = (int)resolution;
			if (_cube_resolution != (int)resolution) {
				_cube_resolution = (int)resolution;
			}
			// 現在のCubeMapのサイズ、アンチエイリアスの値が合わなくなっていたらCubeMapを破棄する。
			if (cubeTexture && cubeTexture.width != _cube_resolution || _antiAliasing != (int)antiAliasing) {
				if (Application.isPlaying)
					Destroy(cubeTexture);
				else
					DestroyImmediate(cubeTexture);
				cubeTexture = null;
			}

			_antiAliasing = (int)antiAliasing;
			// CubeMapが無ければ生成する。
			if (! cubeTexture) {
				cubeTexture = new RenderTexture(_cube_resolution, _cube_resolution, 0,RenderTextureFormat.ARGB32);
				cubeTexture.antiAliasing = _antiAliasing;
				cubeTexture.dimension = UnityEngine.Rendering.TextureDimension.Cube;
				cubeTexture.hideFlags = HideFlags.HideAndDontSave;
			}
			// 図法変換用マテリアルが無ければ生成する。
			if (! equirectangularMaterial) {
				Shader shader = Shader.Find("Hidden/EquirectangularPlusShader");
				equirectangularMaterial = new Material(shader);
				equirectangularMaterial.hideFlags = HideFlags.HideAndDontSave;
			}

			// カメラの回転をシェーダーの変数に設定する。
			Quaternion rot = transform.rotation;
			if (forwrdAsCenter) {
				rot *= Quaternion.FromToRotation (Vector3.up, Vector3.forward);
			}
			equirectangularMaterial.SetVector("_Rotation", new Vector4(rot.x, rot.y, rot.z, rot.w));
			equirectangularMaterial.SetFloat ("_Zoom", displacement/180f);
			equirectangularMaterial.SetFloat("_Margin", margin);
			equirectangularMaterial.SetFloat("_Brightness", brightness);
			if (hFlip)
				equirectangularMaterial.EnableKeyword ("USE_HFLIP");
			else
				equirectangularMaterial.DisableKeyword("USE_HFLIP");

			if (mode == Mode.DomeMaster)
				equirectangularMaterial.EnableKeyword ("USE_DOMEMODE");
			else
				equirectangularMaterial.DisableKeyword("USE_DOMEMODE");

			if (isSquare)
				equirectangularMaterial.EnableKeyword ("IS_SQUARE");
			else
				equirectangularMaterial.DisableKeyword("IS_SQUARE");

			// CubeMapをレンダリングする
			_on_cube_render = true;
			int cubeMask = 63; //(mode == Mode.DomeMaster) ? (63 - 8) : 63; //zoomがあるので下面もいる
			cubeCam.RenderToCubemap(cubeTexture, cubeMask);
			_on_cube_render = false;
		}

		void OnPreCull()
		{
			if (! _on_cube_render && cubeCam) {
				_cullingMask = cubeCam.cullingMask;
				cubeCam.cullingMask = 0;
			}
		}

		void OnPostRender()
		{
			if (! _on_cube_render && cubeCam)
				cubeCam.cullingMask = _cullingMask;
		}

		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (!isCaptureImage)
				Graphics.Blit(src, dest);
			else
				Graphics.Blit(cubeTexture, dest, equirectangularMaterial); //, (int)mode);
		}

		void OnDisable()
		{
			if (Application.isPlaying) {
				Destroy(cubeTexture);
				Destroy(equirectangularMaterial);
			} else {
				DestroyImmediate(cubeTexture);
				DestroyImmediate(equirectangularMaterial);
			}
			cubeTexture = null;
			equirectangularMaterial = null;
		}

	}
}
