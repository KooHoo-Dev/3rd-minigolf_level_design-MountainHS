using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CupTrigger : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    [SerializeField] private TMP_Text winText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (sceneIndex == -1)
            {
                QuitGame().Forget();
                return;
            }
            SceneManager.LoadScene(sceneIndex);
        }
    }

    private async UniTask QuitGame()
    {
        winText.gameObject.SetActive(true);
        await UniTask.WaitForSeconds(3f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}
