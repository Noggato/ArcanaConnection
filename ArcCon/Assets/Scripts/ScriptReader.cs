using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using UnityEngine.SceneManagement;

public class ScriptReader : MonoBehaviour
{
    [SerializeField]
    private TextAsset _InkJsonFile;
    private Story _StoryScript;

    public TMP_Text dialogueBox;
    public TMP_Text nameTag;
    public Image characterIcon;
    public Image character;
    public Image background;
    
    private Coroutine _currentTypingCoroutine;

    void Start()
    {
        LoadStory();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            DisplayNextLine();
        }
    }

    void LoadStory() 
    {
        _StoryScript = new Story(_InkJsonFile.text);
        _StoryScript.BindExternalFunction("Character", (string charName) => {
            ChangeCharacter(charName);
        });
        _StoryScript.BindExternalFunction("Sound", (string soundName) => PlaySound(soundName));
        _StoryScript.BindExternalFunction("StartMiniGame", (string sceneName) => StartMiniGameFromDialogue(sceneName));
        _StoryScript.BindExternalFunction("Background", (string BackgroundName) => ChangeBackground(BackgroundName));
        string continueLabel = PlayerPrefs.GetString("ContinueLabel", "");
        if (!string.IsNullOrEmpty(continueLabel))
        {
            _StoryScript.ChoosePathString(continueLabel);
            PlayerPrefs.DeleteKey("ContinueLabel"); // Очищаем после использования
        }
        DisplayNextLine();
    }

    public void ChangeCharacter(string charName)
    {
        character.sprite = Resources.Load<Sprite>(charName + "/" + charName + "Height");
        characterIcon.sprite = Resources.Load<Sprite>(charName + "/" + charName + "Icon");
        
        if(charName == "Dad") nameTag.text = "Аркадий Семенёнович";
        else if(charName == "Alice") nameTag.text = "Алиса";
        else if(charName == "Mark") nameTag.text = "Марк";
    }
    public void ChangeBackground(string BackgroundName)
    {
        background.sprite = Resources.Load<Sprite>("Background" + "/" + BackgroundName);
    }
    public void DisplayNextLine()
    {
        if (_currentTypingCoroutine != null)
        {
            StopCoroutine(_currentTypingCoroutine);
            _currentTypingCoroutine = null;
        }
        
        if (_StoryScript.canContinue)
        {
            string text = _StoryScript.Continue();
            text = text?.Trim();
            _currentTypingCoroutine = StartCoroutine(TypeTextCoroutine(text));            
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    private IEnumerator TypeTextCoroutine(string text){
        dialogueBox.text = "";
        foreach (char letter in text)
        {
            dialogueBox.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        _currentTypingCoroutine = null;
    }

    public void StartMiniGameFromDialogue(string sceneName)
    {
        PlayerPrefs.SetString("ContinueLabel", "after_" + sceneName.ToLower());
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }
    
    public void PlaySound(string soundName)
    {
        AudioClip sound = Resources.Load<AudioClip>("Sounds/" + soundName);
        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);
        }
    }
}