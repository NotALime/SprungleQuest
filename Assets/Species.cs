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

    public void ApplySpecies(Entity mob)
    {
        ApplyBaseSpecies(mob, this);
        ApplyOverride(mob);

        if (generatesName)
        {
            mob.baseEntity.gameName = nameStructure.GenerateName();
        }
    }

    public void ApplyBaseSpecies(Entity mob, Species species)
    {
        mob.mob.scale *= Random.Range(species.minScale, species.maxScale);

        Color tintColor = species.tint.Evaluate(Random.Range(0f, 1f));
        foreach (Renderer r in mob.renderer)
        {
            r.material.SetColor("_Tint", tintColor);
        }

        mob.mob.stats.health *= Random.Range(species.stats.minHealth, species.stats.maxHealth);
        mob.UpdateHealth();

        mob.mob.stats.damage *= Random.Range(species.stats.minDamage, stats.maxDamage);

        mob.mob.stats.moveSpeed *= Random.Range(species.stats.minSpeed, species.stats.maxSpeed);
        mob.mob.stats.attackSpeed *= Random.Range(species.stats.minAttackSpeed, species.stats.maxAttackSpeed);
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
        string chosenName = nameStructure[Random.Range(0, nameStructure.Length)];

        foreach(NamePiece p in pieces)
        {
            Debug.Log("Generating name...");
            chosenName.Replace(p.toReplace, p.pieces[Random.Range(0, p.pieces.Length)]);
        }
        Debug.Log("Generated name " + chosenName);
        return chosenName;
    }
}
[System.Serializable]
public class NamePiece
{
    public string toReplace;
    [Header("Possible Name Pieces")]
    public string[] pieces;
}


