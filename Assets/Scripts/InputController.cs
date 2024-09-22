using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private float m_currentSpeed;
    private float _velocity;
    [SerializeField] private float _smoothTime;
    private PlayerController _playerController;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>(); 
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var xInput = Input.GetAxis("Horizontal");
        var yInput = Input.GetAxis("Vertical");

        var inputMovement = new Vector3(xInput, 0, yInput);
        if(inputMovement.sqrMagnitude > 1 ) inputMovement= inputMovement.normalized;

        m_currentSpeed = Mathf.SmoothDamp(m_currentSpeed, inputMovement.magnitude, ref _velocity, _smoothTime);
        _playerController.SetSpeed(m_currentSpeed);

        if (inputMovement.sqrMagnitude > .1f) transform.rotation = Quaternion.LookRotation(inputMovement);

    }
}
