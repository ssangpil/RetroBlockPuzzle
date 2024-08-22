using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    private void Awake()
    {
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void VibrationOff()
    {
        
    }
}
