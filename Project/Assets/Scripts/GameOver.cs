using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Parrador
{
	public class GameOver : MonoBehaviour 
	{
		[SerializeField]
		public Text m_Text;
		
		void Start () 
		{
			if( GameManager.instance.gameOverState == true )
			{
				m_Text.text = "Congratulations";
			}
			else
			{
				m_Text.text = "Game Over";
			}
		}
	
		void Update () 
		{
			
		}
	}
}
