using System.Collections;
using System.Collections.Generic;
using CriWare;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject configPanel;
    public GameObject pausePanel;

    CriAtomSource bgmSource;
    GameObject bgm;
    GameObject se;
    SEController seController;

    bool isPause;

    public float duckSpeed = 0.00001f;

    // Start is called before the first frame update
    void Start()
    {
        bgm = GameObject.FindGameObjectWithTag("BGM");
        bgmSource = bgm.GetComponent<CriAtomSource>();
        se = GameObject.FindGameObjectWithTag("SE");
        seController = se.GetComponent<SEController>();


    }

    // Update is called once per frame
    void Update()
    {
        if (isPause == false && PlayerController3.gameState == "playing")
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                seController.SEPlay(SEType.Pause);
                configPanel.SetActive(false);
                isPause = true;
                PlayerController3.gameState = "pause";
                bgmSource.volume = Mathf.Lerp(0.6f, 1.0f, duckSpeed * Time.deltaTime);
                pausePanel.SetActive(true);

            }
        }
        else if (isPause == false && PlayerController3.gameState == "pause") return;

        else
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                seController.SEPlay(SEType.Pause);
                pausePanel.SetActive(false);
                isPause = false;
                PlayerController3.gameState = "playing";
                bgmSource.volume = Mathf.Lerp(1.0f, 0.6f, duckSpeed * Time.deltaTime);
                configPanel.SetActive(true);
            }
        }


    }
}
