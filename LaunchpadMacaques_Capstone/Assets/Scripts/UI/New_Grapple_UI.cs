﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class New_Grapple_UI : MonoBehaviour
{
    [Header("UI Settings")] 
    [SerializeField] Sprite deafultReticle;
    [SerializeField] Sprite activeReticle;
    [SerializeField] Image retiicle;


    GrapplingGun gg;

    
    // Start is called before the first frame update
    void Start()
    {
        gg = FindObjectOfType<GrapplingGun>();
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
        if (gg.CanFindGrappleLocation())
        {
            retiicle.sprite = activeReticle;
        }

        else
        {
            retiicle.sprite = deafultReticle;
        }
    }
}