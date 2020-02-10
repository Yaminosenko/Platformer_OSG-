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

    public int _recallIndex = 5;
    public float _freezeTime = 0.1f;
    public float _recalTime = 0.5f;
    public Transform _ghostTransform;

    private float startTime;

    private float fractionOfJourney;
    private float distCovered;
    private float TimeTravel;
    private Vector3[] ArrayFreeze;
    private int _index;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _capsuleCharacter = GetComponent<CapsuleCollider>();

        Transform _ghostInstantaite = Instantiate(_ghostTransform);
        _ghostTransform = _ghostInstantaite;

        //_ghostInstantaite.gameObject.GetComponentInChildren<GhostController>().characterController = _characterController;
        //_characterController._ghostAnim = _ghostInstantaite.gameObject.GetComponentInChildren<GhostController>();


        player = ReInput.players.GetPlayer(PlayerID);
        startTime = Time.time;

    }
    private void Update()
    {
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
    }

    //Recall en suivant le trail
    void RecallPosition()
    {

        //for (int i = ArrayFreeze.Length-1; i > 0; i--)
        //{
        //    transform.position = ArrayFreeze[i];
        //}
        if(_index > 0)
        {
            transform.position = ArrayFreeze[_index];
            _index = _index-_recallIndex;

        }
        else
        {
            _freezeGhost = false;
            _capsuleCharacter.isTrigger = false;
            _characterController._recallDisableHit = false;
            _recallEnabled = false;

        }

    }

    //Track de la position du joueur
    void TrackPositionsGhost()
    {
        //Debug.Log(positions);
        if (timer > recallPeriod)
        {
            _positionGhost.RemoveAt(0);
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
        //Debug.Log("hi");
        if (_recallWithoutTrail == true)
        {
            StartCoroutine(FreezeTime());
            TimeTravel = 0f;
        }
        else
        {
            StartCoroutine(FreezeTime());
            List<Vector3> FreezeList = new List<Vector3>();
            FreezeList = _positionPlayer;
            ArrayFreeze = FreezeList.ToArray();
            _index = ArrayFreeze.Length - 1;
            Debug.Log(ArrayFreeze.Length);
            //_recallEnabled = true;
        }
    }

    //Freeze du joueur en appuyant sur l'input recall
    IEnumerator FreezeTime()
    {
        _getPos = transform.position;
        _freezeGhost = true;
        _freezeCharacter = true;
        yield return new WaitForSeconds(_freezeTime);
        _freezeCharacter = false;
        _freezeGhost = false;
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
        yield return new WaitForSeconds(TimeTravel + 0.5f);
        _freezeGhost = false;
        _capsuleCharacter.isTrigger = false;
        _characterController._recallDisableHit = false;
        _recallEnabled = false;
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