using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ItemDash : MonoBehaviour
{
    Item item;
    public float dashSpeed;
    public float dashTime;

    public float dashCooldown = 2f;

    bool dashing = false;
    Vector3 dashInput;

    Rigidbody rbApplied;
    private void Awake()
    {
        item = GetComponent<Item>();

        item.itemDescription += "\n" + dashSpeed.ToString() + " speed dash over " + dashTime.ToString() + " seconds with " + dashCooldown.ToString() + " cooldown";
    }
    public void Dash(Entity inv)
    {
        Debug.Log(inv.baseEntity.gameName + " tried dashing");
        if (item.cooldown <= 0 && inv.mob.specialInput)
        {
            inv.mob.rb.useGravity = false;
         //   inv.mob.rb.velocity = Vector3.zero;


            dashInput = inv.mob.input.z * inv.mob.orientation.forward + inv.mob.input.x * inv.mob.orientation.right;
            dashInput.y = 0;
            rbApplied = inv.mob.rb;
            dashing = true;

            item.cooldown = dashCooldown;
            StartCoroutine(StopDash(inv));
        }
    }

    public void FixedUpdate()
    {
        if (dashing)
        {
            rbApplied.velocity = dashInput * dashSpeed;
        }
    }

    IEnumerator StopDash(Entity inv)
    {
        yield return new WaitForSeconds(dashTime);
        inv.mob.rb.useGravity = true;
        dashing = false;
    }

    public void AIDodge(Inventory inv)
    {
        Collider[] projectileCheck = Physics.OverlapSphere(inv.transform.position, inv.owner.entity.mob.stats.visionRange * 0.05f);
        bool detectedProjectile = false;

        foreach (Collider c in projectileCheck)
        {
            if (c.GetComponent<Projectile>() && Entity.CompareTeams(inv.owner.entity, c.GetComponent<Projectile>().origin))
            {
                detectedProjectile = true;
            }
        }

        if (detectedProjectile && Random.Range(0f, 1f) <= 0.05f * inv.owner.entity.mob.stats.level)
        {
            inv.owner.entity.mob.input = Random.insideUnitSphere.normalized;
            Dash(inv.owner.entity);
        }

        if (inv.owner.entity.mob.target != null)
        {
            if (Vector2.Distance(inv.owner.entity.transform.position, inv.owner.entity.mob.target.transform.position) > 30)
            {
                if (detectedProjectile && Random.Range(0f, 1f) <= 0.01f * inv.owner.entity.mob.stats.level)
                {
                    inv.owner.entity.mob.input = Vector3.forward;
                    inv.owner.entity.mob.specialInput = true;
                    Dash(inv.owner.entity);
                    inv.owner.entity.mob.specialInput = false;
                }
            }
        }
        if (Random.Range(0f, 1f) <= 0.001f * inv.owner.entity.mob.stats.level)
        {
            inv.owner.entity.mob.input = Vector3.forward;
            inv.owner.entity.mob.specialInput = true;
            Dash(inv.owner.entity);
            inv.owner.entity.mob.specialInput = false;
        }
    }
}
