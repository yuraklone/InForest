using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController2 : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;

    [SerializeField] private float turnSpeed = 10.0f;   // 回転速度
    [SerializeField] private Transform player;          // 注視対象プレイヤー

    [SerializeField] private Quaternion vRotation;      // カメラの垂直回転(見下ろし回転)
    [SerializeField] public Quaternion hRotation;      // カメラの水平回転
    private float angleX = 0f;
    private float angleY = 0f;




    void Start()
    {
        // 回転の初期化
        vRotation = Quaternion.Euler(0, 0, 0);         // 垂直回転(X軸を軸とする回転)は、30度見下ろす回転
        hRotation = Quaternion.identity;                // 水平回転(Y軸を軸とする回転)は、無回転
        transform.rotation = hRotation * vRotation;     // 最終的なカメラの回転は、垂直回転してから水平回転する合成回転

    }

    void LateUpdate()
    {

        if (Input.GetMouseButton(0) && PlayerController3.gameState == "playing")
        {
            angleX += Input.GetAxis("Mouse X") * turnSpeed;
            angleY -= Input.GetAxis("Mouse Y") * turnSpeed;
            angleY = Mathf.Clamp(angleY, -20f, 80f);

            hRotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * turnSpeed, 0);

        }


        Quaternion rotation = Quaternion.Euler(angleY, angleX, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        transform.position = position;
        transform.LookAt(target);

 
    }
}
