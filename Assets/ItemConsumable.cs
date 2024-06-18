using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemConsumable : MonoBehaviour
{
    public Item item;
    public List<EntityEffect> effects;

    public float consumeTime;

    public ParticleSystem particle;

    public AudioPlayer drinkSound;
    private void Start()
    {
        item = GetComponent<Item>();
        drinkSound.gameObject.SetActive(false);
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
        drinkSound.PlaySound();
        drinkSound.gameObject.SetActive(true);
        ai.handAnimator.SetTrigger("Drink");
        particle.enableEmission = true;
        item.cooldown = consumeTime;
        yield return new WaitForSeconds(consumeTime);
        particle.enableEmission = false;
        ai.TakeItem(item);
    }
}
