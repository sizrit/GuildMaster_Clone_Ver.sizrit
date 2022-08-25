using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public class ViewInfoModeManager : MonoBehaviour
    {
        #region singleTon

        private static ViewInfoModeManager _instance;

        public static ViewInfoModeManager GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<ViewInfoModeManager>();
                if (obj == null)
                {
                    Debug.LogError("Error! InfoModeManager is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion
        
        [SerializeField] private Sprite checkOnImage;
        [SerializeField] private Sprite checkOffImage;
        [SerializeField] private GameObject checkImageGameObject;
        [SerializeField] private bool isInfoModeOn = false;

        [SerializeField] private GameObject nodeInfoPrefab;
        [SerializeField] private GameObject lineInfoPrefab;
        [SerializeField] private GameObject infoLayer;

        private readonly Dictionary<Node, GameObject> nodeInfoList = new();
        private readonly Dictionary<Line, GameObject> lineInfoList = new();
        

        private void OnEnable()
        {
            checkImageGameObject.GetComponent<Image>().sprite = checkOffImage;
        }

        public void Click(RaycastHit2D[] hits)
        {
            foreach (var hit in hits)
            {
                if (hit.transform == this.transform)
                {
                    isInfoModeOn = !isInfoModeOn;
                    SetCheckImage();
                }
            }
        }

        private void SetCheckImage()
        {
            if (isInfoModeOn)
            {
                checkImageGameObject.GetComponent<Image>().sprite = checkOnImage;
                ViewInfo();
            }
            else
            {
                checkImageGameObject.GetComponent<Image>().sprite = checkOffImage;
                RemoveInfo();
            }
        }

        private void RemoveInfo()
        {
            foreach (var nodeInfo in nodeInfoList)
            {
                Destroy(nodeInfo.Value);
            }
            
            foreach (var lineInfo in lineInfoList)
            {
                Destroy(lineInfo.Value);
            }
            
            nodeInfoList.Clear();
            lineInfoList.Clear();
        }
        
        private void ViewInfo()
        {
            List<Node> nodeList = NodeManager.GetInstance().GetAllNode();
            List<Line> lineList = LineManager.GetInstance().GetAllLine();

            foreach (var node in nodeList)
            {
                GameObject newInfoObject = Instantiate(nodeInfoPrefab, infoLayer.transform);
                newInfoObject.GetComponent<ViewInfoPrefabManager>().SetInfo(node);
                newInfoObject.name = node.id;
                nodeInfoList.Add(node,newInfoObject);
            } 

            foreach (var line in lineList)
            {
                GameObject newInfoObject = Instantiate(nodeInfoPrefab, infoLayer.transform);
                newInfoObject.GetComponent<ViewInfoPrefabManager>().SetInfo(line);
                newInfoObject.name = line.lineId;
                lineInfoList.Add(line,newInfoObject);
            }
        }
        
        public void UpdateInfo()
        {
            if (isInfoModeOn)
            {
                RemoveInfo();
                ViewInfo();
            }
        }
    }
}
