using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public GameObject codeInputScreen;
    public GameObject codeInputPanel;
    public string correctCode = "555555";
    string enteredCode = "";
    void Start()
    {
        codeInputPanel.SetActive(false);
        codeInputScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowCodeInput()
    {
        if (!codeInputScreen.activeSelf)
        {
            codeInputScreen.SetActive(true);
            codeInputPanel.SetActive(true);
        }
    }
    public void HideCodeInput()
    {
        if (codeInputScreen.activeSelf)
        {
            codeInputScreen.SetActive(false);
            codeInputPanel.SetActive(false);
        }
    }


    public void CheckCode()
    {
        if (enteredCode == correctCode)
        {
            Debug.Log("Код верный! Доступ разрешен.");
            SceneManager.LoadScene(1);
            ClearCode();
        }
        else
        {
            Debug.Log("Неверный код! Попробуйте снова.");
            ClearCode();
        }
    }

    public void GetNumberOnClick(int number)
    {
        if (enteredCode.Length < 6)
        {
            enteredCode += number.ToString();
            Debug.Log(enteredCode);


        }
        if (enteredCode.Length >= 6)
            {
                CheckCode();
                
            }
    }
    
    public void ClearCode()
    {
        enteredCode = "";
        Debug.Log("Код очищен");
    }
}

