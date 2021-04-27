using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlay : A_InputType
{
    [SerializeField] GameObject keyBoard;
    [SerializeField] GameObject controller;

    public override void ChangeUI()
    {
        if (controllerDetected)
        {
            keyBoard.SetActive(false);
            controller.SetActive(true);
        }

        else
        {
            controller.SetActive(false);
            keyBoard.SetActive(true);
        }
    }

    private new void OnEnable()
    {
        base.OnEnable();
        ChangeUI();
    }
}
