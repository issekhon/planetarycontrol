using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float gravity = -12;
    public float jumpHeight = 1;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;
    public Vector3 velocity;

    Animator animator;
    Transform cameraT;
    CharacterController controller;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        velocity = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        //if (inputDir != Vector2.zero)
        //{
            float targetRotation = Mathf.Atan2(0, 1) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        //}

        bool running = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        velocityY += Time.deltaTime * gravity;
        velocity = Vector3.zero;
        if (inputDir.x > 0)
        {
            velocity = transform.right * currentSpeed + Vector3.up * velocityY;
        } else if (inputDir.x < 0)
        {
            velocity = -1 * transform.right * currentSpeed + Vector3.up * velocityY;
        }
        if (inputDir.y > 0)
        {
            if (velocity == Vector3.zero)
            {
                velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
            } else
            {
                velocity /= 2;
                velocity += transform.forward * currentSpeed / 1.2f;
            }
        } else if (inputDir.y < 0)
        {
            if (velocity == Vector3.zero)
            {
                velocity = -1 * transform.forward * currentSpeed + Vector3.up * velocityY;
            }
            else
            {
                velocity /= 2;
                velocity += -1 * transform.forward * currentSpeed / 1.2f;
            }
        }
        if (inputDir.x == 0 && inputDir.y == 0)
        {
            velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        }
        //Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
        {
            velocityY = 0;
        }

        float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
	}

    void Jump()
    {
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
        }
    }
}
