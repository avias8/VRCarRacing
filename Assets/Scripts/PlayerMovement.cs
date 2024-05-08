using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;

    private CharacterController _controller;

    public float PlayerSpeed = 2f;

    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    public Camera Camera;


    private void Awake()
    {
        //Retrieve the MainCamera by finding the object with the tag "MainCamera"
        Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public override void Spawned()
    {
    }

    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.Two))
        {
            _jumpPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false)
        {
            return;
        }

        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        
        //Get the forward direction of the car
        Vector3 forward = transform.forward;

        //Define a controller move
        Vector3 move = new Vector3(0, 0, 0);

        //Set the move direction based on the camera forward and right directions
        move += forward * OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);

        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y += JumpForce;
        }
        _controller.Move(move + _velocity * Runner.DeltaTime);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        _jumpPressed = false;
    }
}