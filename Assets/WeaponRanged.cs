using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRanged : MonoBehaviour
{
    public Item item;

    public Projectile projectile;

    public float projectilesPerShot = 1;

    public float spread;

    public float cooldown = 0.5f;

    public Transform shootPoint;

    public float recoil;

    public AudioPlayer castSound;

    [Header("Overheat")]
    public int shotsToBig;
    public int overHeatLevel;
    int shotsFired;
    private void Start()
    {
        item = GetComponent<Item>();
    }
    public void Attack(Inventory inv)
    {
        shotsFired++;

        Projectile proj = null;
        inv.handAnimator.SetBool("Active", true);

        if (rotatedToAttack)
        {
            for (int i = 0; i < projectilesPerShot; i++)
            {
                proj = inv.owner.entity.SpawnProjectile(projectile, shootPoint.position, shootPoint.rotation * Quaternion.Euler(new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread))));
                proj.ApplyLevel(proj.level.level);
                if (shotsFired == shotsToBig)
                {
                    proj.ApplyLevel(overHeatLevel);
                    shotsFired = 0;
                }
            }
            if (shotsToBig > 0)
            {
                castSound.PlaySound(1 + (shotsFired / Mathf.Clamp(shotsToBig, 1, Mathf.Infinity)));
            }
            else
            {
                castSound.PlaySound();
            }
            item.cooldown = (cooldown * proj.cooldown) / inv.owner.entity.mob.stats.attackSpeed;
        }
    }

    [Header("Player Specific")]
    public float aimFOV;
    public void Aim(Inventory inv)
    { 
           // inv.hand.connectedBody.angularVelocity *= 0.5f;

            if (inv.owner.entity.player)
            {
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, aimFOV, 10 * Time.deltaTime);
            }
    }

    bool rotatedToAttack;
    public void ArmAnim(Inventory inv)
    {
        inv.handAnimator.SetBool("Active", (inv.owner.entity.mob.primaryInput || inv.owner.entity.mob.secondaryInput));
        if (inv.owner.entity.mob.primaryInput || inv.owner.entity.mob.secondaryInput)
        {
            inv.owner.rig.armRight.shoulder.transform.rotation = inv.owner.rig.spine.neck.transform.rotation * Quaternion.Euler(new Vector3(0, 0, -83.622f));
            rotatedToAttack = true;
        }
        else
        {
            inv.owner.rig.armRight.shoulder.transform.localRotation = Quaternion.Slerp(inv.owner.rig.armRight.shoulder.transform.localRotation, inv.owner.rig.armRight.shoulder.initialRot, 10 * Time.deltaTime);
            rotatedToAttack = false;
        }
    }

    public void AIShootWhenSaw(Inventory e)
    {
            if (e.owner.entity.mob.target != null)
            {
                e.owner.entity.mob.orientation.LookAt(e.owner.entity.mob.target.mob.orientation);
                if (Vector2.Distance(e.transform.position, e.owner.entity.mob.target.transform.position) <= e.owner.entity.mob.stats.visionRange * 0.25f)
                {
                    e.owner.entity.mob.input.z = -1;
                    e.owner.entity.mob.input = (e.owner.entity.mob.orientation.right * Random.Range(-1, 1));
                }

                if (Vector2.Distance(e.transform.position, e.owner.entity.mob.target.transform.position) > e.owner.entity.mob.stats.visionRange * 1.2f)
                {
                    e.owner.entity.mob.input.z = 1;
                }
                if (Random.Range(0f, 1f) < 0.01f)
                {
                    if (Random.Range(0f, 1f) <= 0.5f)
                    {
                        e.owner.entity.mob.input.x = 1;
                    }
                    else
                    {
                        e.owner.entity.mob.input.x = -1;
                    }
                }
                if (Random.Range(0f, 1f) < 0.05f)
                {
                    e.owner.entity.mob.input.y = 1;
                }
                else
                {
                    e.owner.entity.mob.input.y = 0;
                }

            RaycastHit hit = new RaycastHit();

            if ((Physics.SphereCast(e.owner.entity.mob.orientation.position, 5, e.owner.entity.mob.orientation.forward, out hit, e.owner.entity.mob.stats.visionRange)))
            {
                if (EvoUtils.PercentChance(0.3f * e.owner.entity.mob.stats.level, true))
                {
                    e.owner.entity.mob.primaryInput = true;
                }
            }
            else
            {
                e.owner.entity.mob.primaryInput = false;
            }
        }
    }
}
