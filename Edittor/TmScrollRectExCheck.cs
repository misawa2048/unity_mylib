using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace TmLib
{
    [CanEditMultipleObjects, CustomEditor(typeof(TmScrollRectEx), true)]
    public class TmScrollRectExEditor : ScrollRectEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("OnExBeginDragEvent"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("OnExEndDragEvent"), true);
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}