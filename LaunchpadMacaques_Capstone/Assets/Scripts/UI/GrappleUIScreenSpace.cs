using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleUIScreenSpace : MonoBehaviour
{
    #region Inspector Variables 
    [Header("UI Settings")] [SerializeField] Sprite uiSprite;
    Image uiImageHolder;
    [Tooltip("The layers the aiming decal should be enabled on.")] [SerializeField] private LayerMask whatIsGrappleable;
    [SerializeField] private LayerMask whatIsNotGrappleable;
    [SerializeField] private Canvas grappleCanvas;
    [SerializeField] private float distanceVariable = .01f;
    [SerializeField] private float minScale = .5f;
    [SerializeField] private float maxScale = .5f;
    #endregion

    #region Private Variables 
    private ConfigJoint configJoint;
    private Camera cam;
    private GameObject player;
    private Vector3 objectHitPoint;
    private bool objectSet = false;
    private Canvas thisCanvas;
    private Vector3 uiPos;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        thisCanvas = Instantiate(grappleCanvas);
        uiImageHolder = thisCanvas.GetComponentInChildren<Image>();
        uiImageHolder.sprite = uiSprite;
        configJoint = FindObjectOfType<ConfigJoint>();
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
        if (objectSet && thisCanvas)
        {
            UpdateUIPos();
        }

        if (uiImageHolder)
        {
            if (uiImageHolder.rectTransform.localScale.x < minScale)
            {
                uiImageHolder.rectTransform.localScale = new Vector3(minScale, minScale, minScale);
            }

            else if(uiImageHolder.rectTransform.localScale.x > maxScale)
            {
                uiImageHolder.rectTransform.localScale = new Vector3(maxScale, maxScale, maxScale);
            }


        }

    }

    private void DisplayUI()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;


        if ((Physics.Raycast(ray, out hitInfo, configJoint.GetMaxGrappleDistance(), whatIsGrappleable)))
        {
            float distance = Vector3.Distance(ray.GetPoint(0), hitInfo.point);


            //if (!Physics.Raycast(transform.position, dir, distance, whatIsNotGrappleable))
            //{
            //    uiImageHolder.rectTransform.localPosition = Vector3.zero;
            //    CreateUI(hitInfo);
            //}


            if (!(Physics.Raycast(ray, distance, whatIsNotGrappleable)))
            {
                uiImageHolder.rectTransform.localPosition = Vector3.zero;
                CreateUI(hitInfo);
            }
            else
            {
                TurnOffUI();
            }
        }
        else
        {
            Debug.Log("Should be turning off");
            TurnOffUI();
        }
    }

    private void CreateUI(RaycastHit hitObject)
    {
        float distance = Vector3.Distance(player.transform.position, hitObject.point);

        //if (configJoint.IsGrappling())
        //{
        //    if (!objectSet)
        //    {
        //        objectHitPoint = hitObject.point;
        //        objectSet = true;
        //    }

        //    distance = Vector3.Distance(player.transform.position, objectHitPoint);
        //    uiImageHolder.rectTransform.localScale = new Vector3(distance * distanceVariable, distance * distanceVariable, distance * distanceVariable);

        //}

        if (true)
        {
            uiImageHolder.enabled = true;
            uiImageHolder.rectTransform.localScale = new Vector3(distance * distanceVariable, distance * distanceVariable, distance * distanceVariable);
            objectSet = false;
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

        if(uiPos.z < 0)
        {
            uiImageHolder.enabled = false;
        }
        if(uiPos.z >= 0)
        {
            uiImageHolder.enabled = true;
            uiImageHolder.transform.position = uiPos;
        }


    }
}