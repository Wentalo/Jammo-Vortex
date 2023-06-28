using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform salida;
    public GameObject player;
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;
    float m_Timer;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            player.transform.position = new Vector3(salida.position.x, salida.position.y, salida.position.z);

            /*
            m_Timer += Time.deltaTime;
            imageCanvasGroup.alpha = m_Timer / fadeDuration;

            if (m_Timer > fadeDuration + displayImageDuration)
            {
                if (doRestart)
                {
                    SceneManager.LoadScene(0);
                }
            }
            */
            
        }
    }
}
