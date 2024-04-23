using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private bool isActive = false;
    private GameObject closest;
    private int closestX;
    private int closestZ;
    private GameObject tmp;
    [SerializeField]
    public GameObject waveManager;
    [SerializeField]
    private Vector3[] attachedWalls;
    public int activeTowers = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        closest = null;
        transform.localScale = Vector3.one*1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            int xLoc = ((int) (Math.Round(transform.position.x) + 5)/ 5) + waveManager.GetComponent<WaveManager>().locOffset;
            int zLoc = ((int) Math.Round(transform.position.z) / 5) + waveManager.GetComponent<WaveManager>().locOffset;
            if (xLoc >= 0 && zLoc >= 0 && xLoc < 20 && zLoc <20) {
                tmp = waveManager.GetComponent<WaveManager>().tileMap[xLoc, zLoc];
                if (tmp != closest && !CheckForOccupied(xLoc,zLoc))
                {
                    ToggleOutlines(false);
                    
                    closest = tmp;
                    closestX = xLoc;
                    closestZ = zLoc;
                    ToggleOutlines(true);
                    
                }
            }
            //Debug.Log("x: "+xLoc + " z: " + zLoc);
            
        }
    }

    public void ToggleActive(bool active)
    {
        transform.localScale = Vector3.one * 5f;
        isActive = active;
        if (!isActive) {
            //check if possible to place, if not outline in red
            transform.position = new Vector3((float)(closest.transform.position.x-2.5), 0, (float)(closest.transform.position.z+2.5));
            ToggleOutlines(false);
            ToggleOccupied(true);
        }
        else
        {
            ToggleOccupied(false);
        }
    }

    private void ToggleOutlines(bool state)
    {
        

        if (closest != null)
        {
            closest.GetComponent<Outline>().enabled = state;
        }

        foreach (Vector3 attached in attachedWalls)
        {
            if (closest != null)
            {
                GameObject attachedWall = waveManager.GetComponent<WaveManager>().tileMap[closestX + (int)attached.x, closestZ + (int)attached.z];
                if (attachedWall != null)
                {
                    attachedWall.GetComponent<Outline>().enabled = state;
                }
                
            }
        }
        
    }

    private void ToggleOccupied(bool state)
    {
        if (closest != null)
        {
            closest.GetComponent<TileState>().occupied = state;
        }

        foreach (Vector3 attached in attachedWalls)
        {
            if (closest != null)
            {
                GameObject attachedWall = waveManager.GetComponent<WaveManager>().tileMap[closestX + (int)attached.x, closestZ + (int)attached.z];
                if (attachedWall != null)
                {
                    attachedWall.GetComponent<TileState>().occupied = state;
                }
                
            }
        }

    }

    private bool CheckForOccupied(int xLoc,int zLoc)
    {
        if(waveManager.GetComponent<WaveManager>().tileMap[xLoc, zLoc].GetComponent<TileState>().occupied)
        {
            return true;
        }
        foreach(Vector3 attached in attachedWalls)
        {
            if (waveManager.GetComponent<WaveManager>().tileMap[xLoc + (int) attached.x, zLoc + (int)attached.z].GetComponent<TileState>().occupied)
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckForValidPath()
    {
        List<Vector2> additions = new List<Vector2>
        {
            new Vector2(closestX, closestZ)
        };
        foreach (Vector3 attached in attachedWalls)
        {
            additions.Add(new Vector2(closestX + attached.x, closestZ + attached.z));
        }
        return waveManager.GetComponent<WaveManager>().TestValidPath(additions);
    }
    
    public void HighlightInvalid()
    {
        if (closest != null)
        {
            closest.GetComponent<TileState>().ChangeToRed();
        }

        foreach (Vector3 attached in attachedWalls)
        {
            if (closest != null)
            {
                GameObject attachedWall = waveManager.GetComponent<WaveManager>().tileMap[closestX + (int)attached.x, closestZ + (int)attached.z];
                if (attachedWall != null)
                {
                    attachedWall.GetComponent<TileState>().ChangeToRed();
                }

            }
        }
    }

    private bool _CheckForOccupied(int xLoc,int zLoc)
    {
        if(waveManager.GetComponent<WaveManager>().tileMap[xLoc, zLoc].GetComponent<TileState>().occupied)
        {
            return true;
        }
        foreach(Vector3 attached in attachedWalls)
        {
            if (waveManager.GetComponent<WaveManager>().tileMap[xLoc + (int) attached.x, zLoc + (int)attached.z].GetComponent<TileState>().occupied)
            {
                return true;
            }
        }

        return false;
    }

    public bool _CheckForValidPath()
    {
        List<Vector2> additions = new List<Vector2>
        {
            new Vector2(closestX, closestZ)
        };
        foreach (Vector3 attached in attachedWalls)
        {
            additions.Add(new Vector2(closestX + attached.x, closestZ + attached.z));
        }
        return waveManager.GetComponent<WaveManager>().TestValidPath(additions);
    }
    
    public void _HighlightInvalid()
    {
        if (closest != null)
        {
            closest.GetComponent<TileState>().ChangeToRed();
        }

        foreach (Vector3 attached in attachedWalls)
        {
            if (closest != null)
            {
                GameObject attachedWall = waveManager.GetComponent<WaveManager>().tileMap[closestX + (int)attached.x, closestZ + (int)attached.z];
                if (attachedWall != null)
                {
                    attachedWall.GetComponent<TileState>().ChangeToRed();
                }

            }
        }
    }
}
