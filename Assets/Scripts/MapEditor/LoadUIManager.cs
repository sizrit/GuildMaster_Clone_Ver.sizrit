using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.MapEditor.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public class LoadUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject loadButton;
        [SerializeField] private GameObject cancelButton;
        [SerializeField] private GameObject textField;
        [SerializeField] private GameObject fileContentPrefab;
        [SerializeField] private GameObject fileListView;
        [SerializeField] private GameObject selectEffectPrefab;
        [SerializeField] private GameObject selectEffectLayer;

        [SerializeField] private GameObject selectedFile;
        
        private readonly Dictionary<string, GameObject> fileContentList = new Dictionary<string, GameObject>();
        private void OnEnable()
        {
            ClickSystem.GetInstance().DisableClickSystem();
            ViewFileList();
        }

        private void ViewFileList()
        {
            string path = Application.streamingAssetsPath + "/Save/";
            DirectoryInfo di = new DirectoryInfo(path);

            foreach (var fileInfo in di.GetFiles("*.json"))
            {
                GameObject newFileContent = Instantiate(fileContentPrefab, fileListView.transform);
                newFileContent.name = fileInfo.Name;
                newFileContent.transform.Find("Text").GetComponent<Text>().text = fileInfo.Name;
                fileContentList.Add(fileInfo.Name, newFileContent);
            }
        }

        private void SelectFile(GameObject selectedGameObject)
        {
            GameObject selectEffect = Instantiate(selectEffectPrefab, selectEffectLayer.transform);
            selectEffect.transform.position = selectedGameObject.transform.position;
            selectedFile = selectedGameObject;
            textField.GetComponent<Text>().text = selectedFile.name;
        }

        private void RemoveSelectEffect()
        {
            for (int i = 0; i < selectEffectLayer.transform.childCount; i++)
            {
                Destroy(selectEffectLayer.transform.GetChild(i).gameObject);
            }
        }
        
        private void Click()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RemoveSelectEffect();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D[] hits2D = Physics2D.GetRayIntersectionAll(ray);

                foreach (var hit in hits2D)
                {
                    foreach (var fileContent in fileContentList)
                    {
                        if (hit.transform == fileContent.Value.transform)
                        {
                            SelectFile(fileContent.Value);
                        }
                    }
                    
                    if (hit.transform == loadButton.transform)
                    {
                        Load();
                    }

                    if (hit.transform == cancelButton.transform)
                    {
                        CloseLoadUI();
                    }
                }
            }
        }

        public void Load()
        {
            if (selectedFile != null)
            {
                NodeManipulator.GetInstance().ClearSelectedNode();
                LineManipulator.GetInstance().ClearSelectedLine();
                NodeManager.GetInstance().ClearAllNode();
                LineManager.GetInstance().ClearAllLine();
                LoadManager.GetInstance().Load(selectedFile.name, CloseLoadUI);
            }
            else
            {
                LogManager.GetInstance().LogError("Load file doesn't selected");
            }
        }

        public void CloseLoadUI()
        {
            ClickSystem.GetInstance().EnableClickSystem();
            Destroy(this.gameObject);
        }

        private void Update()
        {
            Click();
        }
    }
}
