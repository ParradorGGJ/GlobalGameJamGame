using UnityEngine;
using UnityEditor;
using System.Collections;


public class DebugLabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.LabelField(position, label, property.stringValue);
        GUI.enabled = true;
    }
	
}
