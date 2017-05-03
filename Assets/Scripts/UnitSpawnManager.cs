using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class Unit
{
    GameObject unit;
    float spawnChance;
    int spawnCount;
}

/// <summary>
/// Spawns units randomly over time based on their chance (spawnWeight) and how many spawn at a time (spawnCont)
/// </summary> 
public class UnitSpawnManager : NetworkBehaviour
{
    //| Spawn Management Data
    public bool spawningEnabled = true; 
    public Transform[] SpawnPoints = null;
    public GameObject[] SpawnedUnits = null;
    public float spawnDelay = 0.95f;

    public int BasicShipWeight = 30,
        DreadnoughtWeight = 20,
        FrigateWeight = 20,
        StingerWeight = 20,
        SwarmWeight = 10;

    private int[] spawnCount;
    private int[] spawnLowerChance;
    private int[] spawnUpperChance;    
    private int TotalSpawnWeight = 100;
    private float lastUnitSpawnTime;




    //| Spawn Unit     Chance   count    
    //|------------------------------
    //| 1. BasicShip   - 30%    1
    //| 2. Dreadnought - 20%    1
    //| 3. Frigate     - 20%    1
    //| 4. Stinger     - 20%    1
    //| 5. Swarm       - 10%    5
    //|-------------------

    void Start()
    {
        spawnCount = new int[SpawnedUnits.Length];
        spawnLowerChance = new int[SpawnedUnits.Length];
        spawnUpperChance = new int[SpawnedUnits.Length];


        //| Assign Spawn weights to defined units
        //|-------------------------------------------
        //| Basic Ship
        spawnCount[0] = 1;
        spawnLowerChance[0] = 0;
        spawnUpperChance[0] = BasicShipWeight;

        //| Dreadnought
        spawnCount[1] = 1;
        spawnLowerChance[1] = spawnUpperChance[0] + 1;
        spawnUpperChance[1] = spawnLowerChance[1] + DreadnoughtWeight;

        //| Frigate
        spawnCount[2] = 2;
        spawnLowerChance[2] = spawnUpperChance[1] + 1;
        spawnUpperChance[2] = spawnLowerChance[2] + FrigateWeight;

        //| Stinger
        spawnCount[3] = 3;
        spawnLowerChance[3] = spawnUpperChance[2] + 1;
        spawnUpperChance[3] = spawnLowerChance[3] + StingerWeight;

        //| Swarm
        spawnCount[4] = 5;
        spawnLowerChance[4] = spawnUpperChance[0] + 1;
        spawnUpperChance[1] = spawnLowerChance[1] + SwarmWeight;
        
        
        //| Assign Spawn Points to array
        //|------------------------------
        if (isServer) {
            for (int i = 0; i < transform.childCount; i++) {
                SpawnPoints[i] = transform.GetChild(i).gameObject.transform;
            }
            lastUnitSpawnTime = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && spawningEnabled && (Time.time - lastUnitSpawnTime >= spawnDelay) )
        {
                lastUnitSpawnTime = Time.time;
                SpawnUnit();
        }
    }

    public void decreaseSpawnDelay(float decrement)
    {
        spawnDelay = (spawnDelay - decrement) > 0.1f ? spawnDelay - decrement : 0.1f;
        Debug.Log("Spawn delay changed to: " + spawnDelay);
    }


    private void SpawnUnit()
    {
        //| Grab a random number to determine who spawns
        System.Random random = new System.Random();
        int randomNum = random.Next(0, 101);
        int unitIndex = 0;
        int spawnAmount = 0;

        for( int i = 0; i < SpawnedUnits.Length; i++)
        {
            if(randomNum >= spawnLowerChance[i] && randomNum <= spawnUpperChance[i])
            {
                unitIndex = i;
                spawnAmount = spawnCount[i];
            }
        }
         

        //| Unit Spawn Loop: per unit count
        for (int i = 0; i <= spawnCount[unitIndex]; i++)
        {
            int spawnIndex = UnityEngine.Random.Range(0, SpawnPoints.Length);
            Vector3 spawnPosition = SpawnPoints[spawnIndex].transform.position;
            Quaternion spawnRotation = SpawnPoints[spawnIndex].transform.rotation;

            GameNetworkManager.singleton.SpawnNetworkAwareObject(SpawnedUnits[unitIndex], spawnPosition, spawnRotation);
        }

    }

}
