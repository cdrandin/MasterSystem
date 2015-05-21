using UnityEngine;
using System.Collections;

public class ButtonNextLevel : MonoBehaviour 
{
	public void NextLevelButton(string levelName)
	{
		Application.LoadLevel(levelName);
	}

	public void NextLevelButtonSave(string levelName)
	{
		SceneManager.LoadLevel(levelName);
	}

	public void PreviousLevelButton()
	{
		SceneManager.LoadPreviousLevel();
	}

	public void Update()
	{
		if(Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Backspace))
		{
			PreviousLevelButton();
		}
	}
}