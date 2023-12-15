using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    private GameObject[] tiles;
    public int money = 60;
    [SerializeField] private TextMeshProUGUI funds;
    [SerializeField] private TextMeshProUGUI activeEnemiesText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Button startWave;
    [SerializeField] private Button buyRepeater;
    [SerializeField] private Button buySniper;
    [SerializeField] private Button buyShockwave;
    [SerializeField]
    private GameObject scoreScreen;
    [SerializeField]
    private TextMeshProUGUI scoreScreenText;
    [SerializeField] private int startX = 12;
    [SerializeField] private int startZ = 8;
    [SerializeField] private int endX = 12;
    [SerializeField] private int endZ = 17;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform towerSpawn;
    [SerializeField] private GameObject[] towers;
    [SerializeField] private GameObject[] walls;
    [SerializeField] private int enemiesPerWave;
    [SerializeField] private int startingWalls;
    [SerializeField] private int wallsPerWave;
    [SerializeField] private GameObject[] enemies;
    
    private List<GameObject> path;
    private List<GameObject> activeTowers = new List<GameObject>();
    private int currWave = 1;
    public GameObject[,] tileMap;

    public int locOffset = 5;
    public int activeEnemies;
    
    public GameObject[] shortestPath;


    //Metrics
    private int enemiesSpawned = 0;
    private int towersBought = 0;
    private int enemiesSurvived = 0;
    private int pathSize = 0;
    private double timeToFindSolution = 0;

    private int totalEnemiesSpawned = 0;
    private int totalEnemiesSurvived = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        funds.text= "Gold: "+money.ToString();
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        tileMap = new GameObject[20, 20];
        foreach (GameObject tile in tiles)
        {
            int xLoc = ((int)Math.Round(tile.transform.position.x) / 5) + locOffset;
            int zLoc = ((int)Math.Round(tile.transform.position.z) / 5) + locOffset;
            //Debug.Log(xLoc + ", " + zLoc );
            tileMap[xLoc, zLoc] = tile;
            tile.GetComponent <TileState>().tileX = xLoc;
            tile.GetComponent <TileState>().tileZ = zLoc;
        }
        SpawnWalls(startingWalls);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            BuySniperTower();
        }else if (Input.GetKeyDown(KeyCode.W))
        {
            StartWave();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            BuyShockwaveTower();
        }
    }
    
    public void StartWave()
    {
        activeEnemies = enemiesPerWave+(currWave*5);
        waveText.text = "Wave: "+currWave.ToString()+"/4";
        activeEnemiesText.text = "Enemies: " + activeEnemies.ToString();
        path = CalculateShortestPath();
        StartCoroutine(SpawnEnemies(enemiesPerWave + (currWave * 5)));
        startWave.interactable = false;
        ToggleAssets(false);
        FreezePlaced();
    }

    public void EndWave()
    {
        CalculateStats();
        if (currWave == 4) {
            StartCoroutine(ShowScoreScreen());
        }
        currWave++;
        ToggleAssets(true);
        SpawnWalls(wallsPerWave);
        money += 50;
        funds.text = "Gold: " + money.ToString();
    }

    private void CalculateStats()
    {
        totalEnemiesSpawned += enemiesSpawned;
        totalEnemiesSurvived += enemiesSurvived;
        //                                                                                                                          0               1        2          3           4           5
        DataManager.Instance.defenseResults = DataManager.Instance.defenseResults + string.Format("{0},{1},{2},{3},{4},{5}\n", enemiesSpawned,towersBought,money,enemiesSurvived,pathSize,timeToFindSolution);
        enemiesSpawned = 0;
        towersBought = 0;
        enemiesSurvived = 0;
        pathSize = 0;
        timeToFindSolution = 0;
}

    private void ToggleAssets(bool active)
    {
        
        buyRepeater.interactable = active;
        buySniper.interactable = active;
        buyShockwave.interactable = active;

        //this.gameObject.GetComponent<InteractableGrid>().enabled = active;
    }

    private void FreezePlaced()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("WallPlace");
        foreach (GameObject wall in walls)
        {
            wall.layer = 0;
        }
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        foreach (GameObject tower in towers)
        {
            tower.layer = 0;
        }
    }

    public void EnemySurvived()
    {
        enemiesSurvived++;
    }
    public void RemoveEnemy(GameObject enemy)
    {
        foreach(GameObject tower in activeTowers)
        {
            tower.GetComponent<Tower>().TryRemoveTarget(enemy);
        }
        Destroy(enemy);
        UpdateActiveEnemies(-1);
    }

    public void UpdateActiveEnemies(int quantity)
    {
        activeEnemies += quantity;
        activeEnemiesText.text = "Enemies: " + activeEnemies.ToString();
        if (activeEnemies <= 0) { EndWave(); }
    }

    public void SpawnWalls(int wallsPerWave)
    {
        for (int i = 0; i < wallsPerWave; i++)
        {
            float type = Random.Range(0.01f, 1f);
            float variation = Random.Range(1, 5);
            int wallType = 0;
            if (type >= 0.2 && type < 0.7)
            {
                wallType = 1;
                if (variation > 2)
                {
                    wallType = 2;
                }
            }else if (type >=0.7 &&  type < 0.85)
            {
                wallType = 3;
                if (variation > 2)
                {
                    wallType = 4;
                }
            }
            else if(type >=0.85)
            {
                wallType = 5;
                if (variation == 2)
                {
                    wallType = 6;
                }else if (variation == 3)
                {
                    wallType = 7;
                }else if(variation == 4)
                {
                    wallType = 8;
                }
            }
            
            Vector3 spawnLocation = new Vector3(towerSpawn.position.x-(i*4.75f),towerSpawn.position.y,towerSpawn.position.z);
            GameObject wall = Instantiate(walls[wallType],spawnLocation,Quaternion.identity);
            wall.GetComponent<Wall>().waveManager = this.gameObject;
        }
    }

    public void BuyRepeaterTower()
    {
        
        if (money - 20 >= 0)
        {
            towersBought++;
            money -= 20;
            GameObject tower = Instantiate(towers[0], towerSpawn.position, Quaternion.identity);
            tower.GetComponent<Tower>().waveManager = this.gameObject;
            activeTowers.Add(tower);
            funds.text = "Gold: " + money.ToString();
        }
    }

    public void BuySniperTower()
    {
        
        if (money - 30 >= 0)
        {
            towersBought++;
            money -= 30;
            GameObject tower = Instantiate(towers[1], towerSpawn.position, Quaternion.identity);
            tower.GetComponent<Tower>().waveManager = this.gameObject;
            activeTowers.Add(tower);
            funds.text = "Gold: " + money.ToString();
        }
    }

    public void BuyShockwaveTower()
    {

        if (money - 40 >= 0)
        {
            towersBought++;
            money -= 40;
            GameObject tower = Instantiate(towers[2], towerSpawn.position, Quaternion.identity);
            tower.GetComponent<Tower>().waveManager = this.gameObject;
            activeTowers.Add(tower);
            funds.text = "Gold: " + money.ToString();
        }
    }

    private List<GameObject> CalculateShortestPath()
    {
        DateTime timeStart = DateTime.Now;
        Queue queue = new Queue();
        int currX = startX;
        int currZ = startZ;
        tileMap[currX,currZ].GetComponent <TileState>().visited = true;
        queue.Enqueue(tileMap[currX,currZ]);
        while (queue.Count > 0)
        {
            GameObject tile = (GameObject) queue.Dequeue();
            currX = tile.GetComponent<TileState>().tileX;
            currZ = tile.GetComponent<TileState>().tileZ;
            
            if (currX == endX && currZ == endZ)
            {
                break;
            }
            //tile.GetComponent<Outline>().enabled = true;
            //Add adjacent Tiles to queue
            
            List<GameObject> adjacent = new List<GameObject>
            {
                tileMap[currX - 1, currZ],
                tileMap[currX + 1, currZ],
                tileMap[currX, currZ - 1],
                tileMap[currX, currZ + 1]
            };
            foreach (var adjTile in adjacent)
            {
                
                if (adjTile != null && !adjTile.GetComponent<TileState>().occupied && !adjTile.GetComponent<TileState>().visited)
                {
                    adjTile.GetComponent<TileState>().visited = true;
                    adjTile.GetComponent<TileState>().prev = tile;
                    queue.Enqueue(adjTile);
                }
            }
        }

        List<GameObject> path = new List<GameObject>();
        GameObject tileBack = tileMap[endX,endZ];
        while (tileBack != null)
        {
            path.Add(tileBack);
            //tileBack.GetComponent<Outline>().enabled = true;
            tileBack = tileBack.GetComponent<TileState>().prev;

        }
        path.Reverse();

        pathSize = path.Count;
        timeToFindSolution = DateTime.Now.Subtract(timeStart).TotalSeconds;

        foreach (GameObject tile in tileMap)
        {
            if (tile != null)
            {
                tile.GetComponent<TileState>().visited = false;
                tile.GetComponent<TileState>().prev = null;
            }
        }

        return path;
    }

    IEnumerator SpawnEnemies(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            GameObject enemy = enemies[Random.Range(0, enemies.Length)];
            GameObject newEnemy = Instantiate(enemy, spawnPoint.position, Quaternion.identity);
            newEnemy.GetComponent<Enemy>().path = path;
            newEnemy.GetComponent<Enemy>().waveManager = this.gameObject;
            newEnemy.GetComponent<Enemy>().health += (30 * (currWave - 1));
            enemiesSpawned ++;
            yield return new WaitForSeconds(4f-(0.5f*currWave));
        }
    }

    public bool TestValidPath(List<Vector2> additions)
    {
        foreach (Vector2 addition in additions)
        {
            tileMap[(int)addition.x, (int)addition.y].GetComponent<TileState>().occupied = true;
        }
        int pathSize = CalculateShortestPath().Count;
        Debug.Log(pathSize);
        foreach (Vector2 addition in additions)
        {
            tileMap[(int)addition.x, (int)addition.y].GetComponent<TileState>().occupied = false;
        }
        if ( pathSize > 3)
        {
            return true;
        }

        return false;
    }

    IEnumerator ShowScoreScreen()
    {
        scoreScreenText.text = "Game Completed\nScore: "+(totalEnemiesSpawned-totalEnemiesSurvived).ToString()+"/"+totalEnemiesSpawned.ToString();
        scoreScreen.SetActive(true);
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
