using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndingSplashScreen : MonoBehaviour
{
    public static EndingSplashScreen instance;

    private bool waitingForInput;
    private PlayerControlls controls;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        transform.GetChild(0).gameObject.SetActive(false);

        controls = new PlayerControlls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.GamePlay.Dialouge.performed += PlayerInput;
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void ActivateEndScreen(bool shouldActivate)
    {
        if(shouldActivate)
        {
            StartCoroutine(EndScreenActivation());
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private IEnumerator EndScreenActivation()
    {

        NarrativeTriggerHandler[] handlers = FindObjectsOfType<NarrativeTriggerHandler>();
        bool shouldLoop = true;

        //Wait for dialogue to appear
        do
        {
            foreach(NarrativeTriggerHandler handler in handlers)
            {
                if(handler.DialogueRunning)
                {
                    shouldLoop = false;
                }
            }

            yield return null;
        } while (shouldLoop);


        //Wait for dialogue to go away
        do
        {
            foreach (NarrativeTriggerHandler handler in handlers)
            {
                if (handler.DialogueRunning)
                {
                    shouldLoop = true;
                    break;
                }
                shouldLoop = false;
            }

            yield return null;
        } while (shouldLoop);

        transform.GetChild(0).gameObject.SetActive(true);

        FindObjectOfType<Matt_PlayerMovement>().SetPlayerCanMove(false);
        FindObjectOfType<GrapplingGun>().SetCanGrapple(false);

        waitingForInput = true;
        while(waitingForInput)
        {
            yield return null;
        }

        ActivateEndScreen(false);

        FindObjectOfType<Matt_PlayerMovement>().SetPlayerCanMove(true);
        FindObjectOfType<GrapplingGun>().SetCanGrapple(true);
    }

    public void PlayerInput(InputAction.CallbackContext cxt)
    {
        if (waitingForInput)
        {
            waitingForInput = false;
        }
    }
}
