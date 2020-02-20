using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGhostLaser : MonoBehaviour
{
    public bool _ghost;
    public bool _laser;

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 9)
        {
            if(_ghost == true)
            {
                col.GetComponent<GhostBehavior>()._InstanciateRecall = true;
                col.GetComponent<GhostBehavior>().GhostMeshInstanciate();
                
            }
            if(_laser == true)
            {
                col.GetComponent<Shoot>()._instantiateLaser = true;
                col.GetComponent<Shoot>()._disableLaser = false;
                
            }
        }

        Destroy(gameObject);
    }
}
