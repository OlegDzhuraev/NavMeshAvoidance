using UnityEngine;

namespace NavMeshAvoidance
{
    public sealed class SquareFormation : IFormation
    {
        public Vector3[] GetPositions(Vector3 startPosition, int count, float indent)
        {
            var resultPositions = new Vector3[count];
            
            var sideCount = Mathf.RoundToInt(Mathf.Sqrt(count));
            int ceil = 0, row = 0;

            for (var i = 0; i < count; i++)
            {
                var offset = new Vector3(indent * ceil, 0, indent * row);
                
                resultPositions[i] = startPosition + offset;
                
                ceil++;

                if (ceil >= sideCount)
                {
                    ceil = 0;
                    row++;
                }
            }

            return resultPositions;
        }
    }
}
