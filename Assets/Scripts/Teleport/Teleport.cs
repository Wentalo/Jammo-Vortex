using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform salida;
    public GameObject player;

    public float fadeDuration = 0.05f;
    public float displayImageDuration = 0.3f;
    public CanvasGroup teleportBackgroundImageCanvasGroup;
    public float m_Timer;
    public bool m_FadeIn = false;
    public bool m_FadeOut = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            m_FadeIn = true;         
        }
    }

    void Update()  
    {
        if (m_FadeIn)
        {
            if (teleportBackgroundImageCanvasGroup.alpha < 1)
            {
                teleportBackgroundImageCanvasGroup.alpha += Time.deltaTime;
                if(teleportBackgroundImageCanvasGroup.alpha >= 1)
                {
                    m_FadeIn = false;
                    m_FadeOut = true;
                    player.transform.position = new Vector3(salida.position.x, salida.position.y, salida.position.z);
                }
            }
        }

        if (m_FadeOut)
        {
            if (teleportBackgroundImageCanvasGroup.alpha >= 0)
            {
                teleportBackgroundImageCanvasGroup.alpha -= Time.deltaTime;
                if (teleportBackgroundImageCanvasGroup.alpha == 0)
                {
                    m_FadeOut = false;
                }
            }
        }

    }

    void FadeInTeleport(CanvasGroup imageCanvasGroup)
    {
        
        m_Timer += Time.deltaTime;

        imageCanvasGroup.alpha += Time.deltaTime;

        if (m_Timer > fadeDuration + displayImageDuration)
        {
            m_FadeIn = false;
            m_FadeOut = true;
        }
        
    }

    void FadeOutTeleport(CanvasGroup imageCanvasGroup)
    {

        m_Timer += Time.deltaTime;

        imageCanvasGroup.alpha = -m_Timer / fadeDuration;

    }

}
