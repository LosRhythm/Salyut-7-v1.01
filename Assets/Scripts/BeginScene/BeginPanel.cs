using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginPanel : BasePanel<BeginPanel>
{
    public CustomGUIButton beginButton;
    public CustomGUIButton quitButton;


    // Start is called before the first frame update
    void Start()
    {
        beginButton.clickEvent += () =>
        {
            SceneManager.LoadScene("LaunchScene");
        };

        quitButton.clickEvent += () =>
        {
            Application.Quit();
        };
    }
}
