﻿using FMOD;
using FMODUnity;

public static class AudioUtilities
{
    public static StudioEventEmitter FindEmitter(ScriptableEmitter[] emitters, string target)
    {
        foreach(ScriptableEmitter emitter in emitters)
        {
            if (emitter.name.Equals(target)) return emitter.emitter;
        }

        return null;
    }
}

[System.Serializable]
public class ScriptableEmitter
{
    public string name;
    public StudioEventEmitter emitter;
}