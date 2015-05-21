using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class BaseItemDatabase : ScriptableObject
{
	[SerializeField]
	private List<BaseItem> _data;
	public List<BaseItem> data
	{
		get { return _data; }
	}

	public BaseItem this[int id]
	{
		get { return _data[id]; }
	}

	[SerializeField]
	private int _current_id;
	public int current_id
	{
		get { return _data.Count; }
	}

	public BaseItemDatabase()
	{
		_data         = new List<BaseItem>();
		_current_id   = 0;
	}

	public void Add(BaseItem item)
	{
		_data.Add(item);
		++_current_id;
	}

	public void Replace(BaseItem item, int index)
	{
		_data[index] = item;
	}

	public void Remove(BaseItem item)
	{
		if(_data.Remove(item))
			--_current_id;
	}

	public void Reset()
	{
		_data.Clear();
		_current_id = 0;
	}

	public void CleanUp()
	{
		ResetIDs();
	}

	private void ResetIDs()
	{
		int i = 0;
		foreach(BaseItem item in _data)
		{
			item.SetID(i++);
		}
	}
}

public class BaseItemDataBaseInstance
{
	private static BaseItemDataBaseInstance _instance;
	public static BaseItemDataBaseInstance instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new BaseItemDataBaseInstance();
			}

			return _instance;
		}
	}

	private string _item_db_path;
	private BaseItemDatabase _main_data;
	public BaseItemDatabase main_data
	{
		get { return _main_data; }
	}

	public void AddToDatabase(BaseItem item)
	{
		item.SetID(_main_data.current_id);
		_main_data.Add(item);
		SaveDB();
	}

	public void ReplaceFromDatabaseAt(int index, BaseItem item)
	{
		_main_data.Replace(item, index);
		SaveDB();
	}

	public void RemoveFromDatabase(BaseItem item)
	{
		if(_main_data.data.Contains(item)) // make sure item is in the container
		{
			_main_data.Remove (item); // call the classes remove
			SaveDB();
		}
	}

	public void Reset()
	{
		_main_data.Reset();
		SaveDB();
	}

	public void CleanUp()
	{
		_main_data.CleanUp();
		SaveDB();
	}

	private void SaveDB()
	{
		ProjectApocalypseCreate.SaveAsset(_main_data);
	}

	private BaseItemDataBaseInstance()
	{
		_item_db_path = "Assets/Resources/Database/ItemDatabase.asset";
		_main_data = AssetDatabase.LoadAssetAtPath(_item_db_path, typeof(BaseItemDatabase)) as BaseItemDatabase;
		if(_main_data == null)
		{
			_main_data = ScriptableObjectUtility.CreateAssetAt<BaseItemDatabase>(_item_db_path, false);
		}
	}
}