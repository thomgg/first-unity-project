using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.SceneManagement;


public class LevelState : MonoBehaviour
{
    private void Awake()
    {
        Load();
    }

    public void Save()
    {
        string filename = Application.persistentDataPath + "/level" + SceneManager.GetActiveScene().buildIndex + ".dat";
        GameObject[] data = GameObject.FindGameObjectsWithTag("SmallData");
        GameObject[] bigdata = GameObject.FindGameObjectsWithTag("BigData");
        CollectionState cs = new CollectionState
        {
            data = new bool[data.Length],
            bigdata = new bool[bigdata.Length]
        };
        for (int i = 0; i < data.Length; i++)
        {
            cs.data[i] = data[i].activeSelf;
        }
        for (int i = 0; i < bigdata.Length; i++)
        {
            cs.bigdata[i] = bigdata[i].activeSelf;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filename, FileMode.OpenOrCreate);
        bf.Serialize(file, cs);
        file.Close();
    }

    public void Load()
    {
        string filename = Application.persistentDataPath + "/level" + SceneManager.GetActiveScene().buildIndex + ".dat";
        if (File.Exists(filename))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filename, FileMode.Open);
            CollectionState cs = (CollectionState)bf.Deserialize(file);
            file.Close();

            GameObject[] data = GameObject.FindGameObjectsWithTag("SmallData");
            GameObject[] bigdata = GameObject.FindGameObjectsWithTag("BigData");
            for (int i = 0; i < cs.data.Length; i++)
            {
                data[i].SetActive(cs.data[i]);
            }
            for (int i = 0; i < cs.bigdata.Length; i++)
            {
                bigdata[i].SetActive(cs.bigdata[i]);
            }
        }
    }
}

[Serializable]
class CollectionState
{
    public bool[] data, bigdata;
}
