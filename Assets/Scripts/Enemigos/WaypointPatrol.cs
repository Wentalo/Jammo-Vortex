using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;  //Lista de los puntos por los que irá

    int m_CurrentWaypointIndex;

    void Start()
    {
        navMeshAgent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            m_CurrentWaypointIndex = m_CurrentWaypointIndex + 1;
            if (m_CurrentWaypointIndex > (waypoints.Length - 1)) m_CurrentWaypointIndex = 0;                    
        }
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }
}
