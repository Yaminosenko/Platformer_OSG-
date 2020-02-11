using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiegeJoueur : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            Die();
        }

        void Die()
        {

        }
    }
}
