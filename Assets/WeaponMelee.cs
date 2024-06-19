using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class WeaponMelee : MonoBehaviour
{
    public Item item;
    public List<Attack> attackCombo;
    public int attackIndex;
    public BoxCollider damageCollider;
    public bool holdAttack;
    public bool canBlock;
    public AudioPlayer hitSound;
    public AudioPlayer parrySound;
    public float parryRadius = 4;
    public float parryCooldown = 0.5f;
    public Projectile projectile;

    public float swingRange;

    private bool attacking;
    private bool blocked;
    private bool previousSecondaryInput;
    private Vector3 thrustDir;
    private Vector3 rotDir = Vector3.zero;

    private void Start()
    {
        item = GetComponent<Item>();
    }
    bool charging;

    public void Attack(Inventory inv)
    {
        if (!inv.owner.entity.mob.secondaryInput)
        {
            inv.owner.entity.mob.secondaryInput = false;
            if (attackIndex < attackCombo.Count - 1 && item.cooldown < 0 && item.cooldown > -0.2)
            {
                attackIndex++;
            }
            else
            {
                attackIndex = 0;
            }

            attacking = true;
            rotDir.z = Random.Range(-90 - swingRange, -110 + swingRange);

            Attack currentAttack = attackCombo[attackIndex];

            inv.handAnimator.SetTrigger(currentAttack.animation);
            if (!currentAttack.charge || inv.owner.entity.mob.primaryInput == false)
            {
                ExecuteAttack(inv, currentAttack);
            }
            else
            {
                ChargeAttack(inv, currentAttack);
            }
        }
    }

    private void ExecuteAttack(Inventory inv, Attack attack)
    {
        inv.owner.entity.mob.primaryInput = false;

        attack.sound.PlaySound();
        item.cooldown = attack.attackCooldown / inv.owner.entity.mob.stats.attackSpeed;
        attack.attack.Invoke(inv);

        inv.handAnimator.speed = ((1 / attack.attackCooldown) * inv.owner.entity.mob.stats.attackSpeed);

        thrustDir = inv.owner.flatForwardOrientation();
        inv.owner.rig.armRight.shoulder.transform.rotation = inv.owner.entity.mob.orientation.rotation * Quaternion.Euler(rotDir);

        if (inv.owner.entity.player && attack.locks)
        {
            inv.owner.entity.currentIframe = attack.attackCooldown / inv.owner.entity.mob.stats.attackSpeed + 0.2f;
        }
        attack.currentChargeTime = 0;
    }

    float chargeMultiplier;
    private void ChargeAttack(Inventory inv, Attack attack)
    {
        charging = true;
        inv.handAnimator.speed = Mathf.Clamp01(1 - (attack.currentChargeTime / attack.chargeTime));
        attack.currentChargeTime += Time.deltaTime * inv.owner.entity.mob.stats.attackSpeed;
    }

    public void Idle(Inventory inv)
    {
        if (item.cooldown >= 0)
        {
            if (attacking)
            {
                HandleAttackIdle(inv);
            }
            else
            {
                RotateShoulder(inv, 10);
            }
        }
        else
        {
            ResetIdleState(inv);
        }

        if (blocked)
        {
            inv.owner.entity.mob.rb.AddForce(inv.owner.flatForwardOrientation() * -500);
        }

        previousSecondaryInput = inv.owner.entity.mob.secondaryInput;
    }

    private void HandleAttackIdle(Inventory inv)
    {
        var currentAttack = attackCombo[attackIndex];
        if (currentAttack.locks)
        {
            inv.owner.movementEnabled = false;
        }
        else
        {
            RotateShoulder(inv, 5);
        }

        inv.owner.entity.mob.rb.AddForce(thrustDir * currentAttack.thrust);
        DamageCheck(inv);
    }

    private void RotateShoulder(Inventory inv, float speed)
    {
        inv.owner.rig.armRight.shoulder.transform.rotation = Quaternion.SlerpUnclamped(inv.owner.rig.armRight.shoulder.transform.rotation, inv.owner.entity.mob.orientation.rotation * Quaternion.Euler(rotDir), speed * Time.deltaTime);
    }

    private void ResetIdleState(Inventory inv)
    {
        inv.handAnimator.SetBool("Block", inv.owner.entity.mob.secondaryInput && canBlock && !attacking);
        blocked = false;
        charging = false;
       // inv.handAnimator.speed = 1;
        inv.owner.movementEnabled = true;
        attacking = false;
        damageCollider.enabled = false;
        inv.owner.rig.armRight.shoulder.transform.localRotation = Quaternion.SlerpUnclamped(inv.owner.rig.armRight.shoulder.transform.localRotation, inv.owner.rig.armRight.shoulder.initialRot, 5 * Time.deltaTime);
    }

    public void Parry(Inventory inv)
    {
        if (!previousSecondaryInput && inv.owner.entity.mob.secondaryInput)
        {
            attacking = false;
            if (TryDeflectProjectile(inv))
            {
                item.cooldown = parryCooldown;
                inv.owner.entity.mob.secondaryInput = false;
                parrySound.PlaySound();
                Debug.Log("DEFLECTED!");
            }
        }
    }

    private bool TryDeflectProjectile(Inventory inv)
    {
        Collider[] projectileCheck = Physics.OverlapSphere(inv.owner.entity.mob.orientation.position + inv.owner.entity.mob.orientation.forward, parryRadius);
        foreach (Collider col in projectileCheck)
        {
            if (col.GetComponent<Projectile>() is Projectile proj)
            {
                proj.origin = inv.owner.entity;
                proj.rb.velocity = inv.owner.entity.mob.orientation.forward * proj.rb.velocity.magnitude;
                return true;
            }
        }
        return false;
    }

    public void Block(Inventory inv)
    {
        if (item.cooldown <= 0 && !attacking)
        {
            rotDir.z = -90;
            RotateShoulder(inv, 20);
            inv.owner.entity.currentIframe = 0.05f;

            if (inv.owner.entity.baseEntity.tookDamage && item.cooldown <= 0)
            {
                inv.owner.entity.baseEntity.tookDamage = false;
                if (!blocked)
                {
                    ExecuteBlock(inv);
                }
            }
        }
    }

    private void ExecuteBlock(Inventory inv)
    {
        rotDir.z = -70;
        inv.owner.entity.currentIframe = parryCooldown;
        item.cooldown = parryCooldown;
        Entity.Stun(inv.owner.entity, parryCooldown);
        inv.owner.entity.mob.secondaryInput = false;
        blocked = true;
        parrySound.PlaySound();
    }

    public void DamageCheck(Inventory inv)
    {
        float sizeMultiplier = 1 + System.Convert.ToInt16(inv.owner.entity.player);
        Collider[] damageCheck = Physics.OverlapBox(damageCollider.transform.position, damageCollider.size * sizeMultiplier, damageCollider.transform.rotation);
        foreach (Collider col in damageCheck)
        {
            if (!attacking) break;

            Entity hitEntity = null;
            if (col.GetComponent<Entity>())
            {
                hitEntity = col.GetComponent<Entity>();
            }
            else if (col.GetComponent<EntityLimb>())
            {
                hitEntity = col.GetComponent<EntityLimb>().entity;
            }
            if (hitEntity != null && Entity.CompareTeams(inv.owner.entity, hitEntity))
            {
                Vector3 dir = inv.owner.flatForwardOrientation();
                if (hitEntity.TakeDamage(attackCombo[attackIndex].damage, inv.owner.entity))
                {
                    hitEntity.TakeDamage(attackCombo[attackIndex].damage * inv.owner.entity.mob.stats.damage, inv.owner.entity);
                    hitSound.PlaySound();
                }
                hitEntity.mob.rb.AddForce(dir * attackCombo[attackIndex].knockback);
            }
        }
    }

    public void FireProjectile(Inventory inv)
    {
        inv.owner.entity.SpawnProjectile(projectile, inv.owner.entity.mob.orientation.position, inv.owner.entity.mob.orientation.rotation);
    }

    public void MeleeAI(Inventory ai)
    {
        var target = ai.owner.entity.mob.target;
        if (target == null) return;

        ai.owner.entity.mob.orientation.LookAt(target.mob.orientation);
        float distance = Vector2.Distance(ai.transform.position, target.transform.position);

        if (distance <= 1f)
        {
            HandleCloseCombat(ai);
        }
        else if (distance <= 5)
        {
            HandleMidRangeCombat(ai);
        }
        else if (distance > 10 * ai.owner.entity.mob.scale)
        {
            HandleLongRangeCombat(ai);
        }
        else
        {
            ai.owner.entity.mob.input.z = 1f;
        }
    }

    private void HandleCloseCombat(Inventory ai)
    {
        if (ai.owner.entity.mob.input.z > 0)
        {
            ai.owner.entity.mob.primaryInput = true;

            if (EvoUtils.PercentChance(0.2f * ai.owner.entity.mob.stats.level, true))
            {
                ai.HoldItem(ai.items[Random.Range(0, ai.items.Length)]);
            }
        }

        if (EvoUtils.PercentChance(0.5f, true))
        {
            ai.owner.entity.mob.input.z = -1;
        }
    }

    private void HandleMidRangeCombat(Inventory ai)
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
                ai.owner.entity.mob.input.x = EvoUtils.PercentChance(0.5f, false) ? -0.5f : 0.5f;
            }

            if (EvoUtils.PercentChance(0.1f * (1 + ai.owner.entity.mob.stats.level), true))
            {
                ai.owner.entity.mob.input = Vector3.forward;
                ai.owner.entity.mob.secondaryInput = false;
            }
        }
    }

    private void HandleLongRangeCombat(Inventory ai)
    {
        ai.owner.entity.mob.primaryInput = false;
        if (ai.owner.entity.mob.input.x == 0)
        {
            ai.owner.entity.mob.input.x = EvoUtils.PercentChance(0.5f, false) ? -0.5f : 0.5f;
        }
        ai.owner.entity.mob.input.z = 0.5f;
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
    public bool charge;
    public float chargeTime;
    public float currentChargeTime;
    public AudioPlayer sound;
    public UnityEvent<Inventory> attack;
}