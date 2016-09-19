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
	GameObject parent = null;

    private float initialLocalRotationX;
    private float initialLocalRotationY;
    private float initialLocalRotationZ;

	// Use this for initialization
	public void Start () {
		CharacterJoint characterJoint = gameObject.GetComponent( typeof(CharacterJoint) ) as CharacterJoint;
		HingeJoint hingeJoint = gameObject.GetComponent (typeof (HingeJoint)) as HingeJoint;

		if (characterJoint != null) {
			jointTransform = characterJoint.transform;
		} else if (hingeJoint != null) {
			jointTransform = hingeJoint.transform;
		}

        initialLocalRotationX = gameObject.transform.localEulerAngles.x;
        initialLocalRotationY = gameObject.transform.localEulerAngles.y;
        initialLocalRotationZ = gameObject.transform.localEulerAngles.z;

		parent = jointTransform.parent.gameObject;     
    }

    // Update is called once per frame
    public void Update()
    {
        float tiltAroundX = 0.0F;
        float tiltAroundY = 0.0F;
        float tiltAroundZ = 0.0F;
        bool move = false;

        if (Input.GetAxis("Vertical") != 0.0F)
        {
            tiltAroundX = Input.GetAxis("Vertical");
            if (tiltAroundX <= 0) tiltAroundX = tiltAroundX * minXTiltAngle;
            else tiltAroundX = tiltAroundX * maxXTiltAngle;
            move = true;
        }
        if (Input.GetKey("e"))
        {
            tiltAroundY = 1.0F * maxYTiltAngle;
            move = true;
        }
        else if (Input.GetKey("q"))
        {
            tiltAroundY = -1.0F * minYTiltAngle;
            move = true;
        }

        if (Input.GetAxis("Horizontal") != 0.0F)
        {
            tiltAroundZ = Input.GetAxis("Horizontal");
            if (tiltAroundZ <= 0) tiltAroundZ = tiltAroundZ * minZTiltAngle;
            else tiltAroundZ = tiltAroundZ * maxZTiltAngle;
            move = true;
        }

        if (move) MoveWithKey(tiltAroundX, tiltAroundY, tiltAroundZ);
    }

    // This function should receive values between -1.0 and 1.0
    public void Move(float tiltAroundX, float tiltAroundY, float tiltAroundZ)
    {
        if (tiltAroundX <= 0) tiltAroundX = tiltAroundX * minXTiltAngle;
        else tiltAroundX = tiltAroundX * maxXTiltAngle;

        if (tiltAroundY <= 0) tiltAroundY = tiltAroundY * minYTiltAngle;
        else tiltAroundY = tiltAroundY * maxYTiltAngle;

        if (tiltAroundZ <= 0) tiltAroundZ = tiltAroundZ * minZTiltAngle;
        else tiltAroundZ = tiltAroundZ * maxZTiltAngle;

        Quaternion target;
        target = parent.transform.rotation * Quaternion.Euler(initialLocalRotationX - tiltAroundX, initialLocalRotationY - tiltAroundY, initialLocalRotationZ - tiltAroundZ);
        jointTransform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
    }

	void MoveWithKey(float tiltAroundX, float tiltAroundY, float tiltAroundZ) {

        Quaternion target;
        target = parent.transform.rotation * Quaternion.Euler(initialLocalRotationX - tiltAroundX, initialLocalRotationY - tiltAroundY, initialLocalRotationZ - tiltAroundZ);
        jointTransform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
    }
}
