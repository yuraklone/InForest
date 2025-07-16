using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3 : MonoBehaviour
{
    public static string gameState;

    [SerializeField] private Vector3 velocity;              // 移動方向
    [SerializeField] private float moveSpeed = 5.0f;        // 移動速度
    [SerializeField] private float applySpeed = 0.2f;       // 振り向きの適用速度
    [SerializeField] private CameraController2 refCamera;  // カメラの水平回転を参照する用

    public Rigidbody rbody;
    Animator animator;

    public float jumpHeight = 3.0f;
    public float groundCheckDistance = 0.1f;
    bool isJump;
    bool isGrounded;


    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        gameState = "playing";
    }
    void Update()
    {

        if (Input.GetButtonDown("Jump"))
        {
            isJump = true;
        }



        // WASD入力から、XZ平面(水平な地面)を移動する方向(velocity)を得る
        velocity = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            velocity.z += 1;
        if (Input.GetKey(KeyCode.A))
            velocity.x -= 1;
        if (Input.GetKey(KeyCode.S))
            velocity.z -= 1;
        if (Input.GetKey(KeyCode.D))
            velocity.x += 1;

        // 速度ベクトルの長さを1秒でmoveSpeedだけ進むように調整する
        velocity = velocity.normalized * moveSpeed * Time.deltaTime;

        // いずれかの方向に移動している場合
        if (velocity.magnitude > 0)
        {
            animator.SetBool("move", true);

            // プレイヤーの回転(transform.rotation)の更新
            // 無回転状態のプレイヤーのZ+方向(後頭部)を、
            // カメラの水平回転(refCamera.hRotation)で回した移動の反対方向(-velocity)に回す回転に段々近づける
            transform.rotation = Quaternion.Slerp
                (
                    transform.rotation,
                    Quaternion.LookRotation(refCamera.hRotation * velocity),
                    applySpeed
                );

            // プレイヤーの位置(transform.position)の更新
            // カメラの水平回転(refCamera.hRotation)で回した移動方向(velocity)を足し込む
            transform.position += refCamera.hRotation * velocity;
        }
        else
        {
            animator.SetBool("move", false);
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(
            transform.position - new Vector3(0, -0.5f, 0),
            Vector3.down,
            groundCheckDistance
             );


        rbody.velocity = new Vector3(refCamera.hRotation.x, rbody.velocity.y, refCamera.hRotation.z);

        if (isJump && isGrounded)
        {
            rbody.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
            animator.SetTrigger("jump");
            isJump = false;
        }







    }
}
