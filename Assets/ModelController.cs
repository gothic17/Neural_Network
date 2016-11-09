using UnityEngine;
using System.Collections;
using NeuralNetwork;

public class ModelController : MonoBehaviour {

	public Collider[] colliders;
	public MoveJoint[] moveJoints; // All moveJoints in the gameObject
    private Vector3 initialPosition;

	// Use this for initialization
	void Start () {
        //initialPosition = gameObject.transform.position;
		//colliders = gameObject.FindObjectsOfType(Collider) as Collider[];
		moveJoints = GetComponentsInChildren<MoveJoint>();

		// Initialization of variables in every moveJoint
		foreach( MoveJoint moveJoint in moveJoints ) {
			moveJoint.Start();
		}

        Neuron neuron = new Neuron(3);
        neuron.RandomizeWeights(0.1, 1.0, 0.3);
        double[] signals = new double[3];
        signals[0] = 0.1;
        signals[1] = 0.5;
        signals[2] = 1.0;
        print(neuron.GetWeights()[0] + ", " + neuron.GetWeights()[1] + ", " + neuron.GetWeights()[2]);
        print("Neuron response = " + neuron.Response(signals));

    }
	
	// Update is called once per frame
	void Update () {
        //moveJoints[0].Move(0.0F, 1.0F, 0.0F);
        

        /*print("X = " + (gameObject.transform.position.x - initialPosition.x));
        print("Y = " + (gameObject.transform.position.y - initialPosition.y));
        print("Z = " + (gameObject.transform.position.z - initialPosition.z));*/
    }

    float MeasureDistanceX() {
        return gameObject.transform.position.x - initialPosition.x;
    }
    float MeasureDistanceY()
    {
        return gameObject.transform.position.y - initialPosition.y;
    }
    float MeasureDistanceZ()
    {
        return gameObject.transform.position.z - initialPosition.z;
    }
}
