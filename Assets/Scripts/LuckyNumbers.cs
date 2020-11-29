using UnityEngine;
using UnityEngine.UI;

public class LuckyNumbers : MonoBehaviour
{
    public Button button;
    public InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        int luckyNumbersSum = 0;
        for (int i = 0; i < 1000000; i++)
        {
            if (IsLucky(i))
                luckyNumbersSum++;
        }
        Debug.Log(luckyNumbersSum);
        Debug.Log(luckyNumbersSum / 1000000.0f);
    }

    bool IsLucky(int number)
    {
        int sum1 = number / 100000 + (number % 100000 / 10000) + (number % 10000 / 1000);        
        int sum2 = (number % 1000 / 100) + (number % 100 / 10) + number % 10;
        if (sum1 == sum2)
            return true;
        return false;
    }
}
