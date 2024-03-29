using System;
using System.IO;
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
    [SerializeField]
    private int numRunnersAlive; // Counter to track no. of runners alive
    private List<Runner> runners;

    [Header("Genetic Algorithm")]
    [SerializeField]
    private float mutateChance = 0.1f;
    [SerializeField]
    private int generationNum = 0;
    [SerializeField]
    private int maxGenNum = 30;
    [SerializeField]
    private float maxFitness = 0f;
    [SerializeField]
    private int maxFitnessGen = 0;
    private List<NN> childrenNN;
    private float[] crossProbabilities;

    [Header("Logging")]
    private string logPath;
    private StreamWriter writer;

    private void Awake()
    {
        manager = this;
        
        // Logging setup
        logPath = Application.dataPath + $"/Logs/Logs/GA_Log_{DateTime.Now.ToString("yyyyMMdd_HH-mm-ss")}.txt";
        Debug.Log($"Log path: {logPath}");
        writer = new StreamWriter(File.Create(logPath));
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
            Runner _runner = Instantiate(runnerPrefab).GetComponent<Runner>();
            _runner.Init(runnerSpawn, null, $"{generationNum}x{i}");
            // Add to list
            runners.Add(_runner);
        }
    }

    private void ResetRunners()
    {
        // Set runner counter to number of runners
        numRunnersAlive = numRunners;
        for (int i = 0; i < numRunners; i++)
        {
            runners[i].Init(runnerSpawn, childrenNN[i], $"{generationNum}x{i}");
        }
    }

    // To be called by runners to track number of alive runners
    public void RunnerDie()
    {
        numRunnersAlive--;
        if(numRunnersAlive <= 0)
            GenEnd();            
    }

    // Get the probability of runner selection based on fitness
    private float[] GetNormalisedRunnerFitness()
    {
        // Create an array of all runners' fitness
        float[] fitness = runners.Select(x => x.fitness).ToArray();
        
        // Get the total fitness
        float sumFitness = fitness.Sum();

        for (int i = 0; i < fitness.Length; i++)
        {
            CheckMaxFitness(fitness[i]);
            fitness[i] = fitness[i] / sumFitness;
        }

        return fitness;
    }

    private void CheckMaxFitness(float fitness)
    {
        if(fitness > maxFitness)
        {
            maxFitness = fitness;
            maxFitnessGen = generationNum;
        }
    }

    private int GetCrossIndex()
    {
        int output = 0;
        // Get a random float between 0 and 1
        float r = UnityEngine.Random.Range(0, 1f);
        // Probability algorithm
        for(int i = 0; i < crossProbabilities.Length; i++)
        {
            r -= crossProbabilities[i];
            if(r < 0)
            {
                output = i;
                break;
            }
        }
        return output;
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
        {
            parent2Index = GetCrossIndex();
        }

        return Tuple.Create(runners[parent1Index].nn, runners[parent2Index].nn);
    }

    private Tuple<NN, NN> CrossOver(NN parent1, NN parent2)
    {
        // Get the flattened DNA of both parents
        float[] parent1Flattened = parent1.GetFlattennedDNA();
        float[] parent2Flattened = parent2.GetFlattennedDNA();

        // Get a random splitting point
        int randomSplitIndex = UnityEngine.Random.Range(0, parent1Flattened.Length);

        // Cross over parents and apply mutation
        float[] child1DNA = Mutate(parent1Flattened.Take(randomSplitIndex).Concat(parent2Flattened.Skip(randomSplitIndex)).ToArray());
        float[] child2DNA = Mutate(parent2Flattened.Take(randomSplitIndex).Concat(parent1Flattened.Skip(randomSplitIndex)).ToArray());

        // Instantiate children NN
        NN child1NN = new NN(parent1.NumInputs, parent1.NumHidden, $"Runner{generationNum}x{childrenNN.Count}", false);
        child1NN.SetBrain(child1DNA);
        NN child2NN = new NN(parent1.NumInputs, parent1.NumHidden, $"Runner{generationNum}x{childrenNN.Count+1}", false);
        child2NN.SetBrain(child2DNA);

        return Tuple.Create(child1NN, child2NN);
    }

    private float[] Mutate(float[] flatChildDNA)
    {
        float[] _flatChildDNA = flatChildDNA;
        // If random mutation chance is met
        if(UnityEngine.Random.Range(0f, 1f) <= mutateChance)
        {
            // Assign a random element in the child DNA to a random value betweeen -1 and 1
            _flatChildDNA[UnityEngine.Random.Range(0, _flatChildDNA.Length)] = UnityEngine.Random.Range(-1f, 1f);
        }

        return  _flatChildDNA;
    }

    private void GenEnd()
    {
        Debug.Log(numRunnersAlive);
        LogGen();
        if(generationNum + 1 == maxGenNum)
        {
            foreach(Runner _runner in runners)
                _runner.Freeze();
            Debug.LogError("Max generation met");
            return;
        }

        // Create an array of normalised fitness scores
        crossProbabilities = GetNormalisedRunnerFitness();

        // Instantiate childrenNN list
        childrenNN = new List<NN>();

        // Use the probability-based system to get runners to cross
        for (int i = 0; i < numRunners / 2; i++)
        {
            // Get the 2 parents to cross
            var (parent1, parent2) = GetCrossParents();
            // Cross over
            var (child1, child2) = CrossOver(parent1, parent2);
            // Add children to list
            childrenNN.Add(child1);
            childrenNN.Add(child2);
        }

        generationNum++;

        // Reset runners
        ResetRunners();
    }

    private void LogGen()
    {
        writer.WriteLine($"===== Gen {generationNum} =====");
        foreach(Runner _runner in runners)
        {
            writer.WriteLine(_runner.GetLoggingInfo());
        }
    }

    public void EndConditionMet(Runner eliteRunner)
    {
        Debug.Log($"===== {eliteRunner.nn.Name} has met the end condition! =====");
        // Freeze all runners
        foreach(Runner runner in runners)
            runner.Freeze();

        // Write eliteRunner DNA to file
        string save = $"{eliteRunner.nn.Name}:\n{DataHelper.GetArrayToString(eliteRunner.nn.GetFlattennedDNA(), false)}";
        string destinationPath = Application.dataPath + $"/Logs/eliteRunnerDNA_{DateTime.Now.ToString("yyyyMMdd_HH-mm-ss")}.txt";
        File.WriteAllText(destinationPath, save);
    }

//    private float[] GetEliteRunnerDNA()
//    {
//        string savePath = Application.dataPath + "/Logs/eliteRunnerDNA.txt";
//        // Get DNA from file and convert it to float[] array
//        return Array.ConvertAll(File.ReadAllLines(savePath)[1].Split(','), float.Parse);
//    }

    private void OnApplicationQuit()
    {
        writer.Close();
    }
}