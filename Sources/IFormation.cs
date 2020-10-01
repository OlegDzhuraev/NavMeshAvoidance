using UnityEngine;

namespace NavMeshAvoidance
{
    public interface IFormation
    {
        Vector3[] GetPositions(Vector3 startPosition, int count, float indent);
    }
}