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
    
    // ДОБАВЛЕНО: переменная для отслеживания текущей корутины
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

        _StoryScript.BindExternalFunction("Name", (string charName) => ChangeName(charName));
        _StoryScript.BindExternalFunction("Icon", (string charName) => ChangeCharacterIcon(charName));
        _StoryScript.BindExternalFunction("Character", (string charName) => ChangeCharacter(charName));
        _StoryScript.BindExternalFunction("Sound", (string soundName) => PlaySound(soundName));
        DisplayNextLine();
        
    }

    public void DisplayNextLine()
    {
        // ДОБАВЛЕНО: Останавливаем предыдущую корутину печати
        if (_currentTypingCoroutine != null)
        {
            StopCoroutine(_currentTypingCoroutine);
            _currentTypingCoroutine = null;
        }
        
        if (_StoryScript.canContinue) // Checking if there is content to go through
        {
            string text = _StoryScript.Continue();
            text = text?.Trim();
            // ИСПРАВЛЕНО: Сохраняем ссылку на корутину
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
        // ДОБАВЛЕНО: Обнуляем ссылку когда корутина завершена
        _currentTypingCoroutine = null;
    }

    public void ChangeName(string name) 
    {
        string SpeakerName = name;

        nameTag.text = SpeakerName;
    }

    public void ChangeCharacterIcon(string charName)
    {
        characterIcon.sprite = Resources.Load<Sprite>(charName + "/" + charName + "Icon");
    }
    public void ChangeCharacter(string charName)
    {
        character.sprite = Resources.Load<Sprite>(charName + "/" + charName + "Height");
    }
     public void PlaySound(string soundName)
    {
        AudioClip sound = Resources.Load<AudioClip>("Sounds/" + soundName);
        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);
            Debug.Log("Проигрываем звук: " + soundName);
        }
        else
        {
            Debug.LogError("Звук не найден: Sounds/" + soundName);
        }
    }
}