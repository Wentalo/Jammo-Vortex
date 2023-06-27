using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    //Se pone tanto en el padre como en el hijo(?)

    [SerializeField] private WaypointPath m_waypointPath;
    [SerializeField] float m_speed;

    private int m_targetWaypointIndex;

    private Transform m_previousWaypoint;
    private Transform m_targetWaypoint;

    private float m_timeToWaypoint;
    private float m_elapsedTime;


    void Start()
    {
        TargetNextWaypoint();
    }

    void FixedUpdate()
    {
        m_elapsedTime += Time.deltaTime;

        float elapsedPercentage = m_elapsedTime / m_timeToWaypoint;     //Sabemos el porcentaje de camino recorrido
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);  //En los limites se suaviza el movimiento
        transform.position = Vector3.Lerp(m_previousWaypoint.position, m_targetWaypoint.position, elapsedPercentage);       //Cambiara de posicion según la cantidad de camino recorrido
        transform.rotation = Quaternion.Lerp(m_previousWaypoint.rotation, m_targetWaypoint.rotation, elapsedPercentage);    //Cambiara de rotacion según la cantidad de camino recorrido

        if (elapsedPercentage >= 1)
        {
            TargetNextWaypoint();
        }
    }

    private void TargetNextWaypoint()
    {
        m_previousWaypoint = m_waypointPath.GetWaypoint(m_targetWaypointIndex);
        m_targetWaypointIndex = m_waypointPath.GetNextWaypointIndex(m_targetWaypointIndex);
        m_targetWaypoint = m_waypointPath.GetWaypoint(m_targetWaypointIndex);

        m_elapsedTime = 0;

        //Asi calculas lo que tardas en llegar
        float distanceToWaypoint = Vector3.Distance(m_previousWaypoint.position, m_targetWaypoint.position);
        m_timeToWaypoint = distanceToWaypoint / m_speed; 
    }


    //Necesarios para que el personaje se mueva con la plataforma (emparentar)
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
