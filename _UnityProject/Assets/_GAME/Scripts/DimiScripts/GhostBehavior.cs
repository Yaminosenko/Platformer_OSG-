using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class GhostBehavior : InputListener
{
    //LASER

    public float _radiusOffset = 1;
    private Vector3 _offsetShoot;
    private Vector3 _distance;
    private Vector3 _transformShoot;
    private Vector3 _dronePos;
    public Transform _drone;
    private EnergieCharge _myCurrentEnergieCharge;
    private GameObject _MyTarget;

    public LineRenderer _laserVFX;

    private Vector3 _LserLookAt;

    public bool _LaserIsActive = false;


    private Shoot _Shootref;
    float timer = 0;
    public bool _recallWithoutTrail = true;
    public bool _enabledRecall = true;
    public float recallPeriod = 2.0f;
    public List<GhostPosition> _positionGhost = new List<GhostPosition>(500);
    public List<GhostPosition> _positionPlayer = new List<GhostPosition>(500);


    public struct GhostPosition
    {
       public Vector3 _positions;
       public float _time;
    }

    public List<GhostRotation> _rotationGhost = new List<GhostRotation>(500);
    [System.Serializable]
    public struct GhostRotation
    {
        public Quaternion _rotation;
        public float _time;
    }


    public Rewired.Player player;
    public int PlayerID = 0;
    public bool _recallEnabled = false;
    private bool _freezeCharacter = false;
    private bool _freezeGhost = false;
    private CharacterController _characterController;
    private CapsuleCollider _capsuleCharacter;
    private Vector3 _RecallPosition;
    private float _distanceGhostPlayer;
    private Vector3 _getPos;
    [SerializeField]private int _recallCount;
    [SerializeField] private int _indexDebug;
    public ParticleSystem _lazerHit;
    private float _distanceBetweenLaser;

    public int _recallIndex = 5;
    public float _freezeTime = 0.1f;
    public float _recalTime = 0.5f;
    public Transform _ghostTransform;

    private float startTime;

    public Transform[] skeletonJointsGhost;
    private CharacterBehaviour _characterBehivour;
    private Transform _character;


    private float fractionOfJourney;
    private float distCovered;
    private float TimeTravel;
    private Vector3[] ArrayFreeze;
    private int _index;
    [SerializeField] private TrailRenderer _mtrailVFX;
    [SerializeField] private GameObject _RecallTrailmesh;
    
 
    public bool _isOnTravel = false;

    private SkinnedMeshRenderer _mrenderer;
    [SerializeField] private GameObject _skeleton;

    public GameObject _skinRenderer;
    public GameObject[] _meshToDisabe;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _characterController._ghostBehavior = GetComponent<GhostBehavior>();
        _capsuleCharacter = GetComponent<CapsuleCollider>();
        _mrenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _characterBehivour = _characterController.characterBehaviour;
        _characterBehivour._ghostBehaviour = GetComponent<GhostBehavior>();

        InstentiateGhostVariable();




        _Shootref = GetComponent<Shoot>();


        player = ReInput.players.GetPlayer(PlayerID);
        startTime = Time.time;
    }

    void InstentiateGhostVariable()
    {
        Transform _ghostInstantaite = Instantiate(_ghostTransform);
        _ghostTransform = _ghostInstantaite;
        _skeleton = _ghostTransform.GetComponentInChildren<Test>().gameObject;
        skeletonJointsGhost = _skeleton.GetComponentsInChildren<Transform>();
        _characterBehivour.SetGhostAnimator(_ghostInstantaite.GetComponentInChildren<Animator>());
        _character = _ghostInstantaite.GetComponentInChildren<Animator>().transform;
        _drone = _ghostInstantaite.GetComponentInChildren<GhostDroneref>().transform;
        _lazerHit = _ghostInstantaite.GetComponentInChildren<LzrHitRef>().GetComponent<ParticleSystem>();
        _laserVFX = _drone.GetComponentInChildren<LineRenderer>();
    }

    void SetSkeletonPos()
    {
        for (int i = 0; i < skeletonJointsGhost.Length; i++)
        {
            skeletonJointsGhost[i].localPosition = _characterBehivour.jointsInfos[0].localPositions[i];
            skeletonJointsGhost[i].localRotation = _characterBehivour.jointsInfos[0].localRotations[i];
        }
    }
    private void Update()
    {
        _SetDroneAndLAserPOstition();

        #region VFX On Travel
        if (_isOnTravel == true)
        {
            _RecallTrailmesh.SetActive(true);
            //_mrenderer.gameObject.SetActive(false);
            _mtrailVFX.emitting = false;
            if (_skinRenderer != null)
            {
                _skinRenderer.GetComponent<SkinnedMeshRenderer>().enabled = false;
                for (int i = 0; i < _meshToDisabe.Length; i++)
                {
                    _meshToDisabe[i].GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
        if (_isOnTravel == false)
        {
            _RecallTrailmesh.SetActive(false);
            //_mrenderer.gameObject.SetActive(true);
            _mtrailVFX.emitting = true;
            if (_skinRenderer != null)
            {
                _skinRenderer.GetComponent<SkinnedMeshRenderer>().enabled = true;
                for (int i = 0; i < _meshToDisabe.Length; i++)
                {
                    _meshToDisabe[i].GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }

        #endregion
         TrackPositionsPlayer();

        if(_freezeGhost == false)
        {
            //Track de la position du joueur et deplacement du ghost
            TrackPositionsGhost();
            //_ghostTransform.position = _positionGhost[0];
            _ghostTransform.position = _positionGhost[0]._positions;
            _character.rotation = _rotationGhost[0]._rotation;
        }
        if (_freezeCharacter == true)
        {
            //freeze au moment du recall
            transform.position = _getPos;
        }
        //Debug.Log(TimeTravel);
        TimeTravel += Time.deltaTime;
        distCovered = (Time.time - startTime) * _recalTime;

        // Fraction of journey completed equals current distance divided by total distance.
         fractionOfJourney = distCovered / _distanceGhostPlayer;

        if (_recallEnabled == true)
        {
            if(_recallWithoutTrail == true)
            {
                transform.position = Vector3.Lerp(transform.position, _RecallPosition, fractionOfJourney);
            }
            else
            {
                RecallPosition();
                //_recallEnabled = false;
            }
        }

        if (_recallCount != 0)
        {
            RecallWaiting();
        }
    }

    
    //Recall en suivant le trail
    void RecallPosition()
    {
        if(_index > 0)
        {
            transform.position = ArrayFreeze[_index];
            _index = _index-_recallIndex;

        }
        else
        {
            transform.position = _RecallPosition;
            _freezeGhost = false;
            _capsuleCharacter.isTrigger = false;
            _characterController._recallDisableHit = false;
            _recallEnabled = false;
            _isOnTravel = false;
        }

    }

    //Track de la position du joueur
    void TrackPositionsGhost()
    {
        GhostRotation gr = new GhostRotation();
        gr._rotation = _characterBehivour.transform.rotation;
        gr._time = Time.time;
        _rotationGhost.Add(gr);
        _rotationGhost.RemoveAll(e => e._time < Time.time - recallPeriod);
        


        GhostPosition gp = new GhostPosition();
        gp._time = Time.time;
        gp._positions = transform.position;
        _positionGhost.Add(gp);

        _positionGhost.RemoveAll(e => e._time < Time.time - recallPeriod);
        SetSkeletonPos();
    }

    void TrackPositionsPlayer()
    {
        //Debug.Log(positions);
        //if (timer > recallPeriod)
        //{
        //    _positionPlayer.RemoveAt(0);
        //    _positionPlayer.Add(transform.position);
        //}
        //else
        //{
        //    _positionPlayer.Add(transform.position);
        //    timer += Time.deltaTime;
        //}
        GhostPosition gp = new GhostPosition();
        gp._time = Time.time;
        gp._positions = transform.position;
        _positionPlayer.Add(gp);

        _positionPlayer.RemoveAll(e => e._time < Time.time - recallPeriod);
        SetSkeletonPos();

    }

    public void Recall()
    {
        
        if (_recallWithoutTrail == true)
        {
            StartCoroutine(FreezeTime());
            TimeTravel = 0f;
        }
        else
        {
            if(_recallCount == 0)
            {
                StartCoroutine(FreezeTime());
                List<Vector3> FreezeList = new List<Vector3>(500);
                for (int i = 0; i < _positionPlayer.ToArray().Length; i++)
                {
                    FreezeList.Add(_positionPlayer[i]._positions);
                }
               
                ArrayFreeze = FreezeList.ToArray();
                _index = ArrayFreeze.Length - 1;
                Debug.Log(ArrayFreeze.Length);
                _recallCount++;


                //_recallEnabled = true;
            }
            else
            {
                _recallWithoutTrail = true;
            }
        }
    }

    void RecallWaiting()
    {
        if(_indexDebug > ArrayFreeze.Length)
        {
            _indexDebug++;
        }
        else
        {
            _recallCount = 0;
            _recallWithoutTrail = false;
        }
    }

    public void LaserInstantiate()
    {
        _laserVFX.transform.LookAt(_LserLookAt);
        Debug.DrawRay(_drone.position, _drone.TransformDirection(Vector3.forward).normalized, Color.magenta);

        //Ray ray = new Ray(_drone.position, _transformShoot - (_drone.position));
        RaycastHit hit;
        if (Physics.Raycast(_drone.position, _drone.TransformDirection(Vector3.forward).normalized, out hit, Mathf.Infinity))
        {
            _lazerHit.transform.position = hit.point - new Vector3(0, 0, -0.12f);
            _distanceBetweenLaser = Vector3.Distance(hit.point, _drone.position);

            _laserVFX.SetPosition(1, new Vector3(0, 0, _distanceBetweenLaser));

            if (hit.collider.gameObject.name == "CatalyseurDeLaser")
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

    //IEnumerator RecallWaiting()
    //{
    //    yield return new WaitForSeconds(5);
    //}

    //Freeze du joueur en appuyant sur l'input recall
    IEnumerator FreezeTime()
    {
        _getPos = transform.position;
      //  _freezeGhost = true;
      //  _freezeCharacter = true;
        yield return new WaitForSeconds(_freezeTime);
        _freezeCharacter = false;
        _freezeGhost = false;
        _isOnTravel = true;
        startTime = Time.time;
        _RecallPosition = _ghostTransform.position;
        _distanceGhostPlayer = Vector3.Distance(transform.position, _RecallPosition);
        _capsuleCharacter.isTrigger = true;
        _characterController._recallDisableHit = true;
        _recallEnabled = true;
        if(_recallWithoutTrail == true)
        {
            StartCoroutine(DelayRecall());
        }
    }
    //Temps durant lequel le character est en mode recall
    IEnumerator DelayRecall()
    {
        yield return new WaitForSeconds(TimeTravel + 0.25f);
        _freezeGhost = false;
        _capsuleCharacter.isTrigger = false;
        _characterController._recallDisableHit = false;
        _recallEnabled = false;
        _isOnTravel = false;
    }


    private void _SetDroneAndLAserPOstition()
    {
        _drone.position = _Shootref._DroneInformations[0]._positionDrone;
        _drone.rotation = _Shootref._DroneInformations[0]._rotationDrone;
        _LserLookAt = _Shootref._DroneInformations[0]._LookAtLaserDrone;
        bool _laserISActive = _Shootref._DroneInformations[0]._laserActivate;
        if (_laserISActive == true)
        {
            _laserVFX.gameObject.SetActive(true);
            LaserInstantiate();
        }
        else
        {
            _laserVFX.gameObject.SetActive(false);
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.magenta;
        if(_recallEnabled)
        Handles.SphereHandleCap(-1, _RecallPosition, Quaternion.identity, 1, EventType.Repaint);
    }
#endif
}