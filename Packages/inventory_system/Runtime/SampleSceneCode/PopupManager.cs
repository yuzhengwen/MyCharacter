using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    private GameObject popupObjPrefab;

    public static PopupManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        popupObjPrefab = transform.GetChild(0).gameObject;
        popupObjPrefab.SetActive(false);
    }
    private string text;
    public void ShowPopup(string text)
    {
        this.text = text;
        StartCoroutine(ShowPopupCo());
    }
    private IEnumerator ShowPopupCo()
    {
        GameObject popupObj = Instantiate(popupObjPrefab, gameObject.transform);
        popupObj.GetComponentInChildren<TextMeshProUGUI>().text = text;
        popupObj.transform.SetParent(gameObject.transform);
        popupObj.SetActive(true);
        yield return new WaitForSeconds(2f);
        Destroy(popupObj);
    }
}
