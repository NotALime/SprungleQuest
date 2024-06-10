using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "Entity/Status Effect")]
public class EntityEffect : ScriptableObject
{
    public VolumeProfile visualEffect;
    public float time = 60;
    [HideInInspector]
    public float currentTime = 60;
    public AnimationCurve effectCurve = AnimationCurve.Constant(0, 1, 1);
    public MobStats effect;

    [HideInInspector]
    public Volume postProcess;

    public static void ApplyEffect(Entity entity, EntityEffect effect, float time)
    {
        EntityEffect effectInstance = Instantiate(effect);
        Entity.RemoveStats(entity, effect.effect);
        effectInstance.time = time;
        if (entity.player && effect.visualEffect != null)
        {
            GameObject obj = Instantiate(new GameObject());
            effectInstance.postProcess = obj.AddComponent<Volume>();
            effectInstance.postProcess.isGlobal = true;
            effectInstance.postProcess.gameObject.layer = 9;
            effectInstance.postProcess.profile = effect.visualEffect;
        }
    }

    public void HandleEffect(Entity ai)
    {
        currentTime += Time.deltaTime;

        if (postProcess != null)
        {
            postProcess.weight = effectCurve.Evaluate(currentTime / time);
        }

        if (currentTime >= time)
        {

        }
    }

    [Header("Regen")]
    public float regenAmount;
    public void Heal(Entity ai)
    {
        ai.baseEntity.health += regenAmount * Time.deltaTime;
    }
}
