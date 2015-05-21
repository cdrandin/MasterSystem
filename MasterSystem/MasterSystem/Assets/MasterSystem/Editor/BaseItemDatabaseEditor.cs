using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor (typeof(BaseItemDatabase))]
public class BaseItemDatabaseEditor : Editor 
{
	private static BaseItemDatabase _main_data;

	private static bool[] _foldouts;
	private static bool _item_foldout;

	void OnEnable()
	{
		_main_data = BaseItemDataBaseInstance.instance.main_data;

		UpdateFoldout();
		_item_foldout = true;
	}

	void OnDisable()
	{
		_main_data = null;
		_foldouts  = null;
	}

	public override void OnInspectorGUI ()
	{
		//base.OnInspectorGUI ();
		DrawReadOnlyInspector();
	}

	public void UpdateFoldout()
	{
		_foldouts  = new bool[_main_data.data.Count];
	}

	void DrawReadOnlyInspector()
	{
		if(_foldouts.Length != _main_data.data.Count)
		{
			UpdateFoldout();
		}

		if(GUILayout.Button("Open Item database window"))
		{
			BaseItemDatabaseWindow.ShowWindow();
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Current item id");
		EditorGUILayout.LabelField(_main_data.current_id.ToString());
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.HelpBox("This is the next valid id that will be assigned to a new item.", MessageType.Info);
		EditorGUILayout.HelpBox("It is also the number of items in the database.", MessageType.Info);

		if(_main_data.data.Count > 0)
		{
			_item_foldout = EditorGUILayout.InspectorTitlebar(_item_foldout, _main_data.data.ToArray());
			if(_item_foldout)
			{
				for(int i = 0; i < _foldouts.Length; ++i)
				{
					_foldouts[i] = EditorGUILayout.Foldout(_foldouts[i], string.Format("id:{1} > {0}", _main_data.data[i].name, _main_data.data[i].id));
					if(_foldouts[i])
					{
						Editor.CreateEditor(_main_data.data[i]).OnInspectorGUI();
					}
				}
			}

			if(GUILayout.Button("Clean Up"))
			{
				BaseItemDataBaseInstance.instance.CleanUp()	;			
			}
			EditorGUILayout.HelpBox("The clean up will remove empty holes in the list. Reassign ids if duplicates found.", MessageType.Info);
		}
	}
}
