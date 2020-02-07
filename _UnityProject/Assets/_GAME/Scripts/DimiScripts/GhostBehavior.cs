using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GhostBehavior : InputListener
{
    float timer = 0;
    [SerializeField] float recallPeriod = 2.0f;
    public List<Vector3> positions = new List<Vector3>();
    public Rewired.Player player;
    public int PlayerID = 0;
    public bool IsWorking = false;
    private bool _freezeTimeRecall;
    private CharacterController _characterController;
    private CapsuleCollider _capsuleCharacter;
    private Vector3 _RecallPosition;
    private float _distanceGhostPlayer;
    private Vector3 _getPos;

    public float _freezeTime = 0.1f;
    public float _recalTime = 0.5f;
    public Transform _ghostTransform;

    private float startTime;

    private float fractionOfJourney;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _capsuleCharacter = GetComponent<CapsuleCollider>();

        Transform _testIns = Instantiate(_ghostTransform);
        _ghostTransform = _testIns;
        player = ReInput.players.GetPlayer(PlayerID);
        startTime = Time.time;

    }
    private void Update()
    {
        if(_freezeTimeRecall == false)
        {
            TrackPositions();
            _ghostTransform.position = positions[0];
        }
        else
        {
            
            transform.position = _getPos;
        }









        float distCovered = (Time.time - startTime) * _recalTime;

        // Fraction of journey completed equals current distance divided by total distance.
         fractionOfJourney = distCovered / _distanceGhostPlayer;

        if (IsWorking == true)
        {
            transform.position = Vector3.Lerp(transform.position, _RecallPosition,fractionOfJourney);
            StartCoroutine(DelayRecall());

        }



      
    }
    void TrackPositions()
    {
        //Debug.Log(positions);
        if (timer > recallPeriod)
        {
            positions.RemoveAt(0);
            positions.Add(transform.position);
        }
        else
        {
            positions.Add(transform.position);
            timer += Time.deltaTime;
        }
    }
    public void Recall()
    {
        StartCoroutine(FreezeTime());
     
        

      

    }


    IEnumerator FreezeTime()
    {
        _getPos = transform.position;
        _freezeTimeRecall = true;
        yield return new WaitForSeconds(_freezeTime);
        _freezeTimeRecall = false;
        startTime = Time.time;
        Debug.Log(this.gameObject);
        _RecallPosition = _ghostTransform.position;
        _distanceGhostPlayer = Vector3.Distance(transform.position, _RecallPosition);
        _capsuleCharacter.isTrigger = true;
        _characterController._recallDisableHit = true;
        IsWorking = true;
    }

    IEnumerator DelayRecall()
    {
        yield return new WaitForSeconds(0.5f);
        _capsuleCharacter.isTrigger = false;
        _characterController._recallDisableHit = false;
        IsWorking = false;
    }
}
