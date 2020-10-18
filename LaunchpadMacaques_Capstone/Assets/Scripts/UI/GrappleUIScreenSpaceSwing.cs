/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (GrappleUIScreenSpaceSwing.CS) 
* (The Script that handles the Verlet Swing UI thing for swinging) 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleUIScreenSpaceSwing : MonoBehaviour
{
    #region Inspector Variables
    [Header("UI Settings")] [SerializeField] Sprite uiSprite;
    Image uiImageHolder;
    [Tooltip("The layers the aiming decal should be enabled on.")] [SerializeField] private LayerMask whatIsGrappleable;
    [SerializeField] private Canvas grappleCanvas;
    [SerializeField] private float distanceVariable = .01f;
    [SerializeField] private float minScale = .5f;
    [SerializeField] private float maxScale = .5f;
    [SerializeField] LayerMask whatIsNotGrappleable;
    #endregion

    #region Private Variables
    private GrapplingGun springJoint;
    private Camera cam;
    private GameObject player;
    private Vector3 objectHitPoint;
    private bool objectSet = false;
    private Canvas thisCanvas;
    private Vector3 uiPos;
    #endregion



    void Start()
    {
        thisCanvas = Instantiate(grappleCanvas);
        uiImageHolder = thisCanvas.GetComponentInChildren<Image>();
        uiImageHolder.sprite = uiSprite;
        springJoint = FindObjectOfType<GrapplingGun>();
        uiImageHolder.enabled = false;
        cam = FindObjectOfType<Camera>();
        player = FindObjectOfType<Matt_PlayerMovement>().gameObject;
        objectHitPoint = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        DisplayUI();
    }

    private void LateUpdate()
    {
        if (objectSet && thisCanvas && Time.timeScale > 0)
        {
            UpdateUIPos();
        }

        if (uiImageHolder)
        {
            if (uiImageHolder.rectTransform.localScale.x < minScale)
            {
                uiImageHolder.rectTransform.localScale = new Vector3(minScale, minScale, minScale);
            }

            else if (uiImageHolder.rectTransform.localScale.x > maxScale)
            {
                uiImageHolder.rectTransform.localScale = new Vector3(maxScale, maxScale, maxScale);
            }


        }

    }


    /// <summary>
    /// Will Check to see if the UI should be diaplayed
    /// Will then call the relevant methods
    /// </summary>
    private void DisplayUI()
    {
        if (springJoint.CanFindGrappleLocation())
        {
            objectHitPoint = springJoint.GetGrappleRayhit().point;
            TurnOnUI(springJoint.GetGrappleRayhit());
        }
        else
        {
            TurnOffUI();
        }
    }

    /// <summary>
    /// Will turn on the UI and will Scale it
    /// </summary>
    /// <param name="hitObject"></param>
    private void TurnOnUI(RaycastHit hitObject)
    {
        float distance = Vector3.Distance(player.transform.position, hitObject.point);


        uiImageHolder.enabled = true;
        uiImageHolder.rectTransform.localScale = new Vector3(distance * distanceVariable, distance * distanceVariable, distance * distanceVariable);
        objectSet = true;

    }

    /// <summary>
    /// Turns of the UI
    /// </summary>
    private void TurnOffUI()
    {
        objectSet = false;
        uiImageHolder.enabled = false;

    }

    /// <summary>
    /// Will update the UI box's position so it is centered around the correct point 
    /// </summary>
    private void UpdateUIPos()
    {

        uiPos = cam.WorldToScreenPoint(objectHitPoint);

        if (uiPos.z < 0)
        {
            uiImageHolder.enabled = false;
        }
        if (uiPos.z >= 0)
        {
            uiImageHolder.enabled = true;
            uiImageHolder.rectTransform.position = uiPos;
        }


    }
}
