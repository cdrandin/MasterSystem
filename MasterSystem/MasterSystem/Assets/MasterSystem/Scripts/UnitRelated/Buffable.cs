using UnityEngine;
using System.Collections;

public enum POSITIVE_ATTRIBUTE
{
	
}

public enum NEGATIVE_ATTRIBUTE
{
	FROZEN
}

public enum ATTRIBUTES
{
	FROZEN,
	NEGATE_MYSTIC
}

public static class COLOR_BUFFABLES
{
	public static readonly Color FROST_BLUE = new Color(135/255f,206/255f,250/255f);
	public static readonly Color BLUE_VIOLET = new Color(138f/255f,43f/255f,226f/255f);
}

public class Buffable 
{
	private ATTRIBUTES _attributes;

	[Range(0f,float.MaxValue)]
	private float _duration; // when it should expire?

	[Range(0f,float.MaxValue)]
	private float _startTime; // when it starts

	[Range(0f,float.MaxValue)]
	private float _repeatTime; // how much time between each effect tick?

	private System.Action<UnitEntity> _buffable_action;
	private Coroutine _buffable_coroutine;

	private UnitEntity _ue;

	/// <summary>
	/// Initializes a new instance of the <see cref="Buffable"/> class.
	/// If the repeat time is >= the duration. Apply effect once and delay to kill it.
	/// </summary>
	/// <param name="attr">Attr.</param>
	/// <param name="duration">Duration.</param>
	/// <param name="repeat_time">Repeat_time.</param>
	public Buffable(ATTRIBUTES attr, float duration, float repeat_time)
	{
		_attributes 	 = attr;
		_duration 		 = duration;
		_repeatTime      = repeat_time;

		_buffable_action = ApplyBuffableAction(attr);
	}

	private System.Action<UnitEntity> ApplyBuffableAction(ATTRIBUTES attr)
	{
		System.Action<UnitEntity> a = (UnitEntity target)=>{Debug.LogWarning("NO BUFFABLE");};

		switch(attr)
		{
		case ATTRIBUTES.FROZEN:
			a = ApplyBuffable_Frozen();
			break;
		case ATTRIBUTES.NEGATE_MYSTIC:
			a = ApplyBuffable_NegateMystic();
			break;
		}
		return a;
	}

	/// <summary>
	/// Applies the effect. If the repeat time is >= the duration. Apply effect once and delay to kill it.
	/// </summary>
	/// <param name="ue">Ue.</param>
	public void ApplyEffect(UnitEntity ue)
	{
		_ue = ue;

		// Apply regular delay function
		if(_repeatTime < _duration)
		{
			_buffable_coroutine = DelayAction.instance.DelayInf(()=>
			                                                    {
				_startTime = Time.time;
				_buffable_action(ue);
			}, _repeatTime,
			()=>
			{
				// stop buff
				if(Time.time >= _startTime + _duration)
				{
					ue.RemoveBuffable(this);
					return true;
				}
				
				return false;
			});
		}
		else // Play buffable action then delay the end effect
		{
			_buffable_action(ue);
			_buffable_coroutine = DelayAction.instance.Delay(()=>{ ue.RemoveBuffable(this); }, Time.time + _duration);
		}
	}

	public void EndEffect()
	{
		Debug.Log(string.Format("{0} has expired", _attributes));
		ChangeUnitEntityGameObjectRendererColor(Color.white);
	}

	private void ChangeUnitEntityGameObjectRendererColor(Color c)
	{
		_ue.unit_game_object.GetComponent<Renderer>().material.color = c;
	}

	// Buffable options
	private System.Action<UnitEntity> ApplyBuffable_Frozen()
	{
		return delegate(UnitEntity target) 
		{
			Debug.Log(string.Format("{0} is Frozen", target));
			ChangeUnitEntityGameObjectRendererColor(COLOR_BUFFABLES.FROST_BLUE);
		};
	}

	private System.Action<UnitEntity> ApplyBuffable_NegateMystic()
	{
		return delegate(UnitEntity target) 
		{
			Debug.Log(string.Format("{0} is immune to mystic", target));
//			ChangeUnitEntityGameObjectRendererColor(COLOR_BUFFABLES.BLUE_VIOLET);
		};
	}

	public bool HasBuffable(ATTRIBUTES attr)
	{
		return _attributes == attr;
	}

	public override string ToString ()
	{
		return string.Format ("[Buffable]: {0}", _attributes);
	}
}
