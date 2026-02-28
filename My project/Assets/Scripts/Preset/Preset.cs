using System;
using System.Collections.Generic;
[Serializable]
public abstract class Preset
{
    public PresetType type;

    public string filename;

    public enum PresetType
    {
        None,
        Effect,
        Condition,
        Card,
        Puddle,
    }

    
}

