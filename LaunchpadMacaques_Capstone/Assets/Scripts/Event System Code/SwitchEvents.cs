using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchEvents : MonoBehaviour
{

    public GameObject Event01;
    public GameObject Event02;
    public GameObject Event03;

    public List<GameObject> EventContainers = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        EventContainers.Add(Event01);
        EventContainers.Add(Event02);
        EventContainers.Add(Event03);

        foreach (GameObject EventContainer in EventContainers)
        {
            EventContainer.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            EventContainers[0].SetActive(true);
            EventContainers[1].SetActive(false);
            EventContainers[2].SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EventContainers[1].SetActive(true);
            EventContainers[0].SetActive(false);
            EventContainers[2].SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EventContainers[2].SetActive(true);
            EventContainers[0].SetActive(false);
            EventContainers[1].SetActive(false);
        }
    }
}
