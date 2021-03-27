/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof, William Nomikos) 
* (OpenDocumentss.CS) 
* (The Script that is placed on the player to handle open and closing text documents) 
*/
using UnityEngine;

public class OpenDocument : MonoBehaviour
{
    GameObject cam;
    GrapplingGun grapplingGun;

    [SerializeField] LayerMask documentLayer;
    [SerializeField] float maxDistance;

    Document tempDocument;
    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<Camera>().gameObject;
        grapplingGun = this.GetComponent<GrapplingGun>();
    }

    /// <summary>
    /// Handles The Open Document Input
    /// </summary>
    public void OpenDocumentInput()
    {
        if(CanSeeDocument().collider != null)
        {
            tempDocument = CanSeeDocument().collider.gameObject.GetComponent<Document>();

            tempDocument.OpenCloseDocument();

            if (!tempDocument.DocumentOpen())
            {
                tempDocument = null;
            }
        }

        else if(tempDocument != null)
        {
            tempDocument.OpenCloseDocument();
            tempDocument = null;
        }

    }

    /// <summary>
    /// Returns a hit collider if the player is hovering over document
    /// </summary>
    /// <returns></returns>
    public RaycastHit CanSeeDocument()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxDistance, documentLayer) && !grapplingGun.IsGrappling())
        {
            return hit;
        }

        else return hit;
    }
}
