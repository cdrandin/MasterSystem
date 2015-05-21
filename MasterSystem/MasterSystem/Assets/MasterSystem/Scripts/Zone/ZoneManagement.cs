using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZoneManagement : MonoBehaviour
{
	public Image[] encounters;
	public Button.ButtonClickedEvent on_click;

	// Use this for initialization
	void Start () 
	{
		UpdateEncounters();
	}

	private void UpdateEncounters()
	{
		int cur = 0;

		// Give each image on click event
		foreach(Image e in encounters)
		{
			if(e.GetComponent<Encounter>().waves.Length >= 0)
			{
				e.GetComponent<Button>().onClick.AddListener(()=>
				{ 
					on_click.Invoke(); 
					string level_name = (Applications.type == COMBAT_TYPE.TIMED) ? "5_Timer_Combat" : "5_Turned_Combat";
					Camera.main.GetComponent<ButtonNextLevel>().NextLevelButtonSave(level_name);
				});
			}
		}
		
		for(int i = 0; i < encounters.Length; ++i)
		{
			if(encounters[i].GetComponent<Encounter>().waves.Length >= 0)
			{
				// Stop at a not completed encounter
				if(!encounters[i].GetComponent<Encounter>().completed)
				{
					cur = i; // current encounter
					SetNewEncounter(encounters[i]);
					break;
				}
				else
				{
					SetOldEncounter(encounters[i]);
				}
			}
		}
		
		for(int i = cur + 1; i < encounters.Length; ++i)
		{
			SetUnavailableEncounter(encounters[i]);
		}

		SetOldEncounter(encounters[encounters.Length - 1]); // fade out boss enocunter icon
		encounters[encounters.Length - 1].GetComponent<Button>().interactable = false; // disable

		// just to show boss up
		SetNewEncounter(encounters[encounters.Length - 1]);
	}

	private void SetNewEncounter(Image encounter)
	{
		encounter.GetComponent<Button>().interactable = true;
		encounter.color = Color.white;
	}

	private void SetOldEncounter(Image encounter)
	{
		encounter.GetComponent<Button>().interactable = true;
		encounter.color = Color.grey;
	}

	private void SetUnavailableEncounter(Image encounter)
	{
		encounter.GetComponent<Button>().interactable = false;
		encounter.color = Color.white;
	}
}
