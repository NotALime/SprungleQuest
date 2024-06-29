using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRanged : MonoBehaviour
{
    public Item item;

    public Projectile projectile;
    public Hitscan hitscan;
    public int hitscanFrames;
    int currentHitscanFrames;

    public float projectilesPerShot = 1;

    public float spread;

    public float cooldown = 0.5f;

    public Transform shootPoint;

    public float recoil;

    public AudioPlayer castSound;

    public bool automatic = true;


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

        if (projectile != null)
        {
            Projectile proj = null;
            for (int i = 0; i < projectilesPerShot; i++)
            {
                //Quaternion.Euler(new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread),Random.Range(-spread, spread)))
                proj = inv.owner.entity.SpawnProjectile(projectile, inv.owner.entity.mob.orientation.position, inv.owner.entity.mob.orientation.rotation);
              // proj.ApplyLevel(proj.level.level);
              // if (shotsFired == shotsToBig)
              // {
              //     proj.ApplyLevel(overHeatLevel);
              //     shotsFired = 0;
              // }
            }

            inv.owner.entity.mob.primaryInput = automatic;
            item.cooldown = cooldown / inv.owner.entity.mob.stats.attackSpeed;
        }
        else if (hitscan != null)
        {
            hitscan.HitscanCast(inv.owner.entity);
            if (castSound != null)
            {
                castSound.PlaySound();
            }
            currentHitscanFrames++;

            if (currentHitscanFrames >= hitscanFrames && hitscanFrames > 0)
            {
                currentHitscanFrames = 0;
                hitscan.line.enabled = false;
                hitscan.sound.enabled = false;
                inv.owner.entity.mob.primaryInput = automatic;
                item.cooldown = cooldown / inv.owner.entity.mob.stats.attackSpeed;
            }
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
    public void ArmAnim(Inventory inv)
    {
        inv.handAnimator.SetBool("Active", true);
        inv.owner.rig.armRight.shoulder.transform.rotation = inv.owner.rig.spine.neck.transform.rotation * Quaternion.Euler(new Vector3(0, 0, -83.622f));
    }

    public void AIShootWhenSaw(Inventory e)
    {
        if (e.owner.entity.mob.target != null)
        {
            e.owner.entity.mob.input.y = 0;
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
            RaycastHit hit = new RaycastHit();

            if ((Physics.SphereCast(e.owner.entity.mob.orientation.position, 5, e.owner.entity.mob.orientation.forward, out hit, e.owner.entity.mob.stats.visionRange)))
            {
                e.owner.entity.mob.primaryInput = true;
            }

            if (EvoUtils.PercentChance(0.1f, true))
            {
                e.owner.entity.mob.primaryInput = false;
                e.HoldItem(e.items[Random.Range(0, e.items.Length)]);
            }
        }
        else
        {
            e.owner.entity.mob.primaryInput = false;
        }
    }

    public void AISingleShot(Inventory e)
    {
        if (e.owner.entity.mob.target != null)
        {
            e.owner.entity.mob.input = Vector3.zero;
            e.owner.entity.mob.orientation.rotation = Quaternion.LookRotation(Vector3.down);
            e.owner.entity.mob.primaryInput = true;
        }
    }
}
