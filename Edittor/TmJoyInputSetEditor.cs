using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;

[InitializeOnLoad]
public static class TmJoyInputSetEditor
{
	static readonly int NUM_SETUP = 6;
	static int m_defAxisNum;
	static SerializedObject m_serializedObject;
	static SerializedProperty m_axesProperty;

	static TmJoyInputSetEditor()
	{
		m_serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		m_axesProperty = m_serializedObject.FindProperty("m_Axes");
		m_defAxisNum = m_axesProperty.arraySize;
		#if false
		EditorApplication.playModeStateChanged += OnStateChanged;
		#else
		ToSetup ();
		#endif
	}
	static void OnStateChanged(PlayModeStateChange _state){
		Debug.Log("OnStateChanged"+_state.ToString());
		switch (_state) {
		case PlayModeStateChange.EnteredPlayMode:
			m_defAxisNum = m_axesProperty.arraySize;
			ToSetup ();
			break;
		case PlayModeStateChange.ExitingPlayMode:
			m_axesProperty.arraySize = m_defAxisNum;
			break;
		}
	}

	static void ToSetup()
	{
		for (int i = 0; i < NUM_SETUP; ++i) {
			string preName = "TmJoy" + (i + 1).ToString () + "_";
			addAxis(preName + "ALX", 2, 0, false, i+1);
			addAxis(preName + "ALY", 2, 1, true,  i+1);
			addAxis(preName + "DRR", 0, 2, false, i+1);
		}
		m_serializedObject.ApplyModifiedProperties();
	}

	private static bool addAxis(string _name, int _type, int _axis, bool _invert, int _joyNum){
		bool ret = HasAxis (m_axesProperty,_name);
		if (!ret) {
			m_axesProperty.arraySize++;
			SerializedProperty childElement = m_axesProperty.GetArrayElementAtIndex(m_axesProperty.arraySize-1);
			GetChildProperty(childElement, "m_Name").stringValue = _name;
			GetChildProperty(childElement, "positiveButton").stringValue = "joystick " + _joyNum.ToString () + " button "+ _axis.ToString();
			GetChildProperty(childElement, "type").intValue = _type;
			GetChildProperty(childElement, "axis").intValue = _axis;
            GetChildProperty(childElement, "invert").boolValue = _invert;
            GetChildProperty(childElement, "joyNum").intValue = _joyNum;

			GetChildProperty(childElement, "dead").floatValue = .5f;
			GetChildProperty(childElement, "sensitivity").floatValue = 1f;
		}
		return ret;
	}
	/*
                GetChildProperty(childElement, "m_Name").stringValue = "Axis 1";
                GetChildProperty(childElement, "descriptiveName").stringValue = "";
                GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                GetChildProperty(childElement, "negativeButton").stringValue = "";
                GetChildProperty(childElement, "positiveButton").stringValue = "";
                GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                GetChildProperty(childElement, "gravity").floatValue = 0f;
                GetChildProperty(childElement, "dead").floatValue = .5f;
                GetChildProperty(childElement, "sensitivity").floatValue = 1f;
                GetChildProperty(childElement, "snap").boolValue = false;
                GetChildProperty(childElement, "invert").boolValue = false;
                GetChildProperty(childElement, "type").intValue = 2;
                GetChildProperty(childElement, "axis").intValue = 0;
                GetChildProperty(childElement, "joyNum").intValue = 0;
	*/

	public static bool HasAxis(SerializedProperty _axesProperty, string _axisName)
	{
		bool ret = false;
		for (int i = 0; i < _axesProperty.arraySize; i++)
		{
			if (GetChildProperty(_axesProperty.GetArrayElementAtIndex(i), "m_Name").stringValue.Equals(_axisName))
			{
				ret = true;
				break;
			}
		}
		return ret;
	}




	public static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
	{

		//copy so we don't iterate original
		SerializedProperty copiedProperty = parent.Copy();

		bool moreChildren = true;

		//step one level into child
		copiedProperty.Next(true);

		//iterate on all properties one level deep
		while (moreChildren)
		{
			//found the child we were looking for
			if (copiedProperty.name.Equals(name))
				return copiedProperty;

			//move to the next property
			moreChildren = copiedProperty.Next(false);
		}

		//if we get here we didn't find it
		return null;
	}

}