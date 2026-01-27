using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModStarter 
{
    public string mod;
    public abstract void Init(ModManager modManager);

    protected void AddPreset(ModManager modManager, string presetfilename, string bundlename)
    {
        modManager.AssetBundlePairs[presetfilename] = bundlename;
    }
}
