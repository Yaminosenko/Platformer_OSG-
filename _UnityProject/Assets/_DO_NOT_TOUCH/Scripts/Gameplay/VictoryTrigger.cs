using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
            GameManager.Instance.LevelEnd();
    }
}
