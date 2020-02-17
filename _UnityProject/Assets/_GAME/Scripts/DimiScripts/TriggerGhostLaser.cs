using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGhostLaser : MonoBehaviour
{
    public bool _ghost;

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 9)
        {
            if(_ghost == true)
            {
                col.GetComponent<GhostBehavior>()._InstanciateRecall = true;
                col.GetComponent<GhostBehavior>().GhostMeshInstanciate();
                Destroy(gameObject);
            }
            else
            {
                col.GetComponent<Shoot>()._instantiateLaser = true;
                Destroy(gameObject);
            }
        }
    }
}
