using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSynchronization : NetworkBehaviour
{
    //Find Root Object for OVR Interaction Rig "OVRCameraRigInteraction"
    public GameObject OVRInteractionRig;
    public Transform Target;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            //Find Object with Tag PlayerRoot
            OVRInteractionRig = GameObject.FindGameObjectWithTag("PlayerRoot");

            //Make OVRInteractionRig a child of the Target
            OVRInteractionRig.transform.parent = Target.transform;

            //Set the position and rotation of the OVRInteractionRig to Zeros.
            OVRInteractionRig.transform.localPosition = Vector3.zero;
            OVRInteractionRig.transform.localRotation = Quaternion.identity;

        }
    }
}