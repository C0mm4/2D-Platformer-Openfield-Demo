using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class UIManager
{
    public GameObject canvas;
    public GameObject interactionUI;

    public void init()
    {
        GameObject go = GameObject.Find("Canvas");
        if (go != null)
        {
            canvas = go;
        }

    }

    public void Update()
    {
        if(canvas == null)
        {
            GameObject go = GameObject.Find("Canvas");
            if (go != null)
            {
                canvas = go;
            }
        }
    }


    public void GenerateInteractionUI()
    {
        if (interactionUI == null)
        {
            GameObject go = InstantiateAsync("InteractionUI");
            Func.SetRectTransform(go);
            interactionUI = go;
        }
    }

    public void DeleteInteractionUI()
    {
        if (interactionUI != null)
        {
            Resource.Destroy(interactionUI.gameObject);
            interactionUI = null;
        }
    }
}

