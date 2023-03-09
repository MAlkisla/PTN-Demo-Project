using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.Scripts.UI
{
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
}
