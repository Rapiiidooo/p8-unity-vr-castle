using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Slider slider;
    public GameObject progressText;
    private TextMeshProUGUI progressTextMeshPro;

    public void PlayGame (int sceneIndex)
    {
        progressTextMeshPro = progressText.GetComponent<TextMeshProUGUI>();
        StartCoroutine(LoadAsynchrone(sceneIndex));
    }

    IEnumerator LoadAsynchrone (int sceneIndex)
    {
        yield return new WaitForSeconds(1.0f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // car operation.isDone == true quand operation.progress = 0.9
            slider.value = progress;
            
            Debug.Log(operation.progress);
            progressTextMeshPro.SetText((progress * 100f) + "%");

            yield return null;
        }
        
    }

    public void QuitGame ()
    {
        Application.Quit();
    }


}
