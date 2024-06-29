using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    [Header("Base Entity Settings")]
    public BaseEntity baseEntity;

    public MobDrop[] drops;
    public Renderer[] renderer;

    [HideInInspector]
    public int significance;
    public static Entity mostSignificantEntity;

    float initialHealth;

    public float iframe = 0.2f;

    public float currentIframe;
    bool dead;
    [Header("Mob Settings")]
    public Mob mob;

    [HideInInspector]
    public bool interior;

    public List<EntityEffect> activeEffects;

    public bool player;

    public bool practiceDeath;
    private void Awake()
    {
        baseEntity.pureName = baseEntity.gameName;
        if (baseEntity.deathEffect != null)
        {
            baseEntity.deathEffect.SetActive(false);
        }
        mob.targetPoint = transform.position;
    }

    private void Start()
    {
        if (mob.species != null)
        {
            mob.species.ApplySpecies(this);
        }

        if (player)
        {
            GameSettings.respawnPoint = transform.position;
        }

        baseEntity.maxHealth = baseEntity.health;
        initialHealth = baseEntity.maxHealth;
        UpdateHealth();

        transform.localScale *= mob.scale;
    }
    private void FixedUpdate()
    {
        currentIframe -= Time.deltaTime;
        if (currentIframe <= 0)
        {
            baseEntity.tookDamage = false;
        }

        if (mob.stats.effects.onIdle != null)
        {
            foreach (UnityEvent<Entity> e in mob.stats.effects.onIdle)
            {
                e.Invoke(this);
            }
        }
    }

    public void AI()
    {
        if (mob.aiEnabled)
        {
            if (mob.target != null)
            {
                if (mob.target.baseEntity.health <= 0)
                {
                    mob.target = null;
                }
            }
            if (mob.passenger == null)
            {
                mob.behavior.ai.Invoke(this);
            }
            else
            {
                mob.input.z = mob.passenger.mob.input.z;
                mob.orientation.Rotate(0, mob.passenger.mob.input.x * 4, 0);

                if (mob.passenger.mob.input.y > 0)
                {
                    Unmount();
                }
            }

            if (baseEntity.idleSound != null)
            {
                if (EvoUtils.PercentChance(0.05f, true))
                {
                    baseEntity.idleSound.PlaySound();
                    if (baseEntity.idleDialogue.Length > 0)
                    {
                        Entity.TalkCycle(this, baseEntity.idleDialogue[Random.Range(0, baseEntity.idleDialogue.Length)]);
                    }
                }
            }
        }
        else
        {
            mob.input = Vector3.zero;
        }
    }

    public static void PickupOnProximity(Entity ai)
    {
        if (ai.mob.target != null)
        {
            if (Vector2.Distance(ai.transform.position, ai.mob.target.transform.position) < 1)
            {
                if (!ai.mob.target.mob.mounted && ai.mob.target.currentIframe < 0 && (EvoUtils.PercentChance(0.5f, true)))
                {
                    ai.mob.target.mob.mounted = true;
                    ai.mob.mountSeat.connectedBody = ai.mob.target.mob.rb;
                }
            }

            if (ai.mob.mountSeat.connectedBody == ai.mob.target.mob.rb && (EvoUtils.PercentChance(0.5f, true) || ai.currentIframe > 0))
            {
                ai.mob.mountSeat.connectedBody = null;
                Entity.Stun(ai, 4);
                if (ai.currentIframe < 0)
                {
                    ai.mob.target.TakeDamage(30, ai);
                }
                ai.mob.target.currentIframe = 0.5f;
                ai.mob.target.mob.rb.AddForce(Vector3.up * 2000);
                ai.mob.target.mob.rb.AddForce(ai.mob.target.mob.orientation.forward * -5000);
                ai.mob.rb.AddForce(ai.mob.orientation.forward * -3000);
                ai.mob.target.mob.mounted = false;
            }
        }

    }
    public static void WalkToPos(Entity ai)
    {
        Vector3 dir = (ai.mob.targetPoint - ai.transform.position).normalized;

        ai.mob.orientation.rotation = Quaternion.LookRotation(dir);
    }
    public static void WalkForward(Entity ai)
    {
        ai.mob.input.z = 1;
        ai.mob.input.x = EvoUtils.NormalizeInt(Mathf.PerlinNoise(Time.deltaTime * 0.5f, 0));
        if (ai.mob.target != null)
        {
            ai.mob.orientation.LookAt(ai.mob.target.transform);
        }
    }
    public static void PatrolAI(Entity ai)
    {
        if (ai.mob.target == null)
        {
            if (ai.mob.leader != null)
            {
                if (ai.mob.leader.mob.target != null)
                {
                    ai.mob.target = ai.mob.leader.mob.target;
                }

                ai.mob.orientation.LookAt(ai.mob.leader.mob.orientation.transform);
                if (Vector2.Distance(ai.transform.position, ai.mob.leader.transform.position) > 5)
                {
                    ai.mob.input = Vector3.forward;
                }
                else
                {
                    ai.mob.input = Vector3.zero;
                }
            }
            else if (Vector2.Distance(new Vector2(ai.transform.position.x, ai.transform.position.z), new Vector2(ai.mob.targetPoint.x, ai.mob.targetPoint.z)) < 2f)
            {
                ai.mob.targetPoint = ai.mob.targetPoint + Random.insideUnitSphere * ai.mob.stats.visionRange;
            }
            if (ai.GetClosestTarget() != null && ai.mob.aggro)
            {
                ai.mob.target = ai.GetClosestTarget();
            }
        }
        else
        {
            if (ai.mob.fleeing)
            {
                Vector3 dir = ai.mob.target.transform.position - ai.transform.position + new Vector3((Mathf.PerlinNoise(Time.deltaTime * 0.5f, 0) * 10) * 2 - 1, 0, (Mathf.PerlinNoise(0, Time.deltaTime * 0.5f) * 10) * 2 - 1);
                ai.mob.orientation.rotation = Quaternion.SlerpUnclamped(ai.mob.orientation.rotation, Quaternion.LookRotation(dir), 1 * Time.deltaTime);
                ai.mob.input = Vector3.forward;
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
            TextMeshPro text = Instantiate(GameSettings.dialogue, entity.transform.position + Vector3.up * 2f, entity.transform.rotation);
            text.transform.parent = entity.transform;
            entity.baseEntity.dialogue = text;
            for (int i = 0; i < dialogue.Length; i++)
            {
                text.text += dialogue[i];
                if (char.IsUpper(dialogue[i]))
                {
                    entity.baseEntity.idleSound.PlaySound(1, Random.Range(0f, 0.5f));
                }
                yield return new WaitForSeconds(0.02f);
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
        if (mob.aggro)
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, mob.stats.visionRange);
            Entity tMin = null;
            float minDist = Mathf.Infinity;
            foreach (Collider c in targets)
            {
                if (c.GetComponent<Entity>())
                {
                    Entity e = c.GetComponent<Entity>();
                    if (Entity.CompareTeams(this, e))
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
        }
        return null;
    }

    public Entity GetSpecificEntity(Entity entity)
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, mob.stats.visionRange);
        foreach (Collider c in targets)
        {
            if (c.GetComponent<Entity>())
            {
                Entity e = c.GetComponent<Entity>();
                if (e.baseEntity.gameName == entity.baseEntity.gameName)
                {
                    return e;
                }
            }
        }
        return null;
    }

    RaycastHit interactRay;
    public GameObject GetObjectLookedAt()
    {
        if (Physics.Raycast(transform.position, mob.orientation.forward, out interactRay, 4))
        {
            return interactRay.collider.gameObject;
        }
        return null;
    }

    public void Mount(Entity e)
    {
        Debug.Log(e.baseEntity.gameName + " is mounting " + baseEntity.gameName);
        mob.team = e.mob.team;
        mob.passenger = e;

        mob.mountSeat.autoConfigureConnectedAnchor = false;
        mob.mountSeat.connectedAnchor = Vector3.zero;
        mob.mountSeat.connectedBody = e.mob.rb;
        mob.mountSeat.xMotion = ConfigurableJointMotion.Locked;
        mob.mountSeat.yMotion = ConfigurableJointMotion.Limited;
        mob.mountSeat.zMotion = ConfigurableJointMotion.Locked;
    }

    public void Tame(Entity e)
    {
        mob.leader = e;
        mob.team = e.mob.team;
        mob.target = null;
    }

    public void Unmount()
    {
        Debug.Log(mob.passenger.baseEntity.gameName + " unmounted " + baseEntity.gameName);
        mob.passenger = null;
        mob.mountSeat.autoConfigureConnectedAnchor = true;
        mob.mountSeat.connectedBody = null;
        mob.mountSeat.xMotion = ConfigurableJointMotion.Free;
        mob.mountSeat.yMotion = ConfigurableJointMotion.Free;
        mob.mountSeat.zMotion = ConfigurableJointMotion.Free;
    }

    public Projectile SpawnProjectile(Projectile p, Vector3 pos, Quaternion rot)
    {
        Projectile spawn = Instantiate(p, pos, rot);
        spawn.transform.position = pos;
        spawn.origin = this;

        return spawn;
    }

    public void UpdateHealth()
    {
        baseEntity.maxHealth = initialHealth * mob.stats.health;    
    }

    IEnumerator destroyHealthBarCountdown;
    virtual public bool TakeDamage(float damage, Entity damager = null, bool ignoreIFrames = false)
    {
        if (!dead && (currentIframe <= 0 || ignoreIFrames))
        {
            if (!ignoreIFrames)
            {
                GameSettings.hitSound.PlaySound();
                if (baseEntity.hurtSound != null)
                {
                    baseEntity.hurtSound.PlaySound();
                }
                currentIframe = iframe;
            }
            if (mob.stats.effects.onHurt != null)
            {
                foreach (UnityEvent<Entity> e in mob.stats.effects.onHurt)
                {
                    e.Invoke(this);
                }
            }

            float damageMultiplier = 1;

            if (damager != null)
            {
                damager.mob.target = this;
                if (damager.mob.stats.effects.onHit != null)
                {
                    foreach (UnityEvent<Entity> e in mob.stats.effects.onHit)
                    {
                        e.Invoke(this);
                    }
                }
                if (mob.revengeful || mob.flees)
                {
                    mob.target = damager;
                    mob.fleeing = mob.flees;
                }
                damageMultiplier = damager.mob.stats.damage;

                if (baseEntity.healthbar == null)
                {
                    if (damager != null && !player)
                    {
                        Healthbar bar = Instantiate(GameSettings.hitBar, transform.position + Vector3.up * 1.5f, transform.rotation);
                        bar.transform.SetParent(transform, true);
                        baseEntity.healthbar = bar;
                        StartCoroutine(EvoUtils.DestroyObject(bar.gameObject, 10));
                        bar.entity = this;
                    }
                }
            }
            float netDamage = GetDefensedDamage(damage * damageMultiplier);

            baseEntity.health -= netDamage;

            Debug.Log(name + " took " + damage.ToString() + " damage");
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
                    if (Random.Range(0f, 1f) <= drop.dropChance * GameSettings.player.mob.stats.luck)
                    {
                        for (int i = 0; i < Random.Range(drop.minAmount, drop.maxAmount); i++)
                        {
                            Item item = Instantiate(drop.item, transform.position, Quaternion.Euler(Random.insideUnitSphere));
                            EvoUtils.DestroyObject(item.gameObject, 120);
                            if (item.rb != null)
                            {
                                item.rb.AddForce(Random.insideUnitSphere * 500);
                            }
                        }
                    }
                }
                if (damager != null)
                {
                    if (mob.stats.effects.onKill != null)
                    {
                        foreach (UnityEvent<Entity> e in mob.stats.effects.onKill)
                        {
                            e.Invoke(this);
                        }
                    }
                }
                if (practiceDeath)
                {
                    Entity.Stun(this, 5);
                    currentIframe = 5;
                    practiceDeath = false;
                    mob.target = null;
                    baseEntity.health = baseEntity.maxHealth;
                }
                else if (!player)
                {
                    if ((mostSignificantEntity == null || mostSignificantEntity.significance <= significance) && significance > 0)
                    {
                        Destroy(mostSignificantEntity.gameObject);
                        mostSignificantEntity = this;
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                }
                else
                {
                    RagdollCamera();
                    StartCoroutine(GameSettings.Respawn());
                }
            }

            if (damage > 0)
            {
                return true;
            }
        }
        baseEntity.tookDamage = true;
        return false;
    }

    public static void SpawnMostSignificant(Vector3 position)
    {
        mostSignificantEntity.transform.position = position;
        mostSignificantEntity.gameObject.SetActive(true);
    }

    public static void RagdollCamera()
    {
        Camera cam = Camera.main;
        cam.transform.parent = null;
        cam.gameObject.AddComponent<BoxCollider>();
        Rigidbody rb = cam.gameObject.AddComponent<Rigidbody>();
        rb.AddForce((Vector3.up + Random.insideUnitSphere.normalized) * 500);
        rb.AddTorque(Random.insideUnitSphere.normalized * 100);
        GameSettings.player.mob.aiEnabled = false;
    }
    public static void ResetCamera()
    {
        Camera cam = Camera.main;

        Destroy(cam.gameObject.GetComponent<BoxCollider>());
        Destroy(cam.gameObject.GetComponent<Rigidbody>());
        cam.transform.parent = GameSettings.player.GetComponent<Humanoid>().rig.spine.neck.transform;
        cam.transform.position = GameSettings.player.GetComponent<Humanoid>().rig.spine.neck.transform.position;
        cam.transform.localPosition = new Vector3(0, 0.00600000005f, 0.0019999037f);
        cam.transform.rotation = GameSettings.player.GetComponent<Humanoid>().rig.spine.neck.transform.rotation;

        GameSettings.player.mob.aiEnabled = true;
    }

    public static void ApplyStats(Entity entity, MobStats mod)
    {
        entity.mob.stats.health += mod.health;
        entity.UpdateHealth();
        entity.mob.stats.moveSpeed += mod.moveSpeed;
        entity.mob.stats.damage += mod.damage;
        entity.mob.stats.attackSpeed += mod.attackSpeed;
        entity.mob.stats.attackSpeed += mod.attackSpeed;
        entity.mob.stats.level += mod.level;

        FunctionEffects effects = mod.effects;

        if (effects.onKill.Count > 0)
        {
            foreach (UnityEvent<Entity> e in effects.onKill)
            {
                entity.mob.stats.effects.onKill.Add(e);
            }
        }
        if (effects.onHurt.Count > 0)
        {
            foreach (UnityEvent<Entity> e in effects.onHurt)
            {
                entity.mob.stats.effects.onHurt.Add(e);
            }
        }
        if (effects.onHit.Count > 0)
        {
            foreach (UnityEvent<Entity> e in effects.onHit)
            {
                entity.mob.stats.effects.onHit.Add(e);
            }
        }
        if (effects.onIdle.Count > 0)
        {
            foreach (UnityEvent<Entity> e in effects.onIdle)
            {
                entity.mob.stats.effects.onIdle.Add(e);
            }
        }
    }
    public static void RemoveStats(Entity entity, MobStats mod)
    {
        entity.mob.stats.health -= mod.health;
        entity.UpdateHealth();
        entity.mob.stats.moveSpeed -= mod.moveSpeed;
        entity.mob.stats.damage -= mod.damage;
        entity.mob.stats.attackSpeed -= mod.attackSpeed;
        entity.mob.stats.attackSpeed -= mod.attackSpeed;
        entity.mob.stats.level -= mod.level;

        FunctionEffects effects = mod.effects;

        if (effects.onKill.Count > 0)
        {
            foreach (UnityEvent<Entity> e in effects.onKill)
            {
                entity.mob.stats.effects.onKill.Remove(e);
            }
        }
        if (effects.onHurt.Count > 0)
        {
            foreach (UnityEvent<Entity> e in effects.onHurt)
            {
                entity.mob.stats.effects.onHurt.Remove(e);
            }
        }
        if (effects.onHit.Count > 0)
        {
            foreach (UnityEvent<Entity> e in effects.onHit)
            {
                entity.mob.stats.effects.onHit.Remove(e);
            }
        }
        if (effects.onIdle.Count > 0)
        {
            foreach (UnityEvent<Entity> e in effects.onIdle)
            {
                entity.mob.stats.effects.onIdle.Remove(e);
            }
        }
    }

    public float GetDefensedDamage(float damage)
    {
        return (100 * damage) / (mob.stats.defense + 100);
    }

    public static IEnumerator EffectActive(Entity entity, EntityEffect effect, float time)
    {
        Entity.ApplyStats(entity, effect.effect);
        yield return new WaitForSeconds(time);
        Entity.RemoveStats(entity, effect.effect);
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
    [HideInInspector]
    public string pureName;
    public float health;

    public List<EntityEffect> effects;

    public float maxHealth;

    [HideInInspector]
    public bool tookDamage;

    public Healthbar healthbar;
    public TextMeshPro dialogue;

    public GameObject deathEffect;

    [Header("Sounds")]
    public AudioPlayer hurtSound;
    public AudioPlayer idleSound;
    public string[] idleDialogue;

}

[System.Serializable]
public class Mob
{
    public MobStats stats;
    public Transform orientation;

    public Rigidbody rb;

    public AIModule behavior;
    public bool aiEnabled;

    [HideInInspector]
    public Vector3 targetPoint;
    [Header("Team")]
    public Entity target;
    public string team = ""; //"" = hostile, "player" = player,
    public Entity leader;
    [Tooltip("Does the entity attack an entity back when damaged")]
    public bool revengeful = true;
    public bool aggro = true;
    public bool flees = false;
    public bool fleeing = false;

    [HideInInspector]
    public bool mountable;
    public bool mounted;
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
    public float defense;
    public float luck = 1;

    public float visionRange = 1;

    public FunctionEffects effects;
}
[System.Serializable]
public class FunctionEffects
{
    public List<UnityEvent<Entity>> onHit;
    public List<UnityEvent<Entity>> onIdle;
    public List<UnityEvent<Entity>> onHurt;
    public List<UnityEvent<Entity>> onKill;
}


[System.Serializable]
public class MobDrop
{
    public Item item;
    public float dropChance = 0.1f;

    public int minAmount;
    public int maxAmount;

}
