using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] float cam_reactivity;
    [SerializeField] float recenter_speed;
    [SerializeField] float vertical_power;
    [SerializeField] float horizontal_power;

    public Ball player;
    public Rigidbody prb;
    private Quaternion initial_rotation;
    GameObject PlayerCenter;

    // Start is called before the first frame update
    void Start()
    {
        initial_rotation = transform.rotation;
        prb = player.GetComponent<Rigidbody>();
        PlayerCenter = GameObject.Find("PlayerCameraCenter");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GetState == GAMESTATE.Play) {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");

            //The camera lean with player input to give the illusion that the player move the stage and not the ball
            float xEuleur = initial_rotation.eulerAngles[0] + moveVertical * vertical_power * -1;
            float zEuleur = initial_rotation.eulerAngles[2] + moveHorizontal * horizontal_power;

            float yEuleur = transform.rotation.eulerAngles[1];
            if (prb.velocity.magnitude > 0.1)
            {
                yEuleur = Vector3.SignedAngle(Vector3.forward, Vector3.Scale(prb.velocity, new Vector3(1, 0, 1)), Vector3.up);
            }

            Quaternion target_rotationXYZ = Quaternion.Slerp(Quaternion.Euler(0, transform.rotation.eulerAngles[1], 0), Quaternion.Euler(0, yEuleur, 0), recenter_speed * Time.deltaTime * (prb.velocity.magnitude));


            transform.rotation = target_rotationXYZ;//* target_rotationXZ;

            //The camera follow the player
            transform.position = player.transform.position;

            Quaternion target_rotationXZ = Quaternion.Slerp(Quaternion.Euler(PlayerCenter.transform.rotation.eulerAngles[0], 0, PlayerCenter.transform.rotation.eulerAngles[2]), Quaternion.Euler(xEuleur, 0, zEuleur), cam_reactivity * Time.deltaTime);
            PlayerCenter.transform.rotation = target_rotationXYZ * target_rotationXZ;
        }
    }

    public void SetBall(Ball ball)
    {
        this.player = ball;
        this.prb = player.GetComponent<Rigidbody>();
    }
}
