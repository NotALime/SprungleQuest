using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Species/Basic Species")]
public class Species : ScriptableObject
{
    [Header("Color")]
    public Gradient tint;

    [Header("Scale")]
    public float minScale = 0.9f;
    public float maxScale = 1.1f;

    [Header("Basic stats")]
    public StatRange stats;

    [Header("Naming")]
    public bool generatesName;
    public NameStructure nameStructure;

    [Header("Mounting")]
    public float chanceToMount;
    public Entity[] possibleMounts;

    [Header("Sound")]
    public float minIdleAdd = -0.5f;
    public float maxIdleAdd = 0.5f;

    public void ApplySpecies(Entity mob)
    {
        mob.mob.scale *= Random.Range(minScale, maxScale);

        Color tintColor = tint.Evaluate(Random.Range(0f, 1f));
        foreach (Renderer r in mob.renderer)
        {
            r.material.SetColor("_Tint", tintColor);
        }

        mob.mob.stats.health *= Random.Range(stats.minHealth, stats.maxHealth);
        mob.UpdateHealth();

        mob.mob.stats.damage *= Random.Range(stats.minDamage, stats.maxDamage);

        mob.mob.stats.moveSpeed *= Random.Range(stats.minSpeed, stats.maxSpeed);
        mob.mob.stats.attackSpeed *= Random.Range(stats.minAttackSpeed, stats.maxAttackSpeed);

        ApplyOverride(mob);

        if (generatesName)
        {
            mob.baseEntity.gameName = nameStructure.GenerateName();
        }

        if (possibleMounts.Length > 0 && EvoUtils.PercentChance(chanceToMount))
        {
            Entity e = Instantiate(possibleMounts[Random.Range(0, possibleMounts.Length)]);
            e.Mount(mob);
        }

        if (mob.baseEntity.idleSound != null)
        {
            float tone = Random.Range(minIdleAdd, maxIdleAdd);
            mob.baseEntity.idleSound.minPitch += tone;
            mob.baseEntity.idleSound.maxPitch += tone;
        }
    }

    public virtual void ApplyOverride(Entity mob)
    {
        //Override this function to create additional species randomization like for humans.
    }
}

[System.Serializable]
public class StatRange
{
    [Header("Health")]
    public float minHealth = 0.9f;
    public float maxHealth = 1.1f;

    [Header("Damage")]
    public float minDamage = 0.9f;
    public float maxDamage = 1.1f;

    [Header("Movement speed")]
    public float minSpeed = 0.9f;
    public float maxSpeed = 1.1f;

    [Header("Attack speed")]
    public float minAttackSpeed = 0.9f;
    public float maxAttackSpeed = 1.1f;
}
[System.Serializable]
public class NameStructure
{
    public string[] nameStructure;
    public NamePiece[] pieces;

    public string GenerateName()
    {
 //     string chosenName = nameStructure[Random.Range(0, nameStructure.Length)];
 //
 //     foreach(NamePiece p in pieces)
 //     {
 //         Debug.Log("Generating name...");
 //         chosenName.Replace(p.toReplace, p.pieces[Random.Range(0, p.pieces.Length)]);
 //     }
 //     Debug.Log("Generated name " + chosenName);
      return "Gluh";
    }
}
[System.Serializable]
public class NamePiece
{
    public string toReplace;
    [Header("Possible Name Pieces")]
    public string[] pieces;
}


