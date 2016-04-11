// http://docs.unity3d.com/ja/current/ScriptReference/Camera.RenderToCubemap.html
// http://stackoverflow.com/questions/34458622/unity-save-cubemap-to-one-circle-image
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.IO;

public class RenderCubemapWizard : ScriptableWizard {

	public Camera camera;
	public Cubemap cubemap;

	void OnWizardUpdate () {
		string helpString = "Select transform to render from and cubemap to render into";
		bool isValid = (camera != null) && (cubemap != null);
	}

	void OnWizardCreate () {
		if (camera == null) {
			camera = Camera.main;
		}
		// render into cubemap		
		camera.RenderToCubemap( cubemap );
		cubemap.Apply ();
		ConvertToPng ();
	}

	[MenuItem("GameObject/Render into Cubemap")]
	static void RenderCubemap () {
		ScriptableWizard.DisplayWizard<RenderCubemapWizard>(
			"Render cubemap", "Render!");
	}

	void ConvertToPng()
	{
		Debug.Log(Application.dataPath + "/" +cubemap.name +"_PositiveX.png");
		var tex = new Texture2D (cubemap.width, cubemap.height, TextureFormat.RGB24, false);
		var bytes = getPlanePixels(tex,CubemapFace.PositiveX);      
		File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_PositiveX.png", bytes);       

		bytes = getPlanePixels(tex,CubemapFace.NegativeX);     
		File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_NegativeX.png", bytes);       

		bytes = getPlanePixels(tex,CubemapFace.PositiveY);     
		File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_PositiveY.png", bytes);       

		bytes = getPlanePixels(tex,CubemapFace.NegativeY);     
		File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_NegativeY.png", bytes);       

		bytes = getPlanePixels(tex,CubemapFace.PositiveZ);     
		File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_PositiveZ.png", bytes);       

		bytes = getPlanePixels(tex,CubemapFace.NegativeZ);     
		File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name   +"_NegativeZ.png", bytes);       
		DestroyImmediate(tex);

	}

	byte[] getPlanePixels(Texture2D _tex, CubemapFace _face){
		Texture2D tmpTex = new Texture2D (cubemap.width, cubemap.height, TextureFormat.RGB24, false);
		tmpTex.SetPixels(cubemap.GetPixels(_face));
		Color[] vline;
		for (int x = 0; x < cubemap.width; ++x) {
			vline = tmpTex.GetPixels (x, 0, 1, cubemap.height);
			_tex.SetPixels (x, 0, 1, cubemap.height,vline.Reverse().ToArray());
		}
		return _tex.EncodeToPNG();     
	}

}