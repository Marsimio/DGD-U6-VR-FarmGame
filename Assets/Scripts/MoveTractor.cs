using UnityEngine;

public class MoveTractor : MonoBehaviour
{
    public HingeJoint throttleHinge;
    public HingeJoint steeringHinge;

    public float maxSpeed = 8f;
    public float turnRate = 90f;
    public float accel = 6f;

    Rigidbody rb;
    float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float t = throttleHinge.angle; // -40-40
        float s = steeringHinge.angle; // -180-180

        if (float.IsNaN(t)) return;
        if (float.IsNaN(s)) s = 0f;
        
        if (float.IsNaN(t) || float.IsNaN(s)) return;

        // 0 -> 0 speed, -40 -> max speed
        float throttle = Mathf.InverseLerp(0f, -40f, t); // 0-1
        throttle = Mathf.Clamp01(throttle);

        float steer = Mathf.InverseLerp(-360f, 360f, s) * 2f - 1f;
        steer = Mathf.Clamp(steer, -1f, 1f);

        float targetSpeed = throttle * maxSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

        // Turn
        if (throttle >= 0.1f)
        {
            float yaw = steer * turnRate * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, yaw, 0f));
        }

        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 vel = forward * currentSpeed;
        vel.y = rb.linearVelocity.y;
        rb.linearVelocity = vel;
    }
}