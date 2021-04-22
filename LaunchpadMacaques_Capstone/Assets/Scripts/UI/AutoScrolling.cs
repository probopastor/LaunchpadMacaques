using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class AutoScrolling : MonoBehaviour
{
    [SerializeField, Tooltip("The speed at which this scroll component should autoscroll")]
    private float scrollSpeed;
    [SerializeField, Tooltip("The time (in seconds) after completion of credits to exit back to the main screen")]
    private float timeUntilExit = 5f;

    private ScrollRect scrollRect;

    // Start is called before the first frame update
    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void OnEnable()
    {
        StartCoroutine(Autoscroll());
    }

    private IEnumerator Autoscroll()
    {
        float currTime = 0;

        bool exitWhenPossible = false;
        float currExitTime = 0;


        while(gameObject.activeInHierarchy)
        {
            //Adjust position
            scrollRect.content.localPosition = new Vector2(scrollRect.content.localPosition.x, currTime * scrollSpeed);

            //If the bottom of the credits is reached
            if (scrollRect.viewport.TransformPoint(new Vector2(0, scrollRect.viewport.rect.yMin)).y < 
                scrollRect.content.TransformPoint(new Vector2(0, scrollRect.content.rect.yMin)).y)
            {;
                exitWhenPossible = true;
            }
          
            //If extra time after end of credits has been reached, exit
            if(currExitTime >= timeUntilExit)
            {
                ExitScreen();
                yield break;
            }
            //Count time after credits
            else if(exitWhenPossible)
            {
                currExitTime += Time.deltaTime;
            }

            currTime += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Exits back to the main screen
    /// </summary>
    private void ExitScreen()
    {
        transform.parent.Find("Back Button Holder").GetComponentInChildren<Button>().onClick.Invoke();
    }
}
