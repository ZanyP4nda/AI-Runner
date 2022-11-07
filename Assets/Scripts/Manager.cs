using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    // 'Singleton' reference
    public static Manager manager;

    [Header("Checkpoints")]
    [SerializeField]
    private GameObject checkpointParent;
    public Transform[] checkpoints;

    [Header("Runners")]
    [SerializeField]
    private GameObject runnerPrefab;
    [SerializeField]
    private Transform runnerSpawn;
    [SerializeField]
    private int numRunners;
    private int numRunnersAlive; // Counter to track no. of runners alive
    private List<Runner> runners;

    private void Awake()
    {
        manager = this;
    }

    private void Start()
    {
        InitialiseCheckpoints();
        SpawnRunners();
    }

    private void InitialiseCheckpoints()
    {
        if(!checkpointParent)
            checkpointParent = GameObject.FindGameObjectWithTag("checkpointParent");

        checkpoints = checkpointParent.GetComponentsInChildren<Transform>();
    }

    private void SpawnRunners()
    {
        // Initialise a new list of runners
        runners = new List<Runner>();
        // Set runner counter to number of runners
        numRunnersAlive = numRunners;
        // Spawn 'numRunners' no. of runners at 'runnerSpawn'
        for (int i = 0; i < numRunners; i++)
        {
            // Instantiate a runner and add its Runner component to the list of runners
            runners.Add(Instantiate(runnerPrefab, runnerSpawn.position, Quaternion.identity).GetComponent<Runner>());
        }
    }

    // To be called by runners to track number of alive runners
    public void RunnerDie()
    {
        numRunnersAlive--;
        if(numRunnersAlive <= 0)
            GenEnd();            
    }

    private void GenEnd()
    {
        // Get top 10% of runners by fitness
        // Assign crossing partners
        // Cross DNA
        // Mutate
    }
}