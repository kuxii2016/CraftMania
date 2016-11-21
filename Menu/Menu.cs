using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;

public class Menu : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject createMenu;
    public GameObject loadMenu;
    public GameObject optionenMenu;
    public GameObject multiPlayerMenu;
    int pages;
    int selectedID;
    
    //--------------------------------------------------------------WORLDMENUECREATE-------------------------
    public InputField nameT;
    public InputField seedT;
    public Text error;
    public LabelHolder labelholder;

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

        error.text = " ";
    }

    public void GenerateMap()
    {
        Play();
    }

    public void Play()
    {
        if (System.IO.Directory.Exists(Application.dataPath + "\\" + nameT.text))
        {
            error.text = "World already exists.!";
            return;
        }
        int seed = Convert.ToInt32(seedT);
        GameManager.seed = seed;
        GameManager.worldName = nameT.text;
        Application.LoadLevel(1);
    }

    //--------------------------------------------------------------WORLDMENUECREATE-------------------------

    //--------------------------------------------------------------WORLDMENUELOAD---------------------------
    public InputField WtL;

    public void UpdateWorldList()
    {
        string[] worlds = Directory.GetDirectories(Application.dataPath + "\\saves\\");
        pages = Mathf.FloorToInt(worlds.Length / 9);
        labelholder.scrolbar.GetComponent<Scrollbar>().numberOfSteps = pages;
        int pagesValue = Mathf.FloorToInt(labelholder.scrolbar.GetComponent<Scrollbar>().value * pages);
        for (int u = 0; u < 9; u++)
        {
            labelholder.Worlds[u].GetComponentInChildren<Text>().text = "";
        }
        for (int i = 0; i < worlds.Length; i++)
        {
            int page = Mathf.FloorToInt(i / 9);
            if (page == pagesValue)
            {
                labelholder.Worlds[i].GetComponentInChildren<Text>().text = getFolderName(worlds[i]);
            }
        }
    }

    string getFolderName(string path)
    {
        string[] s = path.Split('/');
        string[] s2 = s[s.Length - 1].Split('\\');
        return s2[s2.Length - 1];
    }

    public void SetSelectedID(int id)
    {
        int pagesValue = Mathf.FloorToInt(labelholder.scrolbar.GetComponent<Scrollbar>().value * pages);
        int i = pagesValue * 9;
        i += id;

        for (int u = 0; u < 9; u++)
        {
            labelholder.Worlds[u].GetComponentInChildren<Text>().color = Color.white;
        }
        labelholder.Worlds[id].GetComponentInChildren<Text>().color = Color.blue;
        selectedID = i;
    }

    public void LoadWorld()
    {
        createMenu.SetActive(false);
        mainMenu.SetActive(false);
        loadMenu.SetActive(true);
    }

    public void PlayMap()
    {
        
        if (!System.IO.Directory.Exists(Application.dataPath + "\\saves\\" + labelholder.Worlds[selectedID].GetComponentInChildren<Text>().text))
        {
            error.text = "World not Found.!";
            return;
        }
        GameManager.worldName = labelholder.Worlds[selectedID].GetComponentInChildren<Text>().text;
        GameManager.seed = int.Parse(System.IO.File.ReadAllText(Application.dataPath + "\\saves\\" + labelholder.Worlds[selectedID].GetComponentInChildren<Text>().text + "\\seed"));
        Application.LoadLevel(1);
    }

    public void DeleteMap()
    {
        string path = Application.dataPath + "\\saves\\" + labelholder.Worlds[selectedID].GetComponentInChildren<Text>().text;
        foreach(string f in Directory.GetFiles(path))
        {
            File.Delete(f);
        }
        error.text = "World Deleted.!";
        UpdateWorldList();
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
