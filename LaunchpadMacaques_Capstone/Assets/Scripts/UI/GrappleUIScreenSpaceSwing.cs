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
        if ( objectSet && thisCanvas && Time.timeScale > 0)
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

    private void DisplayUI()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;


        //if (Physics.SphereCast(springJoint.GetCamera().position, springJoint.GetSphereSphereRadius(), springJoint.GetCamera().forward, out hitInfo, springJoint.GetMaxGrappleDistance(), springJoint.GetGrappleLayer()))
        //{
        if (Physics.Raycast(ray, out hitInfo, springJoint.GetMaxGrappleDistance(), springJoint.GetGrappleLayer()))
        {
            float distance = Vector3.Distance(springJoint.GetCamera().position, hitInfo.point);


            if (!(Physics.Raycast(ray, distance, springJoint.GetUnGrappleLayer())))
            {
                //uiImageHolder.rectTransform.localPosition = Vector3.zero;
                objectHitPoint = hitInfo.point;
                CreateUI(hitInfo);
            }
            else
            {
                TurnOffUI();
            }
        }

        else if (Physics.SphereCast(springJoint.GetCamera().position, springJoint.GetSphereSphereRadius(), springJoint.GetCamera().forward, out hitInfo, springJoint.GetMaxGrappleDistance(), springJoint.GetGrappleLayer()) && player.GetComponent<Rigidbody>().velocity.magnitude > springJoint.GetAutoAimVelocity())
        {
            float distance = Vector3.Distance(springJoint.GetCamera().position, hitInfo.point);


            if (!(Physics.Raycast(ray, distance, springJoint.GetUnGrappleLayer())))
            {
                //uiImageHolder.rectTransform.localPosition = Vector3.zero;
                objectHitPoint = hitInfo.point;
                CreateUI(hitInfo);
            }
            else
            {
                TurnOffUI();
            }
        }
        else
                {
            TurnOffUI();
        }



    }

    private void CreateUI(RaycastHit hitObject)
    {
        float distance = Vector3.Distance(player.transform.position, hitObject.point);

        if (true)
        {
            uiImageHolder.enabled = true;
            uiImageHolder.rectTransform.localScale = new Vector3(distance * distanceVariable, distance * distanceVariable, distance * distanceVariable);
            objectSet = true;

        }

    }

    private void TurnOffUI()
    {
        objectSet = false;
        uiImageHolder.enabled = false;

    }

    private void UpdateUIPos()
    {

        uiPos = cam.WorldToScreenPoint(objectHitPoint);
        Debug.Log(objectHitPoint);

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
