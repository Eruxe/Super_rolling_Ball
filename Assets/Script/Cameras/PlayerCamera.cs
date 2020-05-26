using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Quaternion initial_rotation;

    [SerializeField] float cam_reactivity;
    [SerializeField] float vertical_power;
    [SerializeField] float horizontal_power;

    

    // Start is called before the first frame update
    void Start()
    {
        initial_rotation = transform.rotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        //The camera lean with player input to give the illusion that the player move the stage and not the ball
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        float xEuleur = initial_rotation.eulerAngles[0] + moveVertical * vertical_power * -1;
        float zEuleur = initial_rotation.eulerAngles[2] + moveHorizontal * horizontal_power;

        Quaternion target_rotationXZ = Quaternion.Slerp(Quaternion.Euler(transform.rotation.eulerAngles[0], 0, transform.rotation.eulerAngles[2]), Quaternion.Euler(xEuleur, 0, zEuleur), cam_reactivity * Time.deltaTime);

        //transform.rotation = target_rotationXZ * Quaternion.Euler(0,transform.rotation.eulerAngles[1],0) ;

    }
}
