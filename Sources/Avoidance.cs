using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshAvoidance
{
    public sealed class Avoidance : MonoBehaviour // todo avoidance - add strength?
    {
        readonly List<NavMeshAgent> agents = new List<NavMeshAgent>();
        
        [Tooltip("Agents will ignore others with distance greather than this value.")]
        [SerializeField, Range(0.1f, 100f)] float maxAvoidanceDistance = 3f;
        [SerializeField] bool showDebugGizmos = true;

        float sqrMaxAvoidanceDistance;

        void Awake()
        {
            sqrMaxAvoidanceDistance = Mathf.Pow(maxAvoidanceDistance, 2);
        }

        void Update() => CalcualteAvoidance();

        void CalcualteAvoidance()
        {
            var agentsTotal = agents.Count;
            var deltaTime = Time.deltaTime;
            
            for (var q = 0; q < agentsTotal; q++)
            {
                var agentAPos = agents[q].transform.position;
                
                var avoidanceVector = Vector3.zero;

                for (var w = 0; w < agentsTotal; w++)
                {
                    var agentBPos = agents[w].transform.position;
                    
                    var direction = agentAPos - agentBPos;
                    var sqrDistance = direction.sqrMagnitude;

                    var weakness = sqrDistance / sqrMaxAvoidanceDistance;
                    
                    if (weakness > 1f)
                        continue;
                    
                    direction.y = 0; // i do not sure we need to use Y coord in navmesh directions, so ignoring it
                    
                    avoidanceVector += Vector3.Lerp(direction, Vector3.zero, weakness);
                }
                
                if (showDebugGizmos)
                    Debug.DrawRay(agentAPos, avoidanceVector, Color.green);
                
                agents[q].Move(avoidanceVector.normalized * deltaTime);
            }
        }
        
        public void AddAgent(NavMeshAgent agent) => agents.Add(agent);
        public void RemoveAgent(NavMeshAgent agent) => agents.Remove(agent);
        
        void OnDrawGizmos()
        {
            if (showDebugGizmos)
                for (var i = 0; i < agents.Count; i++)
                    Gizmos.DrawRay(agents[i].destination, Vector3.up);
        }
    }
}