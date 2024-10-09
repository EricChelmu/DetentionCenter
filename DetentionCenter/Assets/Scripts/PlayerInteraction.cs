using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float playerReach = 3f;
        Interactable currentInteractable;
        [SerializeField] private InputActionAsset playerControls;
        private InputAction interactAction;

        private void Awake()
        {
            interactAction = playerControls.FindActionMap("Player").FindAction("Interact");
            interactAction.Enable();
        }

        // Update is called once per frame
        void Update()
        {
            CheckInteraction();
            if (interactAction.triggered && currentInteractable != null) 
            {
                currentInteractable.Interact();
            }
        }

        void CheckInteraction()
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (Physics.Raycast(ray, out hit, playerReach)) 
            { 
                if (hit.collider.tag == "Interactable")
                {
                    Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                    if (currentInteractable && newInteractable != currentInteractable)
                    {
                        currentInteractable.DisableOutline();
                    }

                    if (newInteractable.enabled)
                    {
                        SetNewCurrentInteractable(newInteractable);
                    }
                    else
                    {
                        DisableCurrentInteractable();
                    }
                }
            }
            else
            {
                DisableCurrentInteractable();
            }
        }

        void SetNewCurrentInteractable(Interactable newInteractable)
        {
            currentInteractable = newInteractable;
            currentInteractable.EnableOutline();
        }

        void DisableCurrentInteractable()
        {
            if (currentInteractable)
            {
                currentInteractable.DisableOutline();
                currentInteractable = null;
            }
        }
    }
}
