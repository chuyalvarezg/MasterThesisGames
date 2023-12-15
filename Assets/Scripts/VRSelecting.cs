using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRSelecting : MonoBehaviour
{
    [SerializeField] InputActionReference gripInputAction;
    [SerializeField] InputActionReference triggerInputAction;
    [SerializeField] LayerMask connectingLayer;
    [SerializeField] LayerMask floorLayer;
    [SerializeField] GameObject waveManager;
    private GameObject currWall;
    private bool cooldown;

    private void Update()
    {
        if (currWall != null)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1000, floorLayer))
            {
                currWall.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }

        }
    }

    private void Awake()
    {

    }

    private void OnEnable()
    {
        gripInputAction.action.performed += GripPressed;
        triggerInputAction.action.performed += TriggerPressed;
    }

    private void GripPressed(InputAction.CallbackContext obj)
    {


    }

    private void TriggerPressed(InputAction.CallbackContext obj)
    {
        if (!cooldown)
        {
            if (currWall == null)
            {
                HandleRaycast();
            }
            else
            {
                Wall tmp = currWall.GetComponent<Wall>();
                if (tmp != null)
                {
                    if (tmp.CheckForValidPath())
                    {
                        tmp.ToggleActive(false);
                        currWall = null;
                    }
                    else
                    {
                        tmp.HighlightInvalid();
                    }
                }
                else
                {
                    currWall.GetComponent<Tower>().ToggleActive(false);
                    currWall = null;
                }
                
            }
            StartCoroutine(ActivateCooldown());
        }
       
    }

    private void OnDisable()
    {
        gripInputAction.action.performed -= GripPressed;
        triggerInputAction.action.performed -= TriggerPressed;
    }

    private void HandleRaycast()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1000, connectingLayer))
        {
            currWall = hit.transform.gameObject;
            Wall tmp = currWall.GetComponent<Wall>();
            if (tmp != null)
            {
                tmp.ToggleActive(true);
            }
            else
            {
                currWall.GetComponent<Tower>().ToggleActive(true);
            }


            //Debug.Log("Selectable");

        }
        else
        {
            //Debug.Log("Not an enemy");
        }
    }

    IEnumerator ActivateCooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(0.5f);
        cooldown = false;
    }
}
