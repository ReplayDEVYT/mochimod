using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Diagnostics;
using System;
using System.IO;
using SimpleJSON;

public class ModLoaderScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        themePicker.color = new Color(themePicker.color.r, themePicker.color.g, themePicker.color.b, 1f);

        gamePathField.text = PlayerPrefs.GetString("GamePathString");
        if (PlayerPrefsExtra.GetColor("UIColor") != null)
            themePicker.color = PlayerPrefsExtra.GetColor("UIColor", new Color(1, 1, 1, 1));
        else
            themePicker.color = new Color(1f, 1f, 1f, 1f);      
        
        RefreshModList();
        SetUIColor(themePicker.color);
    }

    // Update is called once per frame
    void Update()
    {
        if (!loadedHowMany.gameObject.activeSelf)
            funnyTime += Time.deltaTime;
    }

    public void SetColorFromPicker()
    {
        SetUIColor(themePicker.color);
    }

    public GameObject[] updateList;
    public void SetUIColor(Color newColor)
    {   
        foreach(GameObject obj in coloredStuff)
        {
            //image
            if(obj.GetComponent<Image>() != null)
                obj.GetComponent<Image>().color = new Color((float)newColor.r, (float)newColor.g, (float)newColor.b, 1.0f);
            //text
            else if(obj.GetComponent<Text>() != null)
                obj.GetComponent<Text>().color = new Color((float)newColor.r, (float)newColor.g, (float)newColor.b, 1.0f);
            //input field
            else if(obj.GetComponent<InputField>() != null)
                obj.GetComponent<InputField>().selectionColor = new Color((float)newColor.r, (float)newColor.g, (float)newColor.b, (float)0.5);
        }
        PlayerPrefsExtra.SetColor("UIColor", new Color((float)newColor.r, (float)newColor.g, (float)newColor.b, 1.0f));

        //update mods

        updateList = GameObject.FindGameObjectsWithTag("ListedMod");
        foreach (GameObject m in updateList)
        {
            m.GetComponent<Image>().color = PlayerPrefsExtra.GetColor("UIColor");
        }       
    }

    //FUNCTIONS FOR LOADING SHIT
    public ModJSON modInfo;
    public string rawJsonString;
    public GameObject[] destroyList;
    public Stopwatch watch;
    public void RefreshModList()
    {
        selectedModDirectory = null;
        
        watch = System.Diagnostics.Stopwatch.StartNew();

        destroyList = GameObject.FindGameObjectsWithTag("ListedMod");
        foreach (GameObject m in destroyList)
        {
            Destroy(m);
        }

        loadedHowMany.gameObject.SetActive(false);

        if(!System.IO.Directory.Exists(gamePathField.text))
        {
            existNotif.gameObject.SetActive(true);
            warningDirecPrompt.SetActive(false);
        } 
        else
        {
            existNotif.gameObject.SetActive(false);
            
            string modsFolderDirectory = System.IO.Path.Combine(gamePathField.text, "mods");
            if(!System.IO.Directory.Exists(modsFolderDirectory))
            {
                warningDirecPrompt.SetActive(true);
            } else {
                warningDirecPrompt.SetActive(false);   
        
                DirectoryInfo modPath = new DirectoryInfo(modsFolderDirectory);
                DirectoryInfo[] modDirs = modPath.GetDirectories("*.*");
        
                foreach (DirectoryInfo dir in modDirs)
                {
                    //print(dir);
                    GameObject newMod = Instantiate(baseModItem, gridLayout);
                    newMod.SetActive(true);
                    newMod.name = "new mod";

                    //set icon
                    newMod.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = LoadSprite(dir + "\\_icon.png");
                    newMod.GetComponent<ListedModScript>().pathAwesome = dir.ToString();
                    newMod.GetComponent<ListedModScript>().modFolder = dir;
                    newMod.GetComponent<Image>().color = PlayerPrefsExtra.GetColor("UIColor");
                    
                    //load info json
                    if (System.IO.File.Exists(dir + "\\_info.json"))
                    {
                        rawJsonString = File.ReadAllText(dir + "\\_info.json");
                        modInfo = JsonUtility.FromJson<ModJSON>(rawJsonString);

                        //set info json
                        if (modInfo.name != "")
                        {
                            newMod.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = modInfo.name;
                        }
                        else
                        {
                            newMod.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "UNKNOWN MODPACK";
                            newMod.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0f, 1f);
                        }

                        if (modInfo.author != "")
                        {
                            newMod.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "by " + modInfo.author;

                        }
                        else
                        {
                            newMod.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "";
                            newMod.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0f, 1f);
                        }

                        if (modInfo.description != "")
                        {
                            newMod.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = modInfo.description;
                        }                   
                        else
                        {
                            newMod.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = "N/A";
                            newMod.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0f, 1f);
                        }
                    } 
                    else 
                    {
                        newMod.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "UNKNOWN MODPACK";
                        newMod.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "";
                        newMod.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = "N/A";
                        newMod.GetComponent<Image>().color = new Color(0.75f, 0f, 0f, 1f);
                    }
                }
                
                watch.Stop();
                loadedHowMany.text = "LOADED " + modDirs.Length + " MODS IN " + (watch.ElapsedMilliseconds).ToString() + " MS";
                loadedHowMany.gameObject.SetActive(true);
                funnyTime = 0;

                //reset mod directories
                modDirs = null;
            }
        }
    }

    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return noIconSprite;
        if (System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        return noIconSprite;
    }

    public void SaveGamePath()
    {
        PlayerPrefs.SetString("GamePathString", gamePathField.text);
        PlayerPrefs.Save();
    }

    //play the mod
    public void PlayMod()
    {
        int fileCount = (int)FileCount(selectedModDirectory);

        if (fileCount > 0)
        {
            loaderPanel.gameObject.SetActive(false);
            launchPanel.gameObject.SetActive(true);
        
            FileInfo[] modFiles = selectedModDirectory.GetFiles();

            foreach (FileInfo mf in modFiles)
            {
                if (mf.Extension.Contains("rpa"))
                {
                    File.Delete(gamePathField.text + "\\game\\" + mf.Name);

                    File.Copy((selectedModDirectory + "\\" + mf.Name).ToString(), (gamePathField.text + "\\game\\" + mf.Name).ToString(), true);
                    print(gamePathField.text + "\\game\\" + mf.Name);
                    launchingStatusText.text = "Moving " + mf.Name;
                    
                    launchBarFill.fillAmount += (1.0f / fileCount);
                }
            }

            if (System.IO.Directory.Exists(selectedModDirectory + "\\scripts"))
            {
                Directory.Delete((gamePathField.text + "\\game\\scripts\\"), true);
                Directory.CreateDirectory(gamePathField.text + "\\game\\scripts\\");

                DirectoryInfo h = new DirectoryInfo(selectedModDirectory + "\\scripts");
                FileInfo[] extraScripts = h.GetFiles();

                foreach(FileInfo s in extraScripts)
                {
                    File.Copy((selectedModDirectory + "\\scripts\\" + s.Name).ToString(), (gamePathField.text + "\\game\\scripts\\" + s.Name).ToString(), true);                   
                }
                
                //Directory.Copy((selectedModDirectory + "\\scripts\\"), (gamePathField.text + "\\game\\scripts\\"), true);
            }

            launchingStatusText.text = "Preparing to launch game\nMetaWare will open shortly";
            StartCoroutine(LaunchAndExit());
        } 
        else
        {
            notifPanel.SetActive(true);
            notifText.text = "No .rpa files have been found within the modpack folder.";
        }
    }

    IEnumerator LaunchAndExit()
    {
        yield return new WaitForSeconds(4);
        print("mod starty");
        System.Diagnostics.Process.Start(gamePathField.text + "/MetaWareHighSchoolDemo.exe");
        launchPanel.gameObject.SetActive(false);
        loaderPanel.gameObject.SetActive(true);
    }

    public void PathNotExist()
    {
        notifPanel.SetActive(true);
        notifText.text = "This directory does not exist, or is not a valid copy of MetaWare High School (Demo).";
    }
    
    //open github
    public void openGithub()
    {
        Application.OpenURL("https://github.com/SilverSpringing/mochimod/");
    }

    public void CreateModsFolder()
    {
        string modsFolderDirectory = System.IO.Path.Combine(gamePathField.text, "mods");
        System.IO.Directory.CreateDirectory(modsFolderDirectory); // returns a DirectoryInfo object
        RefreshModList();
    }
    
    public static long FileCount(DirectoryInfo d)
    {
        int i = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            if (fi.Extension.Contains("rpa"))
                i++;
        }
        return i;
    }


    [HideInInspector] public float funnyTime = 0; 
    public Scrollbar modsScrollbar;
    public RectTransform modListContainer, gridLayout, loaderPanel, settingsPanel, launchPanel;
    public Text loadedHowMany, launchingStatusText, notifText;
    public Button playMod, refreshList, github, settings, createModsButton;
    public InputField gamePathField;
    public GameObject warningDirecPrompt, baseModItem, notifPanel;
    public Image launchBarFill, existNotif;
    public string selFolder;

    public GameObject[] coloredStuff;
    public Text[] coloredTexts;
    public InputField[] coloredFields;
    public FlexibleColorPicker themePicker;

    public Sprite noIconSprite;

    public DirectoryInfo selectedModDirectory;

    public Color uiTheme = new Color(1, 1, 1, 1);
}