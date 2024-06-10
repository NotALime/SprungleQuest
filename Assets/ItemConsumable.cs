using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemConsumable : MonoBehaviour
{
    public Item item;
    public List<EntityEffect> effects;

    public float consumeTime;

    public ParticleSystem particle;
    private void Start()
    {
        item = GetComponent<Item>();
        particle.enableEmission = false;
    }

    public void ApplyEffects(Inventory ai)
    {
        StartCoroutine(ConsumeItem(ai));
    }

    public IEnumerator ConsumeItem(Inventory ai)
    {
        foreach (EntityEffect effect in effects)
        {
            EntityEffect.ApplyEffect(ai.owner.entity, effect, effect.time);
        }
        ai.handAnimator.SetTrigger("Drink");
        particle.enableEmission = true;
        ai.handAnimator.speed = ((1 / consumeTime));
        item.cooldown = consumeTime;
        yield return new WaitForSeconds(consumeTime);

        particle.enableEmission = false;
        ai.handAnimator.speed = 1;
        ai.TakeItem(item);
    }
}
