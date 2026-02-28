using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectUtils
{
    private static LevelManager levelManager;
    public static LevelManager LevelManager
    {
        get
        {
            if (levelManager == null)
            {
                levelManager = Object.FindAnyObjectByType<LevelManager>();
            }
            return levelManager;
        }
        protected set
        {
            levelManager = value;
        }
    }

    private static AssetManager assetManager;
    public static AssetManager AssetManager
    {
        get
        {
            if (assetManager == null)
            {
                
                AssetManager o = Object.FindAnyObjectByType<AssetManager>();
                if (o != null)
                {
                    assetManager = o;
                }
                else
                {
                    GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefab/AssetManager"));
                    assetManager = obj.GetComponent<AssetManager>();
                }
            }
            return assetManager;
        }
        protected set
        {
            assetManager = value;
        }
    }

    private static ModManager modManager;
    public static ModManager ModManager
    {
        get
        {
            if (modManager == null)
            {
                modManager = Object.FindAnyObjectByType<ModManager>();
            }
            return modManager;
        }
        protected set
        {
            modManager = value;
        }
    }

    private static Manager manager;
    public static Manager Manager
    {
        get
        {
            if (manager == null)
            {
                manager = Object.FindAnyObjectByType<Manager>();
            }
            return manager;
        }
        protected set
        {
            manager = value;
        }
    }

    
}