using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Healthbar : MonoBehaviour
{
    public Image health;
    public TextMeshPro nameText;
    public TextMeshPro healthText;

    public Entity entity;

    private void Start()
    {
    //    health = GetComponent<Image>();
        nameText.text = entity.baseEntity.gameName;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        health.fillAmount = Mathf.LerpUnclamped(health.fillAmount, entity.baseEntity.health / entity.baseEntity.maxHealth, 0.1f);
        healthText.text = entity.baseEntity.health.ToString() + "/" + entity.baseEntity.maxHealth.ToString();
    }
}
