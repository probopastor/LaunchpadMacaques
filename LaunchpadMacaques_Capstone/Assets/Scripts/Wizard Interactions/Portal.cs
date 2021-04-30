using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    public enum PortalStates {INACTIVE, OPEN, CLOSE };

    public PortalStates portalState = PortalStates.INACTIVE;

    private int wizardCollisions = 0;


    private void Update()
    {
        // Check Portal State...
        CheckPortalState(portalState);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wizard")
        {
            wizardCollisions++;
            Debug.Log("The wizard has collided with the portal! with a collision count of: " + wizardCollisions);
        }
    }

    private void CheckPortalState(PortalStates currentPortalState)
    {
        if(currentPortalState == PortalStates.INACTIVE)
        {
            gameObject.SetActive(false);
        }
        else if(currentPortalState == PortalStates.OPEN)
        {
            gameObject.SetActive(true);
        }
        else if(currentPortalState == PortalStates.CLOSE)
        {
            gameObject.SetActive(false);
        }
    }

    #region Getters/Setters
    public PortalStates PortalStatesReference
    {
        get
        {
            return portalState;
        }
        set
        {
            portalState = value;
        }
    }

    public int WizardCollisions
    {
        get
        {
            return wizardCollisions;
        }
        set
        {
            wizardCollisions = value;
        }
    }

    #endregion

}
