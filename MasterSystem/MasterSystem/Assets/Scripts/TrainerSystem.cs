using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(MasterSystem))]
public class TrainerSystem : MonoBehaviour {

	public Currency player_currency;
	private Coroutine _training_stop_event;

	public void Start()
	{
		_training_stop_event = null;

		if(_training_stop_event == null)
		{
			UpdateTraining();
		}
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{

		}

		if(Input.GetKeyDown(KeyCode.Backspace))
		{
			PlayerPrefs.DeleteKey("training_timer_server_side");
//			PlayerPrefs.DeleteKey("server_side_player_attr_exp_amount");
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
//		set_currency.AddTo(CURRENCY_TYPE.DEEP_IRON, 1000);

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
			DateTime now  = DateTime.UtcNow.ToUniversalTime();
			DateTime end = XMLUtil.Deserialize<DateTime>(sti.xml_datetime_end_date).ToUniversalTime();
			Debug.Log(string.Format("Now time: {0}", now.ToLocalTime()));
			Debug.Log(string.Format("End time: {0}", end.ToLocalTime()));

			float delay_in_seconds = (float)(end.ToUniversalTime() - now).TotalSeconds;
			Debug.Log(string.Format("delay_in_seconds: {0}", delay_in_seconds));
			_training_stop_event = StartCoroutine(TrainingCompleted(delay_in_seconds));
		}
		else
		{
			Debug.Log(sti.response_msg);
		}
	}

	IEnumerator TrainingCompleted(float delay)
	{
		yield return new WaitForSeconds(delay);
		Debug.Log("Training fully completed. Check with server to confirm.");
	}

	public void UpdateTraining()
	{
		// check if training is complete, don't need any payload. Just ping it.
		// Possibly in the future send over client id to distinguish client
		Request request = new Request();
		request.id = "UpdateTraining";
		request.payload = "";
		request.callback = UpdateTrainingCallback;
		GameMaster.SendRequest(request);
	}

	public void UpdateTrainingCallback(Response response)
	{
		ServerSideUpdateTrainerInfo sti = XMLUtil.Deserialize<ServerSideUpdateTrainerInfo> (response.payload);

		if(sti.training_completed)
		{
			// send EndTraining request 
			Debug.Log("Send EndTraining request");
			EndTraining();
//			PlayerPrefs.DeleteKey("training_timer_server_side"); // for now
		}
		else
		{
			Debug.Log("Training is not completed yet or has not started");
			// coroutine not active yet
			if(_training_stop_event == null)
			{
				if(sti.xml_datetime_end_date != null && sti.xml_datetime_end_date != "")
				{
					DateTime now  = DateTime.UtcNow.ToUniversalTime();
					DateTime end = XMLUtil.Deserialize<DateTime>(sti.xml_datetime_end_date).ToUniversalTime();
					
					float delay_in_seconds = (float)(end.ToUniversalTime() - now).TotalSeconds;
					_training_stop_event = StartCoroutine(TrainingCompleted(delay_in_seconds));
					Debug.Log(string.Format("Set coroutine: End training in {0} seconds ({1} local time)", delay_in_seconds, end.ToLocalTime()));	
				}
			}
		}
	}

	public void EndTraining(bool force_terminate = false)
	{
		// send simple request to server to say we want to process a terminate
		EndTrainerInfo ti = new EndTrainerInfo();
		ti.force_terminate = force_terminate;

		Request request = new Request();
		request.id = "EndTraining";
		request.payload = XMLUtil.Serialize<EndTrainerInfo>(ti);
		request.callback = EndTrainingCallback;
		GameMaster.SendRequest(request);
	}

	public void EndTrainingCallback(Response response)
	{
		ServerSideEndTrainerInfo sti = XMLUtil.Deserialize<ServerSideEndTrainerInfo> (response.payload);

		if(sti.training_completed)
		{
			Debug.Log("Congratz, training offically done");
			this.GetComponent<MasterSystem>().SetTotalAttrExp(sti.exp_gained_amount); 

			// do some visual thingy
		}
		else
		{
			Debug.Log("Training is not completed yet");

			// coroutine not active yet
			if(_training_stop_event == null)
			{
				if(sti.xml_datetime_end_date != null && sti.xml_datetime_end_date != "")
				{
					DateTime now  = DateTime.UtcNow.ToUniversalTime();
					DateTime end = XMLUtil.Deserialize<DateTime>(sti.xml_datetime_end_date).ToUniversalTime();
					
					float delay_in_seconds = (float)(end.ToUniversalTime() - now).TotalSeconds;
					_training_stop_event = StartCoroutine(TrainingCompleted(delay_in_seconds));
					Debug.Log(string.Format("Set coroutine: End training in {0} seconds ({1} local time)", delay_in_seconds, end.ToLocalTime()));	
				}
			}
		}
	}
}
