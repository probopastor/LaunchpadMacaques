using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCollectScriptVariant : GravityModifierCollectibleScript
{
    [SerializeField, Tooltip("Time between disabling and enabling of collectible")] public int timerValue = 6;
    /// <summary>
    /// Destroys the current collectible.
    /// </summary>
    public override void DestroyCollectible()
    {

        StartCoroutine(Enabler());
    }
    IEnumerator Enabler()
    {   

        Debug.Log("Script Firing");
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;

        yield return new WaitForSecondsRealtime(timerValue);
        
        Debug.Log("Waited Succesfully");
        GetCollectibleController().SetIsActive(false);
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
    }
}