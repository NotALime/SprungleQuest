using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class DragonAI : MonoBehaviour
{
    public Entity ai;
    public float speed;
    public float upSpeed;
    public float rotateSpeed;
    public float limbRotateSpeed = 20;
    public float limbSpeed;

    public Transform[] snake;
    public float segmentDistance = 2;

    public LayerMask ground;

    public float onCrashDamage;


    public Projectile projectile;
    public int projectileAmount = 20;
    public float projectileDelay = 0.05f;
    public Transform firePoint;
    public float BreathRange = 100;
    public float spread = 20;

    public Explosion landExplosion;
    public Projectile idleProjectile;

    public AudioPlayer sleepSound;
    public AudioPlayer flapSound;

    public Transform visual;

    Ray groundRay;
    public void Flap()
    {
        flapSound.PlaySound();
    }
    public void FixedUpdate()
    {
        ai.AI();
        ai.mob.rb.AddForce(ai.mob.orientation.forward * speed * ai.mob.input.z);
        ai.mob.rb.AddForce(Vector3.up * upSpeed * ai.mob.input.y);

        if (ai.mob.input.z == 0 && state != AIState.Grounded)
        {
            ai.mob.rb.velocity *= 0.95f;
            anim.SetBool("Hover", true);
        }
        else
        {
            anim.SetBool("Hover", false);
            ai.mob.rb.velocity *= 0.98f;
        }
        if (Physics.Raycast(transform.position, Vector3.down, 30, ground) && state != AIState.Grounded)
        {
            ai.mob.input.y = 1;
        }
        else 
        {
            ai.mob.input.y = 0;
        }
        //   for (int i = 1; i < snake.Length; i++)
        //   {
        //       // Calculate rotation based on the direction to the previous segment.
        //       Transform currentSegment = snake[i];
        //       Transform previousSegment = snake[i - 1];
        //
        //       Quaternion targetRotation = Quaternion.LookRotation(previousSegment.position - currentSegment.position, transform.up);
        //       currentSegment.rotation = Quaternion.Slerp(currentSegment.rotation, targetRotation, limbRotateSpeed * Time.deltaTime);
        //       currentSegment.position = Vector3.Lerp(currentSegment.position, previousSegment.position, limbSpeed);
        //   }
    }

    public GameObject[] treasuresToDestroy;
    public void DragonAILogic(Entity ai)
    {
        if (state == AIState.Idle)
        {
            if (ai.mob.target != null)
            {
                ai.mob.targetPoint = ai.mob.target.transform.position;

                if (EvoUtils.PercentChance(0.075f, true))
                    ai.SpawnProjectile(idleProjectile, firePoint.position, Quaternion.LookRotation(ai.mob.targetPoint - ai.mob.orientation.position));

                if (Vector2.Distance(ai.mob.target.transform.position, ai.transform.position) <= BreathRange && canAttack)
                {
                    StartCoroutine(Attack());
                }
            }
            else
            {
                if (treasuresToDestroy.Length > 0)
                {
                    foreach (GameObject obj in treasuresToDestroy)
                    {
                        if (obj == null)
                        {
                            if (ai.GetClosestTarget())
                            {
                                ai.mob.target = ai.GetClosestTarget();
                            }
                            break;
                        }
                    }
                }
                else
                {
                    if (ai.GetClosestTarget())
                    {
                        ai.mob.target = ai.GetClosestTarget();
                    }
                }
            }

            Vector3 dir = (ai.mob.targetPoint + Vector3.up * 50) - ai.mob.orientation.position;
            ai.mob.orientation.rotation = Quaternion.RotateTowards(ai.mob.orientation.rotation, Quaternion.LookRotation(dir), rotateSpeed * Time.deltaTime);
            ai.mob.input.z = 1;

            visual.forward = Vector3.LerpUnclamped(visual.forward, ai.mob.rb.velocity.normalized, rotateSpeed * Time.deltaTime);

        }
        else if (state == AIState.Attack)
        {
            Vector3 dir = ai.mob.target.transform.position - ai.mob.orientation.position;
            ai.mob.orientation.rotation = Quaternion.RotateTowards(ai.mob.orientation.rotation, Quaternion.LookRotation(dir), rotateSpeed * 3 * Time.deltaTime);
            visual.rotation = ai.mob.orientation.rotation;
        }
        else if (state == AIState.Grounded)
        {
            ai.mob.rb.velocity *= 0.7f;
            ai.mob.input = Vector3.zero;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (state != AIState.Grounded)
        {
          //  visual.up = -ai.mob.rb.velocity.normalized;
            ai.TakeDamage(onCrashDamage);
            Instantiate(landExplosion, transform.position, Quaternion.identity);
            StartCoroutine(Recover());
        }
    }

    enum AIState
    {
        Idle,
        Recover,
        Attack,
        Grounded
    };

    AIState state = AIState.Idle;

    public Animator anim;
    public IEnumerator Recover()
    {
        state = AIState.Grounded;
        ai.mob.input = Vector3.zero;
        anim.SetBool("Ground", true);
        yield return new WaitForSeconds(15);
        ai.mob.input = Vector3.up;
        ai.mob.rb.AddForce(Vector3.up * upSpeed * 30);
        state = AIState.Idle;
        anim.SetBool("Ground", false);
        yield return new WaitForSeconds(4);
        yield return new WaitUntil(() => !Physics.Raycast(transform.position, Vector3.down, 50, ground));
    }
    bool canAttack = true;
    public IEnumerator Attack()
    {
        state = AIState.Attack;
        ai.mob.input = Vector3.zero;
        canAttack = false;
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < projectileAmount; i++)
        {
            ai.SpawnProjectile(projectile, firePoint.position, firePoint.rotation * Quaternion.Euler(Random.insideUnitSphere * spread));
            yield return new WaitForSeconds(projectileDelay);
        }
        ai.SpawnProjectile(idleProjectile, firePoint.position, Quaternion.LookRotation(ai.mob.target.transform.position - ai.mob.orientation.position));
        state = AIState.Idle;
        yield return new WaitForSeconds(15);
        canAttack = true;
    }
}
