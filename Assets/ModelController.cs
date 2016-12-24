using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NeuralNetwork;
using System.Diagnostics;



public class ModelController : MonoBehaviour {
    public Collider[] colliders;
    public MoveJoint[] moveJoints; // All moveJoints in the gameObject
    public MoveJoint[] movableParts; // All moveJoints which can be moved by program (so far only Spine and Legs)
    private Vector3 initialPosition;
    private SelfOrganisingNetwork kohonensNetwork;
    private LinearNetwork outputLayer;
    private double previousSpineInclination = 0.0;
    System.Random randomGenerator;
    private double previousDistance;
    private double[] previousMoves;

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

    public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false) {
        using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create)) {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }
    }

    public static T ReadFromBinaryFile<T>(string filePath) {
        using (Stream stream = File.Open(filePath, FileMode.Open)) {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
        }
    }

    private void run_cmd(string cmd, string args) {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = cmd;//cmd is full path to python.exe
        start.Arguments = args;//args is path to .py file and any cmd line args
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        using (Process process = Process.Start(start)) {
            using (StreamReader reader = process.StandardOutput) {
                string result = reader.ReadToEnd();
                Console.Write(result);
            }
        }
    }

    public static bool IsFileReady(String sFilename) {
        // If the file can be opened for exclusive access it means that the file
        // is no longer locked by another process.
        try {
            using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None)) {
                if (inputStream.Length > 0) {
                    return true;
                } else {
                    return false;
                }
            }
        } catch (Exception) {
            return false;
        }
    }

    public double MeasureDistance() {
        //return Math.Sqrt(Math.Pow(gameObject.transform.GetChild(0).GetChild(0).position.x - initialPosition.x, 2) + Math.Pow(gameObject.transform.GetChild(0).GetChild(0).position.y - initialPosition.y, 2));
        return gameObject.transform.GetChild(0).GetChild(0).position.x - initialPosition.x;
    }

    public double SpineForward() {
        if (previousMoves[0] > -0.9) {
            previousMoves[0] = previousMoves[0] - 0.1;
        }
        return previousMoves[0];
    }

    public double SpineBackward() {
        if (previousMoves[0] < 0.9) {
            previousMoves[0] = previousMoves[0] + 0.1;
        }
        return previousMoves[0];
    }

    public double UpperLegLForward() {
        if (previousMoves[1] > -0.9) {
            previousMoves[1] = previousMoves[1] - 0.1;
        }
        return previousMoves[1];
    }

    public double UpperLegLBackward() {
        if (previousMoves[1] < 0.9) {
            previousMoves[1] = previousMoves[1] + 0.1;
        }
        return previousMoves[1];
    }

    public double LowerLegLForward() {
        if (previousMoves[2] < 0.9) {
            previousMoves[2] = previousMoves[2] + 0.1;
        }
        return previousMoves[2];
    }

    public double LowerLegLBackward() {
        if (previousMoves[2] > -0.9) {
            previousMoves[2] = previousMoves[2] - 0.1;
        }
        return previousMoves[2];
    }

    public double UpperLegRForward() {
        if (previousMoves[3] > -0.9) {
            previousMoves[3] = previousMoves[3] - 0.1;
        }
        return previousMoves[3];
    }

    public double UpperLegRBackward() {
        if (previousMoves[3] < 0.9) {
            previousMoves[3] = previousMoves[3] + 0.1;
        }
        return previousMoves[3];
    }

    public double LowerLegRForward() {
        if (previousMoves[4] < 0.9) {
            previousMoves[4] = previousMoves[4] + 0.1;
        }
        return previousMoves[4];
    }

    public double LowerLegRBackward() {
        if (previousMoves[4] > -0.9) {
            previousMoves[4] = previousMoves[4] - 0.1;
        }
        return previousMoves[4];
    }

    // Use this for initialization
    void Start() {
        initialPosition = gameObject.transform.GetChild(0).GetChild(0).position;
        moveJoints = GetComponentsInChildren<MoveJoint>();

        // Initialization of variables in every moveJoint
        foreach (MoveJoint moveJoint in moveJoints) {
            moveJoint.Start();
        }

        movableParts = new MoveJoint[5];
        movableParts[0] = moveJoints[0]; // Spine
        movableParts[1] = moveJoints[1]; // UpperLeg.L
        movableParts[2] = moveJoints[2]; // LowerLeg.L
        movableParts[3] = moveJoints[3]; // UpperLeg.R
        movableParts[4] = moveJoints[4]; // LowerLeg.R

        // Set each cell in previousMoves table to 0 (beginning state of each MovablePart)
        previousMoves = new double[5];
        for (int i = 0; i < 5; i++) {
            previousMoves[i] = 0.0;
        }

        previousSpineInclination = movableParts[0].transform.rotation.eulerAngles.x; // Save inclination of spine
        previousDistance = MeasureDistance(); // Save distance (probably equal to 0)
        randomGenerator = new System.Random(); // Create new Random Generator
    }

    // Update is called once per frame
    void Update() {
        //Time.timeScale = 0.40F;
        //print(gameObject.transform.GetChild(0).rotation.eulerAngles.x);
        double[] inputSignals = new double[11];

        // Version with transform.eulerAngles.x/y/z
        // First three fields of inputSignals are positions of Spine
        double spinePositionX = movableParts[0].transform.position.x;
        double spinePositionY = movableParts[0].transform.position.y;
        inputSignals[0] = movableParts[0].transform.position.y; // Height of the spine above the ground
        inputSignals[1] = movableParts[0].transform.rotation.eulerAngles.x; // Rotation of the spine in relation to y axis

        for (int i = 1; i < movableParts.Length; i++) { // In this loop we set difference between movableParts positions and Spine position
            inputSignals[(2 * i)] = movableParts[i].transform.position.x - spinePositionX;
            inputSignals[(2 * i) + 1] = movableParts[i].transform.position.y - spinePositionY;
        }

        inputSignals[10] = MeasureDistance();

        /*for (int i = 0; i < 11; i++) {
            print(i + " = " + inputSignals[i]);
        }*/

        String args = "";
        for (int i = 0; i < 11; i++) {
            if (i != 10) {
                args += inputSignals[i] + Environment.NewLine;
            } else {
                args += inputSignals[i];
            }
        }

        bool isFileReady = false;
        while (!isFileReady) {
            isFileReady = IsFileReady(@"Assets\input.txt");
        }
        System.IO.StreamWriter inputFile = new System.IO.StreamWriter(@"Assets\input.txt");
        inputFile.WriteLine(args);
        inputFile.Close();
        
        double currentSpineInclination = movableParts[0].transform.eulerAngles.x;
        double[] output = new double[10];

        isFileReady = false;
        while (!isFileReady) {
            isFileReady = IsFileReady(@"Assets\output.txt");
        }
        // Read the file and display it line by line.
        //System.IO.StreamReader outputFile = new System.IO.StreamReader(@"Assets\output.txt");        
        //outputFile.Close();
        string outputText = File.ReadAllText(@"Assets\output.txt");
        string[] lines = outputText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        for (int i = 0; i < lines.Length; i++) {
            if (!lines[i].Equals("")) {
                output[i] = Double.Parse(lines[i]);
            }
        }
        
        /*for (int i = 0; i < output.Length; i++) {
            print(i + " = " + inputSignals[i]);
            print(i + " = " + output[i]);
        }*/

        // Get maximal value among output of neural network
        double maxOutput = -999999999.0;
        int action = -1;
        for (int i = 0; i < output.Length; i++) {
            if (output[i] > maxOutput) {
                maxOutput = output[i];
                action = i;
            }
        }

        double x = movableParts[0].transform.rotation.eulerAngles.x;
        //print("Wysokość = " + movableParts[0].transform.position.y);


        //movableParts[0].Move((float)SpineForward(), 0.0F, 0.0F);
        //movableParts[1].Move(0.0F, 0.0F, (float)UpperLegLForward());
        //movableParts[2].Move((float)LowerLegLBackward(), 0.0F, 0.0F);
        //movableParts[3].Move(0.0F, 0.0F, (float)UpperLegRForward());
        //movableParts[4].Move((float)LowerLegRBackward(), 0.0F, 0.0F);

        switch (action) {
            case 0:
                movableParts[0].Move((float)SpineForward(), 0.0F, 0.0F);
                break;
            case 1:
                movableParts[0].Move((float)SpineBackward(), 0.0F, 0.0F);
                break;
            case 2:
                movableParts[1].Move(0.0F, 0.0F, (float)UpperLegLForward());
                break;
            case 3:
                movableParts[1].Move(0.0F, 0.0F, (float)UpperLegLBackward());
                break;
            case 4:
                movableParts[2].Move((float)LowerLegLForward(), 0.0F, 0.0F);
                break;
            case 5:
                movableParts[2].Move((float)LowerLegLBackward(), 0.0F, 0.0F);
                break;
            case 6:
                movableParts[3].Move(0.0F, 0.0F, (float)UpperLegRForward());
                break;
            case 7:
                movableParts[3].Move(0.0F, 0.0F, (float)UpperLegRBackward());
                break;
            case 8:
                movableParts[4].Move((float)LowerLegRForward(), 0.0F, 0.0F);
                break;
            case 9:
                movableParts[4].Move((float)LowerLegRBackward(), 0.0F, 0.0F);
                break;
        }

        previousDistance = MeasureDistance();
        //print(previousDistance);
        previousSpineInclination = currentSpineInclination;

    }
}
