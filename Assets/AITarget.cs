using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITarget : MonoBehaviour
{
    public Entity targetEntity;
    public void TargetEntity(Inventory ai)
    {
            if (ai.owner.entity.mob.target == null && ai.owner.entity.GetSpecificEntity(targetEntity) != null)
            {
                ai.owner.entity.mob.target = ai.owner.entity.GetSpecificEntity(targetEntity);
            }
    }
}
