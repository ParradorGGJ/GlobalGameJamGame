using UnityEngine;
using System.Collections;

public class PlaySoundTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_AudioSource = null;

    [SerializeField]
    private AudioClip m_OnUseClip = null;

    [SerializeField]
    private AudioClip m_OnEnterClip = null;

    [SerializeField]
    private bool m_PlayOnUse = false;

    [SerializeField]
    private bool m_PlayOnEnter = false;

    void OnTriggerStay(Collider aCollider)
    {
        if (aCollider.CompareTag("Player") == false) { return; }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if (m_AudioSource == null || m_OnUseClip == null) { return; }

            m_AudioSource.PlayOneShot(m_OnUseClip);
        }
    }

    void OnTriggerEnter(Collider aCollider)
    {
        if (aCollider.CompareTag("Player") == false) { return; }

        if (m_AudioSource == null || m_OnUseClip == null) { return; }

        m_AudioSource.PlayOneShot(m_OnEnterClip);
    }
}
