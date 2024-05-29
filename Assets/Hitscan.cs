using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitscan : MonoBehaviour
{
    public LineRenderer line;
    public float range;
    public float damage;
    public int extraJoints;
    public float extraJointNoiseSpeed;
    public float extraJointNoiseMagnitude;

    private void Start()
    {
    //    line.positionCount = extraJoints + 1;
    }
    public void HitscanCast(Entity ai)
    {
        RaycastHit hit;
        line.enabled = true;
        line.SetPosition(0, Vector3.LerpUnclamped(line.GetPosition(0), transform.position, 50 * Time.deltaTime));
        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            if (hit.transform.gameObject.GetComponent<EntityLimb>())
            {
                if(Entity.CompareTeams(ai, hit.transform.gameObject.GetComponent<EntityLimb>().entity))
                hit.transform.gameObject.GetComponent<EntityLimb>().entity.TakeDamage(damage * hit.transform.gameObject.GetComponent<EntityLimb>().damageMultiplier, ai);
            }
            else if (hit.transform.gameObject.GetComponent<Entity>())
            {
                if (Entity.CompareTeams(ai, hit.transform.gameObject.GetComponent<Entity>()))
                    hit.transform.gameObject.GetComponent<Entity>().TakeDamage(damage, ai);
            }
            line.SetPosition(1, transform.position + transform.forward * hit.distance);
        }
        line.SetPosition(1, transform.position + transform.forward * range);

     //   Invoke("DisableRay", 0.1f);
    }

    public void DisableRay()
    {
        line.enabled = false;
    }
}
