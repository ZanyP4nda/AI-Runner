using System;
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

    private float[] GetNormalisedRunnerFitness()
    {
        // Create an array of all runners' fitness
        float[] fitness = runners.Select(x => x.fitness).ToArray();

        // Normalise fitness scores
        float minFitness = fitness.Min();
        float maxFitness = fitness.Max();
        for (int i = 0; i < fitness.Length; i++)
        {
            fitness[i] = (fitness[i] - minFitness) / (maxFitness - minFitness);
        }

        return fitness;
    }

    private int GetCrossIndex(float[] crossProbabilities)
    {
        // Get a random float between 0 and 1
        float r = UnityEngine.Random.Range(0, 1f);
        // Probability algorithm
        int index = 0;
        while(r > 0)
        {
            r -= crossProbabilities[index];
            index++;
        }
        return (index - 1);
    }

    private void CrossOver(float[] crossProbabilities)
    {
        // Get 1 index
        int parent1Index = GetCrossIndex(crossProbabilities);
        // Get another index
        int parent2Index = GetCrossIndex(crossProbabilities);
        // Make sure the 2nd index is not the same as the first
        while(parent2Index == parent1Index)
            parent2Index = GetCrossIndex(crossProbabilities);

        NN parent1 = runners[parent1Index].nn;
        NN parent2 = runners[parent2Index].nn;
    }

    private void GenEnd()
    {
        // Create an array of normalised fitness scores
        float[] normalisedFitness = GetNormalisedRunnerFitness();
        // Use the probability-based system to get runners to cross
        // Assign crossing partners
        // Cross DNA
        // Mutate
    }
}