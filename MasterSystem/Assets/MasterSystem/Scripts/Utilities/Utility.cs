using UnityEngine;
using System.Collections;

public class Utility
{
	public static void Swap<T>(ref T lhs, ref T rhs)
	{
		T temp;
		temp = lhs;
		lhs = rhs;
		rhs = temp;
	}
}
