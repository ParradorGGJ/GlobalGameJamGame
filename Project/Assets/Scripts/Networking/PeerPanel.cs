using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Parrador
{

    [RequireComponent(typeof(RectTransform))]
    public class PeerPanel : MonoBehaviour
    {
        // -- Serialized Fields
        [SerializeField]
        private Text m_HostName = null;
        [SerializeField]
        private Text m_Comment = null;
        [SerializeField]
        private Button m_JoinButton = null;


        // -- Non-Serialized Fields
        private RectTransform m_RectTransform = null;

        public void Start()
        {
            m_RectTransform = GetComponent<RectTransform>();
        }
        public void SetParent(Transform aParent)
        {
            m_RectTransform.SetParent(aParent);
        }

        public string hostName
        {
            get { return m_HostName == null ? string.Empty : m_HostName.text; }
            set { if (m_HostName != null) { m_HostName.text = value; } }
        }
        public string comment
        {
            get { return m_Comment == null ? string.Empty : m_Comment.text; }
            set { if (m_Comment != null) { m_Comment.text = value; } }
        }
        public Button joinButton
        {
            get { return m_JoinButton; }
        }
        public Vector2 position
        {
            get { return m_RectTransform.anchoredPosition; }
            set { m_RectTransform.anchoredPosition = value; }
        }
        public float height
        {
            get { return m_RectTransform.sizeDelta.y; }
        }
        
    }
}

