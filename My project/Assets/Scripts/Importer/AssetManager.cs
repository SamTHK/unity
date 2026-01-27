using RoslynCSharp;
using System;
using System.Collections.Generic;
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
    private Dictionary<string, InternalWork> internals = new();



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
    public async void PreLoadAsset<AnyThing>(string thing) where AnyThing : UnityEngine.Object
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

    public AnyThing LoadAsset<AnyThing>(string thing) where AnyThing : UnityEngine.Object
    {
        string code = thing + "-" + typeof(AnyThing).ToString();
        if (objects.TryGetValue(code, out object obj))
        {
            return obj as AnyThing;
        }
        else
        {

            AsyncOperationHandle<AnyThing> handlesave = Addressables.LoadAssetAsync<AnyThing>(thing);
            handlesave.WaitForCompletion();

            if (handlesave.IsValid())
            {
                loaded.Add(handlesave);
                objects[code] = handlesave.Result;
                return objects[code] as AnyThing;

            }
            else
            {
                Addressables.Release(handlesave);
            }
        }
        Debug.Log("Error: FILE " + thing + " NOT FOUND");
        return null;
    }
    public async Task<AnyThing> LoadAssetAsync<AnyThing>(string thing) where AnyThing : UnityEngine.Object
    {

        string code = thing + "-" + typeof(AnyThing).ToString();
        if (objects.TryGetValue(code, out object obj))
        {
            return obj as AnyThing;
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
                                
                    return objects[code] as AnyThing;

            }
            else
            {
                Addressables.Release(handlesave);
            }
        }
        Debug.Log("Error: FILE " + thing + " NOT FOUND");
        return null;
        
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

    public SomeThing LoadInternalWork<SomeThing>(string name) where SomeThing : InternalWork
    {
        if (internals.TryGetValue(name, out InternalWork obj))
        {
            return (SomeThing)obj;
        }
        else
        {
            if (ObjectUtils.ModManager.InternalWorks.TryGetValue(name, out ScriptType scripttype))
            {
                SomeThing interworks = scripttype.CreateInstanceAs<SomeThing>();
                internals[name] = interworks;
                  return interworks;

                
            }
        }

        Debug.Log("Error: FILE " + name +" NOT FOUND");
        return null;

    }


}

