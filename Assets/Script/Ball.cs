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

    //SOUND
    float SoundTimer;
    RaycastHit ray;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxspeed;

        GameManager.Pausing += OnPause;
        GameManager.Resuming += OnResume;

        SoundTimer = 0;

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
        if (GameManager.GetState != GAMESTATE.Victory)
        {
            Physics.gravity = Vector3.ClampMagnitude(new Vector3(0, -9.81f, 0) + verticalGravity + horizontalGravity, 9.81f) * gravity;
        }
        else
        {
            Physics.gravity = Vector3.zero;
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 5 * Time.deltaTime);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, 5 * Time.deltaTime);
        }

        if (transform.position.y < -10 && GameManager.GetState == GAMESTATE.Play)
        {
            GameManager.Instance.Falling();
        }

        //Moving Sound
        SoundTimer +=0.06f;
        if (Physics.SphereCast(transform.position, 0.65f, transform.up,out this.ray,0.60f)) {
            
            if (rb.velocity.magnitude > 0.001 && SoundTimer > 10 / rb.velocity.magnitude)
            {
                if (GameManager.isAudio) AudioManager.Instance.PlayOneShot("Rolling",0.7f + rb.velocity.magnitude/125);
                SoundTimer = 0;
            }
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
        //CollisionSound
        if (collision.impulse.magnitude > 2.8) { if (GameManager.isAudio) AudioManager.Instance.Play("CollisionHard"); }
        else if (collision.impulse.magnitude > 1) { if (GameManager.isAudio) AudioManager.Instance.Play("CollisionSoft"); }
    }

}
