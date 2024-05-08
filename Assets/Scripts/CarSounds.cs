using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSounds : MonoBehaviour
{
    public float minTorque;
    [SerializeField]
    private float maxTorque;
    [SerializeField]
    private float appliedTorque;

    private Rigidbody carRb;
    [SerializeField]
    private AudioSource carAudio;
    private CarControl carControl;

    [SerializeField]
    private AudioClip CarRunning;
    [SerializeField]
    private AudioClip CarIdle;
    [SerializeField]
    private AudioClip CarIgnition;
    [SerializeField]
    private AudioClip CarStop;

    public float minPitch;
    public float maxPitch;
    [SerializeField]
    private float pitchFromCar;
    private bool starterIgnited;

    private float idleTime = 0f;
    public float idleTimeThreshold = 15f;
    private bool isIdle = false;
    private bool isEngineRunning = false;

    void Start()
    {
        carAudio = GetComponent<AudioSource>();
        Debug.Log("AudioSource component found: " + (carAudio != null));

        carRb = GetComponent<Rigidbody>();
        
        //Grab the CarController script
        carControl = GetComponent<CarControl>();
        if (carControl != null)
        {
            maxTorque = carControl.motorTorque;
        }
        //Disable the CarControl script
        carControl.throttleEnabled = false;
    }

    void FixedUpdate()
    {
        if (isIdle)
        {
            idleTime += Time.deltaTime;
            if (idleTime >= idleTimeThreshold)
            {
                StartCoroutine(StopEngineSound());
                idleTime = 0f;
            }
        }
        EngineSound();
    }

    void EngineSound()
    {
        if (carControl != null)
        {
            
            //Map the MotorTorque to a Magnitude (magnitude = Torque / 2000)
            appliedTorque = carControl.appliedTorque;
            Debug.Log("Applied Torque: " + appliedTorque);
        }

        pitchFromCar = (maxPitch - minPitch) * appliedTorque / maxTorque;

        ////If the Car is not Grounded
        //if (!Physics.Raycast(transform.position, Vector3.down, 1f))
        //{
        //    StopEngineSound();
        //    return;
        //}

        //If the Engine is not running and the player presses the trigger on release:
        if(isEngineRunning == false && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            StartCoroutine(StartEngineSoundCoroutine());
        }

        if (isEngineRunning)
        {      
            //If the Engine torque is being applied
            if (carRb.velocity.magnitude > 0.1)
            {
                Debug.Log("Switching to Running");
                StartCoroutine(RunningEngineSoundCoroutine());
                
                //Rest of the code remains the same...
                if (appliedTorque <= minTorque)
                {
                    carAudio.pitch = minPitch;
                }

                if (appliedTorque > minTorque && appliedTorque < maxTorque)
                {
                    carAudio.pitch = minPitch + pitchFromCar;
                }

                if (appliedTorque >= maxTorque)
                {
                    carAudio.pitch = maxPitch;
                }
            }
        }
    }

    //Code to Start the Engine Sound
    IEnumerator StartEngineSoundCoroutine()
    {
        if (!starterIgnited)
        {
            starterIgnited = true;

            carAudio.pitch = 1;

            carAudio.PlayOneShot(CarIgnition);

            yield return new WaitForSecondsRealtime(CarIgnition.length);

            //Enable the CarControl script
            carControl.throttleEnabled = true;

            isEngineRunning = true;
            isIdle = false;
            idleTime = 0f;

            StartCoroutine(IdleEngineSoundCoroutine());
        }
    }

    IEnumerator IdleEngineSoundCoroutine()
    {
        if (carAudio.clip != CarIdle || !isIdle)
        {
            isIdle = true;
            idleTime = 0f;
            carAudio.pitch = 1;

            carAudio.clip = CarIdle;

            carAudio.Play();

            carControl.throttleEnabled = true;

            yield return null;
        }
    }

    IEnumerator RunningEngineSoundCoroutine()
    {
        if (carAudio.clip != CarRunning)
        {
            isIdle = false;
            idleTime = 0f;
            carAudio.clip = CarRunning;
            carAudio.Play();

            yield return null;
        }
    }

    IEnumerator StopEngineSound()
    {
        if (isEngineRunning)
        {
            carAudio.Stop();

            carAudio.pitch = 1;

            carAudio.PlayOneShot(CarStop);

            yield return new WaitForSecondsRealtime(CarIgnition.length);         

            isEngineRunning = false;
            isIdle = false;
            starterIgnited = false;
            //Disable the CarControl script acceleration
            carControl.throttleEnabled = false;
        }
        yield return null;
    }
}
