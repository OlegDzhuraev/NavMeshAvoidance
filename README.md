# Nav Mesh Avoidance
Custom Nav Mesh Avoidance to replace default one. 

This is simple avoidance implementation to prevent situation when nav mesh agents moving too close to each other. This algorithm is best suitable for average crowds of 10-100 agents. 
Not tested with bigger numbers.

## How to use
First of all, add **Avoidance** component to any GameObject (once). Next, when you spawn any agent, you need to add it to the Avoidance class like this:
```cs
using NavMeshAvoidance;
using UnityEngine;
using UnityEngine.AI;

public Avoidance Avoidance;

void Start()
{
  var agent = GetComponent<NavMeshAgent>();
  
  Avoidance.AddAgent(agent);
}
```

And now it will work with avoidance of others agents. You also can disable default avoidance, sometimes agents will move through each other, but in priority for them will be to avoid others.

When destroying any agent, dont forget to remove it from avoidance too:

```cs
Avoidance.RemoveAgent(agent);
```

## Ordering and Formation
Basic classes added as controls example for agents groups. You can ignore these scripts if you have your own group controls.

## Example
Check SampleScene to see **Avoidance** work example.

## Donate
You can support development using cryptocurrency :)

**Zilliqa $ZIL (ZIL):** zil1p6j0js2d5dat36n9zpp4g9xj776vghukkaa32l

**Stellar Lumens (XLM):** GBY4Q6AKWLQIOT37D3X643LLEVZA7WRWBUU4RCVEG6PN57QCIWT7LRIX
