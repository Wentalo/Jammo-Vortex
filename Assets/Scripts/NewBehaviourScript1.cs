using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript1 : MonoBehaviour
{
    public Transform player;
    public Transform next;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            player.transform.position = new Vector3(next.position.x, next.position.y, next.position.z);
        }
    }
}
