using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMStarter : MonoBehaviour
{
    GameObject bgm;
    BGMController bgmController;
    public string thisScene;
    public float startDelay = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        bgm = GameObject.FindGameObjectWithTag("BGM");
        bgmController = bgm.GetComponent<BGMController>();
        bgmController.StopBgm();

        Invoke("BGMStart", startDelay);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BGMStart()
    {
        if (thisScene != null)
        {
            if (thisScene == "Title")
            {
                bgmController.PlayBgm(BGMType.Title);
            }
            else if (thisScene == "InGame")
            {
                bgmController.PlayBgm(BGMType.InGame);
            }
        }
    }
}
