using CodeMonkey;
using RoslynCSharp;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class ModManager : MonoBehaviour
{
    private ScriptDomain domain;
    [SerializeField] GameObject Manager;
    public Dictionary<string, ScriptType> InternalWorks;
    public Dictionary<string, string> AssetBundlePairs;
  

    private void Awake()
    {
        DontDestroyOnLoad(this);
        InternalWorks = new Dictionary<string, ScriptType>();
        domain = new();
        AssetBundlePairs = new();
        TMP_Text.OnSpriteAssetRequest += (i, str) => { return ObjectUtils.AssetManager.LoadAsset<TMP_SpriteAsset>(str); };
    }
    public async Task Init(List<string> mods)
    {

        if (mods != null)
        {
            foreach (string mod in mods)
            {
                string mod_name = mod.Split(Path.DirectorySeparatorChar).Last();
                string mod_path;

                mod_path = Path.Combine(mod, "code_data");
                if (Directory.Exists(mod_path))
                {
                    string[] datas = Directory.GetFiles(mod_path);
                    foreach (string data in datas)
                    {
                        string[] whole = data.Split(".");
                        string end = whole.LastOrDefault();
                        string start = whole.FirstOrDefault().Split(Path.DirectorySeparatorChar).Last();
                        if (end == "cs" || end == "txt")
                        {
                            
                            string source = File.ReadAllText(data);
                            ScriptType type = domain.CompileAndLoadMainSource(source);
                            InternalWorks[start] = type;
                        }
                    }
                }

                mod_path = Path.Combine(mod, "asset_data");
                if (Directory.Exists(mod_path))
                {
                    string[] paths = Directory.GetFiles(mod_path);
                    foreach (string path in paths)
                    {
                        string[] p = path.Split(".");
                        if (p.Last() == "json" || p.Last() == "hash")
                        {

                            AsyncOperationHandle OperationHandle = Addressables.LoadContentCatalogAsync(path, true );
                            while (!OperationHandle.IsDone)
                            {
                                await Task.Yield();
                            }
                        }
                       
                    }

                }

                mod_path = Path.Combine(mod, "starter_data");
                if (Directory.Exists(mod_path))
                {
                    string[] datas = Directory.GetFiles(mod_path);
                    foreach (string data in datas)
                    {
                        string end = data.Split(".").LastOrDefault();
                        if (end == "cs" || end == "txt")
                        {
                            string source = File.ReadAllText(data);
                            ScriptType type = domain.CompileAndLoadMainSource(source);
                            ModStarter pD = type.CreateInstanceAs<ModStarter>();
                            pD.mod = mod_name;
                            pD.Init(this);
                        }
                    }
                }


                
            }
        }
        string[] paths_van = Directory.GetFiles("Assets/StreamingAssets/vanilla");
        foreach (string path in paths_van)
        {
            string[] p = path.Split(".");
            if (p.Last() == "json" || p.Last() == "hash")
            {
           

                AsyncOperationHandle OperationHandle = Addressables.LoadContentCatalogAsync(path, true);
                while (!OperationHandle.IsDone)
                {
                    await Task.Yield();
                }
            }
        }

        Instantiate(Manager);
    }

    private void Update()
    {
        

    }
}

