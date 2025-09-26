using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ammor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Ammo Collected");
            Destroy(gameObject);
        }
    }
}
