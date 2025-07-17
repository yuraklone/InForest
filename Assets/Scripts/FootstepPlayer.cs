using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CriWare;
using UnityEngine;


public class FootstepPlayer : MonoBehaviour
{
    float[] slatmap = new float[0];

    public CriAtomSource criAtomSource;
    string cueName; 
    public string groundTypeTag;

    // 足音の種類毎にタグ名とオーディオクリップを登録する
    // Terrain Layersと足音判定用タグの対応関係を記入する
    [SerializeField] string[] terrainLayerToTag;


    public GameObject rayCastCore;

    bool isGround;

    RaycastHit hitInfo;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        isGround = Physics.Raycast(rayCastCore.transform.position,Vector3.down, out hitInfo, 2f, ~(1 << 6));


    }

    public void PlayFootStepAudio()
    {

        Debug.Log("メソッド");
       
        if (isGround) 
        {
            Debug.Log(hitInfo.collider.tag);
            SwitchFootStepAudio(hitInfo);
            Debug.Log("足音");
        }
        else
        {
            Debug.Log("レイキャストエラー");
        }

        Debug.Log(criAtomSource.cueName);
        criAtomSource.Play();

    }

    void SwitchFootStepAudio(RaycastHit hitInfo)
    {

        switch (hitInfo.collider.tag)
        {
            case "Terrain":
                Debug.Log("テレイン認識");

                // テレインデータ
                TerrainData terrainData = hitInfo.collider.gameObject.GetComponent<Terrain>().terrainData;

                // アルファマップ 
                float[,,] alphaMaps = terrainData.GetAlphamaps(Mathf.FloorToInt(hitInfo.textureCoord.x * terrainData.alphamapWidth), Mathf.FloorToInt(hitInfo.textureCoord.y * terrainData.alphamapHeight), 1, 1);

                int layerCount = terrainData.alphamapLayers; // テレインレイヤーの数

                // 他のテレインに移ったら配列の要素数を変える
                if (slatmap.Length != layerCount) slatmap = new float[layerCount];

                // 三番目の配列を取り出す
                for (int i = 0; i < layerCount; i++)
                {
                    slatmap[i] = alphaMaps[0, 0, i];
                }

                // 最大値のインデックス
                int maxIndex = Array.IndexOf(slatmap, Mathf.Max(slatmap));
                Debug.Log(maxIndex);

                // テレインを名前で判別

                        switch(maxIndex)
                    {
                            case 0:
                                criAtomSource.cueName = "deerfoot_glass";
                                break;
                            case 1:
                                criAtomSource.cueName = "deerfoot_cray";
                                break;
                            case 2:
                                criAtomSource.cueName = "deerfoot_rock";
                                break;
                            case 3:
                                criAtomSource.cueName = "deerfoot_cray";
                                break;
                            case 4:
                                criAtomSource.cueName = "deerfoot_rock";
                                break;
                            default:
                                
                                break;
                            }
                break;

            case "WoodFloor":
                criAtomSource.cueName = "deerfoot_wood";
                break;
            case "Rock":
                criAtomSource.cueName = "deerfoot_rock";
                break;



        }
    }
}
