using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[Serializable]
class SaveGame
{
    public readonly int dataPointsCount, batteryCount, bigDataPointsCount, health;

    public SaveGame(int data, int battery, int bigData, int h)
    {
        dataPointsCount= data;
        batteryCount = battery;
        bigDataPointsCount = bigData;
        health = h;
    }
}
public class GameplayController : MonoBehaviour
{
    public static GameplayController game;
    public Text txtDataPoints, txtBattery, txtBigDataPoints;
    public int dataPointsCount, batteryCount, bigDataPointsCount, health, maxHealth;
    public int difficulty;

    public Sprite[] health_bar_sprites;
    public Image health_UI;

    void Awake()
    {
        if (game == null)
        {
            DontDestroyOnLoad(gameObject);
            game = this;
        }
        else if (game != this)
        {
            game.txtDataPoints = txtDataPoints;
            game.txtBattery = txtBattery;
            game.txtBigDataPoints = txtBigDataPoints;
            game.health_UI = health_UI;
            game.txtDataPoints.text = game.dataPointsCount.ToString();
            game.txtBattery.text = game.batteryCount.ToString();
            game.txtBigDataPoints.text = game.bigDataPointsCount.ToString();
            game.health_UI = health_UI;
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        dataPointsCount = 0;
        batteryCount = 0;
        bigDataPointsCount = 0;
        health = 3;
        if (PlayerPrefs.HasKey("Difficulty")) difficulty = PlayerPrefs.GetInt("Difficulty");
        else difficulty = 0;
        maxHealth = 3 - difficulty;
        Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddDataPoint()
    {
        txtDataPoints.text = (++dataPointsCount).ToString();
    }

    public void AddBattery()
    {
        txtBattery.text = (++batteryCount).ToString();
    }

    public void AddBigDataPoint()
    {
        txtBigDataPoints.text = (++bigDataPointsCount).ToString();
    }

    public void ChangeHealth(int change)
    {
        health += change;
        if (health > maxHealth) health = maxHealth;
        if (health > 0)
        switch (difficulty)
        {
            case 0:
                health_UI.sprite = health_bar_sprites[health];
                break;
            case 1:
                health_UI.sprite = health_bar_sprites[health + 3];
                break;
            case 2:
                health_UI.sprite = health_bar_sprites[health + 5];
                break;
        }
        else health_UI.sprite = health_bar_sprites[0];
    }

    public void Save()
    {
        string filename = Application.persistentDataPath + "/savegame.dat";
        SaveGame sg = new SaveGame(dataPointsCount, batteryCount, bigDataPointsCount, health);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filename, FileMode.OpenOrCreate);
        bf.Serialize(file, sg);
        file.Close();
    }

    public void Load()
    {
        string filename = Application.persistentDataPath + "/savegame.dat";
        if (File.Exists(filename)) 
        { 
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filename, FileMode.OpenOrCreate);
            SaveGame sg = (SaveGame)bf.Deserialize(file);
            dataPointsCount = sg.dataPointsCount;
            batteryCount = sg.batteryCount;
            bigDataPointsCount = sg.bigDataPointsCount;
            health = sg.health;
            file.Close(); 
        }
    }
}
