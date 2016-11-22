﻿using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NeuralNetwork;

public class ModelController : MonoBehaviour {
	public Collider[] colliders;
	public MoveJoint[] moveJoints; // All moveJoints in the gameObject
    public MoveJoint[] movableParts; // All moveJoints which can be moved by program (so far only Spine and Legs)
    private Vector3 initialPosition;
    private SelfOrganisingNetwork kohonensNetwork;
    private LinearNetwork outputLayer;

    // Returns strength of given array. It is calculated according to the Euclidean norm - 
    // Sqrt(Sum(weight(i)^2)). It can be later expanded to use other norms.
    public double Strength(double[] inputSignals) {
        double strenght = 0.0;
        for (int i = 0; i < inputSignals.Length; i++) {
            strenght += Math.Pow(inputSignals[i], 2);
        }
        strenght = Math.Sqrt(strenght);

        return strenght;
    }

    // Normalizes input signals so that they are <= 1
    public double[] Normalize(double[] inputSignals) {
        double strength = Strength(inputSignals);
        for (int i = 0; i < inputSignals.Length; i++) {
            inputSignals[i] /= strength;
        }
        return inputSignals;
    }

    /// <summary>
    /// Writes the given object instance to a binary file.
    /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
    /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
    /// </summary>
    /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
    /// <param name="filePath">The file path to write the object instance to.</param>
    /// <param name="objectToWrite">The object instance to write to the XML file.</param>
    /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
    public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false) {
        using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create)) {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }
    }

    /// <summary>
    /// Reads an object instance from a binary file.
    /// </summary>
    /// <typeparam name="T">The type of object to read from the XML.</typeparam>
    /// <param name="filePath">The file path to read the object instance from.</param>
    /// <returns>Returns a new instance of the object read from the binary file.</returns>
    public static T ReadFromBinaryFile<T>(string filePath) {
        using (Stream stream = File.Open(filePath, FileMode.Open)) {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
        }
    }

    // Use this for initialization
    void Start () {
        initialPosition = gameObject.transform.GetChild(0).GetChild(0).position;
        //colliders = gameObject.FindObjectsOfType(Collider) as Collider[];
        moveJoints = GetComponentsInChildren<MoveJoint>();

		// Initialization of variables in every moveJoint
		foreach( MoveJoint moveJoint in moveJoints ) {
			moveJoint.Start();
		}

        movableParts = new MoveJoint[7];
        movableParts[0] = moveJoints[0]; // Spine
        movableParts[1] = moveJoints[6]; // UpperLeg.L
        movableParts[2] = moveJoints[7]; // LowerLeg.L
        movableParts[3] = moveJoints[8]; // Foot.L
        movableParts[4] = moveJoints[9]; // UpperLeg.R
        movableParts[5] = moveJoints[10]; // LowerLeg.R
        movableParts[6] = moveJoints[11]; // Foot.R

        System.Random randomGenerator = new System.Random();

        if (File.Exists("C:\\Users\\Bartas\\Documents\\Unity_Projects\\Neural_Network\\Assets\\KohonensNetwork.txt")) {
            kohonensNetwork = ReadFromBinaryFile<SelfOrganisingNetwork>("C:\\Users\\Bartas\\Documents\\Unity_Projects\\Neural_Network\\Assets\\KohonensNetwork.txt");
        } else {
            kohonensNetwork = new SelfOrganisingNetwork(23, 10, 10, randomGenerator); // We set manually size of Kohonen's network as 10x10
            kohonensNetwork.RandomizeWeightsOfAllNeurons(-1.0, 1.0, 0.01);
        }

        if (File.Exists("C:\\Users\\Bartas\\Documents\\Unity_Projects\\Neural_Network\\Assets\\OutputLayer.txt")) {
            outputLayer = ReadFromBinaryFile<LinearNetwork>("C:\\Users\\Bartas\\Documents\\Unity_Projects\\Neural_Network\\Assets\\OutputLayer.txt");
        } else {
            outputLayer = new LinearNetwork(10 * 10, 21, randomGenerator);
            outputLayer.RandomizeWeightsOfAllNeurons(-1.0, 1.0, 0.01);
        }

        //WriteToBinaryFile<SelfOrganisingNetwork>("C:\\Users\\Bartas\\Documents\\Unity_Projects\\Neural_Network\\Assets\\KohonensNetwork.txt", kohonensNetwork);
        //WriteToBinaryFile<LinearNetwork>("C:\\Users\\Bartas\\Documents\\Unity_Projects\\Neural_Network\\Assets\\OutputLayer.txt", outputLayer);

        print(kohonensNetwork.neurons[0, 0].GetWeights()[0] + " / " + outputLayer.neurons[0].GetWeights()[0]);
    }
	
	// Update is called once per frame
	void Update () {
        //Time.timeScale = 0.10F;

        double[] inputSignals = new double[23];
        // First three fields of inputSignals are positions of Spine
        inputSignals[0] = movableParts[0].transform.position.x;
        inputSignals[1] = movableParts[0].transform.position.y;
        inputSignals[2] = movableParts[0].transform.position.z;
        for (int i = 1; i < movableParts.Length; i++) { // In this loop we set difference between movableParts positions and Spine position
            inputSignals[(3*i)] = movableParts[i].transform.position.x - inputSignals[0];
            inputSignals[(3 * i) + 1] = movableParts[i].transform.position.y - inputSignals[1];
            inputSignals[(3 * i) + 2] = movableParts[i].transform.position.z - inputSignals[2];
        }
        // Collisions of right and left foot
        if (movableParts[3].transform.GetComponent<OnCollision>().GetTouchingFloor() == false) inputSignals[21] = -1.0;
        else inputSignals[21] = 1.0;
        double footLTouchingGround = inputSignals[21];
        if (movableParts[6].transform.GetComponent<OnCollision>().GetTouchingFloor() == false) inputSignals[22] = -1.0;
        else inputSignals[22] = 1.0;
        double footRTouchingGround = inputSignals[22];

        inputSignals = Normalize(inputSignals); // We have to normalize all signals, but last two cells of arrays are true/false variables telling, if foots are touching the ground, so we should restore them to values -1 or 1
        inputSignals[21] = footLTouchingGround;
        inputSignals[22] = footRTouchingGround;
        //----------------------FINISHED CREATION OF INPUT SIGNALS FOR KOHONEN'S NETWORK-------------------------

        double[] kohonensNetworkResponse = kohonensNetwork.Response(inputSignals);
        print("Kohonen = " + kohonensNetworkResponse[0] + ", " + kohonensNetworkResponse[13] + ", " + kohonensNetworkResponse[20]);
        //print(kohonensNetworkResponse[0] + ", " + kohonensNetworkResponse[21] + ", " + kohonensNetworkResponse[80]);

        //double[] outputLayerResponse = outputLayer.Response(Normalize(kohonensNetworkResponse));
        double[] outputLayerResponse = outputLayer.Response(kohonensNetworkResponse);
        print("Wyjsciowa = " + outputLayerResponse[0] + ", " + outputLayerResponse[13] + ", " + outputLayerResponse[20]);
        //outputLayerResponse = Normalize(outputLayerResponse);

        for (int i = 0; i < movableParts.Length; i++) {
            movableParts[i].Move((float)outputLayerResponse[(3 * i)], (float)outputLayerResponse[(3 * i) + 1], (float)outputLayerResponse[(3 * i) + 2]);
        }

        //print(movableParts[0].transform.rotation.eulerAngles.x); // Rotation of Spine
        //print(MeasureDistance());
    }

    double MeasureDistance() {
        return Math.Sqrt(Math.Pow(gameObject.transform.GetChild(0).GetChild(0).position.x - initialPosition.x, 2) + Math.Pow(gameObject.transform.GetChild(0).GetChild(0).position.y - initialPosition.y, 2));
    }
}
