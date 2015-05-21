using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Contains info per encounter such as...
// # of waves
[RequireComponent (typeof(Button))]
public class Encounter : MonoBehaviour
{
	public EncounterWave[] waves;
	[SerializeField]
	private bool _completed;
	public bool completed
	{
		get { return _completed; }
	}

	private string _encounter_key;

	void Start()
	{
		_encounter_key = string.Format("{0}", this);

		if(!PlayerPrefs.HasKey(_encounter_key))
		{
			PlayerPrefs.SetInt(_encounter_key, 0);
		}

		_completed = (PlayerPrefs.GetInt(_encounter_key) == 1) ? true : false;

		// On click to an encounter
		this.GetComponent<Button>().onClick.AddListener(()=>{ EncounterManagement.SetCurrentEncounter(this); });
	}

	public void SetToComplete()
	{
		_completed = true;
		PlayerPrefs.SetInt(_encounter_key, 1);
	}

	public void WipePlayerPrefs()
	{
		PlayerPrefs.DeleteKey(_encounter_key);
	}
}
