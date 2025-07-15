using System.Collections;
using System.Collections.Generic;
using CriWare;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{

    public Slider masterVolSlider;
    public Slider bgmVolSlider;
    public Slider envVolSlider;
    public Slider seVolSlider;

    private float masterVolume = 1.0f;

    public string bgmCatName = "BGM";
    public string envCatName = "Enviroment";
    public string seCatName = "SE";
    public string masterBusName = "MasterOut";

    public GameObject configPanel;
    public GameObject volumePanel;

    bool isVolSetting;

    private void Awake()
    {
        CriAtomExAsr.SetBusVolume(masterBusName, masterVolume);
        masterVolSlider.onValueChanged.AddListener
            (
            (value) =>
            {
                masterVolume = value;
                CriAtomExAsr.SetBusVolume(masterBusName, masterVolume);
            });

        bgmVolSlider.onValueChanged.AddListener
            ((value) => { CriAtom.SetCategoryVolume(bgmCatName, value); });
        envVolSlider.onValueChanged.AddListener
            ((value) => { CriAtom.SetCategoryVolume(envCatName, value); });
        seVolSlider.onValueChanged.AddListener
            ((value) => { CriAtom.SetCategoryVolume(seCatName, value); });

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isVolSetting == false)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                configPanel.SetActive(false);
                isVolSetting = true;
                masterVolSlider.value = masterVolume;
                bgmVolSlider.value = CriAtom.GetCategoryVolume(bgmCatName);
                envVolSlider.value = CriAtom.GetCategoryVolume(envCatName);
                seVolSlider.value = CriAtom.GetCategoryVolume(seCatName);
                volumePanel.SetActive(true);

            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                volumePanel.SetActive(false);
                isVolSetting = false;
                configPanel.SetActive(true);
            }
        }



        
    }
}
