using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using JetBrains.Annotations;
using System.Security.Principal;
using System.Collections.Concurrent;

public class WeaponMelee : MonoBehaviour
{
    public Item item;
    public List<Attack> attackCombo;
    public int attackIndex;

    public BoxCollider damageCollider;

    public bool holdAttack;

    public AudioPlayer hitSound;
    public AudioPlayer parrySound;

    private void Start()
    {
        item = GetComponent<Item>();    
    }
    public void Attack(Inventory inv)
    {
        inv.owner.entity.mob.secondaryInput = false;
        if (!holdAttack)
        {
            inv.owner.entity.mob.primaryInput = false;
        }
        if (attackIndex < attackCombo.Count - 1 && item.cooldown < 0 && item.cooldown > -0.2)
        {
            attackIndex++;
        }
        else
        {
            attackIndex = 0;
        }

        rotDir.z = Random.Range(-50, -130);

        inv.handAnimator.speed = (1 / attackCombo[attackIndex].attackCooldown) * inv.owner.entity.mob.stats.attackSpeed;
        inv.handAnimator.SetTrigger(attackCombo[attackIndex].animation);

        attackCombo[attackIndex].sound.PlaySound();

        item.cooldown = (attackCombo[attackIndex].attackCooldown) / inv.owner.entity.mob.stats.attackSpeed;
        attackCombo[attackIndex].attack.Invoke(inv);

        thrustDir = inv.owner.flatForwardOrientation();

        inv.owner.rig.armRight.shoulder.transform.rotation = inv.owner.entity.mob.orientation.rotation * Quaternion.Euler(rotDir);

        if (inv.owner.entity.player && attackCombo[attackIndex].locks)
        {
            inv.owner.entity.currentIframe = (attackCombo[attackIndex].attackCooldown) / inv.owner.entity.mob.stats.attackSpeed + 0.2f;
        }
        attacking = true;
    }
    [HideInInspector]
    public bool attacking;
    Vector3 thrustDir;

    Vector3 rotDir = Vector3.zero;

    bool previousSecondaryInput;
    public void Idle(Inventory inv)
    {
        if (item.cooldown >= 0)
        {
            if (attacking)
            {
                if (attackCombo[attackIndex].locks)
                {
                    inv.owner.movementEnabled = false;
                }
                else
                {
                    inv.owner.rig.armRight.shoulder.transform.rotation = Quaternion.SlerpUnclamped(inv.owner.rig.armRight.shoulder.transform.rotation, inv.owner.entity.mob.orientation.rotation * Quaternion.Euler(rotDir), 5 * Time.deltaTime);
                }
                inv.owner.entity.mob.rb.AddForce(thrustDir * attackCombo[attackIndex].thrust);
                DamageCheck(inv);
            }
            else
            {
                inv.owner.rig.armRight.shoulder.transform.rotation = Quaternion.SlerpUnclamped(inv.owner.rig.armRight.shoulder.transform.rotation, inv.owner.entity.mob.orientation.rotation * Quaternion.Euler(rotDir), 10 * Time.deltaTime);
            }
        }
        else
        {
            blocked = false;
            inv.handAnimator.speed = 1;
            inv.owner.movementEnabled = true;
            attacking = false;
            damageCollider.enabled = false;
            if (inv.owner.entity.mob.secondaryInput)
            {
                inv.owner.rig.armRight.shoulder.transform.rotation = Quaternion.SlerpUnclamped(inv.owner.rig.armRight.shoulder.transform.rotation, inv.owner.entity.mob.orientation.rotation * Quaternion.Euler(rotDir), 10 * Time.deltaTime);
            }
            else
            {
                inv.owner.rig.armRight.shoulder.transform.localRotation = Quaternion.SlerpUnclamped(inv.owner.rig.armRight.shoulder.transform.localRotation, inv.owner.rig.armRight.shoulder.initialRot, 5 * Time.deltaTime);
            }
        }
        if(blocked)
        {
            inv.owner.entity.mob.rb.AddForce(inv.owner.flatForwardOrientation() * -500);
        }

        inv.handAnimator.SetBool("Block", inv.owner.entity.mob.secondaryInput || blocked && !attacking);

        previousFrameSecondInput = inv.owner.entity.mob.secondaryInput;
    }

    public float parryRadius = 4;
    public float parryCooldown = 0.5f;

    bool blocked;
    bool previousFrameSecondInput;

    public void Parry(Inventory inv)
    {
        if (previousFrameSecondInput == false && inv.owner.entity.mob.secondaryInput == true)
        {
            attacking = false;
            Collider[] projectileCheck = Physics.OverlapSphere(inv.owner.entity.mob.orientation.position + inv.owner.entity.mob.orientation.forward, parryRadius);

            bool detectedProjectile = false;

            foreach (Collider col in projectileCheck)
            {
                if (col.GetComponent<Projectile>())
                {
                    Projectile proj = col.GetComponent<Projectile>();

                    proj.origin = inv.owner.entity;
                    proj.rb.velocity = inv.owner.entity.mob.orientation.forward * proj.rb.velocity.magnitude;

                    detectedProjectile = true;
                }
            }

            if (detectedProjectile)
            {
                item.cooldown = parryCooldown;
                inv.owner.entity.mob.secondaryInput = false;
                parrySound.PlaySound();
                Debug.Log("DEFLECTED!");
            }
        }
    }

    public void Block(Inventory inv)
    {
        rotDir.z = -90;
        inv.owner.entity.currentIframe = 0.05f;
        if (inv.owner.entity.baseEntity.tookDamage == true && item.cooldown <= 0)
        {
            inv.owner.entity.baseEntity.tookDamage = false;
            if (!blocked)
            {
                rotDir.z = -70;
                inv.owner.entity.currentIframe = parryCooldown;

                Entity.Stun(inv.owner.entity, parryCooldown);
                item.cooldown = parryCooldown;
                inv.owner.entity.mob.secondaryInput = false;
                blocked = true;
                parrySound.PlaySound();
            }
        }
    }

    public void DamageCheck(Inventory inv)
    {
        Collider[] damageCheck = Physics.OverlapBox(damageCollider.transform.position, damageCollider.size * 1.5f, damageCollider.transform.rotation);

        foreach (Collider col in damageCheck)
        {
            if (!attacking)
            {
                break;
            }
            if (col.GetComponent<Entity>())
            {
                Entity hitEntity = col.GetComponent<Entity>();
                if (Entity.CompareTeams(inv.owner.entity, hitEntity))
                {
                    Vector3 dir = inv.owner.flatForwardOrientation();
                    hitEntity.mob.rb.AddForce(dir * attackCombo[attackIndex].knockback);
                    if (hitEntity.TakeDamage(attackCombo[attackIndex].damage, inv.owner.entity))
                    {
                        hitEntity.TakeDamage(attackCombo[attackIndex].damage * inv.owner.entity.mob.stats.damage, inv.owner.entity);
                        hitSound.PlaySound();
                        if (!hitEntity.player)
                        {
                        //    StartCoroutine(Entity.Stun(hitEntity, attackCombo[attackIndex].stunTime));
                        }
                    }
                }
            }
        }
    }
    // private void OnDrawGizmos()
    // {
    //         Gizmos.DrawWireCube(damageCollider.transform.position, damageCollider.size, damageCollider.transform.rotation);
    // }

    [Header("Projectile")]
    public Projectile projectile;
    public void FireProjectile(Inventory inv)
    {
            inv.owner.entity.SpawnProjectile(projectile, inv.owner.entity.mob.orientation.position, inv.owner.entity.mob.orientation.rotation);
    }

    public void MeleeAI(Inventory ai)
    {
        if (ai.owner.entity.mob.target == null)
        {
            if (ai.owner.entity.GetClosestTarget() != null)
                ai.owner.entity.mob.target = ai.owner.entity.GetClosestTarget();
                ai.owner.entity.mob.input = Vector3.forward;
        }
        else
        {
            ai.owner.entity.mob.orientation.LookAt(ai.owner.entity.mob.target.mob.orientation);

            if (Vector2.Distance(ai.transform.position, ai.owner.entity.mob.target.transform.position) <= 1f)
            {
                if (ai.owner.entity.mob.input.z > 0)
                {
                    ai.owner.entity.mob.primaryInput = true;
                }
                if (EvoUtils.PercentChance(0.5f, true))
                {
                    ai.owner.entity.mob.input.z = -1;
                }
            }
            else if (Vector2.Distance(ai.transform.position, ai.owner.entity.mob.target.transform.position) <= 5)
            {
                ai.owner.entity.mob.primaryInput = false;
                if (ai.owner.entity.mob.input.x != 0)
                {
                    ai.owner.entity.mob.input.z = 0;
                    if (EvoUtils.PercentChance(0.3f, true))
                    {
                        if (EvoUtils.PercentChance(0.5f * ai.owner.entity.mob.stats.level, false))
                        {
                            ai.owner.entity.mob.secondaryInput = true;
                        }
                        if (EvoUtils.PercentChance(0.5f, false))
                        {
                            ai.owner.entity.mob.input.x = -0.5f;
                        }
                        else
                        {
                            ai.owner.entity.mob.input.x = 0.5f;
                        }
                    }
                    if (EvoUtils.PercentChance(0.1f * (1 + ai.owner.entity.mob.stats.level), true))
                    {
                        ai.owner.entity.mob.input = Vector3.forward;
                        ai.owner.entity.mob.secondaryInput = false;
                    }
                }
            }
            else if (Vector2.Distance(ai.transform.position, ai.owner.entity.mob.target.transform.position) > 10 * ai.owner.entity.mob.scale)
            {
                ai.owner.entity.mob.primaryInput = false;
                if (ai.owner.entity.mob.input.x == 0)
                {
                    if (EvoUtils.PercentChance(0.5f, false))
                    {
                        ai.owner.entity.mob.input.x = -0.5f;
                    }
                    else
                    {
                        ai.owner.entity.mob.input.x = 0.5f;
                    }
                }
                ai.owner.entity.mob.input.z = 0.5f;
            }
            else
            {
                ai.owner.entity.mob.input.z = 1f;
            }
            //   }

            //  if (ai.owner.entity.mob.input.z != 0)
            //  {
            //      ai.owner.entity.mob.secondaryInput = false;
            //  }



        }
    }
}
[System.Serializable]
public class Attack
{
    public string animation;
    public float attackCooldown;
    public float thrust;

    public bool locks;

    public float damage;
    public float knockback;
    public float stunTime = 0.5f;

    public AudioPlayer sound;

    public UnityEvent<Inventory> attack;
}
