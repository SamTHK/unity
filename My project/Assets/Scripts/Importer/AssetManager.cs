using NUnit.Framework;
using RoslynCSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

public class AssetManager : MonoBehaviour
{
    private List<AsyncOperationHandle> loaded = new();
    private Dictionary<string, object> objects = new();
    private Dictionary<string, Package> presets = new();
    private Dictionary<string, object> internals = new();



    private void OnDestroy()
    {
        foreach (AsyncOperationHandle handle in loaded)
        {
            if (handle.Result != null)
            {
                Addressables.Release(handle);
            }
        }
        objects.Clear();
        loaded.Clear();
        presets.Clear();
        internals.Clear();
    }
    public async void PreLoadAsset<AnyThing>(string thing) 
    {
        string code = thing + "-" + typeof(AnyThing).ToString();
        if (objects[code] == null)
        {
            AsyncOperationHandle<AnyThing> handlesave = Addressables.LoadAssetAsync<AnyThing>(thing);
            while (!handlesave.IsDone)
            {
                await Task.Yield();
            }

            if (handlesave.IsValid())
            {
                loaded.Add(handlesave);
                objects[code] = handlesave.Result;
                return;
            }
            else
            {
                Addressables.Release(handlesave);
            }
            Debug.Log("Error: FILE " + thing + " NOT FOUND");
        }
    }

    public GameObject LoadPrefab(string thing)
    {
        string code = thing + "-" + typeof(GameObject).ToString();
            AsyncOperationHandle<GameObject> handlesave = Addressables.LoadAssetAsync<GameObject>(thing);
            handlesave.WaitForCompletion();
            if (handlesave.IsValid())
            {
                loaded.Add(handlesave);
                return Instantiate(handlesave.Result);
            }
            else
            {
                Addressables.Release(handlesave);
            }
        
        Debug.Log("Error: FILE " + thing + " NOT FOUND");
        return null;
    }

    public async Task<GameObject> LoadPrefabAsync(string thing)
    {
        string code = thing + "-" + typeof(GameObject).ToString();

            AsyncOperationHandle<GameObject> handlesave = Addressables.LoadAssetAsync<GameObject>(thing);
            while (!handlesave.IsDone)
            {
                await Task.Yield();
            }

            if (handlesave.IsValid())
            {
                loaded.Add(handlesave);
                return Instantiate(handlesave.Result);
            }
            else
            {
                Addressables.Release(handlesave);
            }
        
        Debug.Log("Error: FILE " + thing + " NOT FOUND");
        return null;
    }
    public AnyThing LoadAsset<AnyThing>(string thing) 
    {
        string code = thing + "-" + typeof(AnyThing).ToString();
        if (objects.TryGetValue(code, out object obj))
        {
            return (AnyThing)obj;
        }
        else
        {

            AsyncOperationHandle<AnyThing> handlesave = Addressables.LoadAssetAsync<AnyThing>(thing);
            handlesave.WaitForCompletion();

            if (handlesave.IsValid())
            {
                loaded.Add(handlesave);
                objects[code] = handlesave.Result;
                return (AnyThing)objects[code];

            }
            else
            {
                Addressables.Release(handlesave);
            }
        }
        Debug.Log("Error: FILE " + thing + " NOT FOUND");
        return default;
    } 
    public async Task<AnyThing> LoadAssetAsync<AnyThing>(string thing)
    {

        string code = thing + "-" + typeof(AnyThing).ToString();
        if (objects.TryGetValue(code, out object obj))
        {
            return (AnyThing)obj;
        }
        else
        {

            AsyncOperationHandle<AnyThing> handlesave = Addressables.LoadAssetAsync<AnyThing>(thing);
            while (!handlesave.IsDone)
            {
                await Task.Yield();
            }
            
            if (handlesave.IsValid())
            {
                loaded.Add(handlesave);
                objects[code] = handlesave.Result;
                                
                    return (AnyThing)objects[code];

            }
            else
            {
                Addressables.Release(handlesave);
            }
        }
        Debug.Log("Error: FILE " + thing + " NOT FOUND");
        return default;
        
    }

    public async Task LoadSceneAsync(string name)
    {
        
        AsyncOperationHandle handlesave = Addressables.LoadSceneAsync(name);
        while (!handlesave.IsDone)
        {
            await Task.Yield();
        }
        
    }    
    public async Task<SomeThing> LoadPresetAsync<SomeThing>(string name) where SomeThing : Preset
    {


        if (ObjectUtils.ModManager.AssetBundlePairs.TryGetValue(name, out string bundle))
        {

            if (presets.TryGetValue(bundle, out Package obj))
            {
                return obj.GetPreset<SomeThing>(name);
            }
            else
            {
                TextAsset json = await LoadAssetAsync<TextAsset>(bundle);
                
                    Package package = JsonUtility.FromJson<Package>(json.text);
                    presets[bundle] = package;
                    return package.GetPreset<SomeThing>(name);
             
            }
        }
        Debug.Log("Error: FILE " + name + " NOT FOUND");
        return null;
    }

    public async Task<Package> LoadPackageAsync(string bundle)
    {
        TextAsset json = await LoadAssetAsync<TextAsset>(bundle);
        if (json != null)
        {
            return JsonUtility.FromJson<Package>(json.text);
        }
        Debug.Log("Error: FILE " + bundle + " NOT FOUND");
        return null;
    }

    public async Task<LocalizationPack> LoadLocalizationAsync(string bundle)
    {
        string bundle_language = bundle + "-" + ObjectUtils.Manager.Language;
        TextAsset json;

        json = await LoadAssetAsync<TextAsset>(bundle_language);
        if (json != null)
        {
            return JsonUtility.FromJson<LocalizationPack>(json.text);
        }
        else
        {
            bundle_language = bundle + "-english";
            json = await LoadAssetAsync<TextAsset>(bundle_language);
            if (json != null)
            {
                return JsonUtility.FromJson<LocalizationPack>(json.text);
            }
        }    
            Debug.Log("Error: FILE " + bundle + " NOT FOUND");
        return null;
    }

    public async Task<List<string>> LoadTextFromLocalization(string bundle, string key)
    {
        LocalizationPack pack =  await LoadLocalizationAsync(bundle);
        if (pack != null) 
        {
            int i = pack.localization_key.FindIndex(x => x == key);
            string text = pack.localization[i];
            return text.Split(";,;").ToList();
        }
        Debug.Log("Error: FILE " + bundle + " NOT FOUND");
        return null;
    }    

    public SomeThing LoadInternalWork<SomeThing>(string name, GameObject gameObject = null, params object[] pa) 
    {


        if (internals.TryGetValue(name, out object obj))
        {
            return (SomeThing)obj;
        }
        else
        {
            if (ObjectUtils.ModManager.InternalWorks.TryGetValue(name, out ScriptType scripttype))
            {
                SomeThing interworks = scripttype.CreateInstanceAs<SomeThing>(gameObject, pa);
                internals[name] = interworks;
                  return interworks;
            }
        }
        Debug.Log("Error: FILE " + name +" NOT FOUND");
        return default;
    }

    


}

