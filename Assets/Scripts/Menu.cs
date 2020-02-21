using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private InputField  mySeedInputField = null;

    [SerializeField]
    private int         myGameScene = -1;

    public void NewGame()
    {
        GameInstance.GetInstance().SetSeed(int.Parse(mySeedInputField.text));
        GameInstance.GetInstance().SetNewGame(true);
        SceneManager.LoadScene(myGameScene);
    }

    public void GenerateRandomSeed()
    {
        mySeedInputField.text = Random.Range(0, 99999).ToString();
    }
}
