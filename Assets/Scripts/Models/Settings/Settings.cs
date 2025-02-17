﻿#region License
// ====================================================
// Project Porcupine Copyright(C) 2016 Team Porcupine
// This program comes with ABSOLUTELY NO WARRANTY; This is free software, 
// and you are welcome to redistribute it under certain conditions; See 
// file LICENSE, which is part of this source code package, for details.
// ====================================================
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections;

[MoonSharpUserData]
public static class Settings
{
    //// Disabled cause not currently used, if used in future undisable
    //// private static readonly string DefaultSettingsFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, System.IO.Path.Combine("Settings", "Settings.json"));

    private static Dictionary<string, string> settingsDict;

    private static string userSettingsFilePath = System.IO.Path.Combine(
        Application.persistentDataPath, "Settings.json");

    public static string GetSettingWithOverwrite(string key, string defaultValue)
    {
        if (settingsDict == null)
        {
            UnityDebugger.Debugger.LogError("Settings", "Settings Dictionary was not loaded!");
            return defaultValue;
        }

        string keyValue;
        if (GetSetting(key, out keyValue))
        {
            return keyValue;
        }

        settingsDict.Add(key, defaultValue);

        SaveSettings();

        return defaultValue;
    }

    public static void SetSetting(string key, object obj)
    {
        SetSetting(key, obj.ToString());
    }

    public static void SetSetting(string key, string value)
    {
        if (settingsDict == null)
        {
            UnityDebugger.Debugger.LogError("Settings", "Settings Dictionary was not loaded!");
            return;
        }

        // If there wasn't a setting already, we are creating a new one
        bool new_setting = settingsDict.Remove(key) == false;
        settingsDict.Add(key, value);
        UnityDebugger.Debugger.Log("Settings", (new_setting ? "Created new setting : " : "Updated setting : ") + key + " to value of " + value);
        //change music volume

        if (key == "sound_volume_music") {
            UnityDebugger.Debugger.Log("hello123");
            Camera camera = Camera.main;
            MusicManager manager = camera.GetComponent<MusicManager>();
            float volume = float.Parse(value);
            if (volume < 1 && volume > 0){
                manager.setVolume(volume);
            } else {
                manager.setVolume(volume/100);
            }
            
        }

        if (key == "sound_advanced_play")
        {
            Camera camera = Camera.main;
            MusicManager manager = camera.GetComponent<MusicManager>();
            if (value == "True")  // GenericToggle
            {
                manager.playMusic();
            }
            else
            {
                manager.pauseMusic();
            }
        }
    }

    public static bool GetSetting<T>(string key, out T result)
        where T : IConvertible
    {
        result = default(T);

        if (settingsDict == null)
        {
            UnityDebugger.Debugger.LogError("Settings", "Settings Dictionary was not loaded!");
            return false;
        }

        string value;
        if (settingsDict.TryGetValue(key, out value))
        {
            try
            {
                result = (T)Convert.ChangeType(value, typeof(T));
                return true;
            }
            catch (Exception exception)
            {
                UnityDebugger.Debugger.LogErrorFormat("Settings", "Exception {0} while trying to convert {1} to type {2}", exception.Message, value, typeof(T));
                return false;
            }
        }

        UnityDebugger.Debugger.LogError("Settings", "Attempted to access a setting that was not loaded from either the SettingsFile or the Template:\t" + key);
        return false;
    }

    public static string GetSetting(string key)
    {
        if (settingsDict == null)
        {
            UnityDebugger.Debugger.LogError("Settings", "Settings Dictionary was not loaded!");
            return null;
        }

        string value;
        if (settingsDict.TryGetValue(key, out value))
        {
            return value;
        }

        UnityDebugger.Debugger.LogError("Settings", "Attempted to access a setting that was not loaded from either the SettingsFile or the Template:\t" + key);
        return null;
    }

    public static void SaveSettings()
    {
        UnityDebugger.Debugger.Log("Settings", "Settings have changed, so there are settings to save!");

        string jsonData = JsonConvert.SerializeObject(settingsDict, Newtonsoft.Json.Formatting.Indented);
        UnityDebugger.Debugger.Log("Settings", "Saving settings :: " + jsonData);

        // Save the document.
        try
        {
            using (StreamWriter writer = new StreamWriter(userSettingsFilePath))
            {
                writer.WriteLine(jsonData);
            }
        }
        catch (Exception e)
        {
            UnityDebugger.Debugger.LogWarning("Settings", "Settings could not be saved to " + userSettingsFilePath);
            UnityDebugger.Debugger.LogWarning("Settings", e.Message);
        }
    }

    public static void LoadSettings()
    {
        string settingsJsonText = string.Empty;

        SetDefaults();

        // Load the settings Json file.
        // First try the user's private settings file in userSettingsFilePath.
        // If that doesn't work fall back to the settingsTemplate above.
        if (System.IO.File.Exists(userSettingsFilePath) == false)
        {
            UnityDebugger.Debugger.Log("Settings", "User settings file could not be found at '" + userSettingsFilePath);
        }
        else
        {
            try
            {
                settingsJsonText = System.IO.File.ReadAllText(userSettingsFilePath);
            }
            catch (Exception e)
            {
                UnityDebugger.Debugger.LogWarning("Settings", "User settings file could not be found at '" + userSettingsFilePath);
                UnityDebugger.Debugger.LogWarning("Settings", e.Message);
            }
        }

        if (settingsJsonText != string.Empty)
        {
            Dictionary<string, string> jsonDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(settingsJsonText);

            foreach (string keyName in jsonDictionary.Keys)
            {
                settingsDict[keyName] = jsonDictionary[keyName];
            }
        }
        else
        {
            UnityDebugger.Debugger.LogError("No User Settings; Saving Defaults to User Data");
            SaveSettings(); // Save Defaults
        }
    }

    // This will also load in a base template
    private static void SetDefaults()
    {
        // This is quite long due to the fact its a dictionary of a dictionary
        // Only done if it can't find file so it's fine
        // It just gets the dictionary values, then the next values, then since its a jagged array it then simplifies it
        SettingsOption[] settingsOptions = PrototypeManager.SettingsCategories.Values
                                                                         .SelectMany(x => x.headings.Values)
                                                                         .SelectMany(x => x)                 // Collapse
                                                                         .ToArray();

        settingsDict = new Dictionary<string, string>();
        for (int i = 0; i < settingsOptions.Length; i++)
        {
            if (settingsOptions[i].defaultValue != null && settingsOptions[i].key != null)
            {
                settingsDict[settingsOptions[i].key] = settingsOptions[i].defaultValue;
            }
        }
    }
}
