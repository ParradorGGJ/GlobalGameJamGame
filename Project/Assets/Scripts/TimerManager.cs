using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Parrador
{
    public class TimerManager : MonoBehaviour
    {
		[SerializeField]
        public Text m_Time;

        private float m_MaxTime = 180.0f;
        private float m_TimeRemaining;

        void Start()
        {
        	GameManager.instance.timeRemaining = m_MaxTime;
            m_TimeRemaining = GameManager.instance.timeRemaining;
        }

        void Update()
        {
        	m_TimeRemaining -= Time.deltaTime; 
            GameManager.instance.SetGameTime(m_TimeRemaining);
			m_Time.text = DisplayTime((int)m_TimeRemaining);
         	
            
            if( GameManager.instance.GetGameTime() <= 0.0f )
            {
            	GameManager.instance.gameOver = true;
            	GameManager.instance.gameOverState = false;
            }
        }

        private string DisplayTime(int time)
        {
            int minutes;
            int seconds;

            minutes = Mathf.Abs(time / 60);
            seconds = time - (minutes * 60);

            return (minutes.ToString("00") + ":" + seconds.ToString("00"));
        } 
        
        private void updateNetworkTimer(float time)
        {
        	Player player = NetworkWorld.GetSelf();
        	NetworkWorld.SendAddTime(player, time);
        }
          
  	  
    }
}
