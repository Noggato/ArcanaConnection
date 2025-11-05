using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;

public class ScriptReader : MonoBehaviour
{
    [SerializeField]
    private TextAsset _InkJsonFile;
    private Story _StoryScript;

    public TMP_Text dialogueBox;
    public TMP_Text nameTag;

    public Image characterIcon;
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
        _StoryScript.BindExternalFunction("CharacterIcon", (string charName) => ChangeCharacterIcon(charName));

        DisplayNextLine();
        
    }

    public void DisplayNextLine() 
    {
        if (_StoryScript.canContinue) // Checking if there is content to go through
        {
            string text = _StoryScript.Continue(); //Gets Next Line
            text = text?.Trim(); //Removes White space from the text
            dialogueBox.text = text; //Displays new text
        }
        else 
        {
            dialogueBox.text = "That's all folks";
        }
    }

    public void ChangeName(string name) 
    {
        string SpeakerName = name;

        nameTag.text = SpeakerName;
    }

    public void ChangeCharacterIcon(string charName) 
    {
        //var characterIconSprite = Resources.Load("CharacterIcons/" + charName) as Sprite;
        characterIcon.sprite = Resources.Load<Sprite>("CharacterIcons/"+charName);
    }
    

}