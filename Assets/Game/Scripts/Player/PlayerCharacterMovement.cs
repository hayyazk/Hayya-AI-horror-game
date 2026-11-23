using System;
using UnityEngine;
public class PlayerCharacterMovement : MonoBehaviour
{
    private Vector3 _movementDirection;
    private float _currentSpeed = 1f;
    [SerializeField] private float _gravityScale = 1f;
    private Vector3 _velocityXZ;
    private float _velocityY;
    private bool _isGrounded;
    private bool _isSprint;
    private bool _isSquat;
    [SerializeField] private float _walkSpeed = 1f;
    [SerializeField] private float _sprintSpeed = 2f;
    [SerializeField] private float _acceleration = 0.5f;
    [SerializeField] private float _jumpPower = 2f;
    [SerializeField] private float _squatSpeed = 0.5f;
    [SerializeField] private float _standHeight = 2f;
    [SerializeField] private float _squatHeight = 1f;
    [SerializeField] private float _squatTransitionSpeed = 0.2f;
    [SerializeField] private Transform _lookPosition;
    [SerializeField] private CharacterController _characterController;

    public bool IsSprint => _isSprint;
    public bool IsSquat => _isSquat;
    public bool Enabled {get; private set;} = true;

    private void CalculateVelocityXZ()
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 xDirection = _movementDirection.x * cameraTransform.right;
        Vector3 zDirection = _movementDirection.z * cameraTransform.forward;
        Vector3 direction = xDirection + zDirection;
        direction.y = 0;
        if (_movementDirection.magnitude >= 0.01)
        {
            _velocityXZ = direction.normalized * _currentSpeed;
        }
        else
        {
            _velocityXZ = Vector3.zero;
        }
    }

    private void CalculateVelocityY()
    {
        _velocityY += Physics.gravity.y * _gravityScale * Time.deltaTime;
    }

    private void CheckIsGrounded()
    {
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        _isGrounded = Physics.CheckSphere(transform.position, 0.5f, groundLayer);
    }

    private void ResetVelocityY()
    {
        if (_isGrounded == true && _velocityY < 0)
        {
            _velocityY = -2f;
        }
    }

    private void CalculateAcceleration()
    {
        if (_movementDirection.magnitude >= 0.01f)
        {
            float targetSpeed;

            if (_isSprint)
            {
                targetSpeed = _sprintSpeed;
            }
            else if (_isSquat)
            {
                targetSpeed = _squatSpeed;
            }
            else
            {
                targetSpeed = _walkSpeed;
            }

            if (_currentSpeed < targetSpeed)
            {
                _currentSpeed += _acceleration;
            }
            else if (_currentSpeed > targetSpeed)
            {
                _currentSpeed -= _acceleration;
            }
            _currentSpeed = Mathf.Clamp(_currentSpeed, _squatSpeed, _sprintSpeed);
        }
        else
        {
            _currentSpeed = 0;
        }
    }
    
    public void SetMoveDirection(Vector2 inputDirection)
    {
        _movementDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
    }

    public void SetSprint(bool isSprint)
    {
        // ignore all sprint inputs when squatting
        if (!(_isSquat && isSprint))
        {
            _isSprint = isSprint;
        }
    }

    public void Move()
    {
        if (Enabled)
        {
            CalculateVelocityXZ();
            CalculateVelocityY();
            Vector3 velocity = new Vector3(_velocityXZ.x, _velocityY, _velocityXZ.z);
            _characterController.Move(velocity * Time.deltaTime);
        }
    }

    public void SetEnabled(bool isEnabled)
    {
        Enabled = isEnabled;
    }

    public void Jump(bool jumpPressed)
    {
        if (jumpPressed && _isGrounded)
        {
            _velocityY = _jumpPower;
        }
    }

    public void ToggleSquat()
    {
        _isSquat = !_isSquat;
        if (_isSquat)
        {
            // cancels sprinting when you go into a squat
            _isSprint = false;
        }
    }

    private void SquatTransition()
    {
        if (_isSquat)
        {
            _characterController.height -= _squatTransitionSpeed;
        }
        else
        {
            _characterController.height += _squatTransitionSpeed;
        }

        Vector3 lookLocalPosition = _lookPosition.localPosition;

        _characterController.height = Mathf.Clamp(_characterController.height, _squatHeight, _standHeight);
        lookLocalPosition.y = Mathf.Clamp(_characterController.height - 0.2f, _squatHeight - 0.2f, _standHeight - 0.2f);

        _lookPosition.localPosition = lookLocalPosition;
        _characterController.center = new Vector3(0, _characterController.height/2f, 0);

    }

    private void Update()
    {
        CheckIsGrounded();
        CalculateAcceleration();
        ResetVelocityY();
        SquatTransition();
        Move();
    }
}
