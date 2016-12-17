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
        return gameObject.transform.GetChild(0).GetChild(0).position.z - initialPosition.z;
    }

    public double SpineForward() {
        previousMoves[0] = previousMoves[0] - 0.5;
        return previousMoves[0];
    }

    public double SpineBackward() {
        previousMoves[0] = previousMoves[0] + 0.5;
        return previousMoves[0];
    }

    public double UpperLegLForward() {
        previousMoves[1] = previousMoves[1] - 0.5;
        return previousMoves[1];
    }

    public double UpperLegLBackward() {
        previousMoves[1] = previousMoves[1] + 0.5;
        return previousMoves[1];
    }

    public double LowerLegLForward() {
        previousMoves[2] = previousMoves[2] + 0.5;
        return previousMoves[2];
    }

    public double LowerLegLBackward() {
        previousMoves[2] = previousMoves[2] - 0.5;
        return previousMoves[2];
    }

    public double FootLForward() {
        previousMoves[3] = previousMoves[3] + 0.5;
        return previousMoves[3];
    }

    public double FootLBackward() {
        previousMoves[3] = previousMoves[3] - 0.5;
        return previousMoves[3];
    }

    public double UpperLegRForward() {
        previousMoves[4] = previousMoves[4] - 0.5;
        return previousMoves[4];
    }

    public double UpperLegRBackward() {
        previousMoves[4] = previousMoves[4] + 0.5;
        return previousMoves[4];
    }

    public double LowerLegRForward() {
        previousMoves[5] = previousMoves[5] + 0.5;
        return previousMoves[5];
    }

    public double LowerLegRBackward() {
        previousMoves[5] = previousMoves[5] - 0.5;
        return previousMoves[5];
    }

    public double FootRForward() {
        previousMoves[6] = previousMoves[6] + 0.5;
        return previousMoves[6];
    }

    public double FootRBackward() {
        previousMoves[6] = previousMoves[6] - 0.5;
        return previousMoves[6];
    }

    // Use this for initialization
    void Start () {
        initialPosition = gameObject.transform.GetChild(0).GetChild(0).position;
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

        // Set each cell in previousMoves table to 0 (beginning state of each MovablePart)
        previousMoves = new double[7];
        for (int i = 0; i < 7; i++) {
            previousMoves[i] = 0.0;
        }

        previousSpineInclination = movableParts[0].transform.rotation.eulerAngles.x; // Save inclination of spine
        previousDistance = MeasureDistance(); // Save distance (probably equal to 0)
        randomGenerator = new System.Random(); // Create new Random Generator
    }
	
	// Update is called once per frame
	void Update () {
        Time.timeScale = 1.0F;
        //print(gameObject.transform.GetChild(0).rotation.eulerAngles.x);
        double[] inputSignals = new double[24];
        // Version with just transfrom.position.x/y/z
        /*
        // First three fields of inputSignals are positions of Spine
        inputSignals[0] = movableParts[0].transform.position.x;
        inputSignals[1] = movableParts[0].transform.position.y;
        inputSignals[2] = movableParts[0].transform.position.z;
        
        for (int i = 1; i < movableParts.Length; i++) { // In this loop we set difference between movableParts positions and Spine position
            inputSignals[(3*i)] = movableParts[i].transform.position.x - inputSignals[0];
            inputSignals[(3 * i) + 1] = movableParts[i].transform.position.y - inputSignals[1];
            inputSignals[(3 * i) + 2] = movableParts[i].transform.position.z - inputSignals[2];
        }*/
        // Version with transform.eulerAngles.x/y/z
        // First three fields of inputSignals are positions of Spine
        inputSignals[0] = movableParts[0].transform.rotation.eulerAngles.x;
        inputSignals[1] = movableParts[0].transform.rotation.eulerAngles.y;
        inputSignals[2] = movableParts[0].transform.rotation.eulerAngles.z;

        for (int i = 1; i < movableParts.Length; i++) { // In this loop we set difference between movableParts positions and Spine position
            inputSignals[(3 * i)] = movableParts[i].transform.rotation.eulerAngles.x - inputSignals[0];
            inputSignals[(3 * i) + 1] = movableParts[i].transform.rotation.eulerAngles.y - inputSignals[1];
            inputSignals[(3 * i) + 2] = movableParts[i].transform.rotation.eulerAngles.z - inputSignals[2];
        }


        // Collisions of right and left foot
        if (movableParts[3].transform.GetComponent<OnCollision>().GetTouchingFloor() == false) inputSignals[21] = -1.0;
        else inputSignals[21] = 1.0;
        double footLTouchingGround = inputSignals[21];
        if (movableParts[6].transform.GetComponent<OnCollision>().GetTouchingFloor() == false) inputSignals[22] = -1.0;
        else inputSignals[22] = 1.0;
        double footRTouchingGround = inputSignals[22];
        // Last variable is distance traveled from initial position
        inputSignals[23] = MeasureDistance();

        /*for (int i = 0; i < 3; i++) {
            print(i + " = " + inputSignals[i]);
        }*/

        String args = "";
        for (int i = 0; i < 24; i++) {
            if (i != 23) {
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
        double[] output = new double[24];

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

        // Get maximal value among output of neural network
        double maxOutput = -999999999.0;
        int action = -1;
        for (int i = 0; i < output.Length; i++) {
            if (output[i] > maxOutput) {
                maxOutput = output[i];
                action = i;
            }
        }

        //movableParts[0].Move((float)SpineForward(), 0.0F, 0.0F);
        //movableParts[1].Move(0.0F, 0.0F, (float)UpperLegLForward());
        //movableParts[2].Move((float)LowerLegLForward(), 0.0F, 0.0F);
        //movableParts[3].Move((float)FootLForward(), 0.0F, 0.0F);
        //movableParts[4].Move(0.0F, 0.0F, (float)UpperLegRForward());
        //movableParts[5].Move((float)LowerLegRForward(), 0.0F, 0.0F);
        //movableParts[6].Move((float)FootRForward(), 0.0F, 0.0F);
        
        switch (action) {
            case 1:
                movableParts[0].Move((float)SpineForward(), 0.0F, 0.0F);
                break;
            case 2:
                movableParts[0].Move((float)SpineBackward(), 0.0F, 0.0F);
                break;
            case 3:
                movableParts[1].Move(0.0F, 0.0F, (float)UpperLegLForward());
                break;
            case 4:
                movableParts[1].Move(0.0F, 0.0F, (float)UpperLegLBackward());
                break;
            case 5:
                movableParts[2].Move((float)LowerLegLForward(), 0.0F, 0.0F);
                break;
            case 6:
                movableParts[2].Move((float)LowerLegLBackward(), 0.0F, 0.0F);
                break;
            case 7:
                movableParts[3].Move((float)FootLForward(), 0.0F, 0.0F);
                break;
            case 8:
                movableParts[3].Move((float)FootLBackward(), 0.0F, 0.0F);
                break;
            case 9:
                movableParts[4].Move(0.0F, 0.0F, (float)UpperLegRForward());
                break;
            case 10:
                movableParts[4].Move(0.0F, 0.0F, (float)UpperLegRBackward());
                break;
            case 11:
                movableParts[5].Move((float)LowerLegRForward(), 0.0F, 0.0F);
                break;
            case 12:
                movableParts[5].Move((float)LowerLegRBackward(), 0.0F, 0.0F);
                break;
            case 13:
                movableParts[6].Move((float)FootRForward(), 0.0F, 0.0F);
                break;
            case 14:
                movableParts[6].Move((float)FootRBackward(), 0.0F, 0.0F);
                break;
        }

        previousDistance = MeasureDistance();
        //print(previousDistance);
        previousSpineInclination = currentSpineInclination;
        
    }
}
