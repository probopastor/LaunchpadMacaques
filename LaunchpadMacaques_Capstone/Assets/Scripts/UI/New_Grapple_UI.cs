﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class New_Grapple_UI : MonoBehaviour
{
    [Header("UI Settings")] 
    [SerializeField] Sprite deafultReticle;
    [SerializeField] Sprite activeReticle;
    [SerializeField] Image retiicle;
    [SerializeField] Animator anim;

    GrapplingGun gg;
    PushPullObjects pushPull;

    [Header("Audio Setting")]
    [SerializeField, EventRef] string targetSoundEffect;
    
    // Start is called before the first frame update
    void Start()
    {
        gg = FindObjectOfType<GrapplingGun>();
        pushPull = FindObjectOfType<PushPullObjects>();
        retiicle.sprite = deafultReticle;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {
            DisplayUI();
        }

    }

   void DisplayUI()
    {
        if (gg.CanFindGrappleLocation() || pushPull.CanSeeBox().collider != null)
        {
            //retiicle.sprite = activeReticle;
            if (anim.GetBool("isHighlighted") == false)
            {
                FMOD.Studio.EventInstance targetInstance = FMODUnity.RuntimeManager.CreateInstance(targetSoundEffect);
                targetInstance.start();
            }
            anim.SetBool("isHighlighted", true);
        }

        else
        {
            anim.SetBool("isHighlighted", false);
            //retiicle.sprite = deafultReticle;
        }
    }
}
