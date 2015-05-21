using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#region Structs & Delegates
public struct Response
{
	public bool error;
	public string payload;
}

public struct Request
{
	public string id;
	public string payload;
	public ResponseDelegate callback;
}

public delegate void ResponseDelegate(Response response);
#endregion

public class GameMaster : MonoBehaviour
{
	

	private static GameMaster			_instance = null;
	private static bool                 _busy = false;
	private static Queue<Request>		_queue = new Queue<Request>();
		

	public static bool			Busy			{ get { return _busy; } }
	
	public static GameMaster instance
	{
		get 
		{
			if (_instance == null)
			{	
				_instance = FindObjectOfType<GameMaster>();
				if (_instance == null)
				{
					GameObject gm = new GameObject("GameMaster");
					_instance = gm.AddComponent<GameMaster>();
					GameLogicGateway.RegisterAllLogic();
				}
			}
			
			return _instance;
		}
	}


	public static void SendRequest(Request request)
	{
		if (_busy)
		{
			Debug.Log("SendRequest(): already busy, adding to queue...");
			_queue.Enqueue(request);
		}
		else
		{
			Debug.Log("SendRequest(): sending!");
			instance.StartCoroutine(instance.SendRequestRoutine( request));
		}
	}
	
	IEnumerator SendRequestRoutine(Request request)
	{
		_busy = true;


		Response response;
		// Insert server sending logic here
		//while (sending) // Waiting for send to server // no server yet, so simulate the exchange
		{
			Debug.Log("Sending Request: "+request.payload);
			response = GameLogicGateway.Process(request);
			yield return new WaitForEndOfFrame (); // Wait for the server to respond.
		}

		_busy = false;
		
		// Advance queue
		if (_queue.Count != 0)
			SendRequest(_queue.Dequeue());

		if(!response.error)
		{
			if (request.callback != null)
				request.callback(response);
		}
		else
		{
			Debug.LogError("Error in response");
			Debug.LogError(response.payload);
		}
	}
}
