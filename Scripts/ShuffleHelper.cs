using System;
using System.Collections.Generic;

public static class ShuffleHelper
{
	private static readonly Random _random = new Random();

	public static void Shuffle<T>(IList<T> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
			int j = _random.Next(i + 1);
			(list[i], list[j]) = (list[j], list[i]);
		}
	}

	public static List<T> ToShuffledList<T>(T[] array)
	{
		var list = new List<T>(array);
		Shuffle(list);
		return list;
	}
}
