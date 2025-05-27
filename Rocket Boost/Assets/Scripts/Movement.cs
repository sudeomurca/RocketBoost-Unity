using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    // Input Actions
    [SerializeField] InputAction thrust;
    [SerializeField] InputAction rotation;

    // Movement Settings
    [SerializeField] float thrustStrength=100f;
    [SerializeField] float rotationStrength=100f;

    // Audio & Effects
    [SerializeField] AudioClip mainEngineSFX;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem rightThrustParticles;
    [SerializeField] ParticleSystem leftThrustParticles;

    // Component references
    AudioSource audioSource;
    Rigidbody rb;
    
    // Unity Events
    void Start()
    {
        // Cache required components
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    } 

    void OnEnable()
    { // Enable input actions when the object is active
        thrust.Enable();
        rotation.Enable();
    }
    
    void FixedUpdate()
    {
         // Handle thrust and rotation in fixed time steps (physics)
        ProcessThrust();
        ProcessRotation();
        
    }

    // Thrust Logic
    void ProcessThrust()
    {
        if (thrust.IsPressed())
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    private void StopThrusting()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void StartThrusting()
    {
        rb.AddRelativeForce(Vector3.up * thrustStrength * Time.fixedDeltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineSFX);
        }
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    // Rotation Logic
    void ProcessRotation()
    {
        float rotationInput = rotation.ReadValue<float>();
        if (rotationInput < 0)
        {
            StartRightThrusting();
        }
        else if (rotationInput > 0)
        {
            StartLeftThrusting();
        }
        else
        {
            StopSidesThrusting();
        }
    }

    private void StopSidesThrusting()
    {
        leftThrustParticles.Stop();
        rightThrustParticles.Stop();
    }

    private void StartLeftThrusting()
    {
        ApplyRotation(-rotationStrength);
        if (!leftThrustParticles.isPlaying)
        {
            rightThrustParticles.Stop();
            leftThrustParticles.Play();
        }
    }

    private void StartRightThrusting()
    {
        ApplyRotation(rotationStrength);
        if (!rightThrustParticles.isPlaying)
        {
            leftThrustParticles.Stop();
            rightThrustParticles.Play();
        }
    }

    private void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.fixedDeltaTime);
        rb.freezeRotation = false;
    }
}

 