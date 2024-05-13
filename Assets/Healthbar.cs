using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Healthbar : MonoBehaviour
{
    public Image health;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;

    public Entity entity;

    public Color baseColor = Color.green;
    public Color invulnerableColor = Color.white;
    public Color damageColor = Color.red;
    // Update is called once per frame
    void FixedUpdate()
    {
        nameText.text = entity.baseEntity.gameName;
        health.fillAmount = Mathf.LerpUnclamped(health.fillAmount, entity.baseEntity.health / entity.baseEntity.maxHealth, 0.5f);
        healthText.text = ((int)entity.baseEntity.health).ToString() + "/" + ((int)entity.baseEntity.maxHealth).ToString();

        if (entity.currentIframe <= 0)
        {
            health.color = baseColor;
        }
        else if (entity.baseEntity.tookDamage)
        {
            health.color = damageColor;
        }
        else
        {
            health.color = invulnerableColor;
        }
    }
}
