using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraSpawnPosition : MonoBehaviour
{
    //Find Root Object for OVR Interaction Rig "OVRCameraRigInteraction"
    public GameObject OVRInteractionRig;
    public Transform Target;


    // Start is called before the first frame update
    void Start()
    {
        //Find Object with Tag PlayerRoot
        OVRInteractionRig = GameObject.FindGameObjectWithTag("PlayerRoot");

        //Make OVRInteractionRig a child of the target gameobject
        OVRInteractionRig.transform.parent = Target.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
