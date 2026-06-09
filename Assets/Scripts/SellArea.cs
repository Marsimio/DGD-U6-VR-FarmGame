using System;
using UnityEngine;


public class SellArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Crop"))
        {
            StatManager.instance.Money += 2;
            Destroy(other.gameObject);
        }
    }
}