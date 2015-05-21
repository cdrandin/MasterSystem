using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager 
{
	private static SceneManager _instance;
	public static SceneManager instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new SceneManager();
			}

			return _instance;
		}
	}

	private Stack<string> _previous_scene;

	private SceneManager()
	{
		_previous_scene = new Stack<string>();
	}

	public static void LoadLevel(string scene_name)
	{
		instance._previous_scene.Push(Application.loadedLevelName);
		Application.LoadLevel(scene_name);
	}

	public static void LoadPreviousLevel()
	{
		try
		{
			string scene = instance._previous_scene.Pop();
			Application.LoadLevel(scene);
		}
		catch(System.Exception e)
		{
			Debug.Log(string.Format("No more scenes to go back to!   {0}", e));
		}
	}

	public static void Clear()
	{
		instance._previous_scene.Clear();
	}
}
