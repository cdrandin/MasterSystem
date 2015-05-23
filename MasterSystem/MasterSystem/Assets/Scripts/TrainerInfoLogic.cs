using UnityEngine;
using System.Collections;
using System;

public class TrainerInfoLogic
{
	private static string server_side_trainer_end_time_id = "training_timer_server_side";
	private static string server_side_player_currency_id = "server_side_player_currency";
	private static string server_side_player_attr_exp_amount_id = "server_side_player_attr_exp_amount";

	public static Response BeginTraining(Request request)
	{
		BeginTrainerInfo ti = XMLUtil.Deserialize<BeginTrainerInfo> (request.payload);

		string end_date_time  = SimpleSerializer.Load<string>(server_side_trainer_end_time_id);
		bool _new = false;

		// no record of end time, set one
		if(end_date_time == null)
		{
			DateTime start_date = DateTime.UtcNow.ToUniversalTime();
			DateTime end_date = start_date.AddHours(ti.delayed_time_in_hours).ToUniversalTime();
			end_date_time = XMLUtil.Serialize(end_date);

			SimpleSerializer.Save<string>(server_side_trainer_end_time_id,  end_date_time);
			_new = true;
			Debug.Log(string.Format("HOUR: {0}", ti.delayed_time_in_hours));
			Debug.Log(string.Format("Start time (UTC): {0}", start_date));
			Debug.Log(string.Format("Create new end date time (UTC): {0}", end_date));
		}
 
		DateTime projected_end_time = XMLUtil.Deserialize<DateTime>(end_date_time).ToUniversalTime();

		Debug.Log(string.Format("Projected end time (UTC): {0}", projected_end_time));

		ServerSideBeginTrainerInfo sti = new ServerSideBeginTrainerInfo();
		bool valid = true; // determines if the training has been thoroughly completed;

		// Training still in progress...
		if(!_new && DateTime.UtcNow < projected_end_time)
		{
			Debug.Log("Training still in progress...");
			sti.response_msg = "Training still in progress!";
			valid = false;
		}

		if(valid)
		{
			Currency player_currency = SimpleSerializer.Load<Currency>(server_side_player_currency_id);
		
			// in the future possibly better to make singleton of player's currency or some unified way to organize and record it
			if(player_currency == null)
			{
				player_currency = new Currency();
				SimpleSerializer.Save<Currency>(server_side_player_currency_id, player_currency);
			}

			Debug.Log(string.Format("Player: {0}", player_currency));
			Debug.Log(string.Format("Cost: {0}", ti.currency));

			// player has enough money to purchase training
			if(player_currency.EnoughOf(ti.currency))
			{
				Debug.Log("Enough to purchase training!");
				sti.response_msg = "Successful";

				// deduct money
				player_currency.SubTo(CURRENCY_TYPE.DEEP_IRON, (uint)ti.currency.deep_iron_amount);
				player_currency.SubTo(CURRENCY_TYPE.DREAM_SHARD, (uint)ti.currency.dream_shard_amount);
				player_currency.SubTo(CURRENCY_TYPE.ETHEREAL_DUST, (uint)ti.currency.ethereal_dust_amount);

				PlayerPrefs.SetInt(server_side_player_attr_exp_amount_id + PlayerPrefs.GetInt(server_side_player_attr_exp_amount_id), ti.exp_gain);
				Debug.Log(string.Format("Total attr exp: {0}", PlayerPrefs.GetInt(server_side_player_attr_exp_amount_id)));
			}
			else
			{
				Debug.Log("Cannot afford to purchase training!");
				sti.response_msg = "Cannot afford to purchase training!";
				valid = false;
			}

			sti.player_remaining_currency = player_currency;
		}

		sti. xml_datetime_end_date = XMLUtil.Serialize<DateTime>(projected_end_time);
		sti.authorize_training_successful = valid;

//		if(!valid)
//		{
//			PlayerPrefs.DeleteKey(server_side_trainer_end_time_id);
//		} 

		Response response = new Response ();
		response.payload = XMLUtil.Serialize<ServerSideBeginTrainerInfo>(sti);
		response.error = false;
		return response;
	}

	public static Response UpdateTraining(Request request)
	{
		ServerSideUpdateTrainerInfo sti = new ServerSideUpdateTrainerInfo();
		sti.training_completed = TrainerInfoLogic.TrainingCompleted();
		sti.xml_datetime_end_date = ""; // no date since completed

		if(sti.training_completed)
		{
			sti.response_msg = "Training Completed";
		}
		else
		{
			sti.response_msg = "Training still in progress...";
			string end_date_time  = SimpleSerializer.Load<string>(server_side_trainer_end_time_id);

			// send end time to client if one exist
			if(end_date_time != null)
			{
				DateTime end = XMLUtil.Deserialize<DateTime>(SimpleSerializer.Load<string>(server_side_trainer_end_time_id)).ToUniversalTime();
				sti.xml_datetime_end_date = XMLUtil.Serialize<DateTime>(end);
			}
		}

		Response response = new Response ();
		response.payload = XMLUtil.Serialize<ServerSideUpdateTrainerInfo>(sti);
		response.error = false;
		return response;
	}

	public static Response EndTraining(Request request)
	{
		EndTrainerInfo ti = XMLUtil.Deserialize<EndTrainerInfo> (request.payload);
		ServerSideEndTrainerInfo sti = new ServerSideEndTrainerInfo();
		sti.training_completed = false;

		if(ti.force_terminate)
		{
			sti.exp_gained_amount = PlayerPrefs.GetInt(server_side_player_attr_exp_amount_id);
			sti.xml_datetime_end_date = ""; // no date since completed
			sti.training_completed = true;

			// delete stores records since training is over
			PlayerPrefs.DeleteKey(server_side_trainer_end_time_id);
//			PlayerPrefs.DeleteKey(server_side_player_attr_exp_amount_id);
		}
		else
		{
			if(TrainerInfoLogic.TrainingCompleted())
			{
				sti.exp_gained_amount = PlayerPrefs.GetInt(server_side_player_attr_exp_amount_id) ;
				sti.xml_datetime_end_date = ""; // no date since completed
				sti.training_completed = true;

				// delete stores records since training is over
				PlayerPrefs.DeleteKey(server_side_trainer_end_time_id);
//				PlayerPrefs.DeleteKey(server_side_player_attr_exp_amount_id);
			}
			else
			{
				// training still in progress. Give end time to client.
				sti.xml_datetime_end_date = SimpleSerializer.Load<string>(server_side_trainer_end_time_id);
			}
		}

		Response response = new Response ();
		response.payload = XMLUtil.Serialize<ServerSideEndTrainerInfo>(sti);
		response.error = false;
		return response;
	}

	private static bool TrainingCompleted()
	{
		string end_date_time  = SimpleSerializer.Load<string>(server_side_trainer_end_time_id);
		bool valid = true;
		
		// no end date stored, so no training in progress. Send false
		if(end_date_time == null)
		{
			valid = false;
		}
		else // end time is stored
		{
			DateTime end = XMLUtil.Deserialize<DateTime>(SimpleSerializer.Load<string>(server_side_trainer_end_time_id)).ToUniversalTime();
			if(DateTime.UtcNow.ToUniversalTime() < end)
			{
				valid = false;
			}
		}
		
		return valid;
	}
}
