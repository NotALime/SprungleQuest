using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
	public float speed;
	public float damage;
    public float cooldown = 1;
	public Rigidbody rb;
	public float lifetime;

	public bool destroysOnImpact = true;

	public bool keepsOriginSpeed = false;

	public GameObject destroyEffect;

	public bool noclip;
	public bool piercing;

	public bool persistant;

	[HideInInspector]
	public Entity origin;

	public ProjectileLevel level;

	public enum Behavior
	{ 
		Basic,
		Accelerative,
		Tilting,
	};

	public Behavior behavior;

	// Use this for initialization
	public void Start()
	{
        ApplyLevel(level.level);

        rb = GetComponent<Rigidbody>();

		if (behavior != Behavior.Accelerative || !keepsOriginSpeed)
		{
			rb.velocity = transform.forward * speed;
		}
		else if (keepsOriginSpeed)
		{
            rb.velocity = origin.mob.rb.velocity + transform.forward * speed;
        }


        if (origin != null)
		{
			foreach (Projectile p in GetComponentsInChildren<Projectile>())
			{
				p.origin = origin;
			}
		}
		if (lifetime > 0)
		{
			StartCoroutine(Lifespan());
		}

	}

    private void FixedUpdate()
    {
        switch (behavior)
        {
            case Behavior.Accelerative:
                rb.AddForce(transform.forward * speed);
                break;
            case Behavior.Tilting:
				rb.velocity = rb.velocity + origin.mob.orientation.forward;
                break;
        }
    }

    public IEnumerator Lifespan()
	{
		yield return new WaitForSeconds(lifetime);
		BulletDestroy();
	}

	public void BulletDestroy()
	{
		if (destroyEffect != null)
		{
			GameObject g = Instantiate(destroyEffect, transform.position, Quaternion.identity);
			if (g.GetComponent<Entity>())
			{
				g.GetComponent<Entity>().mob.team = origin.mob.team;
			}
		}

		foreach (ParticleSystem child in GetComponentsInChildren<ParticleSystem>())
		{
			child.enableEmission = false;
			child.transform.parent = null;
		}
		foreach (AudioSource child in GetComponentsInChildren<AudioSource>())
		{
			child.transform.parent = null;
		}
		foreach (TrailRenderer child in GetComponentsInChildren<TrailRenderer>())
		{
			child.emitting = false;
			child.transform.parent = null;
		}
		Destroy(gameObject);
	}
	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.GetComponent<Entity>())
		{
			if (Entity.CompareTeams(origin, collision.gameObject.GetComponent<Entity>()))
			{
				collision.gameObject.GetComponent<Entity>().TakeDamage(damage, origin);
				if (!piercing)
				{
					BulletDestroy();
				}
			}
		}
        else if (collision.gameObject.GetComponent<EntityLimb>())
        {
            if (Entity.CompareTeams(origin, collision.gameObject.GetComponent<EntityLimb>().entity))
            {
                collision.gameObject.GetComponent<EntityLimb>().entity.TakeDamage(damage, origin);
                if (!piercing)
                {
                    BulletDestroy();
                }
            }
        }
       	else if(destroysOnImpact)
        {
			Debug.Log(collision.gameObject.name);
       		BulletDestroy();
       	}
    }

	public void ApplyLevel(int l = 1)
	{
		for (int i = 0; i < l - 1; i++)
		{
			speed *= level.speedPerLevel;
            damage *= level.damagePerLevel;
            cooldown *= level.cooldownPerLevel;
            lifetime *= level.lifeTimePerLevel;
            transform.localScale *= level.sizePerLevel;

			Debug.Log("Leveled up " + this.gameObject.name);
        }
	}
}

[System.Serializable]

public class ProjectileLevel
{
	public int level = 1;

	public float speedPerLevel = 0.8f;
	public float damagePerLevel = 1.2f;
	public float cooldownPerLevel = 1.5f;
	public float lifeTimePerLevel = 1f;
	public float sizePerLevel = 1.2f;
}
