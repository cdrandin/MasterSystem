using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UnitEntitySO))]
[CanEditMultipleObjects]
public class UnitEntitySOEditor : Editor
{
	private SerializedProperty _unit_base;
	private SerializedProperty _primary_item;
	private SerializedProperty _secondary_item;
	private SerializedProperty _soul_item;

	private SerializedProperty _unit_image;

	void OnEnable()
	{
		_unit_base      = this.serializedObject.FindProperty("base_entity_so");
		_primary_item   = this.serializedObject.FindProperty("primary");
		_secondary_item = this.serializedObject.FindProperty("secondary");
		_soul_item      = this.serializedObject.FindProperty("soul");
		_unit_image     = this.serializedObject.FindProperty("unit_image");
	}

	public override void OnInspectorGUI ()
	{
		//base.OnInspectorGUI ();
		EditorGUILayout.InspectorTitlebar(true, this.target);
		
		// Unit image
		DisplayPropertyField(_unit_image, false);

		// Base Entity
		DisplayPropertyField(_unit_base, true);
	
		// 3 Base Items
		DisplayPropertyField(_primary_item, true);
		DisplayPropertyField(_secondary_item, true);
		DisplayPropertyField(_soul_item, true);

		EditorGUILayout.Space();
		if(GUILayout.Button(new GUIContent("Open Item Creator",""), EditorStyles.miniButtonRight))
		{
			CreateItemWindow.ShowWindow();
		}
		base.Repaint();
	}

	private void DisplayPropertyField(SerializedProperty sp, bool show_children)
	{
		sp.serializedObject.UpdateIfDirtyOrScript(); // update changes
		EditorGUILayout.PropertyField(sp, show_children);
		sp.serializedObject.ApplyModifiedProperties(); // set changes
	}
}
