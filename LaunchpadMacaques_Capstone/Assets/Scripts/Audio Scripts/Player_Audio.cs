using FMODUnity;
using UnityEngine;

public class Player_Audio : MonoBehaviour
{
    public float PLAYER_TOP_SPEED;
    public LayerMask collisions;
    public LayerMask corruption;
    public float corruptionAmbientMaxRange;

    public ScriptableEmitter[] emitters;

    private StudioEventEmitter m_swing;
    private StudioEventEmitter m_land;
    private StudioEventEmitter m_footsteps;
    private StudioEventEmitter m_corruption;

    private Rigidbody m_rb;
    private bool grounded;
    private bool landed;

    private static float magnitude;

    void Start()
    {
        m_swing = AudioUtilities.FindEmitter(emitters, "Swing");
        m_land = AudioUtilities.FindEmitter(emitters, "Land");
        m_footsteps = AudioUtilities.FindEmitter(emitters, "Footsteps");
        m_corruption = AudioUtilities.FindEmitter(emitters, "Corruption");

        m_rb = GetComponent<Rigidbody>();

        grounded = true;
        landed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.5f, collisions)) grounded = true;
        else grounded = false;

        if (grounded && !landed)
        {   
            landed = true;
            m_land.Play();
        }

        landed = grounded;

        if (grounded && landed && !m_footsteps.IsPlaying() && magnitude > 0.1f)
        {
            m_footsteps.Play();
        }

        if (m_footsteps.IsPlaying() && (!grounded || magnitude < 0.1f))
        {
            m_footsteps.Stop();
        }

    }

    private void LateUpdate()
    {
        magnitude = Mathf.Lerp(magnitude, m_rb.velocity.magnitude / PLAYER_TOP_SPEED, 0.1f);
        if (Input.GetKeyDown(KeyCode.M)) Debug.Log(magnitude);
        m_swing.SetParameter("Magnitude", magnitude);
        m_land.SetParameter("Magnitude", magnitude);
        m_footsteps.SetParameter("Magnitude", magnitude);

        //ambience
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, corruptionAmbientMaxRange, transform.forward, 1, corruption);
        Transform nearestCorruptable = null;
        foreach (RaycastHit hit in hits)
        {
            if (nearestCorruptable == null)
            {
                nearestCorruptable = hit.transform;
                continue;
            }

            if (Vector3.Distance(hit.transform.position, transform.position) < Vector3.Distance(nearestCorruptable.position, transform.position))
            {
                nearestCorruptable = hit.transform;
                continue;
            }
        }

        if (nearestCorruptable != null)
        {
            m_corruption.SetParameter("Proximity", (corruptionAmbientMaxRange - Vector3.Distance(nearestCorruptable.position, transform.position)) / corruptionAmbientMaxRange);

        }
    }

    public static float GetMagnitude()
    {
        return magnitude;
    }
}
