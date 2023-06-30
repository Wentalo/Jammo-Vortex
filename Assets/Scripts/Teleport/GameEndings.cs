using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndings : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    public float displayImageDuration = 1f;
    public GameObject player;
    public Transform startPoint;
    public CanvasGroup victoryBackgroundImageCanvasGroup;
    public CanvasGroup caughtBackgroundImageCanvasGroup;

    public AudioSource teleportAudio;
    public AudioSource endingAudio;
    bool m_HasAudioPlayed;

    bool m_IsPlayerAtExit;
    bool m_IsPlayerCaught;
    float m_Timer;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            m_IsPlayerAtExit = true;
        }
    }

    public void CaughtPlayer()
    {
        m_IsPlayerCaught = true;
    }


    void Update()
    {
        
        if (m_IsPlayerAtExit)
        {
            EndLevel(victoryBackgroundImageCanvasGroup, false, endingAudio);
        }
        else if (m_IsPlayerCaught)
        {
            EndLevel(caughtBackgroundImageCanvasGroup, true, teleportAudio);
        }
        
    }

    void EndLevel(CanvasGroup imageCanvasGroup, bool doRestart, AudioSource audioSource)
    {
        if (!m_HasAudioPlayed)
        {
            audioSource.Play();
            m_HasAudioPlayed = true;
        }

        m_Timer += Time.deltaTime;

        imageCanvasGroup.alpha = m_Timer / fadeDuration;

        if (m_Timer > fadeDuration + displayImageDuration)
        {
            if (doRestart)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                Application.Quit();
            }
        }
        else if (m_Timer > ((fadeDuration + displayImageDuration) / 2)){
            player.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, startPoint.position.z);
        }

    }
}
