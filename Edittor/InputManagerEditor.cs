#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class InputManagerEditor : Editor {
	//Same as InputManager
	public enum AxisType {
		KeyOrMouseButton = 0,
		MouseMovement = 1,
		JoystickAxis = 2
	};
	public enum AxisId {
		X = 1, Y = 2, Joy3AndWheel = 3,
		Joy4 = 4, Joy5 = 5, Joy6 = 6, Joy7 = 7, Joy8 = 8, Joy9 = 9,	Joy10 = 10,
		Joy11 = 11,	Joy12 = 12, Joy13 = 13, Joy14 = 14, Joy15 = 15,
		Joy16 = 16, Joy17 = 17, Joy18 = 18, Joy19 = 19, Joy20 = 20,
	};

	//Same as InputManager
	public class InputAxis {
		public string name="Joy";
		public string descriptiveName;
		public string descriptiveNegativeName;
		public string negativeButton;
		public string positiveButton;
		public string altNegativeButton;
		public string altPositiveButton;
		public float gravity=1;
		public float dead=0;
		public float sensitivity=1;
		public bool snap = false;
		public bool invert = false;
		public AxisType type=AxisType.JoystickAxis;
		public int axis=0;
		public int joyNum=0;
	};

	public static InputAxis CreateAxis(string name, int joystickNum, AxisId axisId, bool invert=false)
	{
		var axis = new InputAxis();
		axis.name = name;
		axis.dead = 0.2f;
		axis.sensitivity = 1;
		axis.invert = invert;
		axis.type = AxisType.JoystickAxis;
		axis.axis = (int)axisId;
		axis.joyNum = joystickNum;

		return axis;
	}

	public static InputAxis CreateButton(string name, int joystickNum, string buttonName, bool invert=false)
	{
		var axis = new InputAxis();
		axis.name = name;
		axis.gravity = 100f;
		axis.dead = 0.01f;
		axis.sensitivity = 100;
		axis.invert = invert;
		axis.type = AxisType.KeyOrMouseButton;
		axis.positiveButton = buttonName;
		axis.joyNum = joystickNum;

		return axis;
	}

	// AddAxis
	public static void AddAxis(SerializedObject serializedObject, InputAxis axis)
	{
		SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

		axesProperty.arraySize++;
		serializedObject.ApplyModifiedProperties();

		SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

		GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
		GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
		GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
		GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
		GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
		GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
		GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
		GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
		GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
		GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
		GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
		GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
		GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
		GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
		GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

		serializedObject.ApplyModifiedProperties();

	}
	//-------------------------------------------------------------------------
	// 子プロパティ取得
	//-------------------------------------------------------------------------
	private static SerializedProperty GetChildProperty(SerializedProperty parent, string name) {
		SerializedProperty child = parent.Copy();
		child.Next(true);
		do {
			if ( child.name == name ) return child;
		}
		while ( child.Next(false) );
		return null;
	}

	//-------------------------------------------------------------------------
	// DebugDispAllProperty
	//-------------------------------------------------------------------------
	[MenuItem("Edit/Project Settings/CreateInput")]
	public static void DebugDispAllProperty(){
		string path = "ProjectSettings/InputManager.asset";
		SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(path)[0]);
		SerializedProperty serializedProperty = serializedObject.GetIterator ();
		serializedProperty = serializedObject.FindProperty("m_Axes");

		serializedProperty.ClearArray ();
		serializedObject.ApplyModifiedProperties();

		int pl = 0;
		// https://www.reddit.com/r/Unity3D/comments/1syswe/ps4_controller_map_for_unity/
		AddAxis( serializedObject, CreateAxis("AnLH",pl,AxisId.X) );
		AddAxis( serializedObject, CreateAxis("AnLV",pl,AxisId.Y, true) );
		AddAxis( serializedObject, CreateAxis("AnRH",pl,AxisId.Joy3AndWheel) );
		AddAxis( serializedObject, CreateAxis("AnRV",pl,AxisId.Joy4, true) );
		AddAxis( serializedObject, CreateAxis("APadLH",pl,AxisId.Joy5) );
		AddAxis( serializedObject, CreateAxis("APadLV",pl,AxisId.Joy6, true) );
		AddAxis( serializedObject, CreateButton("DPadRL",pl,"joystick button 0") );
		AddAxis( serializedObject, CreateButton("DPadRD",pl,"joystick button 1") );
		AddAxis( serializedObject, CreateButton("DPadRR",pl,"joystick button 2") );
		AddAxis( serializedObject, CreateButton("DPadRU",pl,"joystick button 3") );
		AddAxis( serializedObject, CreateButton("DPadL1",pl,"joystick button 4") );
		AddAxis( serializedObject, CreateButton("DPadR1",pl,"joystick button 5") );
		AddAxis( serializedObject, CreateButton("DPadL2",pl,"joystick button 6") );
		AddAxis( serializedObject, CreateButton("DPadR2",pl,"joystick button 7") );

		debugDispProperty (serializedProperty, 2);
	}

	private static void debugDispProperty(SerializedProperty _prop, int _maxDepth, int _depth=0) {
		int cnt = 0;
		SerializedProperty prop = _prop.Copy ();
		if(_depth < _maxDepth){
			bool enterChild = true;
			while ( prop.Next(enterChild) ){
				enterChild = false;
				if (prop.type == "string") {
					cnt++;
				}
				Debug.Log(_depth + "("+prop.depth+"):" + prop.name+"("+prop.displayName+"):"+((prop.type=="string") ? prop.stringValue : prop.type ));
				if(prop.isArray){
					for (int i = 0; i < prop.arraySize; ++i) {
						debugDispProperty(prop.GetArrayElementAtIndex(i), _maxDepth, _depth+1);
					}
				}
			}
		}
	}


}
#endif