using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(MasterSystem))]
public class TrainerSystem : MonoBehaviour {

	public Currency player_currency;

	// Player's total attr exp on client side. On any major event this will make sure to update with server
	public int total_attr_exp_amount;
	private int _max_atr_exp_amount;
	public int max_atr_exp_amount
	{
		get { return _max_atr_exp_amount; }
	}

	private Coroutine _training_stop_event;
	private MasterSystemGUI _msg;

	public void Awake()
	{
		_msg = this.GetComponent<MasterSystemGUI>();

		_training_stop_event = null;
	}

	public void Start()
	{
		UpdateTraining();
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{

		}

		if(Input.GetKeyDown(KeyCode.P))
		{
			Currency set_currency = SimpleSerializer.GetOrCreate<Currency>(TrainerInfoLogic.server_side_player_currency_id);
			set_currency.AddTo(CURRENCY_TYPE.DEEP_IRON, 1000);
			set_currency.AddTo(CURRENCY_TYPE.DREAM_SHARD, 1000);
			set_currency.AddTo(CURRENCY_TYPE.ETHEREAL_DUST, 1000);
			SimpleSerializer.Save<Currency>(TrainerInfoLogic.server_side_player_currency_id, set_currency);
			player_currency = set_currency;
		}

		if(Input.GetKeyDown(KeyCode.PageDown))
		{
			PlayerPrefs.DeleteAll();
//			PlayerPrefs.DeleteKey(TrainerInfoLogic.server_side_trainer_end_time_id);
//			PlayerPrefs.DeleteKey("server_side_player_attr_exp_amount");
		}

		if(Input.GetKeyDown(KeyCode.Q))
		{
			BeginTraining();
		}
	}

	public void BeginTrainingWithInfo(TrainingSessionInfo tsi)
	{
		// Process attempt to train. Server will validate if we can:
		//  - valid currency amount
		//  - currently not in training
		
		BeginTrainerInfo trainer_info = new BeginTrainerInfo();
		
		trainer_info.cost = tsi.cost;   // cost of training
		Debug.Log(string.Format("BeginTraining: {0}", trainer_info.cost));
		trainer_info.exp_gain = tsi.exp_gain_amount;		    // how much exp will be gained form the training 
		trainer_info.delayed_time_in_hours = tsi.time_amount_in_hours;// (10s) // how long training will take
		
		trainer_info.xml_datetime_start_date = XMLUtil.Serialize<DateTime>(DateTime.UtcNow); // timing stuff my not be synced properly given timezone differences
		
		string data = XMLUtil.Serialize<BeginTrainerInfo> (trainer_info);
		
		Request request = new Request();
		request.id = "BeingTraining";
		request.payload = data;
		request.callback = BeingTrainingCallback;
		GameMaster.SendRequest(request);
	}

	public void BeginTraining()
	{
		// Process attempt to train. Server will validate if we can:
		//  - valid currency amount
		//  - currently not in training

		BeginTrainerInfo trainer_info = new BeginTrainerInfo();

		Currency cost = new Currency();
		cost.AddTo(CURRENCY_TYPE.DEEP_IRON, 500);

		trainer_info.cost = cost;   // cost of training
		Debug.Log(string.Format("BeginTraining: {0}", trainer_info.cost));
		trainer_info.exp_gain = 500;		    // how much exp will be gained form the training 
		trainer_info.delayed_time_in_hours = 0.00277778f;// (10s) // how long training will take

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
		total_attr_exp_amount = sti.total_attr_exp_amount;
		_max_atr_exp_amount = total_attr_exp_amount;

		if(sti.authorize_training_successful)
		{
			Debug.Log("BEGIN TRAINING");
			DateTime now  = DateTime.UtcNow.ToUniversalTime();
			DateTime end = XMLUtil.Deserialize<DateTime>(sti.xml_datetime_end_date).ToUniversalTime();
			_msg.end_datetime_text.text = string.Format("End time: {0}", end.ToLocalTime());

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

		_msg.SetResponseText(sti.response_msg);
	}

	IEnumerator TrainingCompleted(float delay)
	{
		yield return new WaitForSeconds(delay+0.5f);
		Debug.Log("Training fully completed. Check with server to confirm.");
		EndTraining();
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
		player_currency = sti.player_remaining_currency;
		total_attr_exp_amount = sti.total_attr_exp_amount;
		_max_atr_exp_amount = total_attr_exp_amount;

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
					_msg.end_datetime_text.text = string.Format("End time: {0}", end.ToLocalTime());

					float delay_in_seconds = (float)(end.ToUniversalTime() - now).TotalSeconds;
					_training_stop_event = StartCoroutine(TrainingCompleted(delay_in_seconds));
					Debug.Log(string.Format("Set coroutine: End training in {0} seconds ({1} local time)", delay_in_seconds, end.ToLocalTime()));	
				}
			}
		}

		_msg.SetResponseText(sti.response_msg);
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
			total_attr_exp_amount = sti.exp_gained_amount;
			_max_atr_exp_amount = total_attr_exp_amount;

			_msg.end_datetime_text.text = "";
			_msg.SetResponseText(string.Format("Training Completed!\n +{0} exp pts", total_attr_exp_amount));
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
					_msg.SetResponseText(sti.response_msg);

					float delay_in_seconds = (float)(end.ToUniversalTime() - now).TotalSeconds;
					_training_stop_event = StartCoroutine(TrainingCompleted(delay_in_seconds));
					Debug.Log(string.Format("Set coroutine: End training in {0} seconds ({1} local time)", delay_in_seconds, end.ToLocalTime()));	
				}
			}
		}
	}
}
