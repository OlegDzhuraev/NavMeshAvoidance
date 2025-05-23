# Nav Mesh Avoidance
Custom Nav Mesh Avoidance to replace default one. 

![Nav Mesh Avoidance](https://media3.giphy.com/media/cA3Xxd4CnB9TBoIMtG/giphy.gif)

This is a simple avoidance implementation, which allows to prevent situation with NavMesh agents moving too close to each other - they trying to move using optimal path, but it just looks weird. 

You can combine this Avoidance with default one (agents will never move through each other) or disable Unity's avoidance (avoidance will look smoother, but agents will be able to move through each other).

If `Use Spatial Hash` parameter enabled, algorithm works fine with up to `~1024` agents. 

If not, algorithm works fine only for small amount of agents, like `10-100`. 


## How to install
You can install plugin via Unity Package Manager as git package from github repository:

```
https://github.com/OlegDzhuraev/NavMeshAvoidance.git
```

You also can download it directly from github and place into Assets folder.

## How to use
First of all, add **Avoidance** component to any GameObject on scene(once). Next, when you spawn any agent, you need to add it to the **Avoidance** class like this:
```cs
using NavMeshAvoidance;
using UnityEngine;
using UnityEngine.AI;

public class AvoidingAgent : MonoBehaviour
{
  [SerializeField] Avoidance avoidance;

  void Start()
  {
    var agent = GetComponent<NavMeshAgent>();

    avoidance.AddAgent(agent);
  }
}
```

And now this agent will avoid the other added agents.

You also can disable default Unity avoidance. In this case agents will sometimes move through each other, but in priority for them will be to avoid others. Idea of disabling default avoidance is better navigation without "friction", which can be noticed when using default avoidance and 2 or more agents try to move through other one.

When **destroying** any agent, dont forget to **remove it from avoidance** too:

```cs
Avoidance.RemoveAgent(agent);
```

## Ordering and Formation
Basic classes added as controls example for agents groups. You can ignore these scripts if you have your own group controls.

## Example
Check **SampleScene** to see **Avoidance** work example.

## Additional info
If you need a more accurate, comprehensive and advanced solution, I recommend you to study the **RVO2 algorithm** - https://github.com/snape/RVO2-CS.
