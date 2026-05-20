using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LocalizationUser : MonoBehaviour
{
    public List<string> strings_to_show = new List<string>();
    private string localization_raw, key;

    public void Reenable(string raw = "", string key = "")
    {
        if (raw != "" && key != "")
        {
            localization_raw = raw;
            this.key = key;
        }   
        findNewText();
        gameObject.SetActive(true);
    }    
    public async void findNewText()
    {
        strings_to_show = await ObjectUtils.AssetManager.LoadTextFromLocalization(localization_raw, key);
    }
    private void OnEnable()
    {
        ObjectUtils.Manager.localizationUsers.Add(this);
    }

    private void OnDisable()
    {
        ObjectUtils.Manager.localizationUsers.Remove(this);
    }
}