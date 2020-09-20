using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleUI : MonoBehaviour
{
    #region Inspector Variables 
    [Header("UI Settings")] [SerializeField] Sprite uiSprite;
    Image uiImageHolder;
    [Tooltip("The layers the aiming decal should be enabled on.")] [SerializeField] private LayerMask whatIsGrappleable;
    [SerializeField] private Canvas grappleCanvas;
    [SerializeField] private float distanceVariable = .01f;
    #endregion

    #region Private Variables 
    private ConfigJoint configJoint;
    private Camera cam;
    private GameObject player;
    private Vector3 objectHitPoint;
    private bool objectSet = false;
    private Canvas thisCanvas;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        thisCanvas = Instantiate(grappleCanvas);
        uiImageHolder = thisCanvas.GetComponentInChildren<Image>();
        uiImageHolder.sprite = uiSprite;
        configJoint = FindObjectOfType<ConfigJoint>();
        thisCanvas.enabled = false;
        cam = FindObjectOfType<Camera>();
        player = FindObjectOfType<Matt_PlayerMovement>().gameObject;
        objectHitPoint = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        DisplayUI();
    }

    private void DisplayUI()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if ((Physics.Raycast(ray, out hitInfo, configJoint.GetMaxGrappleDistance(), whatIsGrappleable)) || (configJoint.IsGrappling()))
        {
            CreateUI(hitInfo);
        }
        else
        {
            TurnOffUI();
        }
    }

    private void CreateUI(RaycastHit hitObject)
    {
        thisCanvas.transform.LookAt(player.transform.position + cam.transform.rotation * Vector3.back, cam.transform.rotation * Vector3.up);
        float distance = Vector3.Distance(player.transform.position, hitObject.point);

        if (configJoint.IsGrappling())
        {
            if (!objectSet)
            {
                objectHitPoint = hitObject.transform.position;
                objectSet = true;
            }

            distance = Vector3.Distance(player.transform.position, objectHitPoint);
            uiImageHolder.rectTransform.localScale = new Vector3(distance * distanceVariable, distance * distanceVariable, distance * distanceVariable);
        }

        if (!configJoint.IsGrappling())
        {
            thisCanvas.enabled = true;
            thisCanvas.transform.position = hitObject.point;
            uiImageHolder.rectTransform.localScale = new Vector3(distance * distanceVariable, distance * distanceVariable, distance * distanceVariable);
            objectSet = false;
        }

    }

    private void TurnOffUI()
    {
        thisCanvas.enabled = false;

    }
}
