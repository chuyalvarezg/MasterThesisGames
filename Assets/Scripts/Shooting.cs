using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Shooting : MonoBehaviour
{
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    [SerializeField] AudioSource shoot;
    [SerializeField] AudioSource reload;
    [SerializeField] AudioSource ding;
    [SerializeField] LayerMask connectingLayer;
    [SerializeField] GameObject roundManager;
    [SerializeField] TrailRenderer bulletTrail;

    [SerializeField] int maxMagazineCount = 6;
    private int currentMagazineCount;
    [SerializeField] TextMeshProUGUI currentMagazineCountText;

    float rotationX = 0;
    float rotationY = 0;

    [HideInInspector]
    public bool canMove = true;

    private void Awake()
    {
        currentMagazineCount = maxMagazineCount;
        currentMagazineCountText.text = currentMagazineCount.ToString();
    }

    void Start()
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY += Input.GetAxis("Mouse X") * lookSpeed;
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        }

        if (Input.GetMouseButtonDown(0) && currentMagazineCount > 0)
        {
            shoot.Play();
            roundManager.GetComponent<RoundManager>().Shoot();
            TrailRenderer trail = Instantiate(bulletTrail, transform.position, Quaternion.identity);
            StartCoroutine(MoveTrail(trail, transform.TransformPoint(Vector3.forward * 20)));
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1000, connectingLayer))
            {
                ding.Play();
                DateTime spawntime = hit.transform.gameObject.GetComponent<Target>().timeSpawned;
                double timeAlive = DateTime.Now.Subtract(spawntime).TotalSeconds;

                Destroy(hit.transform.gameObject);

                roundManager.GetComponent<RoundManager>().TargetHit(timeAlive);
            }
            currentMagazineCount--;
            currentMagazineCountText.text = currentMagazineCount.ToString();
        } else if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(ReloadWeapon());
        }

    }

    IEnumerator MoveTrail(TrailRenderer trail, Vector3 position)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, position, time);
            time += Time.deltaTime / trail.time;

            yield return null;

            Destroy(trail.gameObject, trail.time);
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
