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
                //col.GetComponent<GhostBehavior>().InstentiateGhostVariable();
                //col.GetComponent<GhostBehavior>()._enabledRecall = true;

                //col.GetComponent<GhostBehavior>()._ghostRenderer.enabled = true;
                //for (int i = 0; i < 2; i++)
                //{
                //    col.GetComponent<GhostBehavior>()._ghostMesh[i].enabled = true;
                //}

                //StartCoroutine(col.GetComponent<GhostBehavior>()._characterBehivour.DeleteAnim());
                //Destroy(gameObject);
            }
            else
            {
                col.GetComponent<GhostBehavior>()._enabledLaser = true;
                Destroy(gameObject);
            }
        }
    }
}
