using TMPro;
using UnityEngine;
using LogManager = Assets.Scripts.MapEditor.Log.LogManager;

namespace Assets.Scripts.MapEditor.NodeInfoManager
{
    public enum InteractiveObjectType
    {
        Node,
        Line
    }
    
    public class ObjectInfoManager : MonoBehaviour
    {
        #region singleTon

        private static ObjectInfoManager _instance;

        public static ObjectInfoManager GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<ObjectInfoManager>();
                if (obj == null)
                {
                    Debug.LogError("Error! ClickSystem is not ready");
                    return null;
                }

                _instance = obj;
            }

            return _instance;
        }

        #endregion

        [SerializeField] private GameObject nodeInfoPrefab;
        [SerializeField] private GameObject lineInfoPrefab;
        [SerializeField] private GameObject infoObject;

        public void ShowObjectInfo(InteractiveObjectType type)
        {
            switch (type)
            {
                case InteractiveObjectType.Node:
                    infoObject = Instantiate(nodeInfoPrefab,this.transform);
                    break;
                
                case InteractiveObjectType.Line:
                    infoObject = Instantiate(lineInfoPrefab,this.transform);
                    break;
            }
            
            infoObject.GetComponent<IObjectInfo>().SetInfo();
        }

        public void Click(RaycastHit2D[] hits)
        {
            infoObject.GetComponent<IObjectInfo>().Click(hits);
        }

        public void CloseObjectInfo()
        {
            if (infoObject!=null)
            {
                infoObject.GetComponent<IObjectInfo>().CloseInfo();
                infoObject = null;
            }
        }

        public void UpdateNodeInfo()
        {
            if (infoObject != null)
            {
                infoObject.GetComponent<NodeInfoManager>().UpdatePositionData();
            }
        }
    }
}
