using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class BeginTrainerInfo  
{
	public int exp_gain;
	public int delayed_time_in_hours;
	public string xml_datetime_start_date;
	public Currency currency;
}

[System.Serializable]
public class ServerSideBeginTrainerInfo  
{
	public bool authorize_training_successful;
	public string response_msg;
	public Currency player_remaining_currency;
}
