using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedButtonFix : MonoBehaviour, IPointerEnterHandler
{

    public bool starterButton = false;

    private void Start()
    {
 
        if (starterButton)
        {
            if (DetectController.instance)
            {
                DetectController.instance.selectedGameObject = this.gameObject;
          
            }

            else
            {
                FindObjectOfType<DetectController>().selectedGameObject = this.gameObject;
          
            }
        }

    }

    public void OnEnable()
    {
        if (starterButton)
        {
            if (DetectController.instance)
            {
                DetectController.instance.selectedGameObject = this.gameObject;
            }

            else
            {
                FindObjectOfType<DetectController>().selectedGameObject = this.gameObject;
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {

        if (DetectController.instance)
        {
            DetectController.instance.selectedGameObject = this.gameObject;
        }

        else
        {
            FindObjectOfType<DetectController>().selectedGameObject = this.gameObject;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (starterButton)
        {
            if (DetectController.instance)
            {
                DetectController.instance.selectedGameObject = this.gameObject;
            }

            else
            {
                FindObjectOfType<DetectController>().selectedGameObject = this.gameObject;
            }
        }

    }
}
