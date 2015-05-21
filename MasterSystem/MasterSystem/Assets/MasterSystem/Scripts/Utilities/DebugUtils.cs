#define DEBUG

using UnityEngine;
using System.Collections;

public class DebugUtils 
{ 
	[System.Diagnostics.Conditional("DEBUG")] 
	public static void Assert(bool condition, string message="") 
	{
		if (!condition) throw new System.Exception(message); 
	} 
}