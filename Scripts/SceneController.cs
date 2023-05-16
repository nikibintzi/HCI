using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartAsAUser()
    {
        SceneManager.LoadScene("Screen2User");
    }

    public void StartAsABusiness()
    {
        SceneManager.LoadScene("Scene6AddBusiness");
    }

    public void Browse()
    {
        SceneManager.LoadScene("ScreenBrowse");
    }
}

