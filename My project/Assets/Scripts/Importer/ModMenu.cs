using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ModMenu : MonoBehaviour
{
    [SerializeField] List<string> modnames, acceptedmodnames;
    [SerializeField] GameObject ModManager;



    private void Awake()
    {
       DontDestroyOnLoad(gameObject);
        if (Directory.Exists(Path.Combine(Application.streamingAssetsPath, "mod")))
        {
            modnames = Directory.GetDirectories(Path.Combine(Application.streamingAssetsPath, "mod")).ToList();
        }
        InitMod();
        

    }

    private async void InitMod()
    {
        acceptedmodnames = modnames;
        GameObject o = Instantiate(ModManager);
        await o.GetComponent<ModManager>().Init(acceptedmodnames);
    }
}
