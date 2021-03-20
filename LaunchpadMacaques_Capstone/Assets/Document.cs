/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof, William Nomikos) 
* (Document.CS) 
* (The Script that is placed on the text documents the player can look at and open) 
*/
using UnityEngine;
using TMPro;

public class Document : MonoBehaviour
{
    [SerializeField] GameObject textDocument;

    [TextArea(3, 5)]
    [SerializeField] string documentText;

    TextMeshProUGUI text;

    Matt_PlayerMovement player;



    private void Start()
    {
        player = FindObjectOfType<Matt_PlayerMovement>();
        text = textDocument.GetComponentInChildren<TextMeshProUGUI>(); 

    }

    /// <summary>
    /// Will Open or close the document
    /// </summary>
    public void OpenCloseDocument()
    {
        if (textDocument.activeSelf)
        {
            CloseDocument();
        }

        else
        {
            OpenDocument();
        }
    }

    /// <summary>
    /// Will open the doucement
    /// </summary>
    public void OpenDocument()
    {
        player.SetPlayerCanMove(false);

        textDocument.SetActive(true);

        text.text = documentText;

    }

    /// <summary>
    /// Will Close the document
    /// </summary>
    public void CloseDocument()
    {
        player.SetPlayerCanMove(true);
        textDocument.SetActive(false);
    }

    /// <summary>
    /// Returns if the Document is Open
    /// </summary>
    /// <returns></returns>
    public bool DocumentOpen()
    {
        return textDocument.activeSelf;
    }
}
