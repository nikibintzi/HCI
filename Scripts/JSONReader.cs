using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class JSONReader : MonoBehaviour
{
    public TextAsset jsonData;
    public ListOfStores mylistOfStores = new ListOfStores();
    //public GameObject Button_Template;
    private List<string> NameList = new List<string>();
    public Text textI;

    [System.Serializable]
    public class Store
    {
        public string Name;
        public string Address;
        public int Price_Range;
        public float Stars;
        public int For_Children;
        public int Accessible;
        public string Description;
        public string Hours;
        public Tag[] Tags;
    }

    [System.Serializable]
    public class Tag
    {
        public List<string> tags;
    }

    [System.Serializable]
    public class ListOfStores
    {
        public Store[] store;
    }

    public void Start()
    {
        //temp = "";
        int j = 1;
        textI = GetComponent<Text>();
        string temp = textI.text;
        //Debug.Log("temp: " + temp);

        mylistOfStores = JsonUtility.FromJson<ListOfStores>(jsonData.text);
        Debug.Log(" mylistOfStores.store.Length: " + mylistOfStores.store.Length);

        for (int i = 0; i < mylistOfStores.store.Length; i++ )
        {
            string store_name = mylistOfStores.store[i].Name;
            //Debug.Log("Found store: " + store_name);
            /*GameObject newButton = new GameObject();
            newButton.AddComponent<Text>(); 
            newButton.AddComponent<Button>();
            newButton.name = (i+1).ToString();
            button_list.Add(newButton);
            Text nameofStore = newButton.GetComponent<Text>();
            nameofStore.text = store_name;*/
            textI.text = textI.text + j + ". "+ store_name + "\n";
            j++;
            //temp = textI.text;
            
        }

    }

    public void ButtonClicked(string str)
    {
        Debug.Log(str + " button clicked.");

    }
}