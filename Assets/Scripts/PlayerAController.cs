using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAController : MonoBehaviour
{
    public Rigidbody rbody;
    Animator animator;
    public float moveSpeed = 3.0f;
    public float jumpHeight = 3.0f;
    public float groundCheckDistance = 0.1f;

    bool isGrounded;

    float axisH;
    float axisV;
    Vector3 move;

    bool isJump;

    public LayerMask groundLayer;

    public float tackleForce = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        axisH = Input.GetAxisRaw("Horizontal");
        axisV = Input.GetAxisRaw("Vertical");
        move = new Vector3(axisH * moveSpeed, 0, axisV * moveSpeed);

        if (Input.GetButtonDown("Jump"))
        {
            isJump = true;
        }

        //向き変更
        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
            animator.SetBool("move",true);
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
            1.0f
             );


        rbody.velocity = new Vector3(move.x, rbody.velocity.y, move.z);

        if (isJump)
        {
            rbody.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
            animator.SetTrigger("jump");
            isJump = false;
        }


    }

}
