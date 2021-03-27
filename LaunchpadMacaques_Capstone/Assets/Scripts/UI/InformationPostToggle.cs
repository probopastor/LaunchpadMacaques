/* 
* Launchpad Macaques - Trial & Error
* William Nomikos
* InformationPostToggle.cs
* Toggles information posts in the scene on and off when the player presses O. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationPostToggle : MonoBehaviour
{
    #region Variables
    [SerializeField, Tooltip("The GameObject that holds all the information posts. ")] private GameObject postHolder;
    private bool toggle;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        toggle = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            InformationPost[] informationPosts = FindObjectsOfType<InformationPost>();

            if(toggle)
            {
                toggle = false;
            }
            else if(!toggle)
            {
                toggle = true;
            }

            postHolder.SetActive(toggle);
        }
    }
}
