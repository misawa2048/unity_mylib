using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.Events;

[CanEditMultipleObjects, CustomEditor(typeof(ScrollRectSticky),true)]
public class ScrollRectStickyEditor : ScrollRectEditor {
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		this.serializedObject.Update();
//		EditorGUILayout.PropertyField(this.serializedObject.FindProperty("beginDragEvent"), true);
//		EditorGUILayout.PropertyField(this.serializedObject.FindProperty("dragEvent"), true);
		EditorGUILayout.PropertyField(this.serializedObject.FindProperty("endDragEvent"), true);
		EditorGUILayout.PropertyField(this.serializedObject.FindProperty("pageChangeEvent"), true);
		this.serializedObject.ApplyModifiedProperties();
	}
}
