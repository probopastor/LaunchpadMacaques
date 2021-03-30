using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PushPull : MonoBehaviour
{
    public LayerMask wallLayer;

    private Rigidbody _rigidbody;
    private Camera _camera;
    
    [SerializeField]
    private GameObject pullPos = null;
    
    [SerializeField]
    private GameObject pushParticle = null;
    private ParticleSystem _pullParticle;
    [SerializeField]
    private float startPullParticleSpeed = 5;
    [SerializeField]
    private float pullingParticleSpeed = 50;
    
    public float pullSpeed = 60f;
    public float pushSpeed = 60f;

    [SerializeField]
    private float pullMaxDistance = 10f;
    [SerializeField]
    private float pushMaxDistance = 5f;

    [SerializeField]
    private Image crossHairUI = null;
    [SerializeField]
    private List<Sprite> crossHairSprites = new List<Sprite>();

    [SerializeField]
    private CinemachineVirtualCamera playerCam = null;
    private float _startCamFOV = 60;
    [SerializeField]
    private float pullCamFov = 90;
    [SerializeField]
    private float pushCamFov = 90;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = Camera.main;
        playerCam.m_Lens.FieldOfView = _startCamFOV;
        _pullParticle = pullPos.GetComponentInChildren<ParticleSystem>();
        _pullParticle.Stop();
        crossHairUI.sprite = crossHairSprites[0];
    }

   
    void Update()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        var pullParticleMain = _pullParticle.main;
        if (Physics.Raycast(ray, out RaycastHit particleHit, pullMaxDistance, wallLayer))
        {
            pullPos.transform.position = particleHit.point;
            pullPos.transform.LookAt(transform.position);
            _pullParticle.Play();
        }
        else
        {
            _pullParticle.Stop();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit pullHit, pullMaxDistance, wallLayer))
            {
                pullPos.transform.position = pullHit.point;
                pullPos.transform.parent = pullHit.transform;
                pullParticleMain.startSpeed = pullingParticleSpeed;
                crossHairUI.sprite = crossHairSprites[1];
            }
            else
            {
                pullPos.transform.parent = transform;
                pullPos.transform.position = transform.position;
            }
        }
        
        //pull mechanic
        if (Input.GetMouseButton(0))
        {
            if (pullPos.transform.parent != transform) //&& Physics.Raycast(ray, out RaycastHit hit, pullMaxDistance, wallLayer))
            {
                //_rigidbody.position = Vector3.Lerp(_rigidbody.position, pullPos.transform.position, Time.deltaTime * pullSpeed);

                var forceDir = pullPos.transform.position - transform.position;
                _rigidbody.AddForce(forceDir * pullSpeed, ForceMode.Acceleration);
                
                if (playerCam.m_Lens.FieldOfView < pullCamFov)
                {
                    playerCam.m_Lens.FieldOfView += Time.deltaTime * 30f;
                }
            }
            else if (playerCam.m_Lens.FieldOfView > _startCamFOV)
            {
                playerCam.m_Lens.FieldOfView -= Time.deltaTime * 50f;
            }
        }
        else if (playerCam.m_Lens.FieldOfView > _startCamFOV)
        {
            playerCam.m_Lens.FieldOfView -= Time.deltaTime * 50f;
        }

        if (Input.GetMouseButtonUp(0))
        {
            pullParticleMain.startSpeed = startPullParticleSpeed;
            crossHairUI.sprite = crossHairSprites[0];
        }


        //push mechanic
        if (Input.GetMouseButtonDown(1))
        {
            //shoots a raycast out at X and checks if it hits something on the wallLayer
            if (Physics.Raycast(ray, out RaycastHit hit, pushMaxDistance, wallLayer))
            {
                //pushes the player in the opposite direction of the hit Raycast
                _rigidbody.AddForce(-ray.direction * pushSpeed, ForceMode.Impulse);
                var newPushParticle = Instantiate(pushParticle, hit.point, Quaternion.identity);
                newPushParticle.transform.LookAt(transform.position);
                playerCam.m_Lens.FieldOfView = pushCamFov;
                crossHairUI.sprite = crossHairSprites[2];
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            crossHairUI.sprite = crossHairSprites[0];
        }
    }
}
