using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationDoor : MonoBehaviour
{
    [SerializeField] private GameObject[] doors;

    [SerializeField] private int buttonsToActivate = 0;
    private int currentButtonsPressed = 0;
    private bool doorsDectivated;

    private void Awake()
    {
        currentButtonsPressed = 0;
        doorsDectivated = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetActiveButtons(int activeGain)
    {
        currentButtonsPressed += activeGain;

        if (currentButtonsPressed < 0)
        {
            currentButtonsPressed = 0;
        }

        CheckButtonActivation();
    }

    private void CheckButtonActivation()
    {
        if((currentButtonsPressed >= buttonsToActivate) && !doorsDectivated)
        {
            doorsDectivated = true;
            DisableDoor();
        }
        else if ((currentButtonsPressed < buttonsToActivate) && doorsDectivated)
        {
            doorsDectivated = false;
            EnableDoor();
        }
    }

    private void DisableDoor()
    {
        for(int i = 0; i < doors.Length; i++)
        {
            doors[i].SetActive(false);
        }
    }

    private void EnableDoor()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].SetActive(true);
        }
    }

    public void SetCurrentButtonsPressed(int newButtonPressedAmount)
    {
        currentButtonsPressed = newButtonPressedAmount;

        if (currentButtonsPressed < 0)
        {
            currentButtonsPressed = 0;
        }

        CheckButtonActivation();
    }

}
