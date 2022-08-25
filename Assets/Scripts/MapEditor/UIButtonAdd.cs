using System;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    public class UIButtonAdd : MonoBehaviour
    {
        #region singleTon

        private static UIButtonAdd _instance;

        public static UIButtonAdd GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<UIButtonAdd>();
                if (obj == null)
                {
                    Debug.LogError("Error! UIButtonAdd is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion
        
        [SerializeField] private GameObject tempNodePrefab;
        [SerializeField] private GameObject tempNodeObject;
        [SerializeField] private GameObject effectLayer;
        
        private Action updateFunc = delegate { };
        
        public void Click()
        {
            ClickSystem.GetInstance().SubscribeCustomCheckClick(0,AddNodeAtPosition);
            ClickSystem.GetInstance().SubscribeCustomCheckClick(1,CancelAddNode);
            ClickSystem.GetInstance().DisableClickSystem();
            tempNodeObject = Instantiate(tempNodePrefab,effectLayer.transform);
            updateFunc += SetNodePosition;
            
            UIButtonManager.GetInstance().DisableAllUIButton();
        }
        
        private void SetNodePosition()
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition.z = 0;
            tempNodeObject.transform.position = newPosition;
        }
        
        private void AddNodeAtPosition()
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition.z = 0;
            updateFunc = delegate { };
            Destroy(tempNodeObject);
            NodeManager.GetInstance().MakeNewNode(newPosition);
            
            UIButtonManager.GetInstance().EnableAllUIButton();
            
            ClickSystem.GetInstance().ResetCustomCheckClick();
            ClickSystem.GetInstance().EnableClickSystem();
        }

        private void CancelAddNode()
        {
            updateFunc = delegate { };
            Destroy(tempNodeObject);
            UIButtonManager.GetInstance().EnableAllUIButton();
            ClickSystem.GetInstance().ResetCustomCheckClick();
            ClickSystem.GetInstance().EnableClickSystem();
        }
        
        void Update()
        {
            updateFunc();
        }
    }
}
