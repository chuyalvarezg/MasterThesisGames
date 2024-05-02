using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;

public class Tower : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject target;
    private int targetID;
    public float damage = 10;
    public float fireRate = 0.5f;
    public float animationRate;
    public GameObject spell;
    public Transform spellSpawn;
    public bool aoe;

    private bool isActive = false;
    private GameObject closest;
    private GameObject oldClosest;
    private GameObject tmp;
    [SerializeField]
    public GameObject waveManager;

    private GameObject[] walls;
    private List<GameObject> targets = new List<GameObject>();
    private Animator animator;
    private Vector3 offsetAim = Vector3.zero;
    private bool attacking = true;

    // Start is called before the first frame update
    void Start()
    {
        closest = null;

        walls = GameObject.FindGameObjectsWithTag("Wall");
        animator = GetComponent<Animator>();
        animator.SetBool("target", false);
        offsetAim = new Vector3(0,3,0);
        StartCoroutine(Fire());
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            tmp = FindClosestWall();
            if (tmp != null)
            {
                oldClosest = closest;
            }
            else
            {
                ToggleOutlines(oldClosest, false);
            }
            closest = tmp;
            if (closest != null && oldClosest != closest)
            {
                ToggleOutlines(oldClosest, false);
                ToggleOutlines(closest,true);
            }
            else
            {
                
            }
        }
        else
        {
            if (targets.Count > 0)
            {
                transform.LookAt(targets[targets.Count-1].transform.position);
                transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
                
                if (!aoe && attacking)
                {
                    spell.SetActive(true);
                    if (Vector3.Distance(spell.transform.position, targets[targets.Count - 1].transform.position+offsetAim) > 0.2f)
                    {

                        spell.transform.position = Vector3.MoveTowards(spell.transform.position, targets[targets.Count - 1].transform.position+offsetAim, 0.1f);
                    }
                    else
                    {
                        StartCoroutine(StartSpellCooldown());
                    }
                }
            }
            else
            {
                if (!aoe)
                {
                    spell.SetActive(false);
                }
            }
        }
    }

    public void ToggleActive(bool active)
    {
        isActive = active;
        if (!isActive)
        {
            if (closest != null)
            {
                transform.position = new Vector3(closest.transform.position.x, 4.1f, closest.transform.position.z);
                ToggleOutlines(closest, false);
                closest.GetComponent<WallState>().occupied = true;
            }else
            {
                transform.position = new Vector3(37, 13.8f, 21.5f);
            }
        }
        else
        {
            walls = GameObject.FindGameObjectsWithTag("Wall");
            if (closest != null)
            {
                closest.GetComponent<WallState>().occupied = false;
            }
        }
    }

    private void ToggleOutlines(GameObject selected,bool state)
    {
        if (selected != null)
        {
            selected.GetComponent<Outline>().enabled = state;
        }
    }

    private void ToggleOccupied(bool state)
    {
        if (closest != null)
        {
            closest.GetComponent<TileState>().occupied = state;
        }
    }

    private GameObject FindClosestWall()
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in walls)
        {
            if (go.GetComponent<WallState>().occupied)
            {
                continue;
            }
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        if (distance < 50)
        {
            return closest;
        }else 
        { 
            return null; 
        }
       
    }

    public void SetTarget(GameObject enemy)
    {
        targets.Add(enemy);
        animator.SetBool("target", true);
        //Debug.Log("added: " + enemy.GetInstanceID().ToString());
        //Debug.Log(targets.Count);
    }

    public void TryRemoveTarget(GameObject enemy)
    {
        if (targets.Contains(enemy))
        {
            targets.Remove(enemy);
            //Debug.Log("removed: " + enemy.GetInstanceID().ToString());
        }
        //Debug.Log(targets.Count);
    }

    IEnumerator Fire()
    {
        while (true) {
            if (targets.Count > 0)
            {
                if (aoe)
                {                 
                    for (int i = 0; i < targets.Count; i++)
                    {
                        GameObject tmpTarget = targets[i];
                        if (tmpTarget != null)
                        {
                            tmpTarget.GetComponent<Enemy>().TakeDamage(damage);
                        }
                    }
                }
                else
                {
                    targets[targets.Count - 1].GetComponent<Enemy>().TakeDamage(damage);
                }
                
                
            }
            else
            {
                animator.SetBool("target", false);
            }
        yield return new WaitForSeconds(fireRate);
        }
    }

    IEnumerator StartSpellCooldown() {
        attacking = false;
        spell.SetActive(false);
        spell.transform.position = spellSpawn.position;
        yield return new WaitForSeconds(fireRate);
        attacking = true;
    }
    public void PlayAOESpell() {
        spell.GetComponent<ParticleSystem>().Play();
    }
}
