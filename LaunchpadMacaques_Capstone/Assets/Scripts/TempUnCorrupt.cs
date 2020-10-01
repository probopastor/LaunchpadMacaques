using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempUnCorrupt : MonoBehaviour
{
    [SerializeField] float clearRadius = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<MakeSpotNotGrappleable>().ClearCorruption(this.transform.position,clearRadius);
        }
    }
}
