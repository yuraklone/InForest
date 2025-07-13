using System.Collections;
using System.Collections.Generic;
using CriWare;
using UnityEngine;

public class SEOneShotPlay : MonoBehaviour
{
    public CriAtomSource criAtomSource;
    public string playSound;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SEPlay(string playSound)
    {
        criAtomSource.Play(playSound);
    }

}
