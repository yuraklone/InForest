using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeSceneLoader : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 2.5f;
    public string loadScene;
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



    //追加した部分
    public void CallCoroutine()
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    public IEnumerator FadeOutAndLoadScene()
    {
        if (loadScene == "InForest")
        {
            seController.SEPlay(SEType.Start);
        }
        else if(loadScene == "Title")
        {
            seController.SEPlay(SEType.Quit);
        }

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
        SceneManager.LoadScene(loadScene); // シーンをロードしてメニューシーンに遷移
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
