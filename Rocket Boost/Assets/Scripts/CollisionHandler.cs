using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    // Configurable parameters
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip successSound;
    [SerializeField] AudioClip deathExplosion;
    [SerializeField] ParticleSystem succesParticles;
    [SerializeField] ParticleSystem explosionParticles;

    // Component references
    AudioSource audioSource;
    
    // State control flags
    bool isControllable = true;  // Can the player control the rocket?
    bool isCollidable = true;    // Are collisions enabled?-for test

    //methods
    void Start()
    {
        // Cache the AudioSource component 
        audioSource = GetComponent<AudioSource>();
        
    }
    void Update()
    {
        // Listen to debug/test keys
        RespondToDebugKeys();   
    }
    void OnCollisionEnter(Collision other)
    {
        // Prevent multiple collision responses 
        if (!isControllable || !isCollidable) { return; }
        
        // Determine action based on object tag 
        switch (other.gameObject.tag)
        {
            case "Friendly":
            // Friendly object - no action needed 
                Debug.Log("Welcome to Mars");
                break;
            case "Finish":
            // Reached the goal - level success
                StartSuccesSequence();
                break;

            default:
            // Any other object causes crash 
                StartCrashSequence();

                break;
        }
    }

    
    void RespondToDebugKeys()
    {
        // Load next level manually when 'L' is pressed
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            LoadNextLevel();
        }
        //test the game
        else if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            isCollidable = !isCollidable;
        }
        
    }
    void StartSuccesSequence()
    {
        // Handle success state 
        isControllable = false;
        audioSource.Stop();
        audioSource.PlayOneShot(successSound);
        succesParticles.Play();
        MovementEnabledController();
        Invoke("LoadNextLevel", levelLoadDelay); // Load next level after delay 
    }

    void StartCrashSequence()
    {
        // Handle crash state
        isControllable = false;
        audioSource.Stop();
        audioSource.PlayOneShot(deathExplosion);
        explosionParticles.Play();
        MovementEnabledController();
        Invoke("ReloadLevel", levelLoadDelay); // Reload level after delay 
    }

    

    void LoadNextLevel()
        {
            // Load the next scene in build settings
            int nextScene = SceneManager.GetActiveScene().buildIndex+1;
            
            // Loop back to first scene if last is reached
            if(nextScene == SceneManager.sceneCountInBuildSettings)
            {
                nextScene = 0;
            }
            SceneManager.LoadScene(nextScene);
        }
        void ReloadLevel()
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene);
        }
    void MovementEnabledController()
    {
            GetComponent<Movement>().enabled = false;
    }    

    }


