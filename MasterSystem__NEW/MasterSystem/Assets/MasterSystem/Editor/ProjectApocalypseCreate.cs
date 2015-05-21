using UnityEngine;
using System.Collections;
using UnityEditor;

public class ProjectApocalypseCreate
{
//	[MenuItem("Assets/Create/Hero Unit")]
//	public static void CreateHeroUnit()
//	{
//		ScriptableObjectUtility.CreateAsset<HeroEntitySO>();
//	}

	[MenuItem("Assets/Delete All PlayerPrefs", false, 0)]
	public static void DeleteAllPlayerPrefs()
	{
		if(EditorUtility.DisplayDialog("Delete all Player Prefs", "This cannot be undone, are you sure?",
		                               "Yes", "No"))
		{
			PlayerPrefs.DeleteAll();
		}
	}

	[MenuItem("Assets/Create/Enemy Unit")]
	public static void CreateEnemyUnit()
	{
		CreateFolderAt("Assets/Resources", "Enemy Units");
		ScriptableObjectUtility.CreateAssetAt<EnemyEntity>("Assets/Resources/Enemy Units/NewEnemyUnit.asset", true);
	}

	[MenuItem("Assets/Create/Unit Entity SO")]
	public static void CreateUnitEntity()
	{
		CreateFolderAt("Assets/Resources", "UnitEntitySO");
		ScriptableObjectUtility.CreateAssetAt<UnitEntitySO>("Assets/Resources/UnitEntitySO/NewUnitEntity-UnitEntitySO.asset", true);
	}

	[MenuItem("Assets/Create/Base Item")]
	public static void CreateBaseItem()
	{
		CreateFolderAt("Assets/Resources", "Items");
		BaseItem item = ScriptableObjectUtility.CreateAssetAt<BaseItem>("Assets/Resources/Items/NewBaseItem.asset", true);
		item.SetName("NewBaseItem");
		item.SetID(BaseItemDataBaseInstance.instance.main_data.current_id);
		SaveAsset(item);
		CreateBaseItemDatabase();
	}

	[MenuItem("Assets/Create/Base Item Database")]
	public static void CreateBaseItemDatabase()
	{
		ProjectApocalypseCreate.CreateFolderAt("Assets/Resources", "Database");
		ScriptableObjectUtility.CreateAssetAt<BaseItemDatabase>("Assets/Resources/Database/ItemDatabase.asset", false);
	}

	/// <summary>
	/// Creates the folder at parent_folder and directory_name.
	/// Usage: CreateFolderAt("Assets/Resources", "Items");
	/// </summary>
	/// <param name="parent_folder">Parent_folder.</param>
	/// <param name="directory_name">Directory_name.</param>
	public static void CreateFolderAt(string parent_folder, string directory_name)
	{
		string filepath = Application.dataPath + "/Resources" + "/" + directory_name;

		if (!System.IO.Directory.Exists(filepath)) 
		{
			AssetDatabase.CreateFolder(parent_folder, directory_name);
		}
	}

	/// <summary>
	/// Saves the asset, assuming it is alredy in the Assets file being tracked. Mainly use on ScriptableObjects
	/// </summary>
	/// <param name="asset">Asset.</param>
	public static void SaveAsset(UnityEngine.Object asset)
	{
		//if(!(asset.GetType() is System.Object)) return;

		// http://forum.unity3d.com/threads/scriptableobject-asset-problem-the-changes-wont-saved-to-disk.229664/
		AssetDatabase.Refresh ();
		EditorUtility.SetDirty(asset);
		AssetDatabase.SaveAssets();
	}

}
