using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringSync : MonoBehaviour
{
    public GameObject wheel;
    float steeringx;
    float steeringy;
    float steeringz;
    float steeringRange;

    void Start()
    {
        CarControl carControl = transform.parent.GetComponent<CarControl>();
        if (carControl == null)
        {
            Debug.LogError("CarControl script not found on parent object");
            return;
        }
        steeringRange = carControl.steeringRange;
        steeringx = transform.localEulerAngles.x;
        steeringy = transform.localEulerAngles.y;
        steeringz = transform.localEulerAngles.z;
    }

    void Update()
    {
        if(wheel == null)
        {
            Debug.LogError("Wheel GameObject not assigned");
            // Set only the z-axis rotation
            Quaternion newRotation = Quaternion.Euler(steeringx, steeringy, steeringz);

            transform.localRotation = newRotation;
            return;
        } else
        {
            // Grab the rotation of the Wheel on the y axis
            float wheelRotation = wheel.transform.localEulerAngles.y;

            //Reject Values of Wheel Rotation that are not within the range using steeringRange
            if (wheelRotation > steeringRange && wheelRotation < 360 - steeringRange)
            {
                return;
            }

            // Set only the z-axis rotation
            Quaternion newRotation = Quaternion.Euler(steeringx, steeringy, wheelRotation);

            transform.localRotation = newRotation;
        }       
    }
}
