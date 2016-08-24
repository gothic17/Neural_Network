using UnityEngine;
using System.Collections;

public class MoveJoint : MonoBehaviour {

		public float minXTiltAngle = 10.0F;
		public float maxXTiltAngle = 10.0F;
		public float minYTiltAngle = 10.0F;
		public float maxYTiltAngle = 10.0F;
		public float minZTiltAngle = 10.0F;
		public float maxZTiltAngle = 10.0F;
    	public float smooth = 10.0F;

		Transform jointTransform = null;
		private float initialXRotation;
		private float initialYRotation;
		private float initialZRotation;

		// Use this for initialization
		public void Start () {
			CharacterJoint characterJoint = gameObject.GetComponent( typeof(CharacterJoint) ) as CharacterJoint;

			if (characterJoint != null) {
				jointTransform = characterJoint.transform;

				initialXRotation = jointTransform.eulerAngles.x;
				initialYRotation = jointTransform.eulerAngles.y;
				initialZRotation = jointTransform.eulerAngles.z;
			}
		}
	
		// Update is called once per frame
		public void Update () {

			float tiltAroundX = 0.0F;//initialXRotation;
			float tiltAroundY = 0.0F;//initialYRotation;
			float tiltAroundZ = 0.0F;//initialZRotation;
			bool move = false;

			if (Input.GetAxis("Vertical") != 0.0F) {
				tiltAroundX = Input.GetAxis("Vertical");
				if (tiltAroundX <= 0) tiltAroundX = tiltAroundX * minXTiltAngle;
				else tiltAroundX = tiltAroundX * maxXTiltAngle;
			}
			if (Input.GetKey("e")) {
				tiltAroundY = 1.0F * maxYTiltAngle;
	 		} else if (Input.GetKey("q")) {
				tiltAroundY = -1.0F * minYTiltAngle;
			}

			if (Input.GetAxis("Horizontal") != 0.0F) {
				tiltAroundZ = Input.GetAxis("Horizontal");
				if (tiltAroundZ <= 0) tiltAroundZ = tiltAroundZ * minZTiltAngle;
				else tiltAroundZ = tiltAroundZ * maxZTiltAngle;
			}

			MoveWithKey(tiltAroundX, tiltAroundY, tiltAroundZ);
	}

	void MoveWithKey(float tiltAroundX, float tiltAroundY, float tiltAroundZ) {

		Quaternion target = Quaternion.Euler(initialXRotation + tiltAroundX, 
		initialYRotation + tiltAroundY,	initialZRotation + tiltAroundZ);

    	jointTransform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
	}

	// This function should receive values between -1.0 and 1.0
	public void Move(float tiltAroundX, float tiltAroundY, float tiltAroundZ) {
		
		if (tiltAroundX <= 0) tiltAroundX = tiltAroundX * minXTiltAngle;
		else tiltAroundX = tiltAroundX * maxXTiltAngle;
			
		if (tiltAroundY <= 0) tiltAroundY = tiltAroundY * minYTiltAngle;
	 	else tiltAroundY = tiltAroundY * maxYTiltAngle;

		if (tiltAroundZ <= 0) tiltAroundZ = tiltAroundZ * minZTiltAngle;
		else tiltAroundZ = tiltAroundZ * maxZTiltAngle;

		Quaternion target = Quaternion.Euler(initialXRotation + tiltAroundX, 
		initialYRotation + tiltAroundY,	initialZRotation + tiltAroundZ);

    	jointTransform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
	}

}
