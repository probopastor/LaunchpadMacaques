using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class Player_Audio : MonoBehaviour
{
    public float playerTopSpeed;
    public LayerMask collisions;
    public float painThreshold;
    
    private Rigidbody m_rb;
    private bool grounded;
    private bool landed;

    private static float magnitude;

    [Header("FMOD References")]
    [EventRef] public string landing;
    [EventRef] public string jumping;
    [EventRef] public string footsteps;
    [EventRef] public string[] painGrunts;
    public StudioEventEmitter swingingEmitter;

    private EventInstance swingInstance;
    private EventInstance landInstance;
    private EventInstance jumpInstance;
    private EventInstance footstepInstance;

    private PauseManager pauseManager;
    private Matt_PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<Matt_PlayerMovement>();
        pauseManager = FindObjectOfType<PauseManager>();
        m_rb = GetComponent<Rigidbody>();

        grounded = true;
        landed = true;

        jumpInstance = RuntimeManager.CreateInstance(jumping);
        footstepInstance = RuntimeManager.CreateInstance(footsteps);
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseManager.GetPaused()) swingingEmitter.Stop();
        else if (!swingingEmitter.IsPlaying()) swingingEmitter.Play();
        jumpInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        footstepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));


        if (Physics.Raycast(transform.position, Vector3.down, 4f, collisions)) grounded = true;
        else grounded = false;

        if (grounded && !landed)
        {   
            landed = true;
            landInstance = RuntimeManager.CreateInstance(landing);
            landInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            landInstance.setParameterByName("parameter:/Player Magnitude", GetMagnitude());
            landInstance.start();
            if (GetMagnitude() > painThreshold) PlayRandom(painGrunts);
        }

        landed = grounded;

        PLAYBACK_STATE footState;
        footstepInstance.getPlaybackState(out footState);
        if (grounded && landed && footState == PLAYBACK_STATE.STOPPED && magnitude > 0.1f)
        {
            footstepInstance.start();
            //Debug.Log("Starting footsteps");
        }

        if ((footState == PLAYBACK_STATE.PLAYING && (!grounded || magnitude < 0.1f)) || pauseManager.GetPaused())
        {
            footstepInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            //Debug.Log("stop footsteps");
        }
    }

    private void LateUpdate()
    {
        magnitude = Mathf.Lerp(magnitude, m_rb.velocity.magnitude / playerTopSpeed, 0.1f);
        if (Input.GetKeyDown(KeyCode.M)) Debug.Log(magnitude);
        swingingEmitter.SetParameter("parameter:/Player Magnitude", magnitude);
    }

    public static float GetMagnitude()
    {
        return magnitude;
    }

    public void PlayRandom(string[] vs)
    {
        string randEvent = vs[Random.Range(0, vs.Length)];
        EventInstance randInstance = RuntimeManager.CreateInstance(randEvent);
        randInstance.start();
        randInstance.release();
    }
}
