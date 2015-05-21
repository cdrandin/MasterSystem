 using UnityEngine;
using System.Collections;

public enum ENEMY_TYPE
{
	MINION,
	BOSS
}
[System.Serializable]
public class EnemyEntity : ScriptableObject
{
	[SerializeField]
	private UnitEntitySO _unit_entity_so;
	public UnitEntitySO unit_entity_so
	{
		get { return _unit_entity_so; }
	}

	[SerializeField]
	private ENEMY_TYPE _type;
	public ENEMY_TYPE type
	{
		get { return _type; }
	}
}
