using Fusion;
using UnityEngine;

public class WheelController : NetworkBehaviour
{
    //Take a float value that uses OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger)
    public float throttle = 0.0f;

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

        throttle = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);

        //Get the forward direction of the wheel
        Vector3 forward = transform.forward;

        //Define a controller move
        Vector3 move = new Vector3(0, 0, 0);

        //Set the move direction based on the camera forward and right directions
        move += forward * throttle;

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

        //Axis2D.PrimaryThumbstick is the left thumbstick on the Oculus Touch controller
        //Example: OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
        //Let's define a variable for the left/right movement of the Primary Thumbstick
        float leftRight = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch).x;

        //Map the left/right to a degree of rotation
        float rotation = leftRight * 90.0f;

        //Make the rotation of the wheel the same as the wheel around the Y axis
        transform.Rotate(0, rotation * Time.deltaTime, 0);


    }
}