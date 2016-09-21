using UnityEngine;
using System.Collections;

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
    }
	
	// Update is called once per frame
	void Update () {
        //moveJoints[0].Move(0.0F, 1.0F, 0.0F);

        print("X = " + (gameObject.transform.position.x - initialPosition.x));
        print("Y = " + (gameObject.transform.position.y - initialPosition.y));
        print("Z = " + (gameObject.transform.position.z - initialPosition.z));
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
