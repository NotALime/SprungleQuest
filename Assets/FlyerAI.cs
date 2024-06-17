using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerAI : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public float maxVelocity = 10f; // Maximum velocity limit
    public float rotationSpeed = 5f; // Speed of rotation

    public float offsetRange;
    public float offsetInterval;
    private Vector3 offset;

    private Rigidbody rb;
    Entity entity;

    [Header("Combat")]
    public float viewDistance = 10;
    public float damage = 20;
    public float dashSpeed = 400;
    public float dashTime = 0.2f;
    public float chargeUp = 0.5f;
    public AudioPlayer attackSound;
    public float attackRange = 1;
    public float cooldown;
    bool canAttack = true;
    bool attacking;

    public Transform attackPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        entity = GetComponent<Entity>();
        StartCoroutine(Interval());
    }

    private void FixedUpdate()
    {
        if (!attacking && entity.mob.target != null)
        {
            Vector3 directionToTarget = EvoUtils.GetDir(transform.position, entity.mob.target.transform.position + offset);

            // Calculate the rotation towards the target object
            Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
            rb.rotation = Quaternion.Slerp(rb.rotation, rotationToTarget, rotationSpeed * Time.fixedDeltaTime);

            rb.AddForce(transform.forward * moveSpeed);

            // Limit velocity if it exceeds the maximum
            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }

            if (Vector2.Distance(transform.position, entity.mob.target.transform.position) <= viewDistance)
            {
                StartCoroutine(Attack());
            }
        }


        if (attacking)
        {
            Collider[] damagedStuff = Physics.OverlapSphere(attackPoint.position, attackRange);
            foreach (Collider e in damagedStuff)
            {
                if (e.GetComponent<Entity>())
                {
                    if (Entity.CompareTeams(entity, e.GetComponent<Entity>()))
                    {
                        e.GetComponent<Entity>().TakeDamage(damage * entity.mob.stats.damage, entity);
                    }
                }
            }
        }
    }

    IEnumerator Attack()
    {
        if (canAttack)
        {
            canAttack = false;
            rb.velocity = Vector3.zero;
            yield return new WaitForSeconds(chargeUp);
            rb.AddForce(transform.forward * dashSpeed);
            attacking = true;
            if (attackSound != null)
            {
                attackSound.PlaySound();
            }
            yield return new WaitForSeconds(dashTime);
            attacking = false;
            yield return new WaitForSeconds(cooldown / entity.mob.stats.attackSpeed);
            canAttack = true;
        }
    }

IEnumerator Interval()
    {
        offset = new Vector3(Random.Range(-offsetRange, offsetRange), Random.Range(-offsetRange, offsetRange), Random.Range(-offsetRange, offsetRange));
        yield return new WaitForSeconds(offsetInterval);
        StartCoroutine(Interval());
    }
}