using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(EnemyEntity))]
[CanEditMultipleObjects]
public class EnemyEntityEditor : Editor
{
	private SerializedProperty _unit_entity;
	private SerializedProperty _unit_enemy_type;

	void OnEnable()
	{
		_unit_entity     = this.serializedObject.FindProperty("_unit_entity_so");
		_unit_enemy_type = this.serializedObject.FindProperty("_type");
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.InspectorTitlebar(true, this.target);
		DisplayPropertyField(_unit_entity, true);
		DisplayPropertyField(_unit_enemy_type, false);
	}

	private void DisplayPropertyField(SerializedProperty sp, bool show_children)
	{
		sp.serializedObject.UpdateIfDirtyOrScript(); // update changes
		EditorGUILayout.PropertyField(sp, show_children);
		sp.serializedObject.ApplyModifiedProperties(); // set changes
	}
}
