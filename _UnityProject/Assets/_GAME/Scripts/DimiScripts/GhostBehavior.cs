using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GhostBehavior : InputListener
{
    public Transform _ghostTransform;
    float timer = 0;
    [SerializeField] float recallPeriod = 2.0f;
    public List<Vector3> positions = new List<Vector3>();
    public Rewired.Player player;
    public int PlayerID = 0;
    public bool IsWorking = false;

    private Vector3 _RecallPosition;

    public float _recalTime = 0.5f;
   


    private void OnEnable()
    {
        Transform _testIns = Instantiate(_ghostTransform);
        _ghostTransform = _testIns;
        player = ReInput.players.GetPlayer(PlayerID);
    }
    private void Update()
    {
        TrackPositions();
        _ghostTransform.position = positions[0];



       

        

        if(IsWorking == true)
        {
            transform.position = Vector3.Lerp(transform.position, _RecallPosition,_recalTime*Time.deltaTime );
        }

      
    }
    void TrackPositions()
    {
        Debug.Log(positions);
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
        Debug.Log(this.gameObject);

        _RecallPosition = _ghostTransform.position;
        IsWorking = true;

    }


    IEnumerator DelayRecall()
    {
        yield return new WaitForSeconds(_recalTime);
        IsWorking = false;
    }
}
