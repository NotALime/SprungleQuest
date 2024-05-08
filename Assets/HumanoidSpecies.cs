using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Humanoid Species")]
public class HumanoidSpecies : ScriptableObject
{
    public Gradient skinTone;

    public Item[] weaponPool;
    public int loadoutSize;
    public Item[] accessoryPool;
    public int wardrobeSize;

    public Item[] hairPool;
    public Sprite[] facePool;

    public RigScale rigScale;

    public void ApplySpecies(Humanoid mob)
    {
        mob.transform.localScale *= Random.Range(rigScale.baseScaleMin, rigScale.baseScaleMax);

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


        mob.rig.renderer.material.SetColor("_Tint", skinTone.Evaluate(Random.Range(0f, 1f)));

        if (hairPool.Length > 0)
        {
            mob.inv.VisualEquip(hairPool[Random.Range(0, hairPool.Length)]);
        }
        if (facePool.Length > 0)
        {
            mob.rig.faceRender.sprite = facePool[Random.Range(0, facePool.Length)];
        }

        mob.inv.items = new Item[loadoutSize];
        for (int i = 0; i < loadoutSize; i++)
        {
            mob.inv.items[i] = Instantiate(weaponPool[Random.Range(0, weaponPool.Length)]);
        }
        mob.inv.accessories = new Item[wardrobeSize];
        for (int i = 0; i < wardrobeSize; i++)
        {
            mob.inv.accessories[i] = Instantiate(accessoryPool[Random.Range(0, accessoryPool.Length)]);
            mob.inv.VisualEquip(mob.inv.accessories[i]);
        }
    }
}

[System.Serializable]
public class RigScale
{
    public float baseScaleMin = 1;
    public float baseScaleMax = 1;
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
