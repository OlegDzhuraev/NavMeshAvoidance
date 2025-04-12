using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshAvoidance
{
    [DisallowMultipleComponent]
    public sealed class Avoidance : MonoBehaviour 
    {
        readonly List<NavMeshAgent> agents = new ();
        readonly List<Transform> agentsTransforms = new ();
        readonly List<Vector3> agentsPositions = new ();

        [Tooltip("Speed of agents \"pushing\" from each other in m/s. Increase to make avoidance more noticeable. Default is 1.")]
        [SerializeField, Range(0f, 300f)] float strength = 1;
        [Tooltip("Agents will try to keep this distance between them. Something like NavMeshAgent.Radius, but same for all agents. Do not make this value bigger than Max Avoidance Distance.")]
        [SerializeField, Range(0.1f, 15f)] float distance = 1;

        [Header("Optimization")]
        [Tooltip("Agents will ignore others with distance greater than this value. Bigger value can decrease performance.")]
        [SerializeField, Range(0.1f, 15f)] float maxAvoidanceDistance = 3f;
        [Tooltip("If enabled, will calculate avoidance only between nearest agents. Significantly improves performance for big agents crowds, but can reduce avoidance accuracy.")]
        [SerializeField] bool useSpatialHash = true;
        [Tooltip("World will be separated to cells, agent will know only about agents in the nearest cells to him")]
        [SerializeField, Range(1, 20)] int spatialHashCellSize = 2;
        [Tooltip("Smaller values increasing accuracy, but can affect performance.")]
        [SerializeField, Range(0.05f, 5f)] float spatialHashUpdatePeriod = 0.5f;
        [Tooltip("Estimate average agent size for your game. Usually it is in range of 1-3f. Correct value prevents spatial hash calculation errors.")]
        [SerializeField, Range(0.1f, 5f)] float averageAgentSize = 1f;

        [Header("Debug")]
        [SerializeField] bool showDebugGizmos = true;
        
        public float Distance => distance;

        float sqrMaxAvoidanceDistance;
        float sqrDistance;
        SpatialHashProcessor spatialHash;

        readonly Vector3 vec3Zero = Vector3.zero;

        void OnValidate()
        {
            if (maxAvoidanceDistance < distance)
            {
                Debug.LogWarning("Max Avoidance Distance value should be bigger than Distance!");
                maxAvoidanceDistance = distance;
            }

            if (spatialHashCellSize <= maxAvoidanceDistance)
            {
                Debug.LogWarning("Spatial Hash cell size should be bigger than Max Avoidance Distance!");
                spatialHashCellSize = (int)(maxAvoidanceDistance + 1);
            }
        }

        void Awake()
        {
            sqrMaxAvoidanceDistance = Mathf.Pow(maxAvoidanceDistance, 2);
            sqrDistance = Mathf.Pow(distance, 2);

            if (useSpatialHash)
            {
                spatialHash = new SpatialHashProcessor(spatialHashCellSize, averageAgentSize);
                UpdatePositions();
                StartCoroutine(UpdateSpatialHash());
            }
        }

        IEnumerator UpdateSpatialHash()
        {
            while (true)
            {
                spatialHash.Rebuild(agentsPositions);
                yield return new WaitForSeconds(spatialHashUpdatePeriod);
            }
        }

        void Update()
        {
            UpdatePositions();
            CalculateAvoidance();
        }

        void UpdatePositions()
        {
            agentsPositions.Clear();

            foreach (var tr in agentsTransforms)
                agentsPositions.Add(tr.position);
        }

        void CalculateAvoidance()
        {
            var agentsTotal = agents.Count;
            var deltaTime = Time.deltaTime;
            
            for (var q = 0; q < agentsTotal; q++)
            {
                var spatialIds = spatialHash?.NearestTargets?[q];
                var otherTotal = useSpatialHash ? spatialIds!.Count : agentsTotal;

                var avoidanceVector = vec3Zero;
                var agentAPos = otherTotal > 0 ? agentsPositions[q] : vec3Zero;

                for (var w = 0; w < otherTotal; w++)
                {
                    var agentBIndex = useSpatialHash ? spatialIds![w] : w;
                    var direction = agentAPos - agentsPositions[agentBIndex];
                    var curSqrDistance = direction.sqrMagnitude;

                    if (curSqrDistance > sqrMaxAvoidanceDistance)
                        continue;

                    var weakness = curSqrDistance / sqrDistance;

                    if (weakness > 1f)
                        continue;

                    direction.y = 0;

                    avoidanceVector += Vector3.Lerp(direction * strength, vec3Zero, weakness);
                }

                if (showDebugGizmos)
                    Debug.DrawRay(agentsPositions[q], avoidanceVector, Color.green);

                agents[q].Move(avoidanceVector * (strength * deltaTime));
            }
        }

        public void AddAgent(NavMeshAgent agent)
        {
            var tr = agent.transform;

            if (useSpatialHash)
                spatialHash.Add(agents.Count); // there can be maximum value of agents.Count, so adding it as current maximum id.

            agents.Add(agent);
            agentsTransforms.Add(tr);
            agentsPositions.Add(tr.position);
        }

        public void RemoveAgent(NavMeshAgent agent)
        {
            var tr = agent.transform;

            var index = agents.IndexOf(agent);
            agents.RemoveAt(index);
            agentsTransforms.Remove(tr);
            agentsPositions.RemoveAt(index);

            if (useSpatialHash)
                spatialHash.Remove(agentsPositions);
        }

        void OnDrawGizmos()
        {
            if (!showDebugGizmos)
                return;

            for (var i = 0; i < agents.Count; i++)
                Gizmos.DrawRay(agents[i].destination, Vector3.up);
        }
    }
}