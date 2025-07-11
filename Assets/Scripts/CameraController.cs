using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float sensitivity = 2f;
    private float angleX = 0f;
    private float angleY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        angleX += Input.GetAxis("Mouse X") * sensitivity;
        angleY -= Input.GetAxis("Mouse Y") * sensitivity;
        angleY = Mathf.Clamp(angleY, -20f, 80f);

        Quaternion rotation = Quaternion.Euler(angleY, angleX, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        transform.position = position;
        transform.LookAt(target);
    }
}
