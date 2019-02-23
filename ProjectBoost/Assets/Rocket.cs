using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 17f;
    [SerializeField] float rcsRotate = 100f;
    static float fRotate;

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
        Thrust();
        Rotate();
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.freezeRotation = true;
            rigidBody.AddRelativeForce(Vector3.up*rcsThrust);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
        rigidBody.freezeRotation = false;
    }

    private void Rotate()
    {
        fRotate = rcsRotate * Time.deltaTime;
        if (Input.GetKey(KeyCode.A) ^ Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.forward*fRotate);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(Vector3.back*fRotate);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Electric":
                rcsRotate *= -1;
                rcsThrust *= -1;
                break;
            case "Finish":
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                break;
            default:
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                //kill player
                break;
        }
    }
}
