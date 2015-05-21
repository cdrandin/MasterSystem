using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitEntitySO : ScriptableObject
{
	public BaseEntitySO base_entity_so;
	public PrimaryItem primary;
	public SecondaryItem secondary;
	public SoulShard soul;
	public Texture2D unit_image;
}
