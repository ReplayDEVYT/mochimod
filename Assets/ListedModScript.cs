using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

public class ListedModScript : MonoBehaviour
{
    public DirectoryInfo modFolder;
    public string pathAwesome;
    public GameObject modLoader, darkOverlay;
    
    public void Start()
    {
        modLoader = GameObject.Find("MANAGER");
    }
    public void SetSelected()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("ListedMod");

        foreach (GameObject mod in gameObjects)
        {

            if (mod == this.gameObject)
            {
                mod.transform.GetChild(1).gameObject.SetActive(false);
                modLoader.GetComponent<ModLoaderScript>().selectedModDirectory = this.modFolder;
                modLoader.GetComponent<ModLoaderScript>().selFolder = this.pathAwesome;
                //print(this.modFolder);
            } else {
                mod.transform.GetChild(1).gameObject.SetActive(true);  
            }
        }

    }
}
