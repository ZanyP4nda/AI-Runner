using System;
using System.Collections.Generic;
using UnityEngine;
using ZestyP4nda.Core;

public class NN
{
    private float[] _inputs; // Inputs 
    private float[,] hLW; // Hidden layer weights
    public float[,] HLW { get{return hLW;} }
    private float[] oW; // Output layer weights (only 1 node)
    public float[] OW { get{return oW;} }

    private int _numInputs; // No. of inputs
    public int NumInputs {get{return _numInputs;}}
    private int _numHidden; // No. of hidden nodes
    public int NumHidden {get{return _numHidden;}}

    private string _name;
    public string Name {get{return _name;}}
    private bool _isInitialiseNN;

    // Activation functions
    private float ReLU(float x)
    {
        return Math.Max(0, x);
    }

    private float Sigmoid(float x)
    {
        return (float)(1 / (1 + Math.Pow(Math.E, -x)));
    }

    // Constructor
    public NN(int numInputs, int numHidden, string name = null, bool isInitialiseNN = true)
    {
        // Set private variables to contructor parameters
        _numInputs = numInputs;
        _numHidden = numHidden;
        _name = name;
        _isInitialiseNN = isInitialiseNN;

        // Initialisation
        InitialiseHiddenLayer();
        InitialiseOutputLayer();
    }

    private void InitialiseHiddenLayer()
    {
        if(_isInitialiseNN)
        {
            // Fill the hidden layer weights matrix with random values between -1 and 1
            hLW = DataHelper.Get2DArrayFuncValue(_numHidden, _numInputs, x => UnityEngine.Random.Range(-1f, 1f));
        }
        else
        {
            // If don't want to initialise NN, fill with zeros
            hLW = DataHelper.Get2DArray(_numHidden, _numInputs, 0f);
        }
    }

    private void InitialiseOutputLayer()
    {
        // Initialise the output weight array
        oW = new float[_numHidden];
        // Fill the output weight array with random values between -1 and 1
        for (int i = 0; i < _numHidden; i++)
        {
            // Set this weight to a random value between -1 and 1 OR 0 if don't want to initialise NN
            oW[i] = (_isInitialiseNN) ? UnityEngine.Random.Range(-1f, 1f) : 0f;
        }
    }

    // Feed forward algorithm
    public float FeedForward(float[] inputs)
    {
        // Set local '_inputs' variable to 'inputs' parameter
        _inputs = inputs;
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
                _hiddenOuti += hLW[i, j] * _inputs[j];
            }
            // Assign the hidden activation value to the activatin of this node
            hiddenActivation[i] = ReLU(_hiddenOuti);
        }

        // Assign a value for the output value of this output node (before activation)
        float _outputOut = 0f;
        // Loop through each weight and multiply by corresponding hidden node activation
        for (int i = 0; i < _numHidden; i++)
        {
            _outputOut += oW[i] * hiddenActivation[i];
        }
        // Return the activation of the output node
        return Sigmoid(_outputOut);
    }

    // Get flattened
    public float[] GetFlattennedDNA()
    {
        List<float> flattened = new List<float>();
        // Flatten hidden layer weights and add to array
        for (int i = 0; i < hLW.GetLength(0); i++)
        {
            for (int j = 0; j < hLW.GetLength(1); j++)
            {
                flattened.Add(hLW[i, j]);
            }
        } 
        // Add all output layer weights to the array
        flattened.AddRange(oW);

        return flattened.ToArray();
    }

    // Takes a 1D array as input, sets the NN values
    public void SetBrain(float[] newBrain)
    {
        Debug.Log($"{_name}: SETTING BRAIN AS: {DataHelper.GetArrayToString(newBrain)}");
        // Set hidden layer weights
        for (int i = 0; i < _numHidden; i++)
        {
            for (int j = 0; j < hLW.GetLength(1); j++)
            {
                hLW[i, j] = newBrain[i * hLW.GetLength(1) + j];
            }
        }

        // Set output layer weights
        int index = 0;
        for (int i = _numHidden * _numInputs; i < newBrain.Length; i++)
        {
            oW[index] = newBrain[i];
            index++;
        }
    }
}