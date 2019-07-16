using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour {

    public const int NUMBER_OF_TRAINING_SETS = 16;

    public bool IsTraining;
    public bool IsTrainingFinished;

    public int GatheredDataSets;

    private ActivationNetwork _neuralNetwork;
    private ParallelResilientBackpropagationLearning _neuralTeacher;

    //double initialStep = 0.0125;
    double sigmoidAlphaValue = 2.0;

    // The inputs are the distance to the next edge
    // and the taken action (jump in this specific case)
    private double[][] _inputs = new double[][] {
        new double[2]
    };

    // The output is wether to jump or not
    private double[][] _outputs = new double[][] {
        new double[2]
    };

    // temporary value storage
    private List<double[]> _dataSetInputs = new List<double[]>();
    private List<double[]> _dataSetOutputs = new List<double[]>();

    // Use this for initialization
    void Start() {
        _neuralNetwork = new ActivationNetwork(new SigmoidFunction(sigmoidAlphaValue), 2, 2, 1);
        _neuralTeacher = new ParallelResilientBackpropagationLearning(_neuralNetwork);

        // set learning rate and momentum
        //_neuralTeacher.Reset(initialStep);
    }

    // Update is called once per frame
    void Update() {
        // start training of neural network
        if (IsTraining) {
            IsTraining = false;

            // convert temporary inputs/outputs
            _inputs = _dataSetInputs.ToArray();
            _outputs = _dataSetOutputs.ToArray();

            for (int i = 0; i < 500; i++) {
                double error = _neuralTeacher.RunEpoch(_inputs, _outputs);
                if (i % 10 == 0) {
                    Debug.Log("Supervised -> " + i + ", Error = " + error);
                }
            }

            int correct = 0;
            for (int i = 0; i < _inputs.Length; i++) {
                double[] outputValues = _neuralNetwork.Compute(_inputs[i]);
                double outputResult = outputValues.First() >= 0.5 ? 1 : 0;

                Debug.Log("For inputs " + String.Join(", ", _inputs[i].Select(o => o.ToString()).ToList().ToArray()) + " output " + outputResult + " was learned");

                if (outputResult == _outputs[i].First()) {
                    correct++;
                }
            }

            Debug.Log("Correct " + correct + "/" + _inputs.Length + ", " + Mathf.Round(((float)correct / (float)_inputs.Length * 100)) + "%");
            IsTrainingFinished = true;
        }
    }

    public void Train(double jumpSensorDistance, double canJump, double jumped) {
        _dataSetInputs.Add(new double[] { jumpSensorDistance, canJump });
        _dataSetOutputs.Add(new double[] { jumped });

        GatheredDataSets++;
        if (!IsTraining && !IsTrainingFinished && GatheredDataSets == NUMBER_OF_TRAINING_SETS) {
            Debug.Log("Start training of the network...");
            IsTraining = true;
        }
    }

    public double[] Compute(double[] inputs) {
        return _neuralNetwork.Compute(inputs);
    }
}
