using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BaseItem))]
[CanEditMultipleObjects]
public class BaseItemEditor : Editor
{
	private bool _itemdb_window_open;

	public override void OnInspectorGUI ()
	{
		_itemdb_window_open = BaseItemDatabaseWindow.is_open;

		ModifyButton();

		EditorGUILayout.HelpBox("The follow is meant to be READ ONLY and cannot be modified directly.", MessageType.Info);
		EditorGUILayout.HelpBox("Click the modify button to change it.", MessageType.Info);
		//base.DrawDefaultInspector();
		DrawReadOnlyInspector();

		ModifyButton();

		if(!BaseItemDataBaseInstance.instance.main_data.data.Contains((BaseItem)this.target))
		{
			AddToDBButton();
		}
		else
		{
			RemoveFromDBButton();
		}
		DeleteButton();

		if(!_itemdb_window_open)
		{
			OpenItemDBButton();
		}

		UpdateButton();
	}

	public override void OnPreviewGUI (Rect r, GUIStyle background)
	{
		//base.OnPreviewGUI (r, background);
		//EditorGUI.HelpBox(r, "OnPreviewGUI", MessageType.Warning);
		GUI.BeginGroup(r);
		this.OnInspectorGUI();
		GUI.EndGroup();
	}

	void DrawReadOnlyInspector()
	{
		BaseItem item = (BaseItem)this.target;

		EditorGUILayout.IntField("Item ID:", item.id);

		EditorGUILayout.ObjectField("Item image:", item.item_image, typeof(Texture2D), false);
		EditorGUILayout.TextField ("Item name:", item.name);
		if(Applications.type == COMBAT_TYPE.TURNED)
		{
			EditorGUILayout.IntField("Item usage cost:", item.cost);
		}
		EditorGUILayout.LabelField("Item description:");
		EditorGUILayout.TextArea(item.text, GUILayout.Width(Screen.width- 22f), GUILayout.Height(50));
		EditorStyles.textArea.wordWrap  = true;
		EditorStyles.textField.wordWrap = true;
		EditorGUILayout.IntField("Item amount on usage:", item.amount);
		EditorGUILayout.IntField("Item cooldown:", item.cooldown);
		EditorGUILayout.EnumPopup("Item weapon handle:", item.hand);
		EditorGUILayout.EnumPopup("Item harmfulness:", item.harmfulness);
		EditorGUILayout.EnumPopup("Item action:", item.action_list);
		EditorGUILayout.EnumPopup("Item ability:", item.ability_list);
		EditorGUILayout.TextField("Item Resource path:", item.on_use_sound_effect_resource_path);
	}

	void ModifyButton()
	{
		if(GUILayout.Button("Modify"))
		{
			CreateItemWindow.ShowWindowWithProperties((BaseItem)this.target);
		}
	}

	void AddToDBButton()
	{
		EditorGUILayout.Space();
		if(GUILayout.Button("Add to Item Database (Unique only)"))
		{
			BaseItem item = (BaseItem)this.target;
			if(!BaseItemDataBaseInstance.instance.main_data.data.Contains(item))
			{
				BaseItemDataBaseInstance.instance.AddToDatabase(item);
			}
		}
	}

	void RemoveFromDBButton()
	{
		EditorGUILayout.Space();
		if(GUILayout.Button("Remove from Item Database"))
		{
			BaseItemDataBaseInstance.instance.RemoveFromDatabase((BaseItem)this.target);
		}
	}

	void DeleteButton()
	{
		EditorGUILayout.Space();
		if(GUILayout.Button("Delete Item"))
		{
			if(EditorUtility.DisplayDialog("Delete Item", "Are you sure?", "Yes", "No"))
			{
				BaseItem item = (BaseItem)this.target;
				BaseItemDataBaseInstance.instance.RemoveFromDatabase(item);
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
			}
		}
	}

	void OpenItemDBButton()
	{
		EditorGUILayout.Space();
		if(GUILayout.Button("Open Item Database"))
		{
			BaseItemDatabaseWindow.ShowWindowOnItem((BaseItem)this.target);
		}
	}

	void UpdateButton()
	{
		EditorGUILayout.Space();
		if(GUILayout.Button("Update"))
		{
			ProjectApocalypseCreate.SaveAsset(this.target);
		}
	}
}
