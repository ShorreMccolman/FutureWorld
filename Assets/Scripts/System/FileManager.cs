using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BIN = System.Runtime.Serialization.Formatters.Binary;

public static class FileManager {

    public static void SaveBinary<T>(T saveData, string filename)
    {
        BIN.BinaryFormatter bf = new BIN.BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + filename + ".gd");
        bf.Serialize(file, saveData);
        file.Close();
    }

    public static T LoadBinary<T>(string filename)
    {
        string filepath = Application.persistentDataPath + "/" + filename + ".gd";
        if (File.Exists(filepath))
        {
            BIN.BinaryFormatter bf = new BIN.BinaryFormatter();
            FileStream file = File.Open(filepath, FileMode.Open);
            T result = (T)bf.Deserialize(file);
            file.Close();
            return result;
        }
        return default(T);
    }

    public static Texture2D LoadPNG(string filename, string location)
    {
        string filepath = Application.dataPath + "/" + location + "/" + filename + ".png";
        if (!File.Exists(filepath))
            return null;

        byte[] bytes = File.ReadAllBytes(filepath);
        Texture2D tex = new Texture2D(Screen.width, Screen.height);
        tex.LoadImage(bytes);
        return tex;
    }

    public static void SaveFile(string filename, string extension, string location, string body)
    {
        string filepath = Application.dataPath + "/" + location + "/" + filename + "." + extension;
        Debug.Log("Saving to filepath " + filepath);
        File.WriteAllText(filepath, body);
    }

    public static string ReadFile(string filename, string extension, string location)
    {
        string filepath = Application.dataPath + "/" + location + "/" + filename + "." + extension;
        Debug.Log("Reading from filepath " + filepath);
        string result = File.ReadAllText(filepath);
        return result;
    }
}