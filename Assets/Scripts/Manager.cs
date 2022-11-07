using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ZestyP4nda.Core;

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

    private float[] crossProbabilities;

    private void Awake()
    {
        manager = this;
    }

    private void Start()
    {
//        InitialiseCheckpoints();
//        SpawnRunners();

        float[] parent1Brain = new float[24];
        for (int i = 0; i < parent1Brain.Length; i++)
        {
            parent1Brain[i] = 1f;
        }

        float[] parent2Brain = new float[24];
        for (int i = 0; i < parent2Brain.Length; i++)
        {
            parent2Brain[i] = 2f;
        }

        NN parent1 = new NN(5, 4, "parent1");
        parent1.SetBrain(parent1Brain);
        Debug.Log("===== Parent 1 =====");
        Debug.Log($"hLW: {DataHelper.Get2DArrayToString(parent1.HLW)}");
        Debug.Log($"oW: {DataHelper.GetArrayToString(parent1.OW)}");

        NN parent2 = new NN(5, 4, "parent2");
        parent2.SetBrain(parent2Brain);
        Debug.Log("===== Parent 2 =====");
        Debug.Log($"hLW: {DataHelper.Get2DArrayToString(parent2.HLW)}");
        Debug.Log($"oW: {DataHelper.GetArrayToString(parent2.OW)}");

        Debug.Log("===== CROSSING OVER =====");
        CrossOver(parent1, parent2);

        Debug.Log("===== Parent 1 =====");
        Debug.Log($"hLW: {DataHelper.Get2DArrayToString(parent1.HLW)}");
        Debug.Log($"oW: {DataHelper.GetArrayToString(parent1.OW)}");

        Debug.Log("===== Parent 2 =====");
        Debug.Log($"hLW: {DataHelper.Get2DArrayToString(parent2.HLW)}");
        Debug.Log($"oW: {DataHelper.GetArrayToString(parent2.OW)}");
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

    private int GetCrossIndex()
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

    // Get 2 parents
    private Tuple<NN, NN> GetCrossParents()
    {
        // Get 1 index
        int parent1Index = GetCrossIndex();
        // Get another index
        int parent2Index = GetCrossIndex();
        // Make sure the 2nd index is not the same as the first
        while(parent2Index == parent1Index)
            parent2Index = GetCrossIndex();

        return Tuple.Create(runners[parent1Index].nn, runners[parent2Index].nn);
    }

    private void CrossOver(NN parent1, NN parent2)
    {
        // Get the flattened DNA of both parents
        float[] parent1Flattened = parent1.GetFlattennedDNA();
        float[] parent2Flattened = parent2.GetFlattennedDNA();

        // Get a random splitting point
        int randomSplitIndex = UnityEngine.Random.Range(0, parent1Flattened.Length);

        // Cross parent 1 and parent 2
        parent1.SetBrain(parent1Flattened.Take(randomSplitIndex).Concat(parent2Flattened.Skip(randomSplitIndex)).ToArray());
        parent2.SetBrain(parent2Flattened.Take(randomSplitIndex).Concat(parent1Flattened.Skip(randomSplitIndex)).ToArray());
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