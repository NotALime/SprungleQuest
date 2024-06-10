using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Species/Humanoid Species")]
public class HumanoidSpecies : Species
{
    [Header("-----------------")]
    [Header("HUMANOID")]
    [Header("-----------------")]
    public Item[] weaponPool;
    public int loadoutSize;
    public Item[] predefinedAccessories;
    public Item[] accessoryPool;
    public int wardrobeSize;

    public Item[] hairPool;
    public Sprite[] facePool;

    public RigScale rigScale;

    [Header("Personality")]
    public EmotionTrait[] traits;
    public int traitAmount = 3;

    public NPCEmotion.Job job;

    public override void ApplyOverride(Entity mob)
    {
        ApplySpecies(mob.GetComponent<Humanoid>());
    }
    public void ApplySpecies(Humanoid mob)
    {
        Debug.Log("Applied humanoid");
        mob.rig.spine.spineLower.ScaleLimb(Vector3.one * Random.Range(rigScale.spineLowerMin, rigScale.spineLowerMax));
        mob.rig.spine.neck.ScaleLimb(Vector3.one * Random.Range(rigScale.neckMin, rigScale.neckMax));
        if (rigScale.uniformShoulders)
        {
            float shoulderScale = Random.Range(rigScale.shoulderMin, rigScale.shoulderMax);
            mob.rig.armRight.shoulder.ScaleLimb(Vector3.one * shoulderScale);
            mob.rig.armLeft.shoulder.ScaleLimb(Vector3.one * shoulderScale);
            mob.rig.spine.spineUpper.ScaleLimb(Vector3.one * shoulderScale);
        }
        else
        {
            mob.rig.armRight.shoulder.ScaleLimb(Vector3.one * Random.Range(rigScale.shoulderMin, rigScale.shoulderMax));
            mob.rig.armLeft.shoulder.ScaleLimb(Vector3.one * Random.Range(rigScale.shoulderMin, rigScale.shoulderMax));
            mob.rig.spine.spineUpper.ScaleLimb((mob.rig.armRight.shoulder.bodyScale + mob.rig.armLeft.shoulder.bodyScale) * 0.5f);
        }
        if (rigScale.uniformArms)
        {
            float shoulderScale = Random.Range(rigScale.armMin, rigScale.armMax);
            mob.rig.armRight.arm.ScaleLimb(Vector3.one * shoulderScale);
            mob.rig.armLeft.arm.ScaleLimb(Vector3.one * shoulderScale);
        }
        else
        {
            mob.rig.armRight.arm.ScaleLimb(Vector3.one * Random.Range(rigScale.armMin, rigScale.armMax));
            mob.rig.armLeft.arm.ScaleLimb(Vector3.one * Random.Range(rigScale.armMin, rigScale.armMax));
        }
        float armScale = Random.Range(rigScale.armMin, rigScale.armMax);
        mob.rig.armLeft.arm.ScaleLimb(Vector3.one * armScale);
        mob.rig.armRight.arm.ScaleLimb(Vector3.one * armScale);

        if (mob.GetComponent<Inventory>())
        {
            Inventory inv = mob.GetComponent<Inventory>();

            if (hairPool.Length > 0)
            {
                inv.EquipItem(hairPool[Random.Range(0, hairPool.Length)]);
            }
            if (facePool.Length > 0)
            {
                mob.rig.faceRender.sprite = facePool[Random.Range(0, facePool.Length)];
            }

            inv.items = new Item[loadoutSize];
            for (int i = 0; i < loadoutSize; i++)
            {
                inv.AddItem(Instantiate(weaponPool[Random.Range(0, weaponPool.Length)]));
            }
            inv.accessories = new Item[wardrobeSize + predefinedAccessories.Length];
            for (int i = 0; i < wardrobeSize; i++)
            {
                inv.accessories[i] = Instantiate(accessoryPool[Random.Range(0, accessoryPool.Length)]);
                inv.EquipItem(inv.accessories[i]);
            }

            for (int i = 0; i < predefinedAccessories.Length; i++)
            {
                inv.accessories[i + wardrobeSize] = Instantiate((predefinedAccessories[i]));
                inv.EquipItem(inv.accessories[i + wardrobeSize]);
            }
        }

        if (traits.Length > 0)
        {
            NPCEmotion d = mob.GetComponent<NPCEmotion>();
            for (int i = 0; i < traitAmount; i++)
            {
                d.traits.Add(traits[Random.Range(0, traits.Length)]);
            }
            d.UpdatePersonality();

            d.job = job;
        }
    }
}

[System.Serializable]
public class RigScale
{
    [Header("Lower spine/Stomach")]
    public float spineLowerMin = 1;
    public float spineLowerMax = 1;
    [Header("Neck/Head")]
    public float neckMin = 1;
    public float neckMax = 1;
    [Header("Shoulders")]
    public bool uniformShoulders = true;
    public float shoulderMin = 1;
    public float shoulderMax = 1;
    [Header("Arms")]
    public bool uniformArms = true;
    public float armMin = 1;
    public float armMax = 1;
}
