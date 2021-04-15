using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using CharTween;

public class Log : MonoBehaviour
{
    public static Log instance;

    [SerializeField, Tooltip("The color a character's name will appear to be in the Log")]
    private Color characterNameColor = Color.yellow;
    [SerializeField, Tooltip("The color the text a character is saying will be")]
    private Color bodyTextColor = Color.white;
    [SerializeField, Tooltip("The color notification messages like \"[Start of Conversation\" will be")]
    private Color notificationTextColor = Color.gray;
    

    private Stack<GameObject> loggedMessages;
    private GameObject messagePrefab;
    private List<LineColoringDetails> linesToColor;

    private struct LineColoringDetails
    {
        public TMP_Text textToColor;
        public Dialogue.Line line;

        public LineColoringDetails(TMP_Text text, Dialogue.Line line)
        {
            textToColor = text;
            this.line = line;
        }
    }

    private void Awake()
    {
        instance = this;
        messagePrefab = (GameObject) Resources.Load("Log Message");
        linesToColor = new List<LineColoringDetails>();
        gameObject.SetActive(false);
    }

    public void ActivateLog(bool active)
    {
        gameObject.SetActive(active);
        if(active)
        {
            ApplyColorsToLines();
            StartCoroutine(WaitForClose());
        }
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void StartNewConversation()
    {
        loggedMessages = new Stack<GameObject>();
        PushToLog("[Start Of Conversation]");
    }

    public void PushToLog(string message)
    {
        GameObject messageObj;
        TMP_Text tmpText;
        loggedMessages.Push(messageObj = Instantiate<GameObject>(messagePrefab, transform.Find("Elements"), false));
        messageObj.transform.SetAsLastSibling();
        (tmpText = loggedMessages.Peek().GetComponent<TMP_Text>()).text = message;
        tmpText.color = notificationTextColor;
    }

    public void PushToLog(Dialogue.Line line)
    {
        GameObject messageObj;
        TMP_Text tmpText;

        loggedMessages.Push(messageObj = Instantiate<GameObject>(messagePrefab, transform.Find("Elements"), false));
        loggedMessages.Peek().transform.SetAsLastSibling();
        (tmpText = loggedMessages.Peek().GetComponent<TMP_Text>()).text = string.Format("{0}: {1}", line.character.characterName, line.text);

        linesToColor.Add(new LineColoringDetails(tmpText, line));

    }

    private void ApplyColorsToLines()
    {
        foreach(LineColoringDetails details in linesToColor)
        {
            CharTweener tween = details.textToColor.GetCharTweener();

            for (int i = 0; i < details.line.character.characterName.Length; i++)
            {
                tween.SetColor(i, details.line.character.textColor);
            }

            for (int i = details.line.character.characterName.Length; i < details.textToColor.text.Length; i++)
            {
                tween.SetColor(i, bodyTextColor);
            }
        }
    }

    private IEnumerator WaitForClose()
    {
        while(!Input.GetButtonDown("Fire1"))
        {
            yield return null;
        }

        yield return null;

        ActivateLog(false);
    }
}
