using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableGrid : MonoBehaviour
{
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private LayerMask connectingLayer;
    [SerializeField] LayerMask floorLayer;
    private GameObject currWall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") )
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
        }
        if(currWall != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            Ray rayOrigin = playerCamera.ScreenPointToRay(mousePosition);   
            if (Physics.Raycast(rayOrigin, out RaycastHit hit, 1000, floorLayer))
            {
                currWall.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
        }

    }

    private void HandleRaycast()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray rayOrigin = playerCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(rayOrigin, out RaycastHit hit, 1000, connectingLayer))
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
}
