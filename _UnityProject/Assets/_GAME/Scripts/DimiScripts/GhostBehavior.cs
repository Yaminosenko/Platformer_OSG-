using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GhostBehavior : InputListener
{
    public Transform _test;
    float timer = 0;
    [SerializeField] float recallPeriod = 2.0f;
    public List<Vector3> positions = new List<Vector3>();
    public Rewired.Player player;
    public int PlayerID = 0;
    public bool IsWorking = false;


   
        private void OnEnable()
    {
        Transform _testIns = Instantiate(_test);
        _test = _testIns;
        player = ReInput.players.GetPlayer(PlayerID);
    }
    private void Update()
    {
        TrackPositions();
        _test.position = positions[0];
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
        
        IsWorking = true;
    }

    //void MakeRecall()
    //{
    //    gameObject.transform.position = 
    //}
}
