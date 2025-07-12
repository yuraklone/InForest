using System.Collections;
using System.Collections.Generic;
using CriWare;
using UnityEngine;

public class EnviromentSoundPlayer : MonoBehaviour
{
    public CriAtomSource criAtomSource;
    public string playSound;

    // Start is called before the first frame update
    void Start()
    {
        criAtomSource.Play(playSound);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
