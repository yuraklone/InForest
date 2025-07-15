using System.Collections;
using System.Collections.Generic;
using CriWare;
using UnityEngine;

public enum SEType
{
    Start, //ゲームクリア
    Quit, //ゲームオーバー
    Pause //発砲時
}

public class SEController : MonoBehaviour
{
    public CriAtomSource seSource;
    public static SEController seController;
    string cueName;

    void Awake()
    {
        if (seController == null)
        {
            seController = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SEPlay(SEType type)
    {
        if(type == SEType.Start)
        {
            cueName = "Start";
            
        }
        else if(type == SEType.Quit)
        {
            cueName = "Quit";
            
        }
        else if (type == SEType.Pause)
        {
            cueName = "Pause";
            
        }
        seSource.Play(cueName);
    }

}
