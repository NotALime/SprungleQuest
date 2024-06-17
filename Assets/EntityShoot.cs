using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityShoot : MonoBehaviour
{
    public Projectile projectile;

    public Transform shootPoint;

    public float cooldown;
    bool canAttack = true;
    public int amount;
    public float spread;
    public float warn;
    public GameObject warnParticle;

    public void Shoot(Entity ai)
    {
        if (ai.mob.target != null)
        {
            if (Vector2.Distance(transform.position, ai.mob.target.transform.position) <= ai.mob.stats.visionRange * 0.7f)
            {
                if (canAttack == true)
                {
                    StartCoroutine(Attack(ai));
                }
            }
        }
    }

    IEnumerator Attack(Entity ai)
    {
        canAttack = false;
        GameObject particle = Instantiate(warnParticle, shootPoint.transform.position, Quaternion.identity);
        particle.SetActive(true);

        EvoUtils.DestroyObject(particle, 10);
        yield return new WaitForSeconds(warn);
        for (int i = 0; i < amount; i++)
        {
            ai.SpawnProjectile(projectile, shootPoint.position, shootPoint.rotation);
        }
        yield return new WaitForSeconds(cooldown / ai.mob.stats.attackSpeed);
        canAttack = true;
    }
}
