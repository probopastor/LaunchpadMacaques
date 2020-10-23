using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class GrappleGun_Audio : MonoBehaviour
{
    public ScriptableEmitter[] emitters;

    [HideInInspector] public StudioEventEmitter m_grapple;
    [HideInInspector] public StudioEventEmitter m_push;
    [HideInInspector] public StudioEventEmitter m_beam;

    // Start is called before the first frame update
    void Start()
    {
        m_grapple = AudioUtilities.FindEmitter(emitters, "Grapple");
        m_push = AudioUtilities.FindEmitter(emitters, "Push");
        m_beam = AudioUtilities.FindEmitter(emitters, "Beam");
    }

    private void Update()
    {
        if (m_beam.IsPlaying())
        {
            m_beam.SetParameter("Magnitude", Player_Audio.GetMagnitude());
        }
    }
}
