using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemAbilitityDescription : MonoBehaviour 
{
	public GameObject description_object;
	public Text item_title;
	public Text item_description;
	public Text item_power_text;
	public Text item_amount_text;

	private TimerBasedCombatSystem _combat_system;
	
	// Use this for initialization
	void Start () 
	{
		_combat_system = TimerBasedCombatSystem.instance;
//		StartCoroutine(DelayUpdate(1f/10f));
		description_object.SetActive(false);
	}

	// Update is called once per frame
	void MyUpdate () 
	{
		if(_combat_system != null)
		{

		}
	}

	public void UpdateDescriptionObject(bool on, BaseItem item = null)
	{
		if(on)
		{
			if(item != null)
			{
				description_object.SetActive(true);
				item_title.text 	  = item.name;
				item_description.text = item.text;
				item_power_text.text  = item.cost.ToString();
				item_amount_text.text = item.amount.ToString();
			}
		}
		else
		{
			description_object.SetActive(false);
		}
	}

	IEnumerator DelayUpdate(float delay)
	{
		while(true)
		{
			MyUpdate();
			yield return new WaitForSeconds(delay);
		}
	}
}
