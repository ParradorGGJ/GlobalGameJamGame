using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Parrador
{
    public class TimerManager : MonoBehaviour
    {
        public Text m_Time;

        private float m_MaxTime = 185.0f;
        private float m_TimeRemaining;

        void Start()
        {
            m_TimeRemaining = m_MaxTime;
        }

        void Update()
        {
            m_TimeRemaining -= Time.deltaTime;

            m_Time.text = DisplayTime((int)m_TimeRemaining);
        }

        private string DisplayTime(int time)
        {
            int minutes;
            int seconds;

            minutes = Mathf.Abs(time / 60);
            seconds = time - (minutes * 60);

            return (minutes.ToString("00") + ":" + seconds.ToString("00"));
        }
        
    }
}
