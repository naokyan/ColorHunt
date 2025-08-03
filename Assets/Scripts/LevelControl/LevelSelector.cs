using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true;
    }

    public void SelectTutorial_all()
    {
        SceneManager.LoadScene("Tutorial-all");
    }

    public void SelectTutorial_1()
    {
        SceneManager.LoadScene("Tutorial-1");
    }
    public void SelectTutorial_2()
    {
        SceneManager.LoadScene("Tutorial-2");
    }
    public void SelectTutorial_3()
    {
        SceneManager.LoadScene("Tutorial-3");
    }
    public void SelectTutorial_4()
    {
        SceneManager.LoadScene("Tutorial-4");
    }
    public void SelectTutorial_5()
    {
        SceneManager.LoadScene("Tutorial-5");
    }

    public void SelectMedium_1()
    {
        SceneManager.LoadScene("Medium-1");
    }
    public void SelectMedium_2()
    {
        SceneManager.LoadScene("Medium-2");
    }
    public void SelectMedium_3()
    {
        SceneManager.LoadScene("Medium-3");
    }
    public void SelectMedium_4()
    {
        SceneManager.LoadScene("Medium-4");
    }

    public void SelectTest()
    {
        SceneManager.LoadScene("Test");
    }
    

    
}
