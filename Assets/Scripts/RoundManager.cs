using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    private int roundTime = 300;
    [SerializeField]
    private int maxWaves = 5;
    [SerializeField]
    private int despawnTimer = 10;
    [SerializeField]
    private int targetsPerWave = 10;
    [SerializeField]
    private TextMeshProUGUI timer;
    [SerializeField]
    private TextMeshProUGUI score;
    [SerializeField]
    private TextMeshProUGUI stats;
    [SerializeField]
    private GameObject scoreScreen;
    [SerializeField]
    private TextMeshProUGUI scoreScreenText;
    [SerializeField]
    private AudioSource soundStart;
    [SerializeField]
    private GameObject[] targets;
    [SerializeField]
    private Transform aimPoint;
    [SerializeField] 
    private Camera playerCamera;
    [SerializeField]
    private LayerMask connectingLayer;
    [SerializeField]
    private Transform[] spawnPoints;
    private DateTime startTime;
    private double timeRemaining;
    private GameObject target;
    private GameObject newTarget;
    private Transform spawnPoint;
    private bool roundActive = true;
    private bool[] occupiedPlaces;

    //metrics
    private double totalTimeAlive;
    private int totalTargetsSpawned;
    private int targetSpawnLocation;
    private int targetsLeft;
    private int currWave = 1;
    private int shotsFired = 0;
    private int timesReloaded = 0;
    private int targetsHit = 0;
    private DateTime roundStartTime;
    // Start is called before the first frame update
    void Start()
    {
        
        score.text = "0";
        startTime = DateTime.Now;
        StartCoroutine(GameStartup());
    }

    // Update is called once per frame
    void Update()
    {
        if (roundActive)
        {
            timeRemaining = DateTime.Now.Subtract(startTime).TotalSeconds-5;
            timer.text = Math.Ceiling(timeRemaining).ToString();

            
            if (timeRemaining >= 0)
            {
                
            }
            else
            {
                
            }
        }
    }

    private void StartWave()
    {
        soundStart.Play();
        stats.text = "Wave: " + currWave + "/" + maxWaves;
        occupiedPlaces = new bool[spawnPoints.Length];
        targetSpawnLocation = Random.Range(0, spawnPoints.Length);
        roundStartTime = DateTime.Now;
        StartCoroutine(SpawnWaveTargets());
        targetsLeft = targetsPerWave;
    }

    public void TargetHit(double timeAlive)
    {
        RemoveTargets();
        totalTimeAlive += timeAlive;
        targetsHit++;
        score.text = (int.Parse(score.text)+10).ToString();
    }

    public void RemoveTargets()
    {
        targetsLeft--;
        if (targetsLeft == 0)
        {
            StartCoroutine(TransitionWaves());
        }
    }

    private void SpawnTarget(int index)
    {
        totalTargetsSpawned++;
        target = targets[Random.Range(0, targets.Length)];
        int options = target.transform.childCount;
        spawnPoint = spawnPoints[index];
        newTarget = Instantiate(target, spawnPoint.position, Quaternion.identity);
        newTarget.GetComponent<Target>().aimPoint = aimPoint;
        newTarget.GetComponent<Target>().startPoint = spawnPoint;
        newTarget.GetComponent<Target>().gameManager = this.gameObject;
        newTarget.GetComponent<Target>().aliveTime = despawnTimer - (1 * currWave);
        newTarget.transform.GetChild(Random.Range(0, options - 1)).gameObject.SetActive(true);
        
    }

   
    private void CalculateStats()
    {
        Debug.Log("calculating Shooter");   
        double roundTimeTaken = Math.Ceiling(DateTime.Now.Subtract(roundStartTime).TotalSeconds);
        double averageTimeAlive = totalTimeAlive / totalTargetsSpawned;
        float accuracy = (float)targetsHit / (float)shotsFired;
        //stats.text = string.Format("STATS  Score: {0}  Average alive time: {1}  Accuracy: {2}", score.text,Math.Round(averageTimeAlive,2),Math.Round(accuracy,2));
        //                                                                                                                                  0               1               2           3             4             5                       6                           7
        DataManager.Instance.shooterResults = DataManager.Instance.shooterResults + string.Format("{0},{1},{2},{3},{4},{5},{6},{7}\n", score.text, totalTargetsSpawned, targetsHit, shotsFired, timesReloaded, roundTimeTaken, Math.Round(averageTimeAlive, 2), Math.Round(accuracy, 2));
        totalTimeAlive = 0;
        timesReloaded = 0;
        totalTargetsSpawned = 0;
        targetsHit = 0;
        shotsFired = 0;
    }

    public void Shoot()
    {
        shotsFired++;
    }

    public void Reload()
    {
        timesReloaded++;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    IEnumerator SpawnWaveTargets()
    {
        for (int i = 0; i < targetsPerWave; i++)
        {

            SpawnTarget(targetSpawnLocation);
            occupiedPlaces[targetSpawnLocation] = true;
            //new target somewhere near last
            int offset = Random.Range(-1, 3);
            if (offset <= 0)
            {
                offset -= 1;
            }
            //Debug.Log("Prev: " + targetSpawnLocation + " offset: " + offset);
            int tmp = (targetSpawnLocation + offset) % spawnPoints.Length;

            if (tmp < 0)
            {
                tmp += spawnPoints.Length;
            }
            targetSpawnLocation = tmp;
            while (occupiedPlaces[targetSpawnLocation])
            {
                //Debug.Log("retrying");
                targetSpawnLocation++;
                if (targetSpawnLocation == spawnPoints.Length)
                {
                    targetSpawnLocation = 0;
                }
            }
            //Debug.Log(targetSpawnLocation);
            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }

    }

    IEnumerator GameStartup()
    {
        yield return new WaitForSeconds(5);
        StartWave();
    }

    IEnumerator TransitionWaves()
    {
        currWave++;
        yield return new WaitForSeconds(3);
        CalculateStats();
        if (currWave > maxWaves)
        {
            roundActive = false;
            Debug.Log(DataManager.Instance.shooterResults);
            StartCoroutine(ShowScoreScreen());
        }
        else
        {
            StartWave();
        }  
    }

    IEnumerator ShowScoreScreen()
    {
        scoreScreenText.text = "Game Completed\nScore: "+score.text;
        scoreScreen.SetActive(true); 
        yield return new WaitForSeconds(5);
        LoadMainMenu();
    }
}
