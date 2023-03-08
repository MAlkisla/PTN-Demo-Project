using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InformationTab : MonoBehaviour
{
    [SerializeField] private GameObject informationParentObject;
    
    [SerializeField] private TMP_Text selectedObjectNameText;
    [SerializeField] private Image selectedObjectNameImage;
    [SerializeField] private TMP_Text selectedObjectInfoText;
    
    public void SetObjectInformation(string objectName, Sprite objectImage, string objectInfo)
    {
        selectedObjectNameText.text = objectName;
        selectedObjectNameImage.sprite = objectImage;
        selectedObjectInfoText.text = objectInfo;
        informationParentObject.SetActive(true);
    }

    public void CloseInformationTab()
    {
        if(informationParentObject.activeSelf) informationParentObject.SetActive(false);
    }
}
