using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class TileState : MonoBehaviour
{
    // Start is called before the first frame update
    public bool occupied = false;
    public int tileX = -1;
    public int tileZ = -1;
    public GameObject prev= null;
    public bool visited= false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeToRed() {
        StartCoroutine(FlickerRed());
    }

    IEnumerator FlickerRed()
    {
        this.gameObject.GetComponent<Outline>().OutlineColor = Color.red;
        yield return new WaitForSeconds(1);
        this.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
    }
}
