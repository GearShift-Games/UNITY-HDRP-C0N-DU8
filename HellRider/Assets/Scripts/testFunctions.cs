using UnityEngine;

public class testFunctions : MonoBehaviour
{
    // Wheel colliders
    public WheelCollider frontWheelCollider;
    public WheelCollider rearWheelCollider;

    // Parameters for bike control
    public float maxSteeringAngle = 30f; // Maximum steering angle
    public float maxMotorTorque = 1500f; // Maximum acceleration (motor torque)
    public float maxBrakeTorque = 3000f; // Maximum braking force
    public float maxTiltAngle = 30f; // Maximum lean angle to prevent falling

    // For smooth turning/leaning
    private float turnInput = 0f;
    private float accelInput = 0f;
    private float brakeInput = 0f;

    // Rigidbody to control physics
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f); // Adjust the center of mass lower for more stability
    }

    void Update()
    {
        // Get input for turning, acceleration, and braking
        turnInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow
        accelInput = Input.GetAxis("Vertical"); // W/S or Up/Down arrow
        brakeInput = Input.GetButton("Fire1") ? 1f : 0f; // Fire1 button for braking (usually Left Shift)

        // Update the bike's turning/leaning
        UpdateTurning();
        UpdateBraking();
    }

    void FixedUpdate()
    {
        // Apply acceleration based on input
        ApplyAcceleration();

        // Prevent the bike from falling over
        PreventFalling();

        // Optional small downward force to keep the bike grounded
        rb.AddForce(Vector3.down * 10f, ForceMode.Force);
    }

    void UpdateTurning()
    {
        // Apply steering angle based on turn input (left/right)
        frontWheelCollider.steerAngle = turnInput * maxSteeringAngle;
    }

    void ApplyAcceleration()
    {
        // Apply motor torque to the rear wheel for acceleration
        float motorTorqueValue = accelInput * maxMotorTorque;
        rearWheelCollider.motorTorque = motorTorqueValue;
    }

    void UpdateBraking()
    {
        // Apply brake torque when the brake button is pressed
        float brakeTorqueValue = brakeInput * maxBrakeTorque;
        frontWheelCollider.brakeTorque = brakeTorqueValue;
        rearWheelCollider.brakeTorque = brakeTorqueValue;
    }

    void PreventFalling()
    {
        // Get the current rotation of the bike
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        // Check if the bike is tilted too far (based on pitch and roll)
        float tiltThreshold = 30f;

        // If the bike is tilted too much in the pitch (forward/backward), apply corrective torque
        if (Mathf.Abs(currentEulerAngles.x) > tiltThreshold)
        {
            // Apply torque to bring the bike upright again (in the x-axis)
            float correctionTorque = Mathf.Sign(currentEulerAngles.x) * 100f;
            rb.AddTorque(Vector3.right * correctionTorque, ForceMode.Force);
        }

        // If the bike is tilted too much in the roll (sideways), apply corrective torque
        if (Mathf.Abs(currentEulerAngles.z) > tiltThreshold)
        {
            // Apply torque to bring the bike upright again (in the z-axis)
            float correctionTorque = Mathf.Sign(currentEulerAngles.z) * 50f;
            rb.AddTorque(Vector3.forward * correctionTorque, ForceMode.Force);
        }
    }
}
