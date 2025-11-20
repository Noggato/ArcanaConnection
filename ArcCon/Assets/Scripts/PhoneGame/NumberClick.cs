using UnityEngine;

public class NumberClick : MonoBehaviour
{
    public GameObject ScreenManager;
    public int Number;

    public void ClickOnNumber()
    {
        ScreenManager.GetComponent<ScreenManager>().GetNumberOnClick(Number);
    }
}
