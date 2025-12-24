using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    [Header("Экраны")]
    public GameObject codeInputScreen;
    public GameObject codeInputPanel;
    
    [Header("Изображения сообщений")]
    public GameObject successImage;  // GameObject с Image компонентом
    public GameObject errorImage;    // GameObject с Image компонентом
    public GameObject hintImage;     // GameObject с подсказкой
    
    [Header("Настройки")]
    public string correctCode = "555555";
    public int attemptsBeforeHint = 3; // Количество попыток до появления подсказки
    
    private string enteredCode = "";
    private int wrongAttempts = 0; // Счетчик неправильных попыток
    private bool hintShown = false; // Флаг, показывалась ли уже подсказка

    void Start()
    {
        codeInputPanel.SetActive(false);
        codeInputScreen.SetActive(false);
        
        // Добавляем Button компоненты к изображениям
        AddButtonToImage(successImage, OnSuccessImageClick);
        AddButtonToImage(errorImage, OnErrorImageClick);
        AddButtonToImage(hintImage, OnHintImageClick);
        
        // Скрываем все изображения сообщений
        if (successImage != null) successImage.SetActive(false);
        if (errorImage != null) errorImage.SetActive(false);
        if (hintImage != null) hintImage.SetActive(false);
    }

    private void AddButtonToImage(GameObject imageObject, UnityEngine.Events.UnityAction onClickAction)
    {
        if (imageObject != null)
        {
            // Добавляем компонент Button если его нет
            Button button = imageObject.GetComponent<Button>();
            if (button == null)
            {
                button = imageObject.AddComponent<Button>();
            }
            
            // Настраиваем кнопку
            button.onClick.AddListener(onClickAction);
            button.transition = Selectable.Transition.None; // Убираем анимацию нажатия
        }
    }

    public void ShowCodeInput()
    {
        if (!codeInputScreen.activeSelf)
        {
            codeInputScreen.SetActive(true);
            codeInputPanel.SetActive(true);
            ClearCode();
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
            ShowSuccessMessage();
        }
        else
        {
            Debug.Log("Неверный код! Попробуйте снова.");
            wrongAttempts++; // Увеличиваем счетчик неправильных попыток
            Debug.Log($"Неправильных попыток: {wrongAttempts}");
            ShowErrorMessage();
        }
    }

    public void GetNumberOnClick(int number)
    {
        if (enteredCode.Length < 6)
        {
            enteredCode += number.ToString();
            Debug.Log("Введено: " + enteredCode + " (" + enteredCode.Length + "/6)");

            if (enteredCode.Length >= 6)
            {
                CheckCode();
            }
        }
    }
    
    public void ClearCode()
    {
        enteredCode = "";
        Debug.Log("Код очищен");
    }

    private void ShowSuccessMessage()
    {
        // Сброс счетчика при успешном вводе
        wrongAttempts = 0;
        hintShown = false;
        
        // Скрываем панель ввода
        if (codeInputPanel != null)
        {
            codeInputPanel.SetActive(false);
        }
        
        // Скрываем подсказку если она была показана
        if (hintImage != null)
        {
            hintImage.SetActive(false);
        }
        
        // Показываем изображение успеха
        if (successImage != null)
        {
            successImage.SetActive(true);
        }
    }

    private void ShowErrorMessage()
    {
        // Скрываем панель ввода
        if (codeInputPanel != null)
        {
            codeInputPanel.SetActive(false);
        }
        
        // Показываем изображение ошибки
        if (errorImage != null)
        {
            errorImage.SetActive(true);
        }
        
        // Проверяем, нужно ли показать подсказку
        CheckForHint();
    }

    private void CheckForHint()
    {
        // Если достигли нужного количества попыток и подсказка еще не показывалась
        if (wrongAttempts >= attemptsBeforeHint && !hintShown && hintImage != null)
        {
            hintShown = true;
            Debug.Log("Достигнуто максимальное количество попыток. Подсказка будет показана после закрытия ошибки.");
        }
    }

    private void OnSuccessImageClick()
    {
        Debug.Log("Нажато изображение успеха - переход на сцену 1");
        
        // Скрываем изображение успеха
        if (successImage != null)
        {
            successImage.SetActive(false);
        }
        
        // Скрываем весь экран ввода кода
        HideCodeInput();
        
        // Переход на сцену 1
        SceneManager.LoadScene(1);
    }

    private void OnErrorImageClick()
    {
        Debug.Log("Нажато изображение ошибки");
        
        // Скрываем изображение ошибки
        if (errorImage != null)
        {
            errorImage.SetActive(false);
        }
        
        // Показываем подсказку если нужно
        if (hintShown && hintImage != null)
        {
            Debug.Log("Показываем подсказку");
            hintImage.SetActive(true);
        }
        else
        {
            // Если подсказка не нужна, сразу показываем панель ввода
            ShowCodeInputPanel();
        }
    }

    private void OnHintImageClick()
    {
        Debug.Log("Нажато изображение подсказки - скрываем её");
        
        // Скрываем подсказку
        if (hintImage != null)
        {
            hintImage.SetActive(false);
        }
        
        // Показываем панель ввода
        ShowCodeInputPanel();
    }

    private void ShowCodeInputPanel()
    {
        // Показываем панель ввода снова
        if (codeInputPanel != null)
        {
            codeInputPanel.SetActive(true);
        }
        
        // Очищаем код для нового ввода
        ClearCode();
    }
}