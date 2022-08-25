using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "Unity.PreferNonAllocApi")]
    public class SaveUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject saveButton;
        [SerializeField] private GameObject cancelButton;
        [SerializeField] private GameObject textField;
        [SerializeField] private GameObject fileContentPrefab;
        [SerializeField] private GameObject fileListView;
        
        public void OnEnable()
        {
            ClickSystem.GetInstance().DisableClickSystem();
            ViewFileList();
        }

        private void Click()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D[] hits2D = Physics2D.GetRayIntersectionAll(ray);

                foreach (var hit in hits2D)
                {
                    if (hit.transform == saveButton.transform)
                    {
                        Save();
                    }

                    if (hit.transform == cancelButton.transform)
                    {
                        CloseSaveUI();
                    }
                }
            }
        }

        private void ViewFileList()
        {
            string folderName = Application.streamingAssetsPath + "/Save/";
            DirectoryInfo di = new DirectoryInfo(folderName);
            
            foreach (var file in di.GetFiles("*.json"))
            {
                GameObject newFileContent = Instantiate(fileContentPrefab, fileListView.transform);
                newFileContent.name = file.Name;
                newFileContent.transform.Find("Text").GetComponent<Text>().text = file.Name;
            }
        }

            private void Save()
        {
            string nameValue = textField.GetComponent<TMP_InputField>().text;
            SaveManager.GetInstance().Save(nameValue,CloseSaveUI);
        }

        private void CloseSaveUI()
        {
            ClickSystem.GetInstance().EnableClickSystem();
            Destroy(this.gameObject);
        }

        void Update()
        {
            Click();
        }
    }
}
