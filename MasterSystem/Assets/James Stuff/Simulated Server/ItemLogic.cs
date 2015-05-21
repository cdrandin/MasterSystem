using System.Collections;

public class ItemLogic 
{
	private static int maxLevel = 10;

	// Example Game Logic
	public static Response LevelUpItem(Request request)
	{
		// Process logic for game rules
		// Super simple logic in this example
		Item i = XMLUtil.Deserialize<Item> (request.payload);
		if(i.level < maxLevel)
			i.level ++; // Level up

		Response response = new Response ();
		response.payload = XMLUtil.Serialize<Item>(i);
		response.error = false;
		return response;
	}
}
