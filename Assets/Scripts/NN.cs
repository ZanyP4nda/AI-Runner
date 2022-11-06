using System;
using UnityEngine;
using ZestyP4nda.Core;

public class NN
{
    private float[] inputs; // Inputs 
    private float[,] hLW; // Hidden layer weights
    private float[] hLB; // Hidden layer biases
    private float[] oW; // Output layer weights (only 1 node)
    private float oB; // Output layer bias (only 1 node)

    private int _numInputs; // No. of inputs
    private int _numHidden; // No. of hidden nodes

    // Activation functions
    private float ReLU(float x)
    {
        return Math.Max(0, x);
    }

    private double Sigmoid(float x)
    {
        return 1 / (1 + Math.Pow(Math.E, -x));
    }

    // Constructor
    public NN(int numInputs, int numHidden)
    {
        // Set private variables to contructor parameters
        _numInputs = numInputs;
        _numHidden = numHidden;

        // Initialisation
        InitialiseHiddenLayer();
        InitialiseOutputLayer();
    }

    private void InitialiseHiddenLayer()
    {
        // Fill the hidden layer weights matrix with random values between 0 and 1
        hLW = DataHelper.Get2DArrayFuncValue(_numHidden, _numInputs, x => UnityEngine.Random.Range(0f, 1f));
        
        // Fill the hidden layer bias array with 0
        hLB = new float[_numHidden];
        Array.Clear(hLB, 0, hLB.Length-1);
    }

    private void InitialiseOutputLayer()
    {
        // Initialise the output weight array
        oW = new float[_numHidden];
        // Fill the output weight array with random values between 0 and 1
        for (int i = 0; i < _numHidden; i++)
        {
            // Set this weight to a random value between 0 and 1
            oW[i] = UnityEngine.Random.Range(0f, 1f);
        }

        // Set the output bias to 0
        oB = 0;
    }

    // Feed forward algorithm
    private void FeedForward()
    {
        // Initialise an array of activation of each hidden node
        float[] hiddenActivation = new float[_numHidden];
        Array.Clear(hiddenActivation, 0, _numHidden - 1);
        // Loop through each hidden node
        for (int i = 0; i < _numHidden; i++)
        {
            // Assign a value for the output value of this hidden node (before activation)
            float _hiddenOuti = 0f;
            // Loop through each weight and multiply by corresponding input value
            for (int j = 0; j < _numInputs; j++)
            {
                _hiddenOuti += hLW[i, j] * inputs[j];
            }
            // Assign the hidden activation value to the activatin of this node
            hiddenActivation[i] = ReLU(_hiddenOuti + hLB[i]);
        }

        // Initialise a double for the activation of the output node
        double outputActivation = 0f;
        // Assign a value for the output value of this output node (before activation)
        float _outputOut = 0f;
        // Loop through each weight and multiply by corresponding hidden node activation
        for (int i = 0; i < _numHidden; i++)
        {
            _outputOut += oW[i] * hiddenActivation[i];
        }
        // Get activation value of the output node
        outputActivation = Sigmoid(_outputOut + oB);
    }
}