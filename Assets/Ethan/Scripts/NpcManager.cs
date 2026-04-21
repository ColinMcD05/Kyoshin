using UnityEngine;

public class NpcManager : MonoBehaviour
{
    public GameObject[] npcPrefabs; // array of npc prefabs
    public Transform[] targets; // array of targets for npc to move to
    public Transform[] spawnPoints; // array of spawn points for npc to spawn at
    public int maxNpcs = 10; // max number of npcs to spawn

    public Transform currentTarget; // current target for npc to move to
    public int npcCount = 0; // number of npcs spawned
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        // Spawn max number of NPCs
        for (int i = 0; i < maxNpcs; i++)
        {
            SpawnNpc();
            npcCount++;
        }

    }

    void SpawnNpc()
    {
        if (npcPrefabs == null || npcPrefabs.Length == 0) // if the npc prefabs is not set or is empty return
        {
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0) // if the spawn points is not set or is empty return
        {
            return;
        }

        int randomPrefabIndex = UnityEngine.Random.Range(0, npcPrefabs.Length); // get random prefab index  
        int randomSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length); // get random spawn index

        GameObject chosenPrefab = npcPrefabs[randomPrefabIndex]; // get chosen prefab
        Transform chosenSpawnPoint = spawnPoints[randomSpawnIndex]; // get chosen spawn point

        GameObject npc = Instantiate(chosenPrefab, chosenSpawnPoint.position, chosenSpawnPoint.rotation); // spawn npc

        if (targets != null && targets.Length > 0) // if the targets is not set or is empty return
        {
            int randomTargetIndex = UnityEngine.Random.Range(0, targets.Length); // get random target index 
            Transform chosenTarget = targets[randomTargetIndex]; // get chosen target

            NpcMovement npcMovement = npc.GetComponent<NpcMovement>(); // get npc movement component
            // Target cannot be the same as the spawn point
            if (chosenTarget == chosenSpawnPoint && targets.Length > 1) // if chosen target equals same index as chosenSpawnPoint and targets length is greater then 1
            {
                int fallbackIndex = (randomTargetIndex + 1) % targets.Length; // use fallbeck index which is randomTargetIndex + 1 and mod targets length so get the remainder of this equation
                chosenTarget = targets[fallbackIndex]; // get chosen target from fallback index
            }
            if (npcMovement != null)
            {
                npcMovement.target = chosenTarget; // set npc movement target to chosen target
                npcMovement.npcManager = this; // set npc movement npc manager to this
            }
        }
    }
}
