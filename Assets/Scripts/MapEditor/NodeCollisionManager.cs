using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class NodeCollisionManager
    {
        #region singleTon

        private static NodeCollisionManager _instance;

        public static NodeCollisionManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new NodeCollisionManager();
            }

            return _instance;
        }

        #endregion
        
        private const float Radius = 0.5f;
        private const float Delta = 0.1f;
        
        public void NodeCollisionCheck(Node targetNode)
        {
            List<Node> collisionNodeList = CollisionCheck(targetNode);
            
            if (collisionNodeList.Count == 1)
            {
                AdjustNodePosition(targetNode, collisionNodeList[0]);
            }
            if (collisionNodeList.Count == 2)
            {
                AdjustNodePosition(targetNode, collisionNodeList[0],collisionNodeList[1]);
            }
            if (collisionNodeList.Count == 3)
            {
                SearchNodePosition(targetNode);
            }

            if (CollisionCheck(targetNode).Count > 0)
            {
                SearchNodePosition(targetNode);
            }
        }

        public void SearchNodePosition(Node node)
        {
            Vector3 mousePosition = node.gameObject.transform.position; // Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            node.gameObject.transform.position = mousePosition;
            
            int count = 0;
            while (true)
            {
                if (count > 50)
                {
                    Debug.Log("Fail To Find New Node Position");
                    return;
                }
                
                for (int x = -1*count; x < count+1; x++)
                {
                    for (int y = -1*count; y < count+1; y++)
                    {
                        if (x == -1 * count || x == count || y == -1 * count || y == count)
                        {
                            Vector3 newPosition = new Vector3(x / 10f, y / 10f, 0);
                            newPosition += mousePosition;
                            node.gameObject.transform.position = newPosition;
                            if (CollisionCheck(node).Count == 0)
                            {
                                NodeManipulator.GetInstance().UpdateNodePosition(node, newPosition);
                                return; 
                            }
                        }
                    }
                }

                count++;
            }
        }


        private List<Node> CollisionCheck(Node targetNode)
        {
            List<Node> collisionNodeList = new List<Node>();
            List<Node> nodeList = NodeManager.GetInstance().GetAllNode();
            foreach (var node in nodeList)
            {
                if (node != targetNode)
                {
                    Vector3 position1 = node.gameObject.transform.position;
                    var position2 = targetNode.gameObject.transform.position;
                    float dx = position2.x -position1.x;
                    float dy = position2.y -position1.y;
                    float d = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
                    if (d < Radius * 2)
                    {
                        collisionNodeList.Add(node);
                    }
                }
            }

            return collisionNodeList;
        }

        public void AdjustNodePosition(Node targetNode, Node collisionNode)
        {
            var t = targetNode.gameObject.transform.position;
            var p = collisionNode.gameObject.transform.position;

            Vector3 v = t - p;
            Vector3 nv = Vector3.Normalize(v);
            Vector3 tv = nv * ( 2 * Radius + Delta ) + p;
            
            NodeManipulator.GetInstance().UpdateNodePosition(targetNode,tv);
        }


        private void AdjustNodePosition(Node targetNode, Node collisionNode01, Node collisionNode02)
        {
            Vector2 p1;
            Vector2 p2;
            var c1 = collisionNode01.gameObject.transform.position;
            var c2 = collisionNode02.gameObject.transform.position;

            if (c1.x < c2.x)
            {
                p1 = new Vector2(c1.x, c1.y);
                p2 = new Vector2(c2.x, c2.y);
            }
            else
            {
                p1 = new Vector2(c2.x, c2.y);
                p2 = new Vector2(c1.x, c1.y);
            }

            float angle01 = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x);
            float d = Vector2.Distance(p1, p2);
            float angle02 = Mathf.Acos(0.5f * d / (2 * Radius + Delta));
            float angle03 = angle02 - angle01;

            Vector3 t = targetNode.gameObject.transform.position;
            float targetAngle = Mathf.Atan2(t.y - p1.y, t.x - p1.x);

            if (targetAngle > angle01)
            {
                targetNode.gameObject.transform.position = new Vector3(
                    p2.x - (2 * Radius + Delta) * Mathf.Cos(angle03),
                    p2.y + (2 * Radius + Delta) * Mathf.Sin(angle03), 0);
            }
            else
            {
                targetNode.gameObject.transform.position = new Vector3(
                    p1.x + (2 * Radius + Delta) * Mathf.Cos(angle03),
                    p1.y - (2 * Radius + Delta) * Mathf.Sin(angle03), 0);
            }

            NodeManipulator.GetInstance().UpdateNodePosition(targetNode, targetNode.gameObject.transform.position);
        }
    }
}
