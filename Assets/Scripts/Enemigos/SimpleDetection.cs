using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDetection : MonoBehaviour
{
    public Transform player;
    public GameEndings gameEnding;
    bool m_isPlayerInRange;

    public Vector3 direccionJammoEnemigo;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            Debug.Log("Pillado");
            m_isPlayerInRange = true;
        }
    }
  
    void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            Debug.Log("No pillado");
            m_isPlayerInRange = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (m_isPlayerInRange)
        {
            Debug.Log("Pillado");

            Vector3 direction = player.position - transform.position + Vector3.up;
            direccionJammoEnemigo = direction;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.transform == player)
                {
                    gameEnding.CaughtPlayer();
                    Debug.Log("Pillado");
                }
            }        
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Debug.DrawLine(origin, origin + direction * currentHitDistance);
        Debug.DrawRay(transform.position, direccionJammoEnemigo , Color.blue);
        // Gizmos.DrawWireSphere(origin + direction * currentHitDistance, cameraCollisionRadius);
    }
}