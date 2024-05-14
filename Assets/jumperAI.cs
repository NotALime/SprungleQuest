using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumperAI : MonoBehaviour
{
    private Rigidbody rb;
    Entity entity;

    bool attacking;
    bool canAttack = true;

    public float offsetRange;
    private Vector3 offset;
    public float cooldownRandomizationRange = 0.2f;

    public AudioPlayer jumpSound;
    public AudioPlayer hitSound;

    public float jumpUp;
    public float jumpForward;
    public float rotateSpeed;
    public float viewDistance = 10;
    //attackStuff
    public float damage = 20;
    public float attackRange = 2;

    public float gravity = 30;

    //Timestuff

    //time to start attack
    public float chargeUp = 0.5f;
    //time between attacks
    public float cooldown = 1;
    //Projectile to spawn
    public Projectile landProjectile;
    //Projectile amount;
    public int projectileAmount;
    //projectileRecoil
    public float projectileSpread = 180;

    public LayerMask ground;

    public Transform groundCheck;

    public Transform visual;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        entity = GetComponent<Entity>();
        StartCoroutine(Attack());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * gravity);
        if (!attacking)
        {
            entity.AI();
        }

        if (attacking)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, entity.mob.orientation.rotation.y, 0));

            Collider[] damagedStuff = Physics.OverlapSphere(transform.position, attackRange);
            foreach (Collider e in damagedStuff)
            {
                if (e.GetComponent<Entity>())
                {
                    if (Entity.CompareTeams(entity, e.GetComponent<Entity>()))
                    {
                        e.GetComponent<Entity>().TakeDamage(damage, entity);
                        if (hitSound != null)
                        {
                            hitSound.PlaySound();
                        }
                    }
                }
            }
        }
    }

    public void EntityAI(Entity ai)
    {
        if (ai.mob.target != null)
        {
            Vector3 dir = EvoUtils.GetDir(transform.position + Random.insideUnitSphere * offsetRange, ai.mob.target.transform.position);
            dir.y = 0;

            //   Quaternion targetRotation = Quaternion.LookRotation(ai.mob.input, Vector3.forward);

            ai.mob.orientation.rotation = Quaternion.LookRotation(dir);
            //ai.mob.orientation.LookAt(ai.mob.target.transform);
            //Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            ai.mob.input = Vector3.forward;
        }
        else
        {
            Entity.PatrolAI(ai);
        }

    }

    IEnumerator Attack()
    {
        if (canAttack && IsGrounded())
        {
            canAttack = false;
            yield return new WaitForSeconds(chargeUp / entity.mob.stats.attackSpeed);
            attacking = true;
            if (jumpSound != null)
            {
                jumpSound.PlaySound();
            }
            rb.AddForce(Vector3.up * jumpUp * Random.Range(0.8f, 1.2f));

            Vector3 forwardForceInput = entity.mob.orientation.forward * entity.mob.input.z + entity.mob.orientation.right * entity.mob.input.x;

            rb.AddForce(forwardForceInput * jumpForward * entity.mob.stats.moveSpeed * Random.Range(0.8f, 1.2f));
            yield return new WaitUntil(() => IsGrounded());
            for (int i = 0; i < projectileAmount; i++)
            {
                entity.SpawnProjectile(landProjectile, transform.position, Quaternion.Euler(new Vector3(Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread))));
            }
            yield return new WaitForSeconds(0.1f);
            attacking = false;
            canAttack = true;
        }
        yield return new WaitForSeconds((cooldown * (1 - Random.Range(-cooldownRandomizationRange, cooldownRandomizationRange) * entity.mob.stats.level)) / entity.mob.stats.attackSpeed);
        StartCoroutine(Attack());
    }

    public bool IsGrounded()
    { 
        return Physics.CheckSphere(groundCheck.position, 1f * transform.localScale.y, ground);
    }
}
