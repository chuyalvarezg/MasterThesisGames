using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform startPoint;
    public DateTime timeSpawned;
    public List<GameObject> path;
    [SerializeField]
    private float speed = 1;
    [SerializeField]
    public float health = 100;
    [SerializeField]
    public GameObject waveManager;
    [SerializeField]
    public TextMeshPro healthBar;
    private Transform currEndPoint;
    private int currNode = 0;
    private Animator animator;
    private Vector3 offset = new Vector3(-2.5f, 0, 2.5f);
    // Start is called before the first frame update
    void Start()
    {
        currEndPoint = path[currNode].transform;
        animator = GetComponent<Animator>();
        animator.SetBool("Moving", true);
        timeSpawned = DateTime.Now;
        healthBar.text = health.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, currEndPoint.position+offset) > 0.1f)
        {
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, currEndPoint.position+ offset, step);
            transform.LookAt(currEndPoint.position+offset);
        }
        else
        {
            if (currNode + 1 < path.Count)
            {
                currNode += 1;
                currEndPoint = path[currNode].transform;
            }
            else
            {
                animator.SetBool("Moving", false);
            }
        }

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.text = health.ToString();
        if (health <= 0)
        {
            waveManager.GetComponent<WaveManager>().RemoveEnemy(gameObject);
            
        }
    }

}
