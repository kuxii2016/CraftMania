using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Menu : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject createMenu;
    public GameObject loadMenu;
    public GameObject optionenMenu;
    public GameObject multiPlayerMenu;


    //--------------------------------------------------------------WORLDMENUECREATE-------------------------
    public InputField nameT;
    public InputField seedT;
    public Text error;

    public void CreateWorldBack()
    {
        createMenu.SetActive(false);
        mainMenu.SetActive(true);
        loadMenu.SetActive(false);
    }

    public void CreateWorld()
    {
        createMenu.SetActive(true);
        mainMenu.SetActive(false);
        loadMenu.SetActive(false);
        nameT.text = " ";
        seedT.text = " ";
        error.text = " ";
    }

    public void GenerateMap()
    {
        Play(nameT.text, seedT.text);
    }

    public void Play(string name, string seeds)
    {
        if(System.IO.Directory.Exists(Application.dataPath + "\\" + name ))
        {
            error.text = "World already exists.!";
            return;
        }
        int seed = Convert.ToInt32(seeds);
        GameManager.seed = seed;
        GameManager.worldName = name;
        Application.LoadLevel(1);
    }

    //--------------------------------------------------------------WORLDMENUECREATE-------------------------

    //--------------------------------------------------------------WORLDMENUELOAD---------------------------
    public InputField WtL;
    public Text error1;

    public void LoadWorld()
    {
        createMenu.SetActive(false);
        mainMenu.SetActive(false);
        loadMenu.SetActive(true);
    }

    public void LoadMap()
    {
        string wName = WtL.text;
        if (!System.IO.Directory.Exists(Application.dataPath + "\\" + WtL))
        {
            error1.text = "World not Found.!";
            return;
        }
        GameManager.worldName = wName;
        GameManager.seed = int.Parse(System.IO.File.ReadAllText(Application.dataPath + "\\" + WtL + "\\seed"));
        Application.LoadLevel(1);
    }
    //--------------------------------------------------------------WORLDMENUELOAD---------------------------
    public void Optionen()
    {

    }

    public void Multiplayer()
    {

    }

    public void Exit()
    {
        Application.Quit();
    }

}
