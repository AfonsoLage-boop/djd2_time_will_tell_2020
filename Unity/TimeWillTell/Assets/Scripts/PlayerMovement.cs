﻿using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement constants
    private const float FORWARD_ACCELERATION    = 10.0f;
    private const float BACKWARD_ACCELERATION   = 10.0f;
    private const float STRAFE_ACCELERATION     = 20.0f;
    private const float GRAVITY_ACCELERATION    = 10.0f;
    private const float MAX_FORWARD_VELOCITY    = 3.5f;
    private const float MAX_BACKWARD_VELOCITY   = 2.5f;
    private const float MAX_STRAFE_VELOCITY     = 3.0f;

    // Camera rotation constants
    private const float ANGULAR_VELOCITY_FACTOR = 2.0f;
    private const float UPPER_HEAD_TILT         = 290.0f;
    private const float LOWER_HEAD_TILT         = 70.0f;
    private const float INTRO_LEFT_PEEK         = 20.0f;
    private const float INTRO_RIGHT_PEEK        = 340f;

    private UserInterface       _ui;
    private Subtitles           _subtitles;
    private CharacterController _controller;
    private Transform           _cameraTransform;
    private Vector3             _acceleration;
    private Vector3             _velocity;

    private void Start()
    {
        _ui                 = UserInterface.instance;
        _subtitles          = _ui.GetComponent<Subtitles>();
        _controller         = GetComponent<CharacterController>();
        _cameraTransform    = GetComponentInChildren<Camera>().transform;
        _acceleration       = Vector3.zero;
        _velocity           = Vector3.zero;

        _ui.HideCursor();
    }

    private void Update()
    {
        if(!Cursor.visible)
        {
            UpdateHeadTilt();
            UpdateRotation();
        }
    }

    private void UpdateHeadTilt()
    {
        Vector3 cameraRotation = _cameraTransform.localEulerAngles;

        cameraRotation.x -= Input.GetAxis("Mouse Y") * ANGULAR_VELOCITY_FACTOR;

        if (cameraRotation.x < 180f)
            cameraRotation.x = Mathf.Min(cameraRotation.x, LOWER_HEAD_TILT);

        else
            cameraRotation.x = Mathf.Max(cameraRotation.x, UPPER_HEAD_TILT);

        _cameraTransform.localEulerAngles = cameraRotation;
    }
    
    private void UpdateRotation()
    {
        float rotation = Input.GetAxis("Mouse X") * ANGULAR_VELOCITY_FACTOR;

        transform.Rotate(0, rotation, 0);
    }

    private void FixedUpdate()
    {
        if(!Cursor.visible)
        {
            UpdateAcceleration();
            UpdateVelocity();
            UpdatePosition();
        }
    }

    private void UpdateAcceleration()
    {
        _acceleration.z = Input.GetAxis("Forward");
        _acceleration.x = Input.GetAxis("Strafe") * STRAFE_ACCELERATION;

        _acceleration.z *= (_acceleration.z >=0) ? 
            FORWARD_ACCELERATION : BACKWARD_ACCELERATION;

        _acceleration.y = -GRAVITY_ACCELERATION;

    }

    private void UpdateVelocity()
    {
        _velocity += _acceleration * Time.fixedDeltaTime;
        
        _velocity.z = (_acceleration.z == 0f || _velocity.z * _acceleration.z < 0) ? 0 : 
            Mathf.Clamp(_velocity.z, -MAX_BACKWARD_VELOCITY, MAX_FORWARD_VELOCITY);

        _velocity.x = (_acceleration.x == 0f || _velocity.x * _acceleration.x < 0) ? 0 : 
            Mathf.Clamp(_velocity.x, -MAX_STRAFE_VELOCITY, MAX_STRAFE_VELOCITY);
    }

    private void UpdatePosition()
    {
        Vector3 motion = _velocity * Time.fixedDeltaTime;

        _controller.Move(transform.TransformVector(motion));
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "SecretRoomEmpty")
            _ui.ShowHelpMsg("You have no memory of this place.");

        if (col.gameObject.name == "SecretRoom")
            _ui.HideHelpMsg();

        if (col.gameObject.name == "Range")
            _subtitles.InRange(true);
        
        if (col.gameObject.name == "PastFilter")
            _subtitles.InRange(false);
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.name == "Range")
            _subtitles.InRange(false);
    }
}
