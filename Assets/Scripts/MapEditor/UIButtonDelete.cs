using System;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    public class UIButtonDelete : MonoBehaviour
    {
        #region singleTon

        private static UIButtonDelete _instance;

        public static UIButtonDelete GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<UIButtonDelete>();
                if (obj == null)
                {
                    Debug.LogError("Error! UIButtonDelete is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion
        
        public void Click()
        {
            Node node = NodeManipulator.GetInstance().GetSelectedNode();
            Line line = LineManipulator.GetInstance().GetSelectedLine();
            if (node != null)
            {
                NodeManipulator.GetInstance().ClearSelectedNode();
                NodeManager.GetInstance().DeleteNode(node);
            }
            else if (line != null)
            {
                LineManipulator.GetInstance().ClearSelectedLine();
                LineManager.GetInstance().RemoveLine(line);
            }
            
            SetDeleteButton();
        }

        public void SetDeleteButton()
        {
            Line line = LineManipulator.GetInstance().GetSelectedLine();
            Node node = NodeManipulator.GetInstance().GetSelectedNode();
            if (line == null && node == null)
            {
                UIButtonManager.GetInstance().DisableUIButton(UIButton.Delete);
            }
            else
            {
                UIButtonManager.GetInstance().EnableUIButton(UIButton.Delete);
            }
        }

        private void GetDeleteButton()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Click();
            }
        }

        private void OnEnable()
        {
            UIButtonManager.GetInstance().DisableUIButton(UIButton.Delete);
        }

        public void Update()
        {
            GetDeleteButton();
        }
    }
}
