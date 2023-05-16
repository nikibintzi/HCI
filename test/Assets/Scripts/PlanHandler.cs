using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlanHandler : MonoBehaviour
{
    [SerializeField] Sprite positiveSymbol;
    [SerializeField] Sprite negativeSymbol;
    [SerializeField] Sprite[] hourSprites;
    [SerializeField] Sprite lockSymbol;
    [SerializeField] Sprite unlockSymbol;

    [SerializeField] TextAsset databaseFile;

    [SerializeField] GameObject hourTabObject;
    [SerializeField] GameObject hourTabGroup;
    [SerializeField] GameObject businessTab;
    [SerializeField] Sprite[] businessSprites;

    Businesses businesses;

    string[] HOURS = new string[7] { "Sunrise", "Morning", "Noon", "Afternoon", "Evening", "Night", "Late Night" };
    Business[][] plan = new Business[7][];
    int[] planIndexes = new int[] { 0, 0, 0, 0, 0, 0, 0 }; //The index showcased on each hour.
    int currentTab = -1;
    int[] tabLocks = new int[] { 0, 0, 0, 0, 0, 0, 0 };


    //PlayerPrefs
    int[] hoursOfInterest = new int[7] { 1, 1, 1, 1, 1, 1, 1 };
    bool wantChildrenFriendly = false;
    bool wantAccessible = false;
    string[] tagsOfInterest;

    void Start()
    {

        businesses = JsonUtility.FromJson<Businesses>(databaseFile.text);
        businesses.setUp();



        tagsOfInterest = PlayerPrefs.GetString("tags").Split(",");



        bool showedFirstPlan = false;
        List<string> takenBusinesses = new List<string>();
        for (int i = 0; i < 7; i++)
        {
            if (hoursOfInterest[i] == 1)
            {
                GameObject tab = GameObject.Instantiate(hourTabObject, hourTabGroup.transform);
                tab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = HOURS[i];
                tab.transform.GetChild(1).GetComponent<Image>().sprite = hourSprites[i];
                

                int temp = i;
                tab.GetComponent<Button>().onClick.AddListener(delegate { changeTab(temp); });

                Business[] tieredBusinesses = getTopBusinessesInHour(i);
                plan[i] = tieredBusinesses;

                Business firstSelection = tieredBusinesses[0];
                int j = 0;
                while (takenBusinesses.Contains(firstSelection.Name))
                {
                    j++;
                    firstSelection = tieredBusinesses[j];
                }
                planIndexes[i] = j;
                takenBusinesses.Add(firstSelection.Name);

                if (!showedFirstPlan) 
                { 
                    showedFirstPlan = true; 
                    currentTab = i;
                    tab.GetComponent<Image>().color = new Color(135f/255f, 119f/255f, 1, 1);
                    displayHourlyPlan(); 
                }
            }
        }

    }


    void displayHourlyPlan()
    {
        Business currBusiness = plan[currentTab][planIndexes[currentTab]];
        businessTab.transform.Find("Name").GetChild(0).GetComponent<TextMeshProUGUI>().text = currBusiness.Name;
        businessTab.transform.Find("Address").GetChild(0).GetComponent<TextMeshProUGUI>().text = currBusiness.Address;
        businessTab.transform.Find("Description").GetChild(0).GetComponent<TextMeshProUGUI>().text = currBusiness.Description;
        businessTab.transform.Find("Stars").GetChild(1).GetComponent<TextMeshProUGUI>().text = currBusiness.Stars.ToString();

        string price = "";
        for (int i = 0; i < currBusiness.Price_Range; i++) { price += "$"; }
        if (price == "") { price = "Free"; }
        businessTab.transform.Find("PriceRange").GetChild(0).GetComponent<TextMeshProUGUI>().text = price;

        Sprite childFriendlySprite = currBusiness.For_Children == 1 ? positiveSymbol : negativeSymbol;
        businessTab.transform.Find("OtherInfo").GetChild(0).GetChild(0).GetComponent<Image>().sprite = childFriendlySprite;

        Sprite acessibilitySprite = currBusiness.Accessible == 1 ? positiveSymbol : negativeSymbol;
        businessTab.transform.Find("OtherInfo").GetChild(1).GetChild(0).GetComponent<Image>().sprite = acessibilitySprite;

        Sprite currLock = tabLocks[currentTab] == 1 ? lockSymbol : unlockSymbol;
        businessTab.transform.Find("LockButton").GetChild(0).GetComponent<Image>().sprite = currLock;

        businessTab.transform.Find("BusinessImage").GetComponent<Image>().sprite = businessSprites[currBusiness.id];

    }

    //Called by button to change tab to another.
    public void changeTab(int hour)
    {
        if (currentTab == hour) return;
        hourTabGroup.transform.GetChild(currentTab).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        hourTabGroup.transform.GetChild(hour).GetComponent<Image>().color = new Color(135f/255f, 119f/255f, 1, 1);
        currentTab = hour;
        
        displayHourlyPlan();
    }

    public void changeBusiness(int direction)
    {
        if (tabLocks[currentTab] == 1) return;
        int nextIndex = planIndexes[currentTab] + direction;
        if (nextIndex == plan[currentTab].Length || nextIndex == -1) return;

        planIndexes[currentTab] += direction;
        displayHourlyPlan();
    }

    public void changeLock()
    {
        tabLocks[currentTab] = tabLocks[currentTab] == 1 ? 0 : 1;
        Sprite currLock = tabLocks[currentTab] == 1 ? lockSymbol : unlockSymbol;
        businessTab.transform.Find("LockButton").GetChild(0).GetComponent<Image>().sprite = currLock;
    }

    //Given an hour period's index,
    //Returns the most high-value businesses according to the user's preferences
    Business[] getTopBusinessesInHour(int hour)
    {
        Dictionary<Business, float> tierList = new Dictionary<Business, float>();
        
        foreach (Business b in businesses.businesses) 
        {
            float score = 0;

            if (b.workPeriods[hour] == 0) { score -= 999; }

            score -= b.Price_Range;
            score += b.Stars;

            if (wantChildrenFriendly && b.For_Children == 0) { score -= 10; }
            if (wantAccessible && b.Accessible == 0) { score -= 10; }

            int numberOfTags = b.Tags.Length;
            foreach (string tag in b.Tags)
            {
                if (tagsOfInterest.Contains(tag))
                {
                    score += 2.0f / numberOfTags;
                }
                else
                {
                    score -= 1.0f / numberOfTags;
                }
            }



            tierList.Add(b, score); 
        }


        var temp = tierList.OrderByDescending(pair => pair.Value).ToArray();
        Business[] output = new Business[temp.Length];
        int index = 0;
        foreach (var i in temp)
        {
            output[index] = i.Key;
            index++;
        }

        return output;
    }
    

}
