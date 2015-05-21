using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerCombatDebugUI : MonoBehaviour 
{
	public Button debug_button;
	public GameObject debug_stuff;
	public Button enable_ai;
	public Text ai_text;
//	public Text ai_text;
//	public Text ai_text;

	private Game_Timer gt;

	// Use this for initialization
	void Start () {
		gt = Camera.main.GetComponent<Game_Timer>();
		debug_stuff.SetActive(true);
		ToggleDebug();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void ToggleDebug()
	{
		debug_stuff.SetActive(!debug_stuff.activeSelf);
		Text text = debug_button.transform.GetComponentInChildren<Text>();
		if(text != null)
		{
			text.text = string.Format("Debug {0}", (debug_stuff.activeSelf) ? "OFF" : "ON" );
		}
	}

	public void ToggleEnemeyAI()
	{
		gt.ai_on = !gt.ai_on;
		UpdateEnemyAI();
	}

	public void UpdateEnemyAI()
	{
		if(ai_text != null)
		{
			ai_text.text = string.Format("Turn AI {0}", (gt.ai_on) ? "OFF" : "ON" );
		}
	}

	public void FullSpirit()
	{
		TimerBasedCombatSystem.instance.ModPower(TimerBasedCombatSystem.instance.max_group_power);
	}

	public void HealAll()
	{
		HealPlayer();
		HealEnemey();
	}

	public void HealPlayer()
	{
		UnitEntity[] player_ue = TimerBasedCombatSystem.instance.LivingUnitsFrom(OWNERSHIP.PLAYER).ToArray();
		foreach(UnitEntity ue in player_ue)
		{
			ue.base_entity.HealUnit(ue.base_entity.max_hp);
		}
	}

	public void HealEnemey()
	{
		UnitEntity[] enemy_ue = TimerBasedCombatSystem.instance.LivingUnitsFrom(OWNERSHIP.ENEMY).ToArray();
		foreach(UnitEntity ue in enemy_ue)
		{
			ue.base_entity.HealUnit(ue.base_entity.max_hp);
		}
	}
}
