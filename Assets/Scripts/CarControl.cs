using UnityEngine;

public class CarControl : MonoBehaviour
{
    public float motorTorque = 2000;
    public float brakeTorque = 2000;
    public float maxSpeed = 20;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;
    public float centreOfGravityOffset = -1f;
    public float appliedTorque = 0f;
    private float currentMotorTorque = 0;

    public bool throttleEnabled = true;

    [SerializeField]
    WheelControl[] wheels;
    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;

        // Find all child GameObjects that have the WheelControl script attached
        //wheels = GetComponentsInChildren<WheelControl>();
    }

    // Update is called once per frame
    void Update()
    {
        //Example: OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
        float bInput = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
        float vInput = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        float hInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch).x;

        appliedTorque = vInput * motorTorque;

        // Calculate current speed in relation to the forward direction of the car
        // (this returns a negative number when traveling backwards)
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);

        // Calculate how close the car is to top speed
        // as a number from zero to one
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed));

        // Use that to calculate how much torque is available 
        // (zero torque at top speed)    
        
        if(throttleEnabled) { 
            currentMotorTorque = Mathf.Lerp(motorTorque, 0, Mathf.Abs(speedFactor)); 
        }
        else
        {
            currentMotorTorque = 0;
        }


        // …and to calculate how much to steer 
        // (the car steers more gently at top speed)
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Check whether the user input is in the same direction 
        // as the car's velocity
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in wheels)
        {
            // Apply steering to Wheel colliders that have "Steerable" enabled
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
            }
            
            if (isAccelerating)
            {
                // Apply torque to Wheel colliders that have "Motorized" enabled
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                }
                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                // If the user is trying to go in the opposite direction
                // apply brakes to all wheels
                wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }

            if (Mathf.Abs(bInput) > 0.1f)
            {
                if (forwardSpeed > 1)
                {
                    Debug.Log("Braking");
                    wheel.WheelCollider.brakeTorque = Mathf.Abs(bInput) * brakeTorque;
                    wheel.WheelCollider.motorTorque = 0;
                }
                else
                {
                    //User is trying to reverse
                    Debug.Log("User is trying to reverse");
                    wheel.WheelCollider.motorTorque = -bInput * currentMotorTorque;
                    wheel.WheelCollider.brakeTorque = 0;

                }
            }
        }
    }
}