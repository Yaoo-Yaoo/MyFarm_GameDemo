using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyFarm.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        [SceneName] public string startSceneName = string.Empty;
        private CanvasGroup fadeCanvasGroup;
        private bool isFade = false;

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        private void Start()
        {
            StartCoroutine(LoadSceneSetActive(startSceneName));
            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
        }

        private void OnTransitionEvent(string sceneToGo, Vector3 posToGo)
        {
            if (!isFade)
                StartCoroutine(Transition(sceneToGo, posToGo));
        }

        private IEnumerator Transition(string sceneName, Vector3 pos)
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);
            EventHandler.CallMoveToPosition(pos);
            yield return Fade(0);
            EventHandler.CallPlayerCanMoveEvent();
        }

        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(newScene);

            EventHandler.CallAfterSceneLoadedEvent();
        }

        /// <summary>
        /// 场景淡入淡出
        /// </summary>
        /// <param name="targetAlpha">1是黑色；0是透明</param>
        /// <returns></returns> <summary>
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;
            fadeCanvasGroup.blocksRaycasts = true;

            float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / Settings.fadeDuration;

            while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }

            fadeCanvasGroup.alpha = targetAlpha;
            isFade = false;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }
}
