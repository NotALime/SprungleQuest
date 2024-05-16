using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    [Header("Base Entity Settings")]
    public BaseEntity baseEntity;

    public MobDrop[] drops;
    public Renderer[] renderer;

    public float iframe = 0.2f;
    [HideInInspector]
    public float currentIframe;
    bool dead;
    [Header("Mob Settings")]
    public Mob mob;

    [HideInInspector]
    public bool interior;

    public bool player;

    private void Awake()
    {
        if (baseEntity.deathEffect != null)
        {
            baseEntity.deathEffect.SetActive(false);
        }
    }

    private void Start()
    {
        baseEntity.maxHealth = baseEntity.health;
        UpdateHealth();

        if (mob.species != null)
        {
            mob.species.ApplySpecies(this);
        }
        transform.localScale *= mob.scale;
    }
    private void FixedUpdate()
    {
        currentIframe -= Time.deltaTime;
        if (currentIframe <= 0)
        {
            baseEntity.tookDamage = false;
        }

        if (mob.effects.onIdle != null)
        {
            mob.effects.onIdle.Invoke(this);
        }
    }

    public void AI()
    {
        if (mob.aiEnabled)
        {
            if (mob.passenger == null)
            {
                mob.behavior.ai.Invoke(this);
            }
            else
            {
                mob.passenger.mob.behavior.ai.Invoke(this);

                if (mob.passenger.mob.interactInput == true && mob.passenger.mob.input.y > 0)
                {
                    Unmount();
                }
            }
        }
        else
        {
            mob.input = Vector3.zero;
        }

        if (mob.rb != null && !player)
        {
            SetChunk();
        }
    }

    public static void PatrolAI(Entity ai)
    {
        if (ai.mob.target == null)
        {
            if (EvoUtils.PercentChance(0.1f, true))
            {
                if (ai.mob.input.z != 0)
                {
                    ai.mob.input.z = 0;
                    if (ai.mob.leader != null)
                    {
                        ai.mob.orientation.LookAt(ai.mob.leader.transform);
                    }
                    else
                    {
                        Vector3 lookDir = Random.insideUnitSphere;
                        lookDir.y = 0;
                        ai.mob.orientation.localRotation = Quaternion.LookRotation(lookDir);
                    }
                    Debug.Log("rotated patroller");
                }
                else
                {
                    ai.mob.input.z = 1;
                }
            }

            if (ai.GetClosestTarget() != null)
            {
                ai.mob.target = ai.GetClosestTarget();
            }
        }
    }

    public static IEnumerator Stun(Entity e, float stunTime = 0.5f)
    {
            e.mob.aiEnabled = false;
            e.mob.input = Vector3.zero;
            e.mob.primaryInput = false;
            e.mob.secondaryInput = false;
            yield return new WaitForSeconds(stunTime);
            e.mob.aiEnabled = true;
    }

    public static void DetectionField(Entity origin, float radius = 10)
    {
        Collider[] detect = Physics.OverlapSphere(origin.transform.position, radius);

        foreach (Collider c in detect)
        {
            if (c.GetComponent<Entity>())
            {
                Entity e = c.GetComponent<Entity>();
                if (Entity.CompareTeams(e, origin))
                {
                    e.mob.target = origin;
                }
            }
        }
    }

    public static IEnumerator TalkCycle(Entity entity, string dialogue)
    {
        if (entity.baseEntity.dialogue == null)
        {
            TextMeshPro text = Instantiate(GameSettings.dialogue, entity.transform.position + Vector3.up * 1.5f, entity.transform.rotation);
            text.transform.parent = entity.transform;
            entity.baseEntity.dialogue = text;
            for (int i = 0; i < dialogue.Length; i++)
            {
                text.text += dialogue[i];
                entity.baseEntity.idleSound.PlaySound();
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(2);
            Destroy(text.gameObject);
        }
    }

    public void SetChunk()
    {

        if (!player && mob.rb != null && !interior)
        {
            mob.rb.isKinematic = Vector2.Distance(transform.position, GameSettings.player.transform.position) > GameSettings.chunkRange * 0.4f;
            mob.aiEnabled = Vector2.Distance(transform.position, GameSettings.player.transform.position) < GameSettings.chunkRange * 0.4f;
        }
    }

    public static bool CompareTeams(Entity source, Entity target) // true = attack, false = homies
    {
        if ((source == null) || (source.mob.team != target.mob.team))
        {
            return true;
        }
        return false;
    }

    public Entity GetClosestTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, mob.stats.visionRange);
        Entity tMin = null;
        float minDist = Mathf.Infinity;
        foreach (Collider c in targets)
        {
            if (c.GetComponent<Entity>())
            {
                Entity e = c.GetComponent<Entity>();
                if (CompareTeams(this, e))
                {
                    if (e.player)
                    {
                        return e;
                    }
                    float dist = Vector2.Distance(e.transform.position, transform.position);
                    if (dist < minDist)
                    {
                        tMin = e;
                        minDist = dist;
                    }
                }
            }
        }
        return tMin;
    }

    RaycastHit interactRay;
    public void Interact()
    {
        if (Physics.Raycast(transform.position, mob.orientation.forward, out interactRay, 4))
        {
            if (interactRay.rigidbody != null && interactRay.collider.GetComponent<Interactable>())
            {
                interactRay.collider.GetComponent<Interactable>().Interact(this);
            }
        }
    }

    public void Mount(Entity e)
    {
        Debug.Log(e.baseEntity.gameName + " is mounting " + baseEntity.gameName);
        mob.passenger = e;

        mob.mountSeat.autoConfigureConnectedAnchor = false;
        mob.mountSeat.connectedAnchor = Vector3.zero;
        mob.mountSeat.connectedBody = e.mob.rb;
    }
    public void Unmount()
    {
        Debug.Log(mob.passenger.baseEntity.gameName + " unmounted " + baseEntity.gameName);
        mob.passenger = null;
        mob.mountSeat.autoConfigureConnectedAnchor = true;
        mob.mountSeat.connectedBody = null;
    }

    public Projectile SpawnProjectile(Projectile p, Vector3 pos, Quaternion rot)
    {
        Projectile spawn = Instantiate(p, pos, rot);
        spawn.origin = this;

        return spawn;
    }

    public void UpdateHealth()
    {
        baseEntity.maxHealth *= mob.stats.health;    
    }

    IEnumerator destroyHealthBarCountdown;
    virtual public bool TakeDamage(float damage, Entity damager = null, bool ignoreIFrames = false)
    {
        if (!dead && (currentIframe <= 0 || ignoreIFrames))
        {
            GameSettings.hitSound.PlaySound();
            currentIframe = iframe;
            if (mob.effects.onHurt != null)
            {
                mob.effects.onHurt.Invoke(this);
            }

            float damageMultiplier = 1;

            if (damager != null)
            {
                if (damager.mob.effects.onHit != null)
                {
                    damager.mob.effects.onHit.Invoke(this);
                }
                damageMultiplier = damager.mob.stats.damage;

                if (baseEntity.healthbar == null)
                {
                    if (damager != null && !player)
                    {
                        Healthbar bar = Instantiate(GameSettings.hitBar, transform.position + Vector3.up * 1.5f, transform.rotation);
                        bar.transform.parent = transform;
                        baseEntity.healthbar = bar;
                        StartCoroutine(EvoUtils.DestroyObject(bar.gameObject, 10));
                        bar.entity = this;
                    }
                }
            }   

            baseEntity.health -= damage * damageMultiplier;
            Debug.Log(name + " took " + damage.ToString() + " damage");
            if (baseEntity.hurtSound != null)
            {
                baseEntity.hurtSound.PlaySound();
            }
            if (baseEntity.health <= 0)
            {
                if (baseEntity.deathEffect != null)
                {
                    baseEntity.deathEffect.SetActive(true);
                    baseEntity.deathEffect.transform.parent = null;
                    StartCoroutine(EvoUtils.DestroyObject(baseEntity.deathEffect));
                }
                foreach (MobDrop drop in drops)
                {
                    if (Random.Range(0f, 1f) <= drop.dropChance)
                    {
                        for (int i = 0; i < Random.Range(drop.minAmount, drop.maxAmount); i++)
                        {
                            Item item = Instantiate(drop.item, transform.position, Quaternion.Euler(Random.insideUnitSphere));
                            if (item.rb != null)
                            {
                                item.rb.AddForce(Random.insideUnitSphere * 500);
                            }
                        }
                    }
                }
                if (damager != null)
                {
                    if (mob.effects.onKill != null)
                    {
                        damager.mob.effects.onKill.Invoke(this);
                    }
                }
                Destroy(this.gameObject);
            }

            if (damage > 0)
            {
                return true;
            }
        }
        baseEntity.tookDamage = true;
        return false;
    }
}
[System.Serializable]
public class DialogueLine
{
    public string[] lines;
    public float chance = 0.1f;
}

[System.Serializable]
public class BaseEntity
{
    public string gameName;
    public float health;

    public float maxHealth;

    [HideInInspector]
    public bool tookDamage;

    public Healthbar healthbar;
    public TextMeshPro dialogue;

    public GameObject deathEffect;

    [Header("Sounds")]
    public AudioPlayer hurtSound;
    public AudioPlayer idleSound;

}

[System.Serializable]
public class Mob
{
    public MobStats stats;
    public Transform orientation;

    public Rigidbody rb;

    public AIModule behavior;
    public bool aiEnabled;

    public FunctionEffects effects;
    [Header("Team")]
    public Entity target;
    public string team = ""; //"" = hostile, "player" = player,
    public Entity leader;

    [HideInInspector]
    public bool mountable;
    [Header("Mount")]
    public Entity passenger;
    public ConfigurableJoint mountSeat;

    [Header("Variety")]
    public float scale = 1;
    public Species species;

    [HideInInspector]
    public Vector3 input;
    [HideInInspector]
    public bool primaryInput;
    [HideInInspector]
    public bool secondaryInput;
    [HideInInspector]
    public bool interactInput;
    [HideInInspector]
    public bool specialInput;
}

[System.Serializable]
public class MobStats
{
    public float health = 1;
    public float moveSpeed = 1;
    public float damage = 1;
    public float attackSpeed = 1;
    public int level = 0;

    public float visionRange = 1;
}
[System.Serializable]
public class FunctionEffects
{
    public UnityEvent<Entity> onHit;
    public UnityEvent<Entity> onIdle;
    public UnityEvent<Entity> onHurt;
    public UnityEvent<Entity> onKill;
}

    [System.Serializable]
public class MobDrop
{
    public Item item;
    public float dropChance = 0.1f;

    public int minAmount;
    public int maxAmount;

}
