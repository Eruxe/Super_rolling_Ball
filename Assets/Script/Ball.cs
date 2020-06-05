using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Ball : MonoBehaviour
{

    [SerializeField] float maxspeed;
    [SerializeField] float speed;
    [SerializeField] float gravity;

    public CameraScript cam;
    private Rigidbody rb;

    //Save Velocity for Pause
    Vector3 SavedVelocity;
    Vector3 SavedAngular;

    //EVENT
    public static event Action OnFalling;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxspeed;

        GameManager.Pausing += OnPause;
        GameManager.Resuming += OnResume;

        this.littlePushForward();
    }

    private void OnDestroy()
    {
        GameManager.Pausing -= OnPause;
        GameManager.Resuming -= OnResume;
    }

    void FixedUpdate()
    {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 verticalGravity = Vector3.Scale(cam.transform.forward, new Vector3(moveVertical, 0, moveVertical)) * speed;
            Vector3 horizontalGravity = Vector3.Scale(cam.transform.right, new Vector3(moveHorizontal, 0, moveHorizontal)) * speed;
            Physics.gravity = Vector3.ClampMagnitude(new Vector3(0, -9.81f, 0) + verticalGravity + horizontalGravity, 9.81f) * gravity;

            if (transform.position.y < -10 && GameManager.GetState == GAMESTATE.Play)
            {
                GameManager.Instance.Falling();
            }
    }

    public void SetCamera(CameraScript cam)
    {
        this.cam = cam;
    }

    public void littlePushForward()
    {
        this.rb.AddForce(Vector3.ClampMagnitude(cam.transform.forward,0.1f));
    }

    public void OnPause()
    {
        SavedVelocity = rb.velocity;
        SavedAngular = rb.angularVelocity;
        rb.isKinematic = true;
    }

    public void OnResume()
    {
        rb.isKinematic = false;
        rb.AddForce(SavedVelocity, ForceMode.VelocityChange);
        rb.AddTorque(SavedAngular, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > 2.8){ AudioManager.Instance.Play("CollisionHard"); }
        else if (collision.impulse.magnitude > 1.2){ AudioManager.Instance.Play("CollisionSoft"); }
    }
}
