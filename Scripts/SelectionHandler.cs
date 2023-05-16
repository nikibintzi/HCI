using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectionHandler : MonoBehaviour
{
    [SerializeField] GameObject[] selectionFields;



    public void moveToPlanPage()
    {
        string tags = "";
        foreach (GameObject obj in selectionFields)
        {
            if (obj.GetComponent<Toggle>().isOn)
            {
                tags += obj.transform.GetChild(2).GetComponent<Text>().text + ",";
            }
        }
        if (tags.Length != 0) { tags = tags.Substring(0, tags.Length - 1); }
        

        PlayerPrefs.SetString("tags", tags);
        SceneManager.LoadScene("ProgramScene");
    }


}
