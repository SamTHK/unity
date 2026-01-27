using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    // Start is called before the first frame update
    public string Language;
    [SerializeField] GameObject levelManager;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadScene("Example1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void LoadScene(string name)
    {
        LevelPreset lP =  ObjectUtils.AssetManager.LoadInternalWork<LevelPreset>(name);
        if (lP != null)
        {
            await ObjectUtils.AssetManager.LoadSceneAsync(lP.mapID);
            GameObject a = Instantiate(levelManager);
            LevelManager lM = a.GetComponent<LevelManager>();
            lM.Init(lP);
        }
        else
        {
            Debug.Log("Error: SCENE " + name + " FAILED TO LOAD");
        }
    }
    
}
