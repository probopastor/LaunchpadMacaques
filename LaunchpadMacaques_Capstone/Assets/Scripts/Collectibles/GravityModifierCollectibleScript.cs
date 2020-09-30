using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifierCollectibleScript : CollectibleBehavior
{

    private float effectDuration = 5f;
    private bool isActive = false;

    public override void OnTriggerEnter(Collider other)
    {
        // This is where collect is called from the base class
        //base.OnTriggerEnter(other);

        Collect();

    }

    public override void Update()
    {
        // base.Update();
        GetCollectibleController().gravityText.SetText("Gravity: " + GetPlayerMovement().gravity);

        Debug.Log("Is the gravity power-up is active: " + isActive);
        Debug.Log("The gravity mutiplier is currently: " + GetPlayerMovement().gravity);

    }


    public override void Collect()
    {
        isActive = true;
        //DestroyCollectible();
        StartCoroutine(EffectTimer());

        if(isActive)
        {
            GetPlayerMovement().gravity = 200f;
        }
        else if(isActive == false)
        {
            GetPlayerMovement().gravity = GetPlayerMovement().defaultGravity;
        }

    }

    public override void DestroyCollectible()
    {
        Destroy(gameObject);
    }

    private IEnumerator EffectTimer()
    {
        yield return new WaitForSeconds(effectDuration);

        isActive = false;
    }

}
