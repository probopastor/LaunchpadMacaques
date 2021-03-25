using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class New_Grapple_UI : MonoBehaviour
{
    [Header("UI Settings")] 
    [SerializeField] Sprite deafultReticle = null;
    [SerializeField] Sprite activeReticle = null;
    [SerializeField] Image retiicle = null;
    [SerializeField] Animator anim = null;

    GrapplingGun gg;
    PushPullObjects pushPull;
    OpenDocument openDocs;

    [Header("Audio Setting")]
    [SerializeField, EventRef] string targetSoundEffect = null;

    public Sprite ActiveReticle { get => activeReticle; set => activeReticle = value; }

    // Start is called before the first frame update
    void Start()
    {
        gg = FindObjectOfType<GrapplingGun>();
        pushPull = FindObjectOfType<PushPullObjects>();
        openDocs = FindObjectOfType<OpenDocument>();
        retiicle.sprite = deafultReticle;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {
            DisplayUI();
        }

    }

   void DisplayUI()
    {
        if (openDocs.CanSeeDocument().collider != null || gg.CanFindGrappleLocation() || (pushPull.CanSeeBox().collider != null && pushPull.CanPickUpObjects()))
        {
            //retiicle.sprite = activeReticle;
            if (anim.GetBool("isHighlighted") == false)
            {
                FMOD.Studio.EventInstance targetInstance = FMODUnity.RuntimeManager.CreateInstance(targetSoundEffect);
                targetInstance.start();
            }
            anim.SetBool("isHighlighted", true);
        }

        else
        {
            anim.SetBool("isHighlighted", false);
            //retiicle.sprite = deafultReticle;
        }
    }
}
