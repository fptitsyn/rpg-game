using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float sprintMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float distanceToGround;

    [SerializeField] private GameObject freeLookCamera;
    
    private InputAction _lookAction;
    private InputAction _walkAction;
    private InputAction _sprintAction;
    private InputAction _jumpAction;

    private const float Gravity = -9.81f;
    
    private CharacterController _controller;
    private CinemachineInputAxisController _inputAxisController;
    private bool _grounded;
    private Vector3 _velocity;
    private float _moveSpeed;
    private float _hp = 100;

    public static Action<float> SensitivityChanged;
    
    private void Start()
    {
        _lookAction = InputSystem.actions.FindAction("Look");
        _walkAction = InputSystem.actions.FindAction("Move");
        _sprintAction = InputSystem.actions.FindAction("Sprint");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        
        _controller = GetComponent<CharacterController>();
        
        _inputAxisController = freeLookCamera.GetComponent<CinemachineInputAxisController>();
        
        ChangeSensitivity(sensitivity);
    }

    private void Update()
    {
        Walk();
        Look();
    }

    private void Walk()
    {
        Vector2 moveValue = _walkAction.ReadValue<Vector2>();
        if (_sprintAction.IsPressed() && moveValue is { y: > 0, x: 0 })
        {
            _moveSpeed = sprintMultiplier * walkSpeed;
        }
        else
        {
            _moveSpeed = walkSpeed;
        }
        
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        
        Vector3 move = transform.right * moveValue.x + transform.forward * moveValue.y;
            
        _controller.Move(move * (_moveSpeed * Time.deltaTime));
            
        _grounded = Physics.Raycast(transform.position, Vector3.down, distanceToGround);
            
        if (_jumpAction.IsPressed() && _grounded)
        {
            _grounded = false;
            _velocity.y = jumpForce;
        }
            
        _velocity.y += Gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Look()
    {
        Vector2 lookValue = _lookAction.ReadValue<Vector2>() * (sensitivity * Time.deltaTime);
        
        transform.Rotate(Vector3.up, lookValue.x);
    }

    private void ReceiveDamage(float damage)
    {
        
    }

    private void ChangeSensitivity(float sens)
    {
        foreach (var c in _inputAxisController.Controllers)
        {
            c.Input.Gain *= sens;
        }
    }
}
