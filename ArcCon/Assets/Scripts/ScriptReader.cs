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
    public Image character;
    public Image background;

    public Button choiceButton1;
    public Button choiceButton2;
    public Button choiceButton3;
    public TMP_Text choiceText1;
    public TMP_Text choiceText2;
    public TMP_Text choiceText3;
    public GameObject choicePanel;
    
    private Coroutine _currentTypingCoroutine;
    private bool _isShowingChoices = false;
    private bool _isTyping = false;
    private string _currentTypingText = "";

    void Start()
    {
        HideAllChoices();
        LoadStory();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if (_isTyping)
            {
                SkipTyping();
            }
            else if (!_isShowingChoices)
            {
                DisplayNextLine();
            }
        }
        
        // Добавим поддержку выбора цифрами (опционально)
        if (_isShowingChoices)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                MakeChoice(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                MakeChoice(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                MakeChoice(2);
        }
    }

    void LoadStory() 
    {
        _StoryScript = new Story(_InkJsonFile.text);
        
        _StoryScript.BindExternalFunction<string>("Character", ChangeCharacter);
        _StoryScript.BindExternalFunction<string>("Sound", PlaySound);
        _StoryScript.BindExternalFunction<string>("StartMiniGame", StartMiniGameFromDialogue);
        _StoryScript.BindExternalFunction<string>("Background", ChangeBackground);
        
        string continueLabel = PlayerPrefs.GetString("ContinueLabel", "");
        if (!string.IsNullOrEmpty(continueLabel))
        {
            _StoryScript.ChoosePathString(continueLabel);
            PlayerPrefs.DeleteKey("ContinueLabel");
        }
        
        DisplayNextLine();
    }

    void HideAllChoices()
    {
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        choiceButton3.gameObject.SetActive(false);
        choicePanel.SetActive(false);
        _isShowingChoices = false;
    }

    public void ChangeCharacter(string charName)
    {
        if (charName == "None" || charName == "NoneHero" || charName == "Hidden")
        {
            character.gameObject.SetActive(false);
            nameTag.text = "";
        }
        else
        {
            character.gameObject.SetActive(true);
            character.sprite = Resources.Load<Sprite>(charName + "/" + charName + "Height");
            
            nameTag.text = charName switch
            {
                "Dad" => "Продавец",
                "Alice" => "Алиса",
                "Mark" => "Марк",
                _ => charName
            };
        }
    }
    
    public void ChangeBackground(string BackgroundName)
    {
        background.sprite = Resources.Load<Sprite>("Background/" + BackgroundName);
    }
    
    public void DisplayNextLine()
    {
        if (_isTyping)
        {
            SkipTyping();
            return;
        }
        
        if (_StoryScript.canContinue)
        {
            string text = _StoryScript.Continue();
            text = text?.Trim();
            
            // Обрабатываем теги текущей строки
            ProcessCurrentTags();
            
            if (!string.IsNullOrEmpty(text))
            {
                StartTyping(text);
            }
            else
            {
                // Если текст пустой но есть выборы - показываем их
                if (_StoryScript.currentChoices.Count > 0)
                {
                    ShowChoices();
                }
                else
                {
                    // Иначе продолжаем
                    DisplayNextLine();
                }
            }
        }
        else if (_StoryScript.currentChoices.Count > 0)
        {
            ShowChoices();
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    // Важно! Обрабатываем теги текущей строки
    void ProcessCurrentTags()
    {
        List<string> tags = _StoryScript.currentTags;
        if (tags != null && tags.Count > 0)
        {
            foreach (string tag in tags)
            {
                Debug.Log($"Тег: {tag}");
                // Здесь можно обрабатывать специальные теги если нужно
            }
        }
    }

    void StartTyping(string text)
    {
        _currentTypingText = text;
        _currentTypingCoroutine = StartCoroutine(TypeTextCoroutine(text));
    }

    void SkipTyping()
    {
        if (_currentTypingCoroutine != null)
        {
            StopCoroutine(_currentTypingCoroutine);
            _currentTypingCoroutine = null;
        }
        
        dialogueBox.text = _currentTypingText;
        _isTyping = false;
    }

    void ShowChoices()
    {
        _isShowingChoices = true;
        dialogueBox.text = "";
        
        // Сначала скрываем все
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        choiceButton3.gameObject.SetActive(false);
        
        // Показываем нужное количество кнопок
        for (int i = 0; i < _StoryScript.currentChoices.Count && i < 3; i++)
        {
            string choiceText = _StoryScript.currentChoices[i].text;
            string cleanedText = CleanChoiceText(choiceText);
            
            switch (i)
            {
                case 0:
                    choiceText1.text = cleanedText;
                    choiceButton1.gameObject.SetActive(true);
                    SetupButtonListener(choiceButton1, 0);
                    break;
                case 1:
                    choiceText2.text = cleanedText;
                    choiceButton2.gameObject.SetActive(true);
                    SetupButtonListener(choiceButton2, 1);
                    break;
                case 2:
                    choiceText3.text = cleanedText;
                    choiceButton3.gameObject.SetActive(true);
                    SetupButtonListener(choiceButton3, 2);
                    break;
            }
        }
        
        choicePanel.SetActive(true);
        
        // Фокус на первую кнопку (опционально, для геймпада/клавиатуры)
        if (_StoryScript.currentChoices.Count > 0)
        {
            // choiceButton1.Select(); // Раскомментировать если нужно
        }
    }

    void SetupButtonListener(Button button, int index)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            // Небольшая задержка чтобы избежать двойных кликов
            if (_isShowingChoices)
            {
                MakeChoice(index);
            }
        });
    }

    string CleanChoiceText(string text)
    {
        text = text.Trim('"', '«', '»');
        
        // Убираем метки типа (+1 Эмпатия)
        if (text.Contains("(+") && text.Contains(")"))
        {
            int start = text.IndexOf("(+");
            if (start > 0)
            {
                text = text.Substring(0, start).Trim();
            }
        }
        
        return text;
    }

    void MakeChoice(int choiceIndex)
    {
        if (!_isShowingChoices) return; // Защита от повторных кликов
        
        Debug.Log($"Выбран вариант {choiceIndex + 1}");
        
        // Выбираем вариант в Ink
        _StoryScript.ChooseChoiceIndex(choiceIndex);
        
        // Скрываем выборы
        HideAllChoices();
        
        // Очищаем диалоговое окно
        dialogueBox.text = "";
        
        // Продолжаем историю
        DisplayNextLine();
    }

    private IEnumerator TypeTextCoroutine(string text)
    {
        _isTyping = true;
        dialogueBox.text = "";
        
        foreach (char letter in text)
        {
            dialogueBox.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        
        _isTyping = false;
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