using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityCooldownDisplay : MonoBehaviour 
{
	public TextMesh primary0_textmesh;
	public TextMesh primary1_textmesh;
	public TextMesh secondary0_textmesh;
	public TextMesh secondary1_textmesh;
	public TextMesh soulshard0_textmesh;

	public MonochromeEffect primay0_chrome_effect;
	public MonochromeEffect primay1_chrome_effect;
	public MonochromeEffect secondary0_chrome_effect;
	public MonochromeEffect secondary1_chrome_effect;
	public MonochromeEffect soulshard0_chrome_effect;

	private TimerBasedCombatSystem _combat_system;

	// Use this for initialization
	void Start () 
	{
		_combat_system = TimerBasedCombatSystem.instance;
		StartCoroutine(DelayUpdate(1f/10f));
	}
	
	// Update is called once per frame
	void MyUpdate () 
	{
		if(_combat_system != null)
		{
			if(_combat_system.selected_unit != null)
			{
				if(_combat_system.selected_unit.primary_slot != null && _combat_system.selected_unit.primary_slot[0] != null)
					SetCDToTextMesh(primary0_textmesh, primay0_chrome_effect, _combat_system.selected_unit.primary_slot[0].item);

				if(_combat_system.selected_unit.primary_slot != null && _combat_system.selected_unit.primary_slot[1] != null)
					SetCDToTextMesh(primary1_textmesh, primay1_chrome_effect, _combat_system.selected_unit.primary_slot[1].item);

				if(_combat_system.selected_unit.secondary_slot != null && _combat_system.selected_unit.secondary_slot[0] != null)
					SetCDToTextMesh(secondary0_textmesh, secondary0_chrome_effect, _combat_system.selected_unit.secondary_slot[0].item);

				if(_combat_system.selected_unit.secondary_slot != null && _combat_system.selected_unit.secondary_slot[1] != null)
					SetCDToTextMesh(secondary1_textmesh, secondary1_chrome_effect, _combat_system.selected_unit.secondary_slot[1].item);

				if(_combat_system.selected_unit.soulshard_slot != null && _combat_system.selected_unit.soulshard_slot[0] != null)
					SetCDToTextMesh(soulshard0_textmesh, soulshard0_chrome_effect, _combat_system.selected_unit.soulshard_slot[0].item);
			}
		}
	}

	void SetCDToTextMesh(TextMesh textmesh, MonochromeEffect monochrome, BaseItem item)
	{
		if(monochrome != null)
		{
			monochrome.UnDim();
		}

		if(textmesh != null)
		{
			int cd = item.time_left;
			if(cd <= .5f)
			{
				if(textmesh.text != "") // Play once
				{
					textmesh.text = "";
					if(monochrome != null)
					{
						monochrome.UnDim();
//						monochrome.ModIntensity(0.5f);
//						DelayAction.instance.Delay(()=>{ monochrome.UnDim(); }, .35f);
					}
				}
			}
			else
			{
				textmesh.text = cd.ToString();
				if(monochrome != null)
					monochrome.Dim (item.cooldown, item.time_left);
			}
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
