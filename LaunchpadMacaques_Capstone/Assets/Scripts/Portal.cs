using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private bool openPortal = false;
    private bool closePortal = false;


    public enum PortalStates {INACTIVE, OPENING, OPEN, CLOSING, CLOSED };

    public PortalStates portalState = PortalStates.INACTIVE;

    private Animator portalAnimator;

    private void Awake()
    {
        portalAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Check Portal State...
        CheckPortalState(portalState);
    }

    private void CheckPortalState(PortalStates currentPortalState)
    {
        if(currentPortalState == PortalStates.INACTIVE)
        {
            gameObject.SetActive(false);
        }
    }

    private void OpenPortal()
    {

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

    #endregion

    //private IEnumerator OpenPortal()
    //{
    //    while(!closePortal)
    //    {
    //        // Play portal opening animation and keep it open.

    //    yield return null;
    //    }
    //}

}
