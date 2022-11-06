using UnityEngine;
using ZestyP4nda.Core;

public class NN : MonoBehaviour
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
        _numInputs = numInputs;
        _numHidden = numHidden;

        // Initialise hLW to a matrix with _numHidden nodes and _numInputs weights 
        hLW = new float[_numHidden, _numInputs];
        // Initialise oW to an array with _numHidden weights
        oW = new float[_numHidden];
    }

    private void Start()
    {
        InitialiseHiddenWeights();
        InitialiseOutputWeights();
    }

    private void InitialiseHiddenWeights()
    {
        // Fill the hidden layer weights matrix with random values between 0 and 1
        hLW = DataHelper.Get2DArrayFuncValue(_numHidden, _numInputs, x => Random.Range(0f, 1f));
    }

    private void InitialiseOutputWeights()
    {
        for (int i = 0; i < _numHidden; i++)
        {
            // Set this weight to a random value between 0 and 1
            oW[i] = Random.Range(0f, 1f);
        }
    }
}