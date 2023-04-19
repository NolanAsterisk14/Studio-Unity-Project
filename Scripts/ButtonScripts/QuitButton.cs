using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    [SerializeField]
    Button quitButton;

    void OnEnable()
    {
        quitButton = this.gameObject.GetComponent<Button>();
        quitButton.onClick.AddListener(QuitGame);
    }

    void QuitGame()
    {
        Application.Quit();
        if (Application.isEditor)
        {
            Debug.Log("You exited the game! But, not really. Nerd.");
        }
    }
}
