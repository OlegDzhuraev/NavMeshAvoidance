using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshAvoidance
{
    public sealed class ExampleSpawner : MonoBehaviour
    {
        [Header("Testing settings")]
        [SerializeField] GameObject agentPrefab;
        [SerializeField, Range(2, 1024)] int count = 16;
        
        [Header("Components links")]
        [SerializeField] Ordering ordering;
        [SerializeField] Avoidance avoidance;

        readonly IFormation spawnFormation = new SquareFormation();
        readonly List<NavMeshAgent> spawnedCollection = new ();

        void Start()
        {
            ordering.Avoidance = avoidance;
            
            var spawnPositions = spawnFormation.GetPositions(Vector3.zero, count, 1);
            
            for (var i = 0; i < count; i++)
                DoSpawn(spawnPositions[i]);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.X) && spawnedCollection.Count > 0)
            {
                avoidance.RemoveAgent(spawnedCollection[0]);
                ordering.RemoveAgent(spawnedCollection[0]);
                Destroy(spawnedCollection[0].gameObject);
                spawnedCollection.RemoveAt(0);
            }
        }

        void DoSpawn(Vector3 position)
        {
            var spawned = Instantiate(agentPrefab, position, Quaternion.identity);
            var agent = spawned.GetComponent<NavMeshAgent>();
                
            ordering.AddAgent(agent);
            avoidance.AddAgent(agent);
            spawnedCollection.Add(agent);
        }
    }
}