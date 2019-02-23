using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 17f;
    [SerializeField] float rcsRotate = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;
    static float fRotate;
    enum State { Alive, Dying, Transcending};
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        switch (state) {
            case State.Alive:
                Thrust();
                Rotate();
                break;
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
        }
        rigidBody.freezeRotation = false;
    }

    private void ApplyThrust()
    {
        rigidBody.freezeRotation = true;
        rigidBody.AddRelativeForce(Vector3.up * rcsThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
    }

    private void Rotate()
    {
        fRotate = rcsRotate * Time.deltaTime;
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) ^ (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(Vector3.forward*fRotate);
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(Vector3.back*fRotate);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (state) {
            case State.Alive:
                switch (collision.gameObject.tag)
                {
                case "Friendly":
                    break;
                case "Electric":
                    rcsRotate *= -1;
                    rcsThrust *= -1;
                    break;
                    case "Finish":
                        StartSuccessSequence();
                        break;
                    default:
                        StartDeathSequence();
                        //kill player
                        break;
                }
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        Invoke("ResetLevel", 2);
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        Invoke("LoadNextScene", 2);
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        state = State.Alive;
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1); // TODO allow for more than 2 levels
        state = State.Alive;
    }
}
