#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TmLib{
	public class TmEditorUtil {
		//------------------------------------------------------------
		// ファイル名の前まで取得:Assets/aaa/bbb
		//------------------------------------------------------------
		public static string GetFilePath(string _path)
		{
			return _path.Substring(0, _path.LastIndexOf('/'));
		}

		//------------------------------------------------------------
		// ファイル名取得:file.xx.asset
		//------------------------------------------------------------
		public static string GetFileName(string _path)
		{
			return _path.Substring(_path.LastIndexOf('/') + 1);
		}

		//------------------------------------------------------------
		// 拡張子名取得:.asset
		//------------------------------------------------------------
		public static string GetExtName(string _path)
		{
			string filename = GetFileName(_path);
			return filename.Substring(filename.LastIndexOf('.') + 1);
		}

		//------------------------------------------------------------
		// 拡張子なしファイル名取得://file.xx
		//------------------------------------------------------------
		public static string GetNoExtFileName(string _path)
		{
			string filename = GetFileName(_path);
			return filename.Substring(0, filename.LastIndexOf('.'));
		}

		//------------------------------------------------------------
		// 最初のピリオドまでのファイル名取得://file
		//------------------------------------------------------------
		public static string GetBaseFileName(string _path)
		{
			string filename = GetFileName(_path);
			return (filename.IndexOf('.') < 0) ? filename : filename.Substring(0, filename.IndexOf('.'));
		}

		//-----------------------------------------------------------------
		// パス以下から特定のアセットを取得
		//-----------------------------------------------------------------
		public static List<T> GetAssets<T>(string _path, bool _containsSubFolders = false) where T : Object
		{
			List<T> retList = new List<T>();
			List<string> pathList = new List<string>();
			if (_containsSubFolders) // サブフォルダも含む 
			{
				string[] subFolders = AssetDatabase.GetSubFolders(_path);
				foreach (string subFolder in subFolders)
				{
					pathList.Add(subFolder);
				}
			}
			else
			{
				pathList.Add(_path);
			}
			string[] pathArr = pathList.ToArray();
			string[] guIDs = AssetDatabase.FindAssets("t:" + typeof(T).Name, pathArr);
			foreach (string idStr in guIDs)
			{
				T asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(idStr));
				if (asset != null)
				{
					retList.Add(asset);
				}
			}
			return retList;
		}
	}
}

#endif
