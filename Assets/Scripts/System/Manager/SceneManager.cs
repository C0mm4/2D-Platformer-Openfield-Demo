using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class SceneManager
{
    Image fadeOutImg;
    private float fadeOutTime = 1f;



    public async Task MoveMap(string mapId, string doorId)
    {
        ChangeUIState(UIState.Loading);

        CreateFadeOutObj();
        await FadeOut("InGameScene");
        Stage.LoadMap(mapId);
        CharactorSpawnInLoad(doorId);
        await FadeIn();
        ChangeUIState(UIState.InPlay);
        player.GetComponent<PlayerController>().canMove = true;
    }


    public void CreateFadeOutObj()
    {
        Canvas canvas = GameManager.UIManager.canvas.GetComponent<Canvas>();

        if (canvas != null)
        {
            GameObject fadeOut = Resource.InstantiateAsync("FadeOut");
            fadeOut.transform.SetParent(canvas.transform, false);
            fadeOut.transform.localPosition = Vector3.zero;

            RectTransform rect = fadeOut.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            fadeOutImg = fadeOut.GetComponent<Image>();

        }
        else
        {
            Debug.LogError("Can't Find Canvas Object in this Scene");
        }
    }

    public void CreateBlackOutObj()
    {
        Canvas canvas = GameManager.UIManager.canvas.GetComponent<Canvas>();

        if (canvas != null)
        {
            GameObject fadeOut = InstantiateAsync("FadeOut");
            fadeOut.transform.SetParent(canvas.transform, false);
            fadeOut.transform.localPosition = Vector3.zero;

            RectTransform rect = fadeOut.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            if (fadeOutImg != null)
            {
                Resource.Destroy(fadeOutImg.gameObject);
            }

            fadeOutImg = fadeOut.GetComponent<Image>();
            fadeOutImg.color = Color.black;
        }
        else
        {
            Debug.LogError("Can't Find Canvas Object in this Scene");
        }
    }

    private async Task FadeOut(string scene)
    {
        float t = 0;
        if (fadeOutImg != null)
        {
            fadeOutImg = GameObject.Find("fadeOut 1(Clone)").GetComponent<Image>();
        }

        if (fadeOutImg != null)
        {
            CreateFadeOutObj();
        }
        Color color = fadeOutImg.color;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeOutTime);
            color.a = alpha;
            fadeOutImg.color = color;

            await Task.Yield();
        }
        var sceneLoadAsync = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
        
        while (!sceneLoadAsync.isDone)
        {
            await Task.Yield();
        }
        CreateFadeOutObj();
        fadeOutImg.color = Color.black;
    }

    public async Task FadeIn()
    {
        float t = 0;
        if (fadeOutImg != null)
        {
            fadeOutImg = GameObject.Find("fadeOut 1(Clone)").GetComponent<Image>();
        }
        Color color = fadeOutImg.color;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            color.a = alpha;
            fadeOutImg.color = color;

            await Task.Yield();
        }

        Resource.Destroy(fadeOutImg.gameObject);
    }

    public async Task FadeOut()
    {

        float t = 0;
        if (fadeOutImg != null)
        {
            fadeOutImg = GameObject.Find("fadeOut 1(Clone)").GetComponent<Image>();
        }

        if (fadeOutImg == null)
        {
            CreateFadeOutObj();
        }
        Color color = fadeOutImg.color;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeOutTime);
            color.a = alpha;
            fadeOutImg.color = color;

            await Task.Yield();
        }
    }
}
