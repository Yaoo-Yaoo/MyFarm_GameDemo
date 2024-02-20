using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyFarm.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        public string startSceneName = string.Empty;

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
        }

        private void OnTransitionEvent(string sceneToGo, Vector3 posToGo)
        {
            StartCoroutine(Transition(sceneToGo, posToGo));
        }

        private IEnumerator Transition(string sceneName, Vector3 pos)
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);
            EventHandler.CallMoveToPosition(pos);
        }

        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(newScene);

            EventHandler.CallAfterSceneLoadedEvent();
        }
    }
}
