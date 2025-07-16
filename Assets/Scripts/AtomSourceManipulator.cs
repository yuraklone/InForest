using System.Collections;
using System.Collections.Generic;
using CriWare;
using UnityEngine;


// シーン中の GameObject にアタッチして利用します。
// 設定された Collider リスト中で CriAtomListner に最も近い位置に
// 指定の CriAtomSource を移動させます。
public class AtomSourceManipulator : MonoBehaviour
{
    // 制御対象の CriAtomSource
    public CriAtomSource atomSource;
    // 比較対象の CriAtomListener
    public CriAtomListener listener;
    // CriAtomSource が内部で動く Collider のリスト
    public List<Collider> colliders;

    public float followSpeed = 0.3f;

    Vector3 minVector;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (colliders.Count == 0 || listener == null || atomSource == null)
        {
            return;
        }

        // 設定された Collider から CriAtomListener との最近傍点を計算。
        float minMagnitude = float.MaxValue;
        minVector = Vector3.zero;
        var listenerPosition = listener.transform.position;

        for (int i = 0; i < colliders.Count; i++)
        {
            var closestPoint = colliders[i].ClosestPoint(listenerPosition);

            var dist = (listenerPosition - closestPoint).magnitude;
            if (dist < minMagnitude)
            {
                minMagnitude = dist;
                minVector = closestPoint;
                minVector.y = 0;
            }
        }

        // 最近傍点に CriAtomSource を移動。
        //atomSource.transform.position = minVector;
        


    }
    void LateUpdate()
    {
        atomSource.transform.position = Vector3.Lerp(transform.position, minVector+new Vector3(0,1,0), followSpeed * Time.deltaTime);
        atomSource.transform.rotation = Quaternion.Lerp(transform.rotation, listener.transform.rotation, followSpeed * Time.deltaTime);

    }

}
