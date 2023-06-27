using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOne : MonoBehaviour
{
    public Transform player;
    bool m_IsPlayerInRange;
    public Animator m_DoorAnimator;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = true;
            DoorAction(m_IsPlayerInRange);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = false;
            DoorAction(m_IsPlayerInRange);
        }
    }

    // Update is called once per frame
    void DoorAction(bool open)
    {
        m_DoorAnimator.SetBool("character_nearby", open);
    }
}
