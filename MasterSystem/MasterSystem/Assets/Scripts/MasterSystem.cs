using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public enum ATTRIBUTE_TYPE
//{
//	STR = 0,
//	DEX = 1,
//	WILL = 2
//}

[RequireComponent (typeof(MasterSystemGUI), typeof(TrainerSystem))]
public class MasterSystem : MonoBehaviour 
{
	public Hashtable attributes;
	private ATTRIBUTE_TYPE _focused_type;
	public Attribute focused_attr
	{
		get	{ return attributes[_focused_type] as Attribute; }
	}

	private int _total_attr_exp_amount = 0;

	public void SetTotalAttrExp(int amount)
	{
		_total_attr_exp_amount = amount;
	}

	private int _current_exp_gain;
	public int current_exp_gain
	{
		get { return _current_exp_gain; }
	}

	private TrainerSystem _ts;

	public void Awake()
	{
		attributes = new Hashtable();
		attributes[ATTRIBUTE_TYPE.STR] = new Attribute(ATTRIBUTE_TYPE.STR);
		attributes[ATTRIBUTE_TYPE.DEX] = new Attribute(ATTRIBUTE_TYPE.DEX);
		attributes[ATTRIBUTE_TYPE.WILL] = new Attribute(ATTRIBUTE_TYPE.WILL);
	}

	public void Start()
	{
		_ts = this.GetComponent<TrainerSystem>();
		UpdateAttributes();
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.PageDown))
		{
			PlayerPrefs.DeleteKey(CharacterAttributesLogic.server_side_character_attributes_id);
		}
	}

	public void SetAttrStrFocus()
	{
		_focused_type = ATTRIBUTE_TYPE.STR;
	}

	public void SetAttrDexFocus()
	{
		_focused_type = ATTRIBUTE_TYPE.DEX;
	}

	public void SetAttrWillFocus()
	{
		_focused_type = ATTRIBUTE_TYPE.WILL;
	}

	public void AddToExpGain(int amount)
	{
		// Subtract amount from total exp that can be used
		if(_ts.total_attr_exp_amount > 0)
		{
			_ts.total_attr_exp_amount = Mathf.Clamp(_ts.total_attr_exp_amount - amount, 0, int.MaxValue);
			_current_exp_gain = Mathf.Clamp(_current_exp_gain + amount, 0, _ts.max_atr_exp_amount);
		}
	}

	public void ResetExpGain()
	{
		_ts.UpdateTraining();
		_current_exp_gain = 0;
	}

	public void AddExpToFocused()
	{
		AddAttributeExp(attributes[_focused_type] as Attribute, _current_exp_gain);
		_current_exp_gain = 0;
	}

	private void AddAttributeExp(Attribute attr, int amount)
	{
		ServerSideAttribute server_attr = new ServerSideAttribute();
		server_attr.attr = attr;
		server_attr.exp_amount = amount;

		string data = XMLUtil.Serialize<ServerSideAttribute> (server_attr);
		Request request = new Request ();
		request.id = "AddAttributeExp";
		request.payload = data;
		request.callback = AddAttributeExpCallback;
		GameMaster.SendRequest (request);
	}

	public void AddAttributeExpCallback(Response response)
	{
		Attribute attr = XMLUtil.Deserialize<Attribute> (response.payload);

		if(attributes.ContainsKey(attr.attr_type))
		{
			attributes[attr.attr_type] = attr;
		}
		else
		{
			Debug.LogError(string.Format("%s invalid attribute name or corrupted", attr.attr_type));
		}
		// Possibly gui here or send singal to update gui
	}


	public void UpdateAttributes()
	{
		Request request = new Request ();
		request.id = "UpdateAttributes";
		request.payload = "";
		request.callback = UpdateAttributesCallback;
		GameMaster.SendRequest (request);
	}

	public void UpdateAttributesCallback(Response response)
	{
		ServerSideUpdateAttribute ssua = XMLUtil.Deserialize<ServerSideUpdateAttribute> (response.payload);
		Debug.Log(string.Format("UpdateAttributesCallback: {0}", ssua));

		attributes[ATTRIBUTE_TYPE.STR] = ssua.str;
		attributes[ATTRIBUTE_TYPE.DEX] = ssua.dex;
		attributes[ATTRIBUTE_TYPE.WILL] = ssua.will;
	}
}
