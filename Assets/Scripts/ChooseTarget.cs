using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChooseTarget : MonoBehaviour
{
    private Tower tower;
    private void Start()
    {
        tower = this.gameObject.GetComponentInParent<Tower>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            tower.SetTarget(other.gameObject);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            tower.TryRemoveTarget(other.gameObject);
        }
    }
}
