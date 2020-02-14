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
    public GhostBehavior _GhostBehaviorRef;
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
    private Vector3 _dronePos;
    public float _deadZone = 0.5f;
    public Transform _drone;


    private GameObject _MyTarget;
    private EnergieCharge _myCurrentEnergieCharge;

    public Vector2 _rightStickAxis;

    public LineRenderer _laserVFX;

    private Vector3 _LserLookAt;

    public bool _LaserIsActive = false;

    public List<DroneInfos> _DroneInformations = new List<DroneInfos>(500);
    [System.Serializable]

    public struct DroneInfos
    {
        public Vector3 _positionDrone;
        public Quaternion _rotationDrone;
        public bool _laserActivate;
        public float _TimeDrone;
        public Vector3 _LookAtLaserDrone;
    }


    private void OnEnable()
    {
        
        player = ReInput.players.GetPlayer(PlayerID);
        cameraVirt = GameObject.Find("CameraController2D");
        camera = cameraVirt.GetComponent<Camera>();
        _GhostBehaviorRef = GetComponent<GhostBehavior>();
    }

    private void Update()
    {
        DronePosition();
        UpdateOffset();
        UpdateMousePosition();
        ArrawIncrementation();


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
        _offsetBlaster.transform.LookAt(_mousePos);
        //_offsetBlaster.transform.rotation = Quaternion.Euler(0, 0, 0);

        
    }


    private void DronePosition()
    {
        _dronePos = _transformShoot;

        if (_rightStickAxis.x > _deadZone  || _rightStickAxis.x < -_deadZone || _rightStickAxis.y >_deadZone || _rightStickAxis.y<-_deadZone)
        {


            _dronePos = new Vector3(_rightStickAxis.x, _rightStickAxis.y, 0) * _radiusOffset + _transformShoot;
            var dirJoystick = new Vector3(_rightStickAxis.x, _rightStickAxis.y, 0);



            Vector3 difference = _dronePos - _transformShoot;
            float distance = difference.magnitude;
            Vector3 directionOnly = difference.normalized;
            Vector3 pointAlongDirection = _transformShoot + (directionOnly * _radiusOffset);

            _dronePos = pointAlongDirection;

        }
        _drone.transform.LookAt(new Vector3(_rightStickAxis.x, _rightStickAxis.y, 0) * (_radiusOffset * 2) + _transformShoot);
        _drone.position = Vector3.Lerp(_drone.position, _dronePos, 0.5f);

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

    public void LaserInstantiate()
    {

        _LserLookAt = new Vector3(_rightStickAxis.x, _rightStickAxis.y, 0) * (_radiusOffset * 2) + _transformShoot;
        //Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.forward);

        _laserVFX.transform.LookAt(_LserLookAt);
        Debug.DrawRay(_drone.position, _drone.TransformDirection(Vector3.forward).normalized, Color.magenta);

        //Ray ray = new Ray(_drone.position, _transformShoot - (_drone.position));
        RaycastHit hit;
        if(Physics.Raycast(_drone.position,_drone.TransformDirection(Vector3.forward).normalized, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(_drone.position, _drone.TransformDirection(Vector3.forward).normalized, Color.magenta);
            Debug.Log(hit.collider.name);
            if(hit.collider.gameObject.name == "CatalyseurDeLaser")
            {
                _MyTarget = hit.collider.gameObject;

                _myCurrentEnergieCharge = _MyTarget.GetComponent<EnergieCharge>();
                _myCurrentEnergieCharge.chargerecieve();

            }
        }
        else
        {
            Debug.DrawRay(_drone.position, _drone.TransformDirection(Vector3.forward).normalized, Color.magenta);
        }
    }

    void SpawnProjectile()
    {
        GameObject _blasterInstanciate = Instantiate(_blasterShot, _offsetBlaster.transform.position, _offsetBlaster.transform.rotation);
        _blasterInstanciate.GetComponent<BlasterScript>().speedVelocity = _speedBlaster;
       Destroy(_blasterInstanciate,2f);
        
    }

    //protected override void GetAxis(InputActionEventData data)
    //{
    //    // All inputs locked
    //    if (lockAllInputs)
    //        return;

    //    switch (data.actionName)
    //    {

    //    }

    //    base.GetAxis(data);
    //}

        void ArrawIncrementation()
    {
        DroneInfos Di = new DroneInfos();

        Di._positionDrone = _drone.position;
        Di._rotationDrone = _drone.rotation;
        Di._TimeDrone = Time.time;
        Di._LookAtLaserDrone = _LserLookAt;
        Di._laserActivate = _LaserIsActive;
        _DroneInformations.Add(Di);

        _DroneInformations.RemoveAll(e => e._TimeDrone < Time.time - _GhostBehaviorRef.recallPeriod );
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
        Handles.SphereHandleCap(-1, new Vector3(_rightStickAxis.x, _rightStickAxis.y, 0) * (_radiusOffset*2) + _transformShoot, Quaternion.identity, 1, EventType.Repaint);
    }
#endif

}
