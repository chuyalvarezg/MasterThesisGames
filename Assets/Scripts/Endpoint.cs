using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Endpoint : MonoBehaviour
{
    [SerializeField]
    private WaveManager waveManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            waveManager.GetComponent<WaveManager>().EnemySurvived();
            other.GetComponent<Enemy>().TakeDamage(1000);
        }

    }
}
