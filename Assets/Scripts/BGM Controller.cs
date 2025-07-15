using System.Collections;
using System.Collections.Generic;
using CriWare;
using UnityEngine;

public enum BGMType
{
    None, //なし
    Title,　//タイトル
    InGame, //ゲーム中
}



public class BGMController : MonoBehaviour
{
    public CriAtomSource bgmSource;
    private CriAtomExPlayer bgmPlayer;
    public static BGMController bgmController;

    public string titleBGMName;
    public string inGameBGMName;
    public int fadeInTime = 1500;
    public int fadeOutTime = 1500;

    public static BGMType playingBGM = BGMType.None;

    void Awake()
    {
        if(bgmController == null)
        {
            bgmController = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        this.bgmPlayer = new CriAtomExPlayer(bgmSource);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBgm(BGMType type)
    {
        if(type != playingBGM)
        {
            playingBGM = type;
            //bgmSource = GetComponent<CriAtomSource>();
            if (type == BGMType.Title)
            {
                bgmSource.cueName = titleBGMName;
            }
            else if (type == BGMType.InGame)
            {
                bgmSource.cueName = inGameBGMName;
            }
            bgmPlayer.AttachFader();
            bgmPlayer.SetFadeInTime(fadeInTime);
            bgmSource.Play();
            Invoke("DetachFader", (fadeInTime / 1000) + 0.5f);
            
        }
    }

    public void StopBgm()
    {
        bgmPlayer.AttachFader();
        bgmPlayer.SetFadeOutTime(fadeOutTime);
        bgmSource.Stop();
        Invoke("DetachFader", (fadeInTime / 1000) + 0.5f);

    }

    public void DetachFader()
    {
        bgmPlayer.DetachFader();
    }

}
