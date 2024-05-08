using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpeedometer : MonoBehaviour
{
    //This script is attached to a TMP Text Component, which is a child of a canvas, which is a child of a canvas with a car.
    //Declare the TMP Text Component - no need for public because it is attached to the same object
    private TMPro.TextMeshProUGUI speedText;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("CarSpeedometer script is attached to " + gameObject.name);

        //Get the TMP Text Component
        speedText = GetComponent<TMPro.TextMeshProUGUI>();

        //Debug
        Debug.Log("Speedometer Text Component is " + speedText.text);
    }

    // Update is called once per frame
    void Update()
    {
        //Get the car's speed from the rigidbody attached to the parent object
        float speed = transform.parent.parent.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
        
        //Update the text
        speedText.text = speed.ToString("000");
    }
}
