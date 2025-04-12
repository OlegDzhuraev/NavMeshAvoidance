using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace NavMeshAvoidance
{
	public class SpatialHash<T>
	{
		readonly bool ignoreY;

		readonly Dictionary<int, List<T>> dict;
		readonly Dictionary<T, int> elements;
		readonly int cellSize;

		public SpatialHash(int cellSize, bool ignoreY = true)
		{
			this.ignoreY = ignoreY;
			this.cellSize = cellSize;
			dict = new Dictionary<int, List<T>>();
			elements = new Dictionary<T, int>();
		}

		public void Insert(Vector3 position, T element)
		{
			var key = Key(position);

			if (dict.TryGetValue(key, out var collection))
				collection.Add(element);
			else
				dict[key] = new List<T> { element };

			elements[element] = key;
		}

		public void UpdatePosition(Vector3 position, T obj)
		{
			if (elements.TryGetValue(obj, out var element))
				dict[element].Remove(obj);

			Insert(position, obj);
		}

		public List<T> QueryPosition(Vector3 position)
		{
			var key = Key(position);
			return dict.TryGetValue(key, out var collection) ? collection : new List<T>();
		}

		public bool ContainsKey(Vector3 position, out int key)
		{
			key = Key(position);
			return dict.ContainsKey(key);
		}

		public void Clear()
		{
			var keys = dict.Keys.ToArray();

			for (var i = 0; i < keys.Length; i++)
				dict[keys[i]].Clear();

			elements.Clear();
		}

		public void Reset()
		{
			dict.Clear();
			elements.Clear();
		}

		const int BigEnoughInt = 16 * 1024;
		const double BigEnoughFloor = BigEnoughInt + 0.0000;
		const int X = 83709564, Y = 91374936, Z = 38947219;

		static int FastFloor(float f) => (int)(f + BigEnoughFloor) - BigEnoughInt;

		int Key(Vector3 p)
		{
			var x = FastFloor(p.x / cellSize) * X;
			var y = FastFloor(p.z / cellSize) * Y;

			if (ignoreY)
				return x ^ y;

			var z = FastFloor(p.z / cellSize) * Z;
			return x ^ y ^ z;
		}
	}
}