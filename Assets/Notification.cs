using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour
{
    public string text;
    bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if(!activated)
        {
            if (other.GetComponent<InventoryUI>())
            {
                GameSettings.ShowTitle(text);
                activated = true;
            }
        }
    }
}
