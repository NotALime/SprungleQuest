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
    public float currentTime = 0;
    public AnimationCurve effectCurve = AnimationCurve.Constant(0, 1, 1);
    public MobStats effect;

    public GameObject particleEffect;

    public GameObject particleInstance;

    [HideInInspector]
    public Volume postProcess;

    public static void ApplyEffect(Entity entity, EntityEffect effect, float time)
    {
        if (!entity.activeEffects.Find(x => x.name == effect.name))
        {
            EntityEffect effectInstance = Instantiate(effect);
            entity.activeEffects.Add(effectInstance);
            effectInstance.name = effect.name;
            effectInstance.time = time;
            effectInstance.currentTime = 0;
            peddling = effectInstance.peddles;
            if (entity.player && effect.visualEffect != null)
            {
                GameObject obj = Instantiate(new GameObject());
                effectInstance.postProcess = obj.AddComponent<Volume>();
                effectInstance.postProcess.weight = 0;
                effectInstance.postProcess.isGlobal = true;
                effectInstance.postProcess.gameObject.layer = 9;
                effectInstance.postProcess.profile = effect.visualEffect;
            }
            if (effect.particleEffect != null)
            {
                effectInstance.particleInstance = Instantiate(effect.particleEffect);
                effectInstance.particleInstance.transform.position = entity.transform.position;
                effectInstance.particleInstance.transform.SetParent(entity.transform, true);
            }
            Entity.ApplyStats(entity, effectInstance.effect);
        }
        else
        {
            entity.activeEffects.Find(x => x.name == effect.name).currentTime -= time;
        }
    }

    public void ApplyEffectDirect(Entity entity)
    {
        EntityEffect effectInstance = Instantiate(this);
        effectInstance.time = time;
        effectInstance.currentTime = 0;
        peddling = effectInstance.peddles;
        if (entity.player && visualEffect != null)
        {
            GameObject obj = Instantiate(new GameObject());
            effectInstance.postProcess = obj.AddComponent<Volume>();
            effectInstance.postProcess.weight = 0;
            effectInstance.postProcess.isGlobal = true;
            effectInstance.postProcess.gameObject.layer = 9;
            effectInstance.postProcess.profile = visualEffect;
        }
        if (effectInstance.particleEffect != null)
        {
            effectInstance.particleInstance = Instantiate(effectInstance.particleEffect);
            effectInstance.particleInstance.transform.position = entity.transform.position;
            effectInstance.particleInstance.transform.SetParent(entity.transform, true);
        }
        Entity.ApplyStats(entity, effectInstance.effect);
    }

    public void HandleEffect(Entity ai)
    {
        currentTime += Time.deltaTime;

        if (postProcess != null)
        {
            postProcess.weight = Mathf.LerpUnclamped(postProcess.weight, effectCurve.Evaluate(currentTime / time), 10 * Time.deltaTime);
        }

        if (currentTime >= time)
        {
            ai.activeEffects.Remove(ai.activeEffects.Find(x => x.name == name));
            if (particleInstance != null)
            {
                Destroy(particleInstance.gameObject);
            }
            if (postProcess != null)
            {
                Destroy(postProcess.gameObject);
            }
            if (peddles)
            {
                peddling = false;
            }
            Entity.RemoveStats(ai, effect);
            Destroy(this);
        }
    }

    [Header("Regen and damage")]
    public float health;
    public void Heal(Entity ai)
    {
        ai.baseEntity.health += health * Time.deltaTime;
    }
    public void DamageOverTime(Entity ai)
    {
        ai.TakeDamage(health * Time.deltaTime, null, true);
    }

    [Header("Peddling")]
    public bool peddles;
    public static bool peddling;
}
