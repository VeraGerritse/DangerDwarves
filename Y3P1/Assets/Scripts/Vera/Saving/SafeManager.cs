
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Y3P1;

public class SafeManager : MonoBehaviour {
    public static SafeManager instance;
    public SafeFile lastSafeFile;
    public Safe safe;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if(safe == null)
        {
            safe = new Safe();
        }
        if (File.Exists(Application.dataPath + "/SavedGame.xml"))
        {
            print("started Loading");
            lastSafeFile = Load();
            byte[] test = lastSafeFile.test;
            if(lastSafeFile.test != null)
            {
                safe = (Safe)ByteArrayToObject(test);
                print(safe +  "    safe");
                //Safe temp = lastSafeFile.safeFile;
                //safe.gold = temp.gold;
                //test.inInv = temp.inInv;
            }

        }
        else
        {
            lastSafeFile = new SafeFile();
            safe = new Safe();
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            //NotificationManager.instance.NewNotification("Started To Save");
            SaveGame();
        }
    }

    void SaveGame()
    {
        SafeFile();
        SavedGame(lastSafeFile);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void SavedGame(SafeFile toSave)
    {
        //NotificationManager.instance.NewNotification("is saving");
        var serializer = new XmlSerializer(typeof(SafeFile));
        using (var stream = new System.IO.FileStream(Application.dataPath + "/SavedGame.xml", FileMode.Create))
        {
            serializer.Serialize(stream, toSave);
        }
        SafeFile();
    }

    public void LoadGame()
    {
        NotificationManager.instance.NewNotification("AAAAAh");
        LoadFile();
    }

    public SafeFile Load()
    {
        print("loading");
        var serializer = new XmlSerializer(typeof(SafeFile));
        using (var stream = new System.IO.FileStream(Application.dataPath + "/SavedGame.xml", FileMode.Open))
        {
            return serializer.Deserialize(stream) as SafeFile;
        }
    }


    public void SafeFile()
    {
        if (Player.localPlayer == null)
        {
            return;
        }

        safe.testItem = new Item();
        safe.testItem.StartUp("test", 1, 1, new Stats(), 1, 1);
        safe.gold = Player.localPlayer.myInventory.totalGoldAmount;
        safe.inInv = Player.localPlayer.myInventory.allItems;
        lastSafeFile.test = ObjectToByteArray(safe);

    }

    public void LoadFile()
    {
        List<bool> isItem = new List<bool>();
        for (int i = 0; i < safe.inInv.Count; i++)
        {
            if (safe.inInv[i] != null)
            {
                isItem.Add(true);
            }
            else
            {
                isItem.Add(false);
            }
        }
        Player.localPlayer.myInventory.LoadInventory(safe.inInv, safe.gold,isItem);
    }


    private byte[] ObjectToByteArray(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    private object ByteArrayToObject(byte[] bytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(bytes, 0, bytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        object obj = binForm.Deserialize(memStream);

        return obj;
    }
}
