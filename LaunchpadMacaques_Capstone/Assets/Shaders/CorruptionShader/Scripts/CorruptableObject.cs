/*
 * This script should be attached to any object that uses the vaporwave corruption shader
 * 
 * The public variables allow you to edit properties of the shader for a single instance,
 * rather than editing them on the material, which affects all objects using that material
 * 
 * 
 */
using UnityEngine;

public class CorruptableObject : MonoBehaviour
{
    // The renderer of this object
    Renderer r;

    // The material property block of the renderer
    MaterialPropertyBlock pBlock;


    public enum CorruptionState
    {
        Uncorrupted, // Fully uncorrupted (normal object)
        Uncorrupting, // Currently un-corrupting
        PausedCorruption, // Partially corrupted, but amount of corruption stays fixed
        Corrupting, // Currently corrupting
        FullyCorrupted // Fully corrupted (all vaporwave)
    }

    [Tooltip("Current state of this object's corruption")]
    public CorruptionState corruptionState;


    [Tooltip("Vaporwave wireframe color")]
    public Color wireframeColor = Color.magenta;

    [Tooltip("Origin point of the corruption")]
    public Vector3 corruptionStartPoint;

    [Tooltip("How far the corruption has spread (>0)")]
    public float corruptionDistance;

    [Tooltip("XYZ: Origin point of corruption. W: Corruption progress")]
    public Vector4[] staticCorruptionPoints;
    Matrix4x4 corruptionPoints;

    // Local coordinates move with object
    // World coordinates are easier for corrupting multiple objects from same point
    [Tooltip("true: Use world coordinates\nfalse: Use local coordinates")]
    public bool useWorldCoordinates;


    // true if object is fully corrupted
    bool corrupted;

    // true if object's corruption is changing
    bool corrupting;

    // true if object is uncorrupting instead of corrupting
    bool reversed;

    // true if corruption is stalled at a specific amount
    bool paused;


    /// <summary>
    /// Update shader properties based on inspector values
    /// </summary>
    private void OnValidate()
    {
        if (corruptionPoints == null) corruptionPoints = new Matrix4x4();
        for (int i = 0; i < 4; i++)
        {
            if (staticCorruptionPoints != null && i < staticCorruptionPoints.Length)
                corruptionPoints.SetRow(i, staticCorruptionPoints[i]);
            else
                corruptionPoints.SetRow(i, new Vector4(0, 0, 0, 0));
        }

        UpdateVariablesFromState();
        UpdateShaderFull();
    }

    /// <summary>
    /// Update shader properties on scene start
    /// </summary>
    private void Start()
    {
        if (corruptionPoints == null) corruptionPoints = new Matrix4x4();
        for (int i = 0; i < 4; i++)
        {
            if (staticCorruptionPoints != null && i < staticCorruptionPoints.Length)
                corruptionPoints.SetRow(i, staticCorruptionPoints[i]);
            else
                corruptionPoints.SetRow(i, new Vector4(0, 0, 0, 0));
        }
        UpdateVariablesFromState();
        UpdateShaderFull();
    }

    private void Update()
    {
        if (corrupting && !paused)
        {
            // increase the corruption amount
            corruptionDistance += Time.deltaTime * (reversed ? 5 : 1);
            corruptionDistance = Mathf.Max(0, corruptionDistance);

            // update shader
            UpdateShaderCorruptionDistance();
        }
    }

    /// <summary>
    /// Tell this object to start corrupting from a specific point (such as grapple point)
    /// </summary>
    /// <param name="startPos"> World space position of point where corruption starts</param>
    public void StartCorrupting(Vector3 startPos)
    {
        // Set the new state
        corruptionState = CorruptionState.Corrupting;

        // Reset amount of corruption
        corruptionDistance = 0;

        // Set the corruption origin
        if (useWorldCoordinates)
        {
            corruptionStartPoint = startPos;
        }
        else
        {
            Vector3 localPos = transform.InverseTransformPoint(startPos);
            corruptionStartPoint = localPos;
        }

        // Update script variables and shader
        UpdateVariablesFromState();
        UpdateShaderFull();
    }

    /// <summary>
    /// Tell this object to start un-corrupting from a specific point (such as a pickup that
    /// removes corruption in an area)
    /// </summary>
    /// <param name="point">World space position of point where uncorruption starts</param>
    public void UncorruptFromPoint(Vector3 point)
    {
        // Set the new state
        corruptionState = CorruptionState.Uncorrupting;
        UpdateVariablesFromState();

        // Set the origin of un-corruption
        corruptionStartPoint = point;
        useWorldCoordinates = true;

        // Reset the amount of corruption / uncorruption
        corruptionDistance = 0;

        // Update shader
        UpdateShaderFull();
    }

    /// <summary>
    /// Instantly remove the corruption effect from the entire object
    /// </summary>
    public void UncorruptInstantly()
    {
        // Set new state
        corruptionState = CorruptionState.Uncorrupted;

        // Update script variables and shader
        UpdateVariablesFromState();
        UpdateShaderFull();
    }

    /// <summary>
    /// Update the boolean variables used in this script based on the current state. Should be
    /// called whenever the state changes.
    /// </summary>
    void UpdateVariablesFromState()
    {
        switch (corruptionState)
        {
            case (CorruptionState.Uncorrupted):
                corrupted = false;
                corrupting = false;
                paused = false;
                reversed = false;
                break;
            case (CorruptionState.Corrupting):
                corrupted = false;
                corrupting = true;
                paused = false;
                reversed = false;
                break;
            case (CorruptionState.PausedCorruption):
                corrupted = false;
                corrupting = true;
                paused = true;
                reversed = false;
                break;
            case (CorruptionState.Uncorrupting):
                corrupted = false;
                corrupting = true;
                paused = false;
                reversed = true;
                break;
            case (CorruptionState.FullyCorrupted):
                corrupted = true;
                corrupting = false;
                paused = false;
                reversed = false;
                break;
        }
    }

    /// <summary>
    /// Update all shader properties to the current values in this script.
    /// </summary>
    void UpdateShaderFull()
    {
        // Grab the renderer and create new property block if necessary
        if (r == null)
            r = GetComponent<Renderer>();
        if (pBlock == null)
            pBlock = new MaterialPropertyBlock();

        // Copy the renderer's property block
        r.GetPropertyBlock(pBlock);

        // Set shader properties based on variables
        pBlock.SetColor("WireframeColor", wireframeColor);
        pBlock.SetVector("CorruptionStartPos", corruptionStartPoint);
        pBlock.SetFloat("CorruptionProgress", corruptionDistance);
        pBlock.SetFloat("UseWorldCoordinates", useWorldCoordinates ? 1 : 0);

        pBlock.SetFloat("Corrupted", corrupted ? 1 : 0);
        pBlock.SetFloat("Corrupting", corrupting ? 1 : 0);
        pBlock.SetFloat("Reversed", reversed ? 1 : 0);

        pBlock.SetMatrix("CorruptionPoints", corruptionPoints);

        // Set new property block in renderer
        r.SetPropertyBlock(pBlock);
    }

    /// <summary>
    /// Updates only the amount of corruption on an object. This is called in update whenever the
    /// amount of corruption is changing.
    /// </summary>
    void UpdateShaderCorruptionDistance()
    {
        // Grab the renderer and create new property block if necessary
        if (r == null)
            r = GetComponent<Renderer>();
        if (pBlock == null)
            pBlock = new MaterialPropertyBlock();

        // Copy the renderer's property block
        r.GetPropertyBlock(pBlock);

        // Set the corruption distance for shader
        pBlock.SetFloat("CorruptionProgress", corruptionDistance);

        // Set new property block in renderer
        r.SetPropertyBlock(pBlock);
    }

}
