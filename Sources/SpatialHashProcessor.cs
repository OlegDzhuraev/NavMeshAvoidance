using System.Collections.Generic;
using UnityEngine;

namespace NavMeshAvoidance
{
	public class SpatialHashProcessor
	{
		// sparse set?
		public readonly List<List<int>> NearestTargets = new(64);

		readonly float avgAgentExtents;

		readonly SpatialHash<int> hash;
		readonly Vector3[] boundingBox = new Vector3[4];
		readonly HashSet<int> tempKeys = new (4);

		public SpatialHashProcessor(int cellSize, float averageAgentSize = 1f)
		{
			hash = new SpatialHash<int>(cellSize);
			avgAgentExtents = averageAgentSize / 2;
		}

		public void Add(int index)
		{
			var curCount = NearestTargets.Count;

			for (var i = curCount; i <= index; i++)
				NearestTargets.Add(new List<int>());
		}

		public void Remove(List<Vector3> positions) => Rebuild(positions); // actually not removing, just keeping old values free and re-arranging

		public void Rebuild(List<Vector3> positions)
		{
			hash.Clear();

			for (var q = 0; q < positions.Count; q++)
				hash.Insert(positions[q], q);

			var index = 0;

			foreach (var collection in NearestTargets)
			{
				collection.Clear();
				tempKeys.Clear();

				CalculateBoundingBox(positions[index]);

				foreach (var boundPoint in boundingBox)
				{
					if (hash.ContainsKey(boundPoint, out var key) && tempKeys.Add(key))
						collection.AddRange(hash.QueryPosition(boundPoint));
				}

				index++;
			}
		}

		void CalculateBoundingBox(Vector3 position)
		{
			var x = position.x; var y = position.y; var z = position.z;

			boundingBox[0] = new Vector3(x + avgAgentExtents, y, z + avgAgentExtents);
			boundingBox[1] = new Vector3(x + avgAgentExtents, y, z - avgAgentExtents);
			boundingBox[2] = new Vector3(x - avgAgentExtents, y, z + avgAgentExtents);
			boundingBox[3] = new Vector3(x - avgAgentExtents, y, z - avgAgentExtents);
		}
	}
}