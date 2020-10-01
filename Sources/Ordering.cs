using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshAvoidance
{
    public sealed class Ordering : MonoBehaviour
    {
        readonly List<NavMeshAgent> controlledAgents = new List<NavMeshAgent>();
        readonly IFormation formation = new SquareFormation();
      
        Camera mainCamera;

        void Awake() => mainCamera = Camera.main;

        void Update()
        {
            if (!Input.GetMouseButtonDown(1) || controlledAgents.Count == 0)
                return;

            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 1000))
                GiveOrder(hit.point);
        }
        
        void GiveOrder(Vector3 position)
        {
            var doubleRadius = controlledAgents[0].radius * 2;
            var positions = formation.GetPositions(position, controlledAgents.Count, doubleRadius);
           
            for (var i = 0; i < controlledAgents.Count; i++)
                controlledAgents[i].SetDestination(positions[i]);
        }

        public void AddAgent(NavMeshAgent agent) => controlledAgents.Add(agent);
        public void RemoveAgent(NavMeshAgent agent) => controlledAgents.Remove(agent);
    }
}