using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace player
{
    public class Character : MonoBehaviour
    {
        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 3.0f;
        [SerializeField] private float sprintMultiplier = 2.0f;

        [Header("Jump Parameters")]
        [SerializeField] private float jumpForce = 5.0f;
        [SerializeField] private float gravity = 9.81f;

        [Header("Look Sensitivity")]
        [SerializeField] private float mouseSensitivity = 2.0f;
        [SerializeField] private float upDownRange = 80.0f;

        [Header("Input Actions")]
        [SerializeField] private InputActionAsset playerControls;

        private int lastPlayedIndex = -1;
        private bool isMoving;
        private Camera mainCamera;
        private float verticalRotation;
        private Vector3 currentMovement = Vector3.zero;
        private CharacterController characterController;

        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction sprintAction;
        private InputAction jumpAction;
        private Vector2 moveInput;
        private Vector2 lookInput;



        // Start is called before the first frame update
        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            mainCamera = Camera.main;

            moveAction = playerControls.FindActionMap("Player").FindAction("Move");
            lookAction = playerControls.FindActionMap("Player").FindAction("Look");
            sprintAction = playerControls.FindActionMap("Player").FindAction("Sprint");
            jumpAction = playerControls.FindActionMap("Player").FindAction("Jump");

            moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
            moveAction.canceled += context => moveInput = Vector2.zero;

            lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
            lookAction.canceled += context => lookInput = Vector2.zero;


        }

        private void OnEnable()
        {
            moveAction.Enable();
            lookAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            lookAction.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            HandleMovement();
            HandleRotation();
        }

        public void HandleMovement()
        {
            float speedMultiplier = sprintAction.ReadValue<float>() > 0 ? sprintMultiplier : 1f;

            float verticalSpeed = moveInput.y * walkSpeed * speedMultiplier;
            float horizontalSpeed = moveInput.x * walkSpeed *speedMultiplier;

            Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
            horizontalMovement = transform.rotation * horizontalMovement;

            HandleGravityAndJumping();

            currentMovement.x = horizontalMovement.x;
            currentMovement.z = horizontalMovement.z;

            characterController.Move(currentMovement * Time.deltaTime);

            isMoving = moveInput.y != 0 || moveInput.x != 0;
        }

        public void HandleGravityAndJumping()
        {
            if (characterController.isGrounded)
            {
                currentMovement.y = -0.5f;

                if (jumpAction.triggered)
                {
                    currentMovement.y = jumpForce;
                }
            }
            else
            {
                currentMovement.y -= gravity * Time.deltaTime;
            }
        }

        public void HandleRotation()
        {
            float mouseXRotation = lookInput.x * mouseSensitivity;
            transform.Rotate(0,mouseXRotation,0);

            verticalRotation -= lookInput.y * mouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
            mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }

    }
}

