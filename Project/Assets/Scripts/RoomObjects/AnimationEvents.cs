using UnityEngine;
using System.Collections;

namespace Parrador
{
    /// <summary>
    /// add more and more events for animation
    /// </summary>
    public class AnimationEvents : MonoBehaviour
    {
        [SerializeField]
        private AudioSource m_AudioSource = null;

        void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();
        }

        public void CreakEvent()
        {
            m_AudioSource.Play();
        }
    }
}