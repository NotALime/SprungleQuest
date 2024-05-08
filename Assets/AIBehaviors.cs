using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu]
public class AIBehaviors : ScriptableObject
{
    public Vector3 MoveForward(Entity ai)
    {
        return new Vector3(0, 0, 1);
    }
    public void RandomMove(Entity ai)
    {
        ai.mob.orientation.rotation = Quaternion.Euler(ai.mob.input);

        if (Random.Range(0f, 1f) < 0.1f)
        {
            ai.mob.input = Random.insideUnitSphere.normalized;
        }
    }
    float verticalRotation = 0;
    float horizontalRotation = 0;

    public void PlayerInput(Entity ai)
    {
        float mouseX = Input.GetAxis("Mouse X") * GameSettings.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * GameSettings.mouseSensitivity * Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            ai.mob.primaryInput = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ai.mob.primaryInput = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ai.mob.specialInput = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ai.mob.specialInput = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            ai.mob.secondaryInput = true;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            ai.mob.secondaryInput = false;
        }

        ai.mob.interactInput = Input.GetKeyDown(KeyCode.E);

        verticalRotation -= mouseY;
        horizontalRotation += mouseX;
        verticalRotation = Mathf.Clamp(verticalRotation, -89, 89);
       
    //    Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        ai.mob.orientation.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);

        ai.mob.input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        ai.mob.input.y = System.Convert.ToInt16(Input.GetKey(KeyCode.Space));
    }

    public void BasicFollowTarget(Entity ai)
    {
     //  if (ai.mob.target == null)
     //  {
     //      RandomMove(ai);
     //      if(ai.GetClosestTarget() != null)
     //      ai.mob.target = ai.GetClosestTarget();
     //  }
     //  else
     //  {
     //      Vector3 dir = (ai.mob.target.transform.position - ai.transform.position).normalized;
     //      ai.mob.orientation.LookAt(ai.mob.target.mob.orientation);
     //      ai.mob.input = (ai.mob.orientation.forward);
     //  }
    }

    
}
