using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHorse : MonoBehaviour
{
    Entity entity;

    public float speed;
    public float turnSpeed;

    public float jumpForce;

    public float gravity;

    public float friction;

    public Transform visual;

    public float accelerationSpeed = 1;
    void Start()
    {
        entity = GetComponent<Entity>();
    }

    float lerpedSpeed;

    private void Update()
    {
        entity.AI();
    }
    private void FixedUpdate()
    {
        if (entity.mob.input.magnitude > 0)
        {
            lerpedSpeed = Mathf.Lerp(lerpedSpeed, speed, accelerationSpeed * Mathf.Clamp01(entity.mob.input.z) * Time.deltaTime);
        }
        else
        {
            lerpedSpeed = Mathf.Lerp(lerpedSpeed, 0, accelerationSpeed * 5 * Time.deltaTime);
        }

        Vector3 lookInput = transform.forward * entity.mob.input.z + transform.right * entity.mob.input.x;
        entity.mob.orientation.forward = lookInput;
        entity.mob.rb.AddForce(Vector3.down * gravity);

        entity.mob.rb.velocity = new Vector3(entity.mob.rb.velocity.x * friction, entity.mob.rb.velocity.y, entity.mob.rb.velocity.z * friction);

        transform.forward = Vector3.Lerp(transform.forward, lookInput, turnSpeed * Time.deltaTime);

        entity.mob.rb.AddForce(transform.forward * lerpedSpeed);
    }

    public Vector3 flatForwardOrientation()
    {
        return Vector3.Normalize(new Vector3(entity.mob.orientation.forward.x, 0, entity.mob.orientation.forward.z));
    }

    public Vector3 flatRightOrientation()
    {
        return Vector3.Normalize(new Vector3(entity.mob.orientation.right.x, 0, entity.mob.orientation.right.z));
    }
}
