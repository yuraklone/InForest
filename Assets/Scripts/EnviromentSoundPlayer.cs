using System.Collections;
using System.Collections.Generic;
using CriWare;
using UnityEngine;

public class EnviromentSoundPlayer : MonoBehaviour
{
    public CriAtomSource criAtomSource;
    public string playSound;
    bool isPlaying;

    // Start is called before the first frame update
    void Start()
    {
        isPlaying = false;
        criAtomSource.Stop();

    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.activeSelf == true && isPlaying == false)
        {
            criAtomSource.Play(playSound);
            isPlaying = true;
        }
        else if(this.gameObject.activeSelf == false && isPlaying == true)
        {
            criAtomSource.Stop();
            isPlaying = false;
        }
    }
}
