using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class bicycleController : MonoBehaviour
{
    // Takes the wheel colliders
    public WheelCollider[] wheel_col;

    // Takes the wheel meshs
    // public Transform[] wheels;

    // Speed
    public float torque = 500;

    // angle
    float angle = 70;
    void Start()
    {
        
    }

    // Update for physics
    void FixedUpdate()
    {
        // Remet le vélo à rotation 0 z
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        eulerRotation.z = 0f;
        Quaternion targetRotation = Quaternion.Euler(eulerRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20000f);  // Adjust speed here

        if (transform.rotation.z > 17 || transform.rotation.z < -17)
        {
            // Fixe la rotation du vélo si il tombe
            // fixedRotation = Quaternion.eulerAngles(transform.rotation.x, transform.rotation.y, 0);
            // transform.rotation = Quaternion.Slerp(transform.rotation, fixedRotation, 1f);
        }
        for (int i = 0;i< wheel_col.Length; i++)
        {
            // Front and back speed
            wheel_col[i].motorTorque = Input.GetAxis("Vertical")*torque;

            // Turns the wheel collider
            if (i == 0)
            {
                wheel_col[i].steerAngle = Input.GetAxis("Horizontal") * angle;
            }

            // Rotates the wheel meshs
            /*
            var pos = transform.position;
            var rot = transform.rotation;
            wheel_col[i].GetWorldPose(out pos, out rot);
            wheels[i].position = pos;
            wheels[i].rotation = rot;
            */
        }
    }
}
