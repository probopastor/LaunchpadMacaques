﻿/* 
* Launchpad Macaques - Neon Oblivion
* Levi Schoof
* HandleBackButton.cs
* Helps handle the back button input on menus
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class HandleBackButton : MonoBehaviour
{
    [SerializeField] BackEvent backEvent;
    PlayerControlls controls;

    private void Awake()
    {
        controls = new PlayerControlls();
        controls.Enable();

        controls.GamePlay.Back.performed += BackEvent;
    }

    private void BackEvent(InputAction.CallbackContext cxt)
    {
        backEvent.Invoke(cxt.ReadValue<float>());
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}

[Serializable] public class BackEvent : UnityEvent<float> { }