using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRShooting : MonoBehaviour
{
    [SerializeField] InputActionReference gripInputAction;
    [SerializeField] InputActionReference triggerInputAction;
    [SerializeField] LayerMask connectingLayer;
    [SerializeField] GameObject roundManager;
    [SerializeField] TrailRenderer bulletTrail;
    [SerializeField] Transform bulletOrigin;
    [SerializeField] int maxMagazineCount = 6;
    [SerializeField] AudioSource shoot;
    [SerializeField] AudioSource reload;
    [SerializeField] AudioSource ding;
    private int currentMagazineCount;
    [SerializeField] TextMeshPro currentMagazineCountText;

    

    private void Awake()
    {
        currentMagazineCount = maxMagazineCount;
        currentMagazineCountText.text = currentMagazineCount.ToString();
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
        if (currentMagazineCount > 0)
        {
            shoot.Play();   
            roundManager.GetComponent<RoundManager>().Shoot();
            TrailRenderer trail = Instantiate(bulletTrail, bulletOrigin.position, Quaternion.identity);
            StartCoroutine(MoveTrail(trail, transform.TransformPoint(Vector3.forward * 20)));
            if (Physics.Raycast(bulletOrigin.position, transform.forward, out RaycastHit hit, 1000, connectingLayer))
            {
                ding.Play();
                DateTime spawntime = hit.transform.gameObject.GetComponent<Target>().timeSpawned;
                double timeAlive = DateTime.Now.Subtract(spawntime).TotalSeconds;

                Destroy(hit.transform.gameObject);

                roundManager.GetComponent<RoundManager>().TargetHit(timeAlive);
            }
            currentMagazineCount--;
            currentMagazineCountText.text = currentMagazineCount.ToString();
        }


    }

    IEnumerator MoveTrail (TrailRenderer trail, Vector3 position)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 1) { 
            trail.transform.position = Vector3.Lerp(startPosition, position, time);
            time += Time.deltaTime /trail.time;

            yield return null;

            Destroy(trail.gameObject, trail.time);
        }

    }

    private void OnDisable()
    {
        gripInputAction.action.performed -= GripPressed;
        triggerInputAction.action.performed -= TriggerPressed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reload")
        {
            StartCoroutine(ReloadWeapon());
        }
    }

    IEnumerator ReloadWeapon()
    {
        currentMagazineCount = 0;
        currentMagazineCountText.text = "-";
        reload.Play();
        yield return new WaitForSeconds(2);
        currentMagazineCount = maxMagazineCount;
        currentMagazineCountText.text = currentMagazineCount.ToString();
    }
}
