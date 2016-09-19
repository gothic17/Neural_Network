using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public GameObject model;
    private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        offset.x = transform.position.x - model.transform.position.x;
        offset.z = transform.position.z - model.transform.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = model.transform.position + offset;
    }
}