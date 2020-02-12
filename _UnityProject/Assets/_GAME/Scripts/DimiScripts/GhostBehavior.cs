using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class GhostBehavior : InputListener
{
    float timer = 0;
    public bool _recallWithoutTrail = true;
    public bool _enabledRecall = true;
    public float recallPeriod = 2.0f;
    public List<Vector3> _positionGhost = new List<Vector3>();
    public List<Vector3> _positionPlayer = new List<Vector3>();
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

    public int _recallIndex = 5;
    public float _freezeTime = 0.1f;
    public float _recalTime = 0.5f;
    public Transform _ghostTransform;

    private float startTime;

    public Transform[] skeletonJointsGhost;
    public List<JointsInfoGhost> jointsInfosGhost = new List<JointsInfoGhost>(500);
    [System.Serializable]
    public struct JointsInfoGhost
    {
        public Vector3[] localPositionsGhost;
        public Quaternion[] localRotationsGhost;
        public float timeGhost;
    }
    private CharacterBehaviour _characterBehivour;


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


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _capsuleCharacter = GetComponent<CapsuleCollider>();
        _mrenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _characterBehivour = _characterController.characterBehaviour;

        Transform _ghostInstantaite = Instantiate(_ghostTransform);
        _ghostTransform = _ghostInstantaite;
        _skeleton = _ghostTransform.GetComponentInChildren <Test>().gameObject;
        skeletonJointsGhost = _skeleton.GetComponentsInChildren<Transform>();
        




        //_characterController.characterBehaviour.SetGhostAnimator(_ghostInstantaite.GetComponentInChildren<Animator>());
        //_ghostInstantaite.gameObject.GetComponentInChildren<GhostController>().characterController = _characterController;
        //_characterController._ghostAnim = _ghostInstantaite.gameObject.GetComponentInChildren<GhostController>();


        player = ReInput.players.GetPlayer(PlayerID);
        startTime = Time.time;
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
        

        #region VFX On Travel
        if (_isOnTravel == true)
        {
            _RecallTrailmesh.SetActive(true);
            _mrenderer.gameObject.SetActive(false);
            _mtrailVFX.emitting = false;
            _skeleton.SetActive(false);
        }
        if (_isOnTravel == false)
        {
            _RecallTrailmesh.SetActive(false);
            _mrenderer.gameObject.SetActive(true);
            _mtrailVFX.emitting = true;
            _skeleton.SetActive(true);
        }

        #endregion
         TrackPositionsPlayer();

        if(_freezeGhost == false)
        {
            //Track de la position du joueur et deplacement du ghost
            TrackPositionsGhost();
            _ghostTransform.position = _positionGhost[0];
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
        //Debug.Log(positions);
        if (timer > recallPeriod)
        {
            _positionGhost.RemoveAt(0);
            SetSkeletonPos();
            _positionGhost.Add(transform.position);
        }
        else
        {
            _positionGhost.Add(transform.position);
            timer += Time.deltaTime;
            //Debug.Log(timer);
        }
    }

    void TrackPositionsPlayer()
    {
        //Debug.Log(positions);
        if (timer > recallPeriod)
        {
            _positionPlayer.RemoveAt(0);
            _positionPlayer.Add(transform.position);
        }
        else
        {
            _positionPlayer.Add(transform.position);
            timer += Time.deltaTime;
        }
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
                List<Vector3> FreezeList = new List<Vector3>();
                FreezeList = _positionPlayer;
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

    //IEnumerator RecallWaiting()
    //{
    //    yield return new WaitForSeconds(5);
    //}

    //Freeze du joueur en appuyant sur l'input recall
    IEnumerator FreezeTime()
    {
        _getPos = transform.position;
        _freezeGhost = true;
        _freezeCharacter = true;
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.magenta;
        if(_recallEnabled)
        Handles.SphereHandleCap(-1, _RecallPosition, Quaternion.identity, 1, EventType.Repaint);
    }
#endif
}