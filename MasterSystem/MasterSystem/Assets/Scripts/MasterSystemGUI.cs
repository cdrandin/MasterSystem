using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(MasterSystem))]
public class MasterSystemGUI : MonoBehaviour 
{
	public Text attr_lvl_text;
	public Text attr_exp_text;
	public Image attr_exp_bar;

	public Text response_text;
	public Text total_attr_exp_gain_text;
	public Text current_attr_exp_gain_text;

	public Text current_datetime_text;
	public Text end_datetime_text;

	public Text deep_iron_amount_text;
	public Text dream_shard_amount_text;
	public Text ethereal_dust_amount_text;

	public Text text_background_unit_name_text;
	public Text text_background_str_text;
	public Text text_background_dex_text;
	public Text text_background_will_text;

	private float delay = 1/25;

	private MasterSystem _ms;
	private TrainerSystem _ts;
	private UnitSelectionLogic _usl;

	private int _lvl_max_amount = 100;
	private int _exp_max_amount = 3000;

	// Use this for initialization
	void Awake () {
		_ms = this.GetComponent<MasterSystem>();
		_ts = this.GetComponent<TrainerSystem>();
		_usl = this.GetComponent<UnitSelectionLogic>();

		response_text.text = "";
	}

	void Start()
	{
		StartCoroutine(MyDelyedUpdate(delay));
	}

	void Update()
	{
		current_datetime_text.text = string.Format("Current time: {0}", System.DateTime.Now.ToLocalTime().ToString());
	}

	IEnumerator MyDelyedUpdate(float time)
	{
		while(true)
		{
			MyUpdate();
			yield return new WaitForSeconds(time);
		}
	}

	void MyUpdate()
	{
		if(_ms.focused_attr != null)
		{
			attr_lvl_text.text = string.Format("{0}/{1}", _ms.focused_attr.current_lvl, _lvl_max_amount);  // " " for max lvl
			attr_exp_text.text = string.Format("+ {0}/{1}", _ms.focused_attr.current_exp_amount.ToString("0.##"), _exp_max_amount.ToString("0.##")); // not sure how to get the max lvl from server or if we care to do so
			attr_exp_bar.fillAmount = (float)_ms.focused_attr.current_exp_amount / (float)_exp_max_amount;
		}

		total_attr_exp_gain_text.text = _ts.total_attr_exp_amount.ToString("0.##");
		current_attr_exp_gain_text.text = _ms.current_exp_gain.ToString("0.##");
	
		deep_iron_amount_text.text = _ts.player_currency.deep_iron_amount.ToString();
		dream_shard_amount_text.text = _ts.player_currency.dream_shard_amount.ToString();
		ethereal_dust_amount_text.text = _ts.player_currency.ethereal_dust_amount.ToString();

		Attribute str  = _ms.attributes[ATTRIBUTE_TYPE.STR] as Attribute;
		Attribute dex  = _ms.attributes[ATTRIBUTE_TYPE.DEX] as Attribute;
		Attribute will = _ms.attributes[ATTRIBUTE_TYPE.WILL] as Attribute;
			
		text_background_unit_name_text.text = string.Format("{0}", _usl.focused_id);
			
		text_background_str_text.text = string.Format("STR: {0}", str.current_lvl);
		text_background_dex_text.text = string.Format("DEX: {0}", dex.current_lvl);
		text_background_will_text.text = string.Format("WILL: {0}", will.current_lvl);
	}

	public void SetResponseText(string str, Color? color = null)
	{
		response_text.text = str;
		response_text.color = (color == null)? Color.white : (Color)color;
		DelayAction.instance.Delay(()=>
		{
			response_text.text = "";
		}, 3f);
	}
}
