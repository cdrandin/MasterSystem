using UnityEngine;
using System.Collections;

public class AllyUnitHealthController : MonoBehaviour 
{
	public TextMesh center_unit;
	public TextMesh left_unit;
	public TextMesh right_unit;

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
			foreach(UnitEntity ue in _combat_system.AllUnitsFrom(OWNERSHIP.PLAYER))
			{
				if(ue == _combat_system.selected_unit)
				{
					// center unit
					if(center_unit != null && _combat_system.selected_unit != null)
						center_unit.text = _combat_system.selected_unit.base_entity.hp.ToString();

					// right unit
					if(right_unit != null && _combat_system.right_unit != null)
						right_unit.text = _combat_system.right_unit.base_entity.hp.ToString();

					// left unit
					if(left_unit != null && _combat_system.left_unit != null)
						left_unit.text = _combat_system.left_unit.base_entity.hp.ToString();

					UpdateHP(left_unit, _combat_system.left_unit);
					UpdateHP(center_unit, _combat_system.selected_unit);
					UpdateHP(right_unit, _combat_system.right_unit);
				}
			}
		}
	}

	void UpdateHP(TextMesh tm, UnitEntity ue)
	{
		if(tm != null && ue != null)
		{
			tm.GetComponent<MeshRenderer>().enabled = true;
			tm.text = ue.base_entity.hp.ToString();
			
			if(ue.base_entity.hp == 0)
			{
				tm.GetComponent<MeshRenderer>().enabled = false;
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
