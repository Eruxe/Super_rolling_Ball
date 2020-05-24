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

    //EVENT
    public static event Action OnFalling;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxspeed;
        this.littlePushForward();
    }

    // Update is called once per frame
    void Update()
    {
        //ne pas utiliser pour la physique de la balle
    }

    void FixedUpdate()
    {
        if (GameManager.GetState == GAMESTATE.Play || GameManager.GetState == GAMESTATE.Practice)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 verticalGravity = Vector3.Scale(cam.transform.forward, new Vector3(moveVertical, 0, moveVertical)) * speed;
            Vector3 horizontalGravity = Vector3.Scale(cam.transform.right, new Vector3(moveHorizontal, 0, moveHorizontal)) * speed;
            Physics.gravity = Vector3.ClampMagnitude(new Vector3(0, -9.81f, 0) + verticalGravity + horizontalGravity, 9.81f) * gravity;

            if (transform.position.y < -10)
            {
                GameManager.Instance.Falling();
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
}
