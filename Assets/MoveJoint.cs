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

    private float previousTiltAroundX;
    private float previousTiltAroundY;
    private float previousTiltAroundZ;
    private Quaternion previousPosition;

    // Use this for initialization
    public void Start () {
		CharacterJoint characterJoint = gameObject.GetComponent( typeof(CharacterJoint) ) as CharacterJoint;
		HingeJoint hingeJoint = gameObject.GetComponent (typeof (HingeJoint)) as HingeJoint;

		if (characterJoint != null) {
			jointTransform = characterJoint.transform;
		} else if (hingeJoint != null) {
			jointTransform = hingeJoint.transform;
		}

        previousTiltAroundX = 0.0F;
        previousTiltAroundY = 0.0F;
        previousTiltAroundZ = 0.0F;

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
            move = true;
        }
        if (Input.GetKey("e"))
        {
            tiltAroundY = 1.0F;
            move = true;
        }
        else if (Input.GetKey("q"))
        {
            tiltAroundY = -1.0F;
            move = true;
        }

        if (Input.GetAxis("Horizontal") != 0.0F)
        {
            tiltAroundZ = Input.GetAxis("Horizontal");
            move = true;
        }

        if (move) MoveWithKey(tiltAroundX, tiltAroundY, tiltAroundZ);
    }

    // This function should receive values between -1.0 and 1.0
    public void Move(float tiltAroundX, float tiltAroundY, float tiltAroundZ) {
        bool moveX = true;
        bool moveY = true;
        bool moveZ = true;
        float currentRotationX = Mathf.Abs(gameObject.transform.localEulerAngles.x - initialLocalRotationX);
        float currentRotationY = Mathf.Abs(gameObject.transform.localEulerAngles.y - initialLocalRotationY);
        float currentRotationZ = Mathf.Abs(gameObject.transform.localEulerAngles.z - initialLocalRotationZ);

        // We have to check six cases, when we shouldn't move in given direction(current stands for current rotation in given direction):
        // 1) |360 - min| - 1 < |current| < |360 - min| + 1
        // 2) |360 - max| - 1 < |current| < |360 - max| + 1
        // 3) |min| - 1 < |current| < |min| + 1
        // 4) |max| - 1 < |current| < |max| + 1
        // 5) |360 + min - max| - 1 < |current| < |360 + min - max| + 1
        // 6) |360 + max - min| - 1 < |current| < |360 + max - min| + 1

        //print("Min = " + minXTiltAngle);
        //print("Max = " + maxXTiltAngle);
        //print("1) " + (Mathf.Abs(360.0F - minXTiltAngle) - 1.0F) + " < " + Mathf.Abs(currentRotationX) + " < " + (Mathf.Abs(360.0F - minXTiltAngle) + 1.0F));
        //print("2) " + (Mathf.Abs(maxXTiltAngle) - 1.0F) + " < " + Mathf.Abs(currentRotationX) + " < " + (Mathf.Abs(maxXTiltAngle) + 1.0F));
        //print("3) " + (Mathf.Abs(360.0F + minXTiltAngle - maxXTiltAngle) - 1.0F) + " < " + Mathf.Abs(currentRotationX) + " < " + (Mathf.Abs(360.0F + minXTiltAngle - maxXTiltAngle) + 1.0F));


        if ((tiltAroundX == 1.0F || tiltAroundX == -1.0F) &&
            (
                ((Mathf.Abs(360.0F - minXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(360.0F - minXTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F - maxXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(360.0F - maxXTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(minXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(minXTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(maxXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(maxXTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + minXTiltAngle - maxXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(360.0F + minXTiltAngle - maxXTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + maxXTiltAngle - minXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(360.0F + maxXTiltAngle - minXTiltAngle) + 1.0F))
            ))
        {
            moveX = false;
            print("X - Osiagnalem punkt, w ktorym sie nie rusze");
        }
        if ((tiltAroundY == 1.0F || tiltAroundY == -1.0F) &&
            (
                ((Mathf.Abs(360.0F - minYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(360.0F - minYTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F - maxYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(360.0F - maxYTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(minYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(minYTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(maxYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(maxYTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + minYTiltAngle - maxYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(360.0F + minYTiltAngle - maxYTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + maxYTiltAngle - minYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(360.0F + maxYTiltAngle - minYTiltAngle) + 1.0F))
            ))
        {
            moveY = false;
            print("Y - Osiagnalem punkt, w ktorym sie nie rusze");
        }
        if ((tiltAroundZ == 1.0F || tiltAroundZ == -1.0F) &&
            (
                ((Mathf.Abs(360.0F - minZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(360.0F - minZTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F - maxZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(360.0F - maxZTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(minZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(minZTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(maxZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(maxZTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + minZTiltAngle - maxZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(360.0F + minZTiltAngle - maxZTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + maxZTiltAngle - minZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(360.0F + maxZTiltAngle - minZTiltAngle) + 1.0F))
            ))
        {
            moveZ = false;
            print("Z - Osiagnalem punkt, w ktorym sie nie rusze");
        }

        if (tiltAroundX <= 0) tiltAroundX = tiltAroundX * minXTiltAngle;
        else tiltAroundX = tiltAroundX * maxXTiltAngle;

        if (tiltAroundY <= 0) tiltAroundY = tiltAroundY * minYTiltAngle;
        else tiltAroundY = tiltAroundY * maxYTiltAngle;

        if (tiltAroundZ <= 0) tiltAroundZ = tiltAroundZ * minZTiltAngle;
        else tiltAroundZ = tiltAroundZ * maxZTiltAngle;

        float rotationX = gameObject.transform.localEulerAngles.x;
        float rotationY = gameObject.transform.localEulerAngles.y;
        float rotationZ = gameObject.transform.localEulerAngles.z;

        Quaternion target = new Quaternion();
        if (moveX) rotationX = initialLocalRotationX - tiltAroundX;
        if (moveY) rotationY = initialLocalRotationY - tiltAroundY;
        if (moveZ) rotationZ = initialLocalRotationZ - tiltAroundZ;

        target = parent.transform.rotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
        jointTransform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
    }

	void MoveWithKey(float tiltAroundX, float tiltAroundY, float tiltAroundZ) {
        bool moveX = true;
        bool moveY = true;
        bool moveZ = true;
        float currentRotationX = Mathf.Abs(gameObject.transform.localEulerAngles.x - initialLocalRotationX);
        float currentRotationY = Mathf.Abs(gameObject.transform.localEulerAngles.y - initialLocalRotationY);
        float currentRotationZ = Mathf.Abs(gameObject.transform.localEulerAngles.z - initialLocalRotationZ);


        // We have to check six cases, when we shouldn't move in given direction(current stands for current rotation in given direction):
        // 1) |360 - min| - 1 < |current| < |360 - min| + 1
        // 2) |360 - max| - 1 < |current| < |360 - max| + 1
        // 3) |min| - 1 < |current| < |min| + 1
        // 4) |max| - 1 < |current| < |max| + 1
        // 5) |360 + min - max| - 1 < |current| < |360 + min - max| + 1
        // 6) |360 + max - min| - 1 < |current| < |360 + max - min| + 1

        //print("Min = " + minXTiltAngle);
        //print("Max = " + maxXTiltAngle);
        //print("1) " + (Mathf.Abs(360.0F - minXTiltAngle) - 1.0F) + " < " + Mathf.Abs(currentRotationX) + " < " + (Mathf.Abs(360.0F - minXTiltAngle) + 1.0F));
        //print("2) " + (Mathf.Abs(maxXTiltAngle) - 1.0F) + " < " + Mathf.Abs(currentRotationX) + " < " + (Mathf.Abs(maxXTiltAngle) + 1.0F));
        //print("3) " + (Mathf.Abs(360.0F + minXTiltAngle - maxXTiltAngle) - 1.0F) + " < " + Mathf.Abs(currentRotationX) + " < " + (Mathf.Abs(360.0F + minXTiltAngle - maxXTiltAngle) + 1.0F));


        if ((tiltAroundX == 1.0F || tiltAroundX == -1.0F) &&
            (
                ((Mathf.Abs(360.0F - minXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(360.0F - minXTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F - maxXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(360.0F - maxXTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(minXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(minXTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(maxXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(maxXTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + minXTiltAngle - maxXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(360.0F + minXTiltAngle - maxXTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + maxXTiltAngle - minXTiltAngle) - 1.0F) < Mathf.Abs(currentRotationX) && Mathf.Abs(currentRotationX) < (Mathf.Abs(360.0F + maxXTiltAngle - minXTiltAngle) + 1.0F))
            ))
        {
            moveX = false;
            //print("X - Osiagnalem punkt, w ktorym sie nie rusze");
        }
        if ((tiltAroundY == 1.0F || tiltAroundY == -1.0F) &&
            (
                ((Mathf.Abs(360.0F - minYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(360.0F - minYTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F - maxYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(360.0F - maxYTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(minYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(minYTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(maxYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(maxYTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + minYTiltAngle - maxYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(360.0F + minYTiltAngle - maxYTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + maxYTiltAngle - minYTiltAngle) - 1.0F) < Mathf.Abs(currentRotationY) && Mathf.Abs(currentRotationY) < (Mathf.Abs(360.0F + maxYTiltAngle - minYTiltAngle) + 1.0F))
            ))
        {
            moveY = false;
            //print("Y - Osiagnalem punkt, w ktorym sie nie rusze");
        }
        if ((tiltAroundZ == 1.0F || tiltAroundZ == -1.0F) &&
            (
                ((Mathf.Abs(360.0F - minZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(360.0F - minZTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F - maxZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(360.0F - maxZTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(minZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(minZTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(maxZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(maxZTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + minZTiltAngle - maxZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(360.0F + minZTiltAngle - maxZTiltAngle) + 1.0F)) ||
                ((Mathf.Abs(360.0F + maxZTiltAngle - minZTiltAngle) - 1.0F) < Mathf.Abs(currentRotationZ) && Mathf.Abs(currentRotationZ) < (Mathf.Abs(360.0F + maxZTiltAngle - minZTiltAngle) + 1.0F))
            ))
        {
            moveZ = false;
            //print("Z - Osiagnalem punkt, w ktorym sie nie rusze");
        }

        if (tiltAroundX <= 0) tiltAroundX = tiltAroundX * minXTiltAngle;
        else tiltAroundX = tiltAroundX * maxXTiltAngle;

        if (tiltAroundY <= 0) tiltAroundY = tiltAroundY * minYTiltAngle;
        else tiltAroundY = tiltAroundY * maxYTiltAngle;

        if (tiltAroundZ <= 0) tiltAroundZ = tiltAroundZ * minZTiltAngle;
        else tiltAroundZ = tiltAroundZ * maxZTiltAngle;

        float rotationX = gameObject.transform.localEulerAngles.x;
        float rotationY = gameObject.transform.localEulerAngles.y;
        float rotationZ = gameObject.transform.localEulerAngles.z;

        Quaternion target = new Quaternion();
        if (moveX) rotationX = initialLocalRotationX - tiltAroundX;
        if (moveY) rotationY = initialLocalRotationY - tiltAroundY;
        if (moveZ) rotationZ = initialLocalRotationZ - tiltAroundZ;

        target = parent.transform.rotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
        jointTransform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
    }
}
