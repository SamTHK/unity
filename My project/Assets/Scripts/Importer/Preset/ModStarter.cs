using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ModStarter 
{
    public string mod;
    public string[] package;
    public abstract void Init(ModManager modManager);

    public async Task Package_GetAsync(ModManager modManager)
    {
        foreach (string pack in package)
        {
            Package package_to_read = await ObjectUtils.AssetManager.LoadPackageAsync(pack);
            foreach (string key in package_to_read.presets_key)
            {
                modManager.AssetBundlePairs[key] = pack;
            }    
        }
    }
   
}
