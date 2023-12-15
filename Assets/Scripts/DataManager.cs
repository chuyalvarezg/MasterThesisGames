using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public string id = "";
    public string shooterResults = "";
    public string defenseResults = "";
    public string difficultyOrder = "";
    public string firstSceneLoaded = "";
    public string difficultyShown = "";

    public static DataManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}