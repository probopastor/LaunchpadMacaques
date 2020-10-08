//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GravityCollectScriptVariant : GravityModifierCollectibleScript
//{
//    [SerializeField, Tooltip("Time between disabling and enabling of collectible")] public int timerValue = 6;

//    /// <summary>
//    /// Destroys the current collectible.
//    /// </summary>
//    public override void DestroyCollectible()
//    {

//        StartCoroutine(Enabler());
//    }
//    IEnumerator Enabler()
//    {   

//        Debug.Log("Script Firing");
//        gameObject.GetComponent<MeshRenderer>().enabled = false;
//        gameObject.GetComponent<BoxCollider>().enabled = false;

//        yield return new WaitForSecondsRealtime(timerValue);
        
//        Debug.Log("Waited Succesfully");
        
//        //collectibleController.SetIsActive(false);

//        gameObject.GetComponent<MeshRenderer>().enabled = true;
//        gameObject.GetComponent<BoxCollider>().enabled = true;
//    }
//}