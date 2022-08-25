using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.MapEditor.NodeInfoManager;
using UnityEngine;
using LogManager = Assets.Scripts.MapEditor.Log.LogManager;

namespace Assets.Scripts.MapEditor
{
    public enum ClickMode
    {
        UI,
        Node,
        ObjectInfo,
        Line,
        Log,
        Null
    }
    
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "Unity.PreferNonAllocApi")]
    public class ClickSystem : MonoBehaviour
    {
        #region singleTon

        private static ClickSystem _instance;

        public static ClickSystem GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<ClickSystem>();
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
        
        [SerializeField] private GameObject currentLeftClickNode;
        [SerializeField] private GameObject currentRightClickNode;
        
        private Action _checkClick = delegate { };

        private Action _customLeftCheckClickFunc = delegate { };
        private Action _customRightCheckClickFunc = delegate { };

        Vector3 prevPosition = Vector3.zero;

        private bool _isClickSystemEnable = true;

        #region Enable & Disable

        public void DisableClickSystem()
        {
            _isClickSystemEnable = false;
        }
        
        public void EnableClickSystem()
        {
            _isClickSystemEnable = true;
        }

        #endregion
        
        #region Subscribe Custom Click Func

        public void SubscribeCustomCheckClick(int button, Action func)
        {
            switch (button)
            {
                case 0:
                    _customLeftCheckClickFunc+= func;
                    break;
                case 1:
                    _customRightCheckClickFunc += func;
                    break;
            }
        }

        public void UnSubscribeCustomCheckClick(int button, Action func)
        {
            switch (button)
            {
                case 0:
                    _customLeftCheckClickFunc += func;
                    break;
                case 1:
                    _customRightCheckClickFunc += func;
                    break;
            }
        }

        public void ResetCustomCheckClick()
        {
            _customLeftCheckClickFunc = delegate { };
            _customRightCheckClickFunc = delegate { };
        }

        #endregion
        
        private void MakeCurrentCheckClick(int button)
        {
            _checkClick = delegate { };

            switch (button)
            {
                case 0:
                    if (_isClickSystemEnable)
                    {
                        _checkClick += CheckLeftClick;
                    }

                    _checkClick += _customLeftCheckClickFunc;
                    break;
                
                case 1:
                    if (_isClickSystemEnable)
                    {
                        _checkClick += CheckRightClick;
                    }

                    _checkClick += _customRightCheckClickFunc;
                    break;
            }
        }

        private ClickMode CheckPriority(RaycastHit2D[]hits2D, RaycastHit[]hits)
        {
            List<(string, ClickMode)> priorityList = new List<(string, ClickMode)>
            {
                ("MapEditorUI", ClickMode.UI),
                ("MapEditorLog", ClickMode.Log),
                ("MapEditorObjectInfo",ClickMode.ObjectInfo),
                ("MapEditorNode", ClickMode.Node),
                ("MapEditorLine", ClickMode.Line),
                ("MapEditorLog", ClickMode.Log)
            };

            foreach (var priority in priorityList)
            {
                foreach (var hit in hits2D)
                {
                    if (hit.transform.CompareTag(priority.Item1))
                    {
                        return priority.Item2;
                    }
                }
            }

            foreach (var priority in priorityList)
            {
                foreach (var hit in hits)
                {
                    if (hit.transform.CompareTag(priority.Item1))
                    {
                        return priority.Item2;
                    }
                }
            }
            return ClickMode.Null;
        }
        
        private GameObject GetNodeFromHits(RaycastHit2D[] hits)
        {
            foreach (var hit in hits)
            {
                if(hit.transform.CompareTag("MapEditorNode"))
                {
                    return hit.transform.gameObject;
                }
            }

            return null;
        }

        private GameObject GetLineFromHits(RaycastHit[] hits)
        {
            foreach (var hit in hits)
            {
                if(hit.transform.CompareTag("MapEditorLine"))
                {
                    return hit.transform.gameObject;
                }
            }

            return null;
        }

        private void CheckLeftClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits2D = Physics2D.GetRayIntersectionAll(ray);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            ClickMode mode = CheckPriority(hits2D, hits);

            switch (mode)
            {
                case ClickMode.UI:
                    ObjectInfoManager.GetInstance().CloseObjectInfo();
                    UIButtonManager.GetInstance().Click(hits2D);
                    ViewInfoModeManager.GetInstance().Click(hits2D);
                    break;
                
                case ClickMode.Log:
                    NodeManipulator.GetInstance().ClearSelectedNode();
                    LineManipulator.GetInstance().ClearSelectedLine();
                    LogManager.GetInstance().Click(hits2D);
                    break;

                case ClickMode.ObjectInfo:
                    ObjectInfoManager.GetInstance().Click(hits2D);
                    break;

                case ClickMode.Node:
                    NodeManipulator.GetInstance().SetSelectedNode(GetNodeFromHits(hits2D));
                    NodeManipulator.GetInstance().NodeLeftClick();

                    ObjectInfoManager.GetInstance().CloseObjectInfo();
                    ObjectInfoManager.GetInstance().ShowObjectInfo(InteractiveObjectType.Node);
                    
                    LineManipulator.GetInstance().ClearSelectedLine();
                    break;
                
                case ClickMode.Line:
                    LineManipulator.GetInstance().SetSelectedLine(GetLineFromHits(hits));
                    LineManipulator.GetInstance().LineLeftClick();
                    
                    ObjectInfoManager.GetInstance().CloseObjectInfo();
                    ObjectInfoManager.GetInstance().ShowObjectInfo(InteractiveObjectType.Line);
                    
                    NodeManipulator.GetInstance().ClearSelectedNode();
                    break;

                case ClickMode.Null:
                    NodeManipulator.GetInstance().ClearSelectedNode();
                    LineManipulator.GetInstance().ClearSelectedLine();
                    ObjectInfoManager.GetInstance().CloseObjectInfo();
                    break;
            }
            
            UIButtonDelete.GetInstance().SetDeleteButton();
        }

        private void CheckRightClick()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D[] hits2D = Physics2D.GetRayIntersectionAll(ray);
                RaycastHit[] hits = Physics.RaycastAll(ray);

                ClickMode mode = CheckPriority(hits2D, hits);

                switch (mode)
                {
                    case ClickMode.Node:
                        NodeManipulator.GetInstance().NodeRightClick(GetNodeFromHits(hits2D));
                        break;
                    
                    case ClickMode.Null:
                        WorldManipulator.GetInstance().WorldRightClick();
                        break;
                }
            }
        }
        
        private void Click()
        {
            if (Input.GetMouseButtonDown(0))
            {
                MakeCurrentCheckClick(0);
                _checkClick();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                MakeCurrentCheckClick(1);
                _checkClick();
            }
        }

        private void Update()
        {
            Click();
        }
    }
}
