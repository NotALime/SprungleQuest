using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius = 5;
    public AnimationCurve damageCurve = AnimationCurve.Linear(0, 1, 1, 0);
    public float centerDamage = 100;
    public float knockback = 2000;

    public Entity origin;
    // Start is called before the first frame update
    void Start()
    {
        Collider[] check = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider hit in check)
        {
            if (hit.GetComponent<Entity>())
            {
                hit.GetComponent<Entity>().TakeDamage(centerDamage);
                Vector3 knockbackDir = -(hit.transform.position - transform.position);
                hit.GetComponent<Entity>().mob.rb.AddForce(knockbackDir * knockback);
            }
        }

        EvoUtils.DestroyObject(this, 10);
    }

}
