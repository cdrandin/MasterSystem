using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;

public class SecurityHelper 
{
	string RandomString(int length, string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
	{       
		var outOfRange = Byte.MaxValue + 1 - (Byte.MaxValue + 1) % alphabet.Length;
		
		return string.Concat(
			Enumerable
			.Repeat(0, Int32.MaxValue)
			.Select(e => RandomByte())
			.Where(randomByte => randomByte < outOfRange)
			.Take(length)
			.Select(randomByte => alphabet[randomByte % alphabet.Length])
			);
	}

	byte RandomByte()
	{
		var randomizationProvider = new RNGCryptoServiceProvider();
		var randomBytes = new byte[1];
		randomizationProvider.GetBytes(randomBytes);
		return randomBytes.Single();
	}
}
