using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Target : MonoBehaviour
{
    public Transform startPoint;
    public Transform aimPoint;
    public DateTime timeSpawned;
    public GameObject gameManager;
    public int aliveTime; 
    [SerializeField]
    private float speed = 1;
    private Transform currEndPoint;
    private int childrenQuantity;
    private int currNode = 0;
    private Animator animator;
    private bool first = true;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position=startPoint.position;
        currEndPoint = startPoint.GetChild(0);
        childrenQuantity = startPoint.childCount;
        animator = GetComponent<Animator>();
        animator.SetBool("Moving", true);
        timeSpawned = DateTime.Now;
        StartCoroutine(DespawnTimer(aliveTime));
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, currEndPoint.position) > 0.1f)
        {
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, currEndPoint.position, step);
            transform.LookAt(currEndPoint.position);
        }
        else
        {
            if(currNode+1 < childrenQuantity)
            {
                currNode += 1;
                currEndPoint = startPoint.GetChild(currNode);
            }
            else 
            {
                
                AimAt();
                if(first)
                {
                    animator.SetBool("Moving", false);
                    
                    first = false;
                }
                
            } 
        }
        
    }

    private void AimAt()
    {
        Vector3 targetDirection = aimPoint.position - transform.position;
        float singleStep = speed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 10.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection * 10, Color.red);

        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    IEnumerator DespawnTimer(int time)
    {
        yield return new WaitForSeconds(time);
        gameManager.GetComponent<RoundManager>().RemoveTargets();
        Destroy(this.gameObject);
    }
}
