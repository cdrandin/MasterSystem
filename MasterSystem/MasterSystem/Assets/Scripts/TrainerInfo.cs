using UnityEngine;
using System.Collections;
using System;

// Begin training objects

[System.Serializable]
public class BeginTrainerInfo  
{
	public int exp_gain;
	public float delayed_time_in_hours;
	public string xml_datetime_start_date;
	public Currency cost;
}

[System.Serializable]
public class ServerSideBeginTrainerInfo  
{
	public bool authorize_training_successful;
	public string response_msg;
	public Currency player_remaining_currency;
	public string xml_datetime_end_date;
	public int total_attr_exp_amount;
}

// -------------------------------------
// Update training objects

[System.Serializable]
public class UpdateTrainerInfo  
{
	public int exp_gain;
	public float delayed_time_in_hours;
	public string xml_datetime_start_date;
	public Currency currency;
}

[System.Serializable]
public class ServerSideUpdateTrainerInfo 
{
	public bool training_completed;
	public string response_msg;
	public Currency player_remaining_currency;
	public string xml_datetime_end_date;
	public int total_attr_exp_amount;
}

// -------------------------------------
// End training objects

[System.Serializable]
public class EndTrainerInfo  
{
	public bool force_terminate;
}

[System.Serializable]
public class ServerSideEndTrainerInfo
{
	public bool training_completed;
	public string response_msg;
	public int exp_gained_amount;
	public string xml_datetime_end_date;
}
