using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Transform _mainCameraRotation;

    [SerializeField]
    private float playerSpeed = 1f;
    [SerializeField, Tooltip("Max speed the player can move at")]
    private float maxPlayerSpeed = 8f;
    [SerializeField]
    private float maxAirSpeed = 10f;
    private Vector2 _movementDir = Vector2.zero;
    [SerializeField] private float jumpForce = 10;
    private bool _inAir = false;
    
    private int _collectiblesCollected = 0;
    private int _totalCollectibles;
    public TextMeshProUGUI sphereText;
    private Vector3 _startingPosition;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _mainCameraRotation = FindObjectOfType<CinemachineBrain>().transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _totalCollectibles = GameObject.FindGameObjectsWithTag("Collectible").Length;
        sphereText.text = "Collected Spheres: " + _collectiblesCollected + " / " + _totalCollectibles;
        _startingPosition = transform.position;
    }

    private void Update()
    {
        MoveToSpawn();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_inAir && Physics.Raycast(transform.position, Vector3.down, 1f))
        {
            _inAir = false;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        _inAir = true;
    }

    void MovePlayer()
    {
        Vector3 velocity = _rigidbody.velocity;
        var camRot = _mainCameraRotation.rotation;
        camRot.eulerAngles = new Vector3(0, camRot.eulerAngles.y, 0);
        if (_inAir)
        {
            velocity += camRot * Vector3.ClampMagnitude(new Vector3(_movementDir.x, 0, _movementDir.y) * playerSpeed, maxAirSpeed);
        }
        else
        {
            velocity += camRot * Vector3.ClampMagnitude(new Vector3(_movementDir.x, 0, _movementDir.y) * playerSpeed, maxPlayerSpeed);
        }
        velocity = new Vector3(velocity.x, _rigidbody.velocity.y, velocity.z);
        _rigidbody.velocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            //Debug.Log("Got it!");
            _collectiblesCollected++;
            Destroy(other.gameObject);
            sphereText.text = "Collected Spheres: " + _collectiblesCollected + " / " + _totalCollectibles;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementDir = context.performed ? context.ReadValue<Vector2>() : Vector2.zero;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1f))
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void MoveToSpawn()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _rigidbody.isKinematic = true;
            transform.position = _startingPosition;
            _rigidbody.isKinematic = false;
        }
    }
}
