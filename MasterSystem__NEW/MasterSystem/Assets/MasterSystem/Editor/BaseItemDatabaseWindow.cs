using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BaseItemDatabaseWindow : EditorWindow 
{
	private static EditorWindow _window;

	private Vector2 _item_selection_scroll_pos;
	private float _height_offset = 20f;
	private static BaseItem _item;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Item Database Management")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		_window = EditorWindow.GetWindow<BaseItemDatabaseWindow>();
		_window.title = "Item Database";
		//window.position = new Rect(window.position.x+100f, window.position.y+100f, window.position.width, window.position.height);
	}

	public static void ShowWindowOnItem(BaseItem item)
	{
		//Show existing window instance. If one doesn't exist, make one.
		_window = EditorWindow.GetWindow<BaseItemDatabaseWindow>();
		_window.title = "Item Database";
		_item = item;
	}

	public static bool is_open
	{
		get { return _window != null; }
	}

	void OnGUI()
	{
		// Scrolling through items
		_item_selection_scroll_pos = GUI.BeginScrollView(new Rect(0f, 0f, position.width/3.5f, position.height), _item_selection_scroll_pos, new Rect(0f, 0f, position.width/4f, position.height*2));
		int i = 0;
		foreach(BaseItem item in BaseItemDataBaseInstance.instance.main_data.data)
		{
			if(GUI.Button(new Rect(0, _height_offset * i++, position.width/3.5f, 20f), item.name))
			{
				_item = item;
			}
		}
		i+=1;
		if(GUI.Button(new Rect(0, _height_offset * i, position.width/3.5f, 20f), "Open Item Creator"))
		{
			CreateItemWindow.ShowWindow();
		}
		i+=1;
		if(GUI.Button(new Rect(0, _height_offset * i, position.width/3.5f, 20f), "Unfocus"))
		{
			_item = null;
		}
		i+=1;
		if(_item != null)
		{
			if(GUI.Button(new Rect(0, _height_offset * i, position.width/3.5f, 20f), "Remove item"))
			{
				BaseItemDataBaseInstance.instance.main_data.Remove(_item);
				_item = null;
			}
		}
		i+=1;
		if(GUI.Button(new Rect(0, _height_offset * i, position.width/3.5f, 20f), "RemoveAll"))
		{
			BaseItemDataBaseInstance.instance.Reset();
			_item = null;
		}
		GUI.EndScrollView();


		Rect right_side = new Rect(position.width/3.5f, 0.0f, position.width - position.width/3.5f - 5f, position.height);

		GUILayout.BeginArea (right_side);
		if(_item != null)
		{
			Editor editor = Editor.CreateEditor(_item);
			editor.OnInspectorGUI();
		}
		GUILayout.EndArea();
		this.Repaint();
	}

	void OnDestroy()
	{
		_window = null;
	}
}
