using System.Collections;
using System.Collections.Generic;

public delegate Response RequestDelegate(Request request);

public class GameLogicGateway 
{
	private static Dictionary  <string, RequestDelegate> requests = new Dictionary<string, RequestDelegate>();

	// This function servs as an entry point for all simulated server side logic.
	public static void RegisterAllLogic()
	{
		// register all callbacks
		AddLogic ("LevelUpItem", ItemLogic.LevelUpItem);
		AddLogic ("AddExp", CharacterAttributesLogic.AddExp);
	}

	public static void AddLogic(string logicRequestId, RequestDelegate requestDelegate)
	{
		if (!requests.ContainsKey (logicRequestId))
			requests.Add (logicRequestId, requestDelegate);
	}
	
	public static Response Process(Request request)
	{
		// Process response logic.
		if(requests.ContainsKey (request.id))
		{
			return requests[request.id](request);
		}

		// if no logic response was registered, return a response error.
		Response errorResponse = new Response ();
		errorResponse.error = true;
		errorResponse.payload = "Error No Logic for request with Id " + request.id;
		return errorResponse;
	}
}
