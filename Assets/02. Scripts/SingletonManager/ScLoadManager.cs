using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScLoadManager : SingletonObject<ScLoadManager>
{
    public float progress;

    private Coroutine _co;

    public void LoadSceneAsync(string targetScene)
    {
        AudioManager.Instance.PlayDon();
        AudioManager.Instance.PlayAudio(sfxType.start1, sfxType.start3);

        if(targetScene == "ScMain")
        {
            GameManager.Instance.curFadeType = fadeType.rainbow;
        }
        else
        {
            GameManager.Instance.curFadeType = fadeType.dan;
        }

        SceneManager.LoadScene("ScLoadStart", LoadSceneMode.Additive);

        _co = StartCoroutine(CoLoadSceneAsync(targetScene));
    }

    IEnumerator CoLoadSceneAsync(string targetScene)
    {       
        progress = 0;

        yield return new WaitForSeconds(1.5f);

        AudioManager.Instance.PauseBgm();

        // 비동기 씬 로드를 시작합니다.
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetScene);
        
        // 씬 로딩 중에는 자동으로 씬을 활성화하지 않도록 설정
        asyncOperation.allowSceneActivation = false;
        
        // 로딩 진행 상황을 확인할 수 있습니다.
        while (asyncOperation.isDone != true)
        {
            // 로딩 진행도(0.0f ~ 0.9f), 0.9f 이후부터는 씬 활성화만 남음
            progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            // 로딩이 끝나면 씬을 활성화할 준비가 완료되었음을 알림 (progress가 0.9f일 때)
            if (asyncOperation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f);

                SceneManager.LoadScene("ScLoadFinish", LoadSceneMode.Additive);

                // 사용자의 입력을 기다리거나 자동으로 씬을 활성화
                asyncOperation.allowSceneActivation = true;

                yield return new WaitForSeconds(1f);

                SceneManager.UnloadSceneAsync("ScLoadFinish");
            }

            yield return null;
        }

        _co = null;
    }
}
