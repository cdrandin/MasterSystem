using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum TIME_MEASUREMENT
{
	SECONDS = 0,
	MINUTES,
	HOURS
}

public class TrainingSessionInfo : MonoBehaviour
{
	public Currency cost;
	public float time_amount;
	public float time_amount_in_hours
	{
		get 
		{
			float amount = 0;
			float SECONDS_TO_HOURS = 0.000277778f;
			float MINUTES_TO_HOURS = 0.0166667f;
			
			switch(time_measurement)
			{
			case TIME_MEASUREMENT.HOURS:
				amount = this.time_amount;
				break;
			case TIME_MEASUREMENT.MINUTES:
				amount = this.time_amount * MINUTES_TO_HOURS;
				break;
			case TIME_MEASUREMENT.SECONDS:
				amount = this.time_amount * SECONDS_TO_HOURS;
				break;
			}
			
			return amount;
		}
	}

	public TIME_MEASUREMENT time_measurement;
	public int exp_gain_amount;

	// gui stuff
	public Text hour_button_text;
	public Text cost_amount_text;
	public Text exp_gain_amount_text;

	// Use this for initialization
	void Start () {
		hour_button_text.text = string.Format("{0} {1}", time_amount, TimeMeasurementEnumToString());
		cost_amount_text.text = string.Format("Cost:\n {0}", SimpleCurrencyToString());
		exp_gain_amount_text.text = string.Format("Gives: {0}", exp_gain_amount.ToString("0.##"));
	}

	private string TimeMeasurementEnumToString()
	{
		string measurement_name = "";
		switch(time_measurement)
		{
		case TIME_MEASUREMENT.HOURS:
			measurement_name = "HR";
			break;
		case TIME_MEASUREMENT.MINUTES:
			measurement_name = "MIN";
			break;
		case TIME_MEASUREMENT.SECONDS:
			measurement_name = "SEC";
			break;
		}

		return measurement_name;
	}

	private string SimpleCurrencyToString()
	{
		return string.Format("Deep Iron: {0}\nDream Shard: {1}\nEthereal Dust: {2}", cost.deep_iron_amount, cost.dream_shard_amount, cost.ethereal_dust_amount);
	}
}
