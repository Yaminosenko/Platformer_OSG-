using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using FigmentGames;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Shoot : InputListener
{
    private CharacterController _characterControler;
    public Rewired.Player player;
    public int PlayerID = 0;
    //private Camera camera;
    public float _radiusOffset = 1;
    public GameObject _blasterShot;
    public float _speedBlaster = 10f;
    private Vector3 _mousePos;
    private GameObject cameraVirt;
    private Camera camera;
    private float VectorDistance;
    private float _direction;
    private Vector3 _offsetShoot;
    private Vector3 _distance;
    private Vector3 _transformShoot;
    private Vector3 directionOnly;
    public GameObject _offsetBlaster;


    private void OnEnable()
    {
        
        player = ReInput.players.GetPlayer(PlayerID);
        cameraVirt = GameObject.Find("CameraController2D");
        camera = cameraVirt.GetComponent<Camera>();
        Debug.Log(camera);
    }

    private void Update()
    {
        UpdateOffset();
        UpdateMousePosition();

        if (Input.GetMouseButtonDown(0))
        {
            SpawnProjectile();
        }
    }

    void UpdateOffset()
    {
        _transformShoot = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        Vector3 difference = _mousePos - _transformShoot;
        float distance = difference.magnitude;
        Vector3 directionOnly = difference.normalized;
        Vector3 pointAlongDirection = _transformShoot + (directionOnly * _radiusOffset);

        _offsetShoot = pointAlongDirection;

        _offsetBlaster.transform.position = _offsetShoot;
        _offsetBlaster.transform.LookAt(_mousePos) ;

        
    }

    void UpdateMousePosition()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            _mousePos = new Vector3(hit.point.x, hit.point.y, 0);
        }
    }


    void SpawnProjectile()
    {
        GameObject _blasterInstanciate = Instantiate(_blasterShot, _offsetBlaster.transform.position, _offsetBlaster.transform.rotation);
        _blasterInstanciate.GetComponent<BlasterScript>().speedVelocity = _speedBlaster;
       Destroy(_blasterInstanciate,2f);
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.SphereHandleCap(-1, _mousePos, Quaternion.identity, 1, EventType.Repaint);

        Handles.color = Color.yellow;
        Handles.CircleHandleCap(-1, _transformShoot, Quaternion.identity, _radiusOffset, EventType.Repaint);
        Handles.DrawLine(_offsetShoot, _mousePos);
        Handles.SphereHandleCap(-1, _offsetShoot, Quaternion.identity, 1, EventType.Repaint);
        Handles.color = Color.cyan;

        Handles.ArrowHandleCap(-1, _offsetShoot, _offsetBlaster.transform.rotation, 5, EventType.Repaint);
    }
#endif

}
