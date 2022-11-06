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
}