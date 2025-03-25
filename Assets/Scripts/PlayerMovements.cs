using System;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    //     [SerializeField] private Vector3 _playerSpeed;
    //     [SerializeField] private Rigidbody _rigidbody;
    //     // Start is called once before the first execution of Update after the MonoBehaviour is created
    //     public void ReinitPosPlayer()
    //     {
    //     }

    //     // Update is called once per frame
    //     void FixedUpdate()
    //     {
    //         float horizontalMovement=Input.GetAxis("Horizontal")*_playerSpeed.x*Time.deltaTime;

    //         float verticalMovement=Input.GetAxis("Vertical")*_playerSpeed.x*Time.deltaTime;

    //         // if(Input.GetKey(KeyCode.W)){
    //         //     transform.position+=Vector3.up*Time.deltaTime*_playerSpeed;
    //         // }
    //         // if(Input.GetKey(KeyCode.S)){
    //         //     transform.position-=Vector3.up*Time.deltaTime*_playerSpeed;
    //         // }
    //         // if(Input.GetKey(KeyCode.A)){
    //         //     transform.position-=Vector3.right*Time.deltaTime*_playerSpeed;
    //         // }
    //         // if(Input.GetKey(KeyCode.D)){
    //         //     transform.position+=Vector3.right*Time.deltaTime*_playerSpeed;
    //         // }

    //         MovePlayer(horizontalMovement,verticalMovement);
    //     }

    //     private void MovePlayer(float movementH,float movementV){
    //         Vector3 targetVelocity=new Vector3(movementH,0,movementV);
    //         _rigidbody.linearVelocity=targetVelocity;
    //         _rigidbody.linearVelocity=Vector3.SmoothDamp(_rigidbody.linearVelocity,targetVelocity,ref targetVelocity,.01f);
    //     }


    // public float MoveSpeed;
    // public float RotationSpeed;
    // public float CameraLerpSpeed = 5f; // Vitesse de lissage de la rotation
    // public float CameraFollowSpeed = 10f; // Vitesse de suivi du personnage

    // private Vector3 _moveDirection;
    // private float _currentRotationY = 0f; // Angle de rotation accumulé
    // private GameObject _playerCamera;
    // private Vector3 _cameraOffset; // Décalage fixe de la caméra

    // private void Start()
    // {
    //     _playerCamera = transform.GetChild(0).gameObject;
    //     _currentRotationY = transform.eulerAngles.y;
    //     _cameraOffset = _playerCamera.transform.position - transform.position; // Stocke le décalage initial
    // }

    // void Update()
    // {
    //     float horizontal = Input.GetAxisRaw("Horizontal"); // Q/D ou A/D
    //     float vertical = Input.GetAxisRaw("Vertical"); // Z/S ou W/S

    //     _moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

    //     if (_moveDirection.magnitude > 0.1f)
    //     {
    //         transform.position += _moveDirection * MoveSpeed * Time.deltaTime;
    //         Debug.Log(horizontal);
    //         if (horizontal != 0)
    //         {
    //             _currentRotationY += horizontal * RotationSpeed * Time.deltaTime;
    //         }
    //     }

    //     Quaternion targetRotation = Quaternion.Euler(0f, _currentRotationY, 0f);
    //     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, CameraLerpSpeed * Time.deltaTime);
    // }

    [Header("References")]
    private CharacterController _controller;
    private Transform _body;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _turningSpeed;

    [Header("Input")]
    private float _moveInput;
    private float _turnInput;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _body = transform.GetChild(0);
    }

    private void Movement()
    {
        GroundMovement();
        Turn();
    }

    private void GroundMovement()
    {
        Vector3 move = new Vector3(_turnInput, 0, _moveInput);
        move.y = 0;
        move *= _moveSpeed;
        _controller.Move(move * Time.deltaTime);
    }

    private void Turn()
    {
        Quaternion targetRotation = Quaternion.Euler(0, _turnInput * 90, 0);

        _body.transform.rotation = targetRotation;
    }

    private void FixedUpdate()
    {
        InputManagement();
        Movement();
    }

    private void InputManagement()
    {
        _moveInput = Input.GetAxis("Vertical");
        _turnInput = Input.GetAxis("Horizontal");
    }
}
