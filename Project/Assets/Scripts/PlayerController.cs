using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterController m_Controller;

    [SerializeField]
    private float m_MoveSpeed = 2.0f;

    [SerializeField]
    private float m_TurnSpeed = 2.0f;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdateRotation();
        UpdateMove();
	}

    private void UpdateMove()
    {
        Vector3 localMoveDir = Input.GetAxis("Vertical") * transform.forward;

        m_Controller.Move(localMoveDir * Time.deltaTime * m_MoveSpeed);
    }

    private void UpdateRotation()
    {
        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * m_TurnSpeed);
    }
}
