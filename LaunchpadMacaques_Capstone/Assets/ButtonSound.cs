using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;
public class ButtonSound : MonoBehaviour
{
    EventSystem es;
    private GameObject lastSelectObject;
    private StudioEventEmitter eventEmiter;

    private void Awake()
    {
        es = FindObjectOfType<EventSystem>();
        eventEmiter = this.GetComponent<StudioEventEmitter>();
    }

    private void Start()
    {
        lastSelectObject = es.currentSelectedGameObject;
    }

    private void Update()
    {
        if (lastSelectObject)
        {
            if (es.currentSelectedGameObject != lastSelectObject)
            {
                lastSelectObject = es.currentSelectedGameObject;
            }
        }

        else
        {
            lastSelectObject = es.currentSelectedGameObject;
        }

    }

    public void Selected()
    {
        if (lastSelectObject)
        {
            if (lastSelectObject != this.gameObject)
            {
                eventEmiter.Play();
            }
        }

    }
}
