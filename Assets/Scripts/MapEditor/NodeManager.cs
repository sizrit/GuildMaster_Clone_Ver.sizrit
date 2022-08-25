using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LogManager = Assets.Scripts.MapEditor.Log.LogManager;

namespace Assets.Scripts.MapEditor
{
    public class Node
    {
        public string id;
        public GameObject gameObject;
        public List<Line> lineList = new List<Line>();
        public NodeData data;

        public Node(string id, GameObject gameObject, NodeData data)
        {
            this.id = id;
            this.gameObject = gameObject;
            this.data = data;
        }
    }
    
    public class NodeManager : MonoBehaviour
    {
        #region singleTon

        private static NodeManager _instance;

        public static NodeManager GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<NodeManager>();
                if (obj == null)
                {
                    Debug.LogError("Error! NodeManager is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion
        
        [SerializeField] private GameObject nodePrefab;

        private readonly Dictionary<string, Node> _currentNodeList = new();
        public int nodeCount = 0;

        public Node GetNode(GameObject nodeGameObject)
        {
            if (_currentNodeList.ContainsKey(nodeGameObject.name))
            {
                return _currentNodeList[nodeGameObject.name];
            }

            return null;
        }

        public Node GetNode(string nodeName)
        {
            if (_currentNodeList.ContainsKey(nodeName))
            {
                return _currentNodeList[nodeName];
            }

            return null;
        }

        public void MakeNewNode(Vector3 position)
        {
            string id = SetNodeId();
            
            GameObject nodeObject = Instantiate(nodePrefab, this.transform);
            nodeObject.transform.position = position;
            nodeObject.name = id;

            NodeData data = new NodeData();

            Node newNode = new Node(id, nodeObject, data);

            NodeCollisionManager.GetInstance().SearchNodePosition(newNode);

            _currentNodeList.Add(id, newNode);
            
            ViewInfoModeManager.GetInstance().UpdateInfo();
            
            LogManager.GetInstance().Log("Add Node [Node Id : "+id+"]");
        }

        public void MakeNewNode(string idValue, Vector3 position, NodeData nodeData)
        {
            GameObject nodeObject = Instantiate(nodePrefab, this.transform);
            nodeObject.transform.position = position;
            nodeObject.name = idValue;
            _currentNodeList.Add(idValue,new Node(idValue,nodeObject,nodeData));
        }
        
        private string SetNodeId()
        {
            string id = "N" + nodeCount++.ToString("D4");
            return id;
        }

        public void DeleteNode(Node node)
        {
            List<Line> connectedLineList = new List<Line>();

            foreach (var line in node.lineList)
            {
                connectedLineList.Add(line);
            }
            
            foreach (var line in connectedLineList)
            {
                LineManager.GetInstance().RemoveLine(line);
            }
            Destroy(node.gameObject);
            _currentNodeList.Remove(node.id);
            
            ViewInfoModeManager.GetInstance().UpdateInfo();
            LogManager.GetInstance().Log("Delete Node [Node Id : "+node.id+"]");
        }

        public List<Node> GetAllNode()
        {
            return _currentNodeList.Values.ToList();
        }

        public void ClearAllNode()
        {
            foreach (var node in _currentNodeList)
            {
                Destroy(node.Value.gameObject);    
            }
            _currentNodeList.Clear();
            
            LogManager.GetInstance().Log("Clear All Node");
        }
    }
}
