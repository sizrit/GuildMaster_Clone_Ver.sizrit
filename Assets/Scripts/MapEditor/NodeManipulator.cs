using System;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.MapEditor.NodeInfoManager;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "Unity.PreferNonAllocApi")]
    public class NodeManipulator : MonoBehaviour
    {
        #region singleTon

        private static NodeManipulator _instance;

        public static NodeManipulator GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<NodeManipulator>();
                if (obj == null)
                {
                    Debug.LogError("Error! NodeManipulator is not ready");
                    return null;
                }

                _instance = obj;
            }

            return _instance;
        }

        #endregion

        private Node selectedNode;

        Action nodeDragFunc = delegate { };

        Vector3 prevPosition = Vector3.zero;

        public void SetSelectedNode(Node node)
        {
            selectedNode = node;
            NodeSelectEffectManager.GetInstance().NodeSelectEffectOn(selectedNode);
        }

        public void SetSelectedNode(GameObject node)
        {
            selectedNode = NodeManager.GetInstance().GetNode(node);
            NodeSelectEffectManager.GetInstance().NodeSelectEffectOn(selectedNode); 
        }

        public void ClearSelectedNode()
        {
            NodeSelectEffectManager.GetInstance().NodeSelectEffectOff();
            selectedNode = null;
        }

        public Node GetSelectedNode()
        {
            return selectedNode;
        }

        public void NodeLeftClick()
        {
            ClickSystem.GetInstance().DisableClickSystem();
            prevPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            prevPosition.z = 0;
            nodeDragFunc = NodeLeftDrag;
        }

        public void NodeRightClick(GameObject nodeGameObject)
        {
            ClickSystem.GetInstance().DisableClickSystem();
            TempLineManager.GetInstance().MakeTempLine(NodeManager.GetInstance().GetNode(nodeGameObject));
            nodeDragFunc = NodeRightDrag;
        }

        private void NodeLeftDrag()
        {
            ObjectInfoManager.GetInstance().UpdateNodeInfo();

            ViewInfoModeManager.GetInstance().UpdateInfo();
            
            if (Input.GetMouseButton(0))
            {
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (newPosition != prevPosition)
                {
                    Vector3 delta = newPosition - prevPosition;
                    delta.z = 0;
                    var position = selectedNode.gameObject.transform.position;
                    position += delta;
                    UpdateNodePosition(selectedNode, position);
                    prevPosition = newPosition;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                NodeCollisionManager.GetInstance().NodeCollisionCheck(selectedNode);
                ClickSystem.GetInstance().EnableClickSystem();
                nodeDragFunc = delegate { };
            }
        }

        private void NodeRightDrag()
        {
            if (Input.GetMouseButton(1))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                TempLineManager.GetInstance().MovePos2OfTempLine(pos);
            }

            if (Input.GetMouseButtonUp(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);
                GameObject tempNode =null;
                
                foreach (var hit in hits)
                {
                    if(hit.transform.CompareTag("MapEditorNode"))
                    {
                        tempNode = hit.transform.gameObject;
                    }
                }

                if (tempNode != null)
                {
                    LineManager.GetInstance().RequestMakeLine(NodeManager.GetInstance().GetNode(tempNode));
                }
                else
                {
                    TempLineManager.GetInstance().RemoveTempLine();
                }
                nodeDragFunc = delegate { };
                ClickSystem.GetInstance().EnableClickSystem();
            }
        }

        public void UpdateNodePosition(Node node, Vector3 position)
        {
            node.gameObject.transform.position = position;
            NodeSelectEffectManager.GetInstance().UpdateNodeSelectEffect(position);
            LineManager.GetInstance().UpdateNodeLinePosition(node);
            foreach (var line in node.lineList)
            {
                LineManipulator.GetInstance().UpdateMeshCollider(line);
            }
        }

        private void Update()
        {
            nodeDragFunc();
        }
    }
}
