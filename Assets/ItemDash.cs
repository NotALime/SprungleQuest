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
    public void Dash(Inventory inv)
    {
        Debug.Log(inv.owner.entity.baseEntity.gameName + " tried dashing");
        if (item.cooldown <= 0 && inv.owner.entity.mob.specialInput)
        {
            inv.owner.entity.mob.rb.useGravity = false;
            inv.owner.entity.mob.rb.velocity = Vector3.zero;


            dashInput = inv.owner.flatForwardOrientation() * inv.owner.entity.mob.input.z + inv.owner.flatRightOrientation() * inv.owner.entity.mob.input.x;
            dashInput.y = 0;
            rbApplied = inv.owner.entity.mob.rb;
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

    IEnumerator StopDash(Inventory inv)
    {
        yield return new WaitForSeconds(dashTime);
        inv.owner.entity.mob.rb.useGravity = true;
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
            Dash(inv);
        }

        if (inv.owner.entity.mob.target != null)
        {
            if (Vector2.Distance(inv.owner.entity.transform.position, inv.owner.entity.mob.target.transform.position) > 30)
            {
                if (detectedProjectile && Random.Range(0f, 1f) <= 0.01f * inv.owner.entity.mob.stats.level)
                {
                    inv.owner.entity.mob.input = Vector3.forward;
                    inv.owner.entity.mob.specialInput = true;
                    Dash(inv);
                    inv.owner.entity.mob.specialInput = false;
                }
            }
        }
        if (Random.Range(0f, 1f) <= 0.001f * inv.owner.entity.mob.stats.level)
        {
            inv.owner.entity.mob.input = Vector3.forward;
            inv.owner.entity.mob.specialInput = true;
            Dash(inv);
            inv.owner.entity.mob.specialInput = false;
        }
    }
}
