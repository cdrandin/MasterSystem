//using UnityEngine;
//using System.Collections;
//
//[System.Serializable]
//public class HeroEntitySO : ScriptableObject
//{
//	public BaseEntitySO base_entity_so;
//	
//	private float _max_experience
//	{
//		get
//		{
//			return base_entity_so.level * 1.5f + 5.0f;
//		}
//	}
//
//	public float max_experience
//	{
//		get
//		{
//			return _max_experience;
//		}
//	}
//	
//	[SerializeField]
//	private float _experience;
//	public float experience
//	{
//		get { return _experience; }
//	}
//	
//	/// <summary>
//	/// Adds the experience.
//	/// </summary>
//	/// <returns><c>true</c>, if leveled up, <c>false</c> otherwise.</returns>
//	/// <param name="value">Value.</param>
//	public bool AddExperience(float value)
//	{
//		_experience += value;
//		if(_experience >= _max_experience)
//		{
//			base_entity_so.level += (int)(_experience/_max_experience);
//			float leftover = _experience%_max_experience;
//			_experience = leftover;
//			
//			return true;
//		}
//		
//		return false;
//	}
//
//	public void Reset()
//	{
//		_experience = 0;
//		base_entity_so.level = 1;
//		base_entity_so.health = 50;
//	}
//}
