using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinaryDataStream
{
    public static void Save<T>(T serializedObject, string fileName)
    {
        var path = Application.persistentDataPath + "/Saves/";
        Directory.CreateDirectory(path);

        var formatter = new BinaryFormatter();
        var fileStream = new FileStream(path + fileName + ".dat", FileMode.Create);

        try
        {
            formatter.Serialize(fileStream, serializedObject);
            //Debug.Log("Saved: path=" + path + ", fileName=" + fileName + ".dat");
        }
        catch (SerializationException e)
        {
            Debug.LogError("Save failed. Error: " + e.Message);
        }
        finally
        {
            fileStream.Close();
        }
    }

    public static bool Exist(string fileName)
    {
        var path = Application.persistentDataPath + "/Saves/";
        var fullFileName = fileName + ".dat";
        return File.Exists(path + fullFileName);
    }

    public static T Read<T>(string fileName)
    {
        if (!Exist(fileName))
            return default;

        var path = Application.persistentDataPath + "/Saves/";
        var formatter = new BinaryFormatter();
        var fileStream = new FileStream(path + fileName + ".dat", FileMode.Open);
        var returnType = default(T);

        try
        {
            returnType = (T)formatter.Deserialize(fileStream);
            //Debug.Log("Read: path=" + path + ", fileName=" + fileName + ".dat");
        }
        catch (SerializationException e)
        {
            Debug.LogError("Read failed. Error: " + e.Message);
        }
        finally
        {
            fileStream.Close();
        }

        return returnType;
    }

    public static void Delete(string fileName)
    {
        var path = Application.persistentDataPath + "/Saves/" + fileName + ".dat";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Delete: path=" + path);
        }        
    }
}
