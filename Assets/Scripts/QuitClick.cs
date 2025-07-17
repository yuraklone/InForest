using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitClick : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 2.5f;

    SEController seController;
    BGMController bgmController;
    GameObject bgm;
    GameObject se;

    // Start is called before the first frame update
    void Start()
    {
        bgm = GameObject.FindGameObjectWithTag("BGM");
        bgmController = bgm.GetComponent<BGMController>();

        se = GameObject.FindGameObjectWithTag("SE");
        seController = se.GetComponent<SEController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    
    public void Click()
    {
        StartCoroutine(QuitFadeout());
    }

    public IEnumerator QuitFadeout()
    {
        bgmController.StopBgm();

        fadePanel.enabled = true;                 // パネルを有効化
        float elapsedTime = 0.0f;                 // 経過時間を初期化
        Color startColor = fadePanel.color;       // フェードパネルの開始色を取得
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1.0f); // フェードパネルの最終色を設定

        // フェードアウトアニメーションを実行
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;                        // 経過時間を増やす
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);  // フェードの進行度を計算
            fadePanel.color = Color.Lerp(startColor, endColor, t); // パネルの色を変更してフェードアウト
            yield return null;                                     // 1フレーム待機
        }

        fadePanel.color = endColor;  // フェードが完了したら最終色に設定
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();

#endif
    }


}
