using UnityEngine;
using System.Collections;
using System;

public class TrainerSystem : MonoBehaviour {

	public Currency player_currency;

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{

		}

		if(Input.GetKeyDown(KeyCode.Backspace))
		{
			PlayerPrefs.DeleteKey("training_timer_server_side");
		}

		if(Input.GetKeyDown(KeyCode.Q))
		{
			BeginTraining();
		}
	}

	public void BeginTraining()
	{
		// Process attempt to train. Server will validate if we can:
		//  - valid currency amount
		//  - currently not in training

		BeginTrainerInfo trainer_info = new BeginTrainerInfo();

		Currency set_currency = new Currency();
		set_currency.AddTo(CURRENCY_TYPE.DEEP_IRON, 1000);

		trainer_info.currency = set_currency;   // cost of training
		Debug.Log(string.Format("BeginTraining: {0}", trainer_info.currency));
		trainer_info.exp_gain = 500;		    // how much exp will be gained form the training 
		trainer_info.delayed_time_in_hours = 1; // how long training will take

		trainer_info.xml_datetime_start_date = XMLUtil.Serialize<DateTime>(DateTime.UtcNow); // timing stuff my not be synced properly given timezone differences

		string data = XMLUtil.Serialize<BeginTrainerInfo> (trainer_info);

		Request request = new Request();
		request.id = "BeingTraining";
		request.payload = data;
		request.callback = BeingTrainingCallback;
		GameMaster.SendRequest(request);
	}
	
	public void BeingTrainingCallback(Response response)
	{
		ServerSideBeginTrainerInfo sti = XMLUtil.Deserialize<ServerSideBeginTrainerInfo> (response.payload);
		player_currency = sti.player_remaining_currency;

		if(sti.authorize_training_successful)
		{
			Debug.Log("BEGIN TRAINING");
		}
		else
		{
			Debug.Log(sti.response_msg);
		}
	}
}
