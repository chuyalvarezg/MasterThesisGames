using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI idText;
    [SerializeField]
    private TextMeshProUGUI connectedText;
    [SerializeField]
    private GameObject phase1;
    [SerializeField]
    private GameObject phase2A;
    [SerializeField]
    private GameObject phase2B;
    [SerializeField]
    private GameObject shooterTrailer;
    [SerializeField]
    private GameObject towerTrailer;
    [SerializeField]
    private GameObject phase3;
    [SerializeField]
    private GameObject phase4A;
    [SerializeField]
    private GameObject phase4B;
    [SerializeField]
    private GameObject phase5;
    [SerializeField] 
    private GameObject phase6;
    [SerializeField]
    private Slider[] ratings;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private GameObject loadingBar;

    private string sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (DataManager.Instance.id == "")
        {
            string id = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString();
            DataManager.Instance.id = id;
        }
        ShowID(DataManager.Instance.id);

        if (PlayerPrefs.GetString("DeviceID", "") == "")
        {
            PlayerPrefs.SetString("DeviceID", DataManager.Instance.id);
        }

        StartCoroutine(getRequest("https://zukyz1pt8i.execute-api.ap-southeast-2.amazonaws.com/items"));

        CheckIfCompletedTests();

        if (DataManager.Instance.firstSceneLoaded != "" && !(DataManager.Instance.shooterResults != "" && DataManager.Instance.defenseResults != ""))
        {
            phase1.SetActive(false);
            if (DataManager.Instance.firstSceneLoaded == "GalleryShooter")
            {
                DataManager.Instance.difficultyOrder = DataManager.Instance.difficultyOrder + "T";

                phase2B.SetActive(true);
                towerTrailer.SetActive(true);
            }
            else
            {
                DataManager.Instance.difficultyOrder = DataManager.Instance.difficultyOrder + "S";
                phase2A.SetActive(true);
                shooterTrailer.SetActive(true);
            }
        }

        //
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            LoadShooter();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            LoadTower();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            ShowPhase2();
        }
    }

    public void ShowPhase2()
    {
        Debug.Log(DataManager.Instance.firstSceneLoaded);
        if (DataManager.Instance.firstSceneLoaded == "")
        {
            int roll = Random.Range(0, 2);
            
            if (roll == 0)
            {
                DataManager.Instance.difficultyOrder = DataManager.Instance.difficultyOrder + "S";
                DataManager.Instance.firstSceneLoaded = "GalleryShooter";
                phase2A.SetActive(true);
                shooterTrailer.SetActive(true);

            }
            else
            {
                DataManager.Instance.difficultyOrder = DataManager.Instance.difficultyOrder + "T";
                DataManager.Instance.firstSceneLoaded = "TowerDefense";
                phase2B.SetActive(true);
                towerTrailer.SetActive(true);
            }
        }
        else
        {
            
        }
    }

    public void ShowPhase3(string sceneName)
    {
        sceneToLoad = sceneName;
        

        if (Random.Range(0, 4) == 0 && DataManager.Instance.difficultyShown == "")
        {
            DataManager.Instance.difficultyOrder = DataManager.Instance.difficultyOrder + "N";
            DataManager.Instance.difficultyShown = "notshown";
            LoadScene();
        }
        else
        {
            DataManager.Instance.difficultyOrder = DataManager.Instance.difficultyOrder + "D";
            phase3.SetActive(true);
        }
        
       
        
    }

    public void ShowPhase4()
    {
        int roll = Random.Range(0, 3);
        if ( roll== 0)
        {
            DataManager.Instance.difficultyOrder = DataManager.Instance.difficultyOrder + "G";
            phase4A.SetActive(true);
            ShowLoading();
        }
        else if ( roll== 1) 
        {
            DataManager.Instance.difficultyOrder = DataManager.Instance.difficultyOrder + "P";
            phase4B.SetActive(true);
            ShowLoading();  
        }
        else
        {
            DataManager.Instance.difficultyOrder = DataManager.Instance.difficultyOrder + "B";
            LoadScene();
        }
    }

    public void Submit()
    {
        submitButton.interactable = false;
        StartCoroutine(postRequest("https://zukyz1pt8i.execute-api.ap-southeast-2.amazonaws.com/items"));
    }

    public void ShowLoading()
    {
        loadingBar.SetActive(true);
        StartCoroutine(FillLoading());
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

    public void ShowID(string id)
    {
        idText.text = "UID: "+id;
    }

    public void SetConnectedStatus()
    {
        connectedText.text = "Status: Connected To Server";
    }
    public void LoadShooter()
    {
        SceneManager.LoadScene("GalleryShooter", LoadSceneMode.Single);
    }

    public void LoadTower()
    {
        SceneManager.LoadScene("TowerDefense", LoadSceneMode.Single);
    }

    private void CheckIfCompletedTests()
    {
        if (DataManager.Instance.shooterResults != "" && DataManager.Instance.defenseResults != "")
        {
            
            phase1.SetActive(false);
            phase5.SetActive(true);

        }
    }

    IEnumerator FillLoading()
    {
        for (int i = 0; i < 100; i++) {

            yield return new WaitForSeconds(0.1f);
            loadingBar.GetComponent<Slider>().value = i/100f;
        }
        LoadScene();
    }

    IEnumerator getRequest(string url)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            SetConnectedStatus();
        }
    }

    IEnumerator postRequest(string url)
    {
        ParticipantData data = new ParticipantData();
        data.id = DataManager.Instance.id;
        data.shooterResults = DataManager.Instance.shooterResults;
        data.defenseResults = DataManager.Instance.defenseResults;
        data.difficultyOrder = DataManager.Instance.difficultyOrder;
        data.deviceID = PlayerPrefs.GetString("DeviceID", "Error");

        foreach(Slider rating in ratings)
        {
            data.ratings += rating.value.ToString() + ",";
        }

        string json = JsonUtility.ToJson(data);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        UnityWebRequest uwr = UnityWebRequest.Put(url, jsonToSend);
        
        //uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        //uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            submitButton.interactable= true;
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            phase5.SetActive(false);
            phase6.SetActive(true);
            
        }
    }
}
