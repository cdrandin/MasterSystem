using UnityEngine;
using System.Collections;


public class SimpleExample : MonoBehaviour 
{

	public Item myItem;
	

	public void OnGUI()
	{
		GUILayout.Label ("Name " + myItem.name);
		GUILayout.Label ("Level " + myItem.level);
		if(GUILayout.Button("Level UP item"))
		{
			LevelupItem();
		}
	}

	public void LevelupItem()
	{
		string data = XMLUtil.Serialize<Item> (myItem);
		Request request = new Request ();
		request.id = "LevelUpItem";
		request.payload = data;
		request.callback = LevelUpItemCallback;
		GameMaster.SendRequest (request);
	}

	public void LevelUpItemCallback(Response response)
	{
		myItem = XMLUtil.Deserialize<Item> (response.payload);
	}
}
