using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    public class Line
    {
        public string lineId = "";
        public LineData data = new LineData();
        public readonly List<Node> nodeList = new List<Node>();
        public GameObject lineGameObject;
        public LineRenderer lineRenderer;
    }

    public class LineData
    {
        public int weight = 1;
        public string etc = "";
    }
    
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class LineManager : MonoBehaviour
    {
        #region singleTon

        private static LineManager _instance;

        public static LineManager GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<LineManager>();
                if (obj == null)
                {
                    Debug.LogError("Error! LineManager is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion

        [SerializeField] private GameObject lineLayer;
        [SerializeField] private GameObject linePrefab;
        public int lineCount = 0;

        private List<Line> _currentLineList = new List<Line>();
        
        private string SetLineId()
        {
            string newLineId = "L" + lineCount.ToString("D4");
            lineCount++;
            return newLineId;
        }
        
        public void RequestMakeLine(Node node)
        {
            TempLine tempLine = TempLineManager.GetInstance().GetTempLine();
            
            if (node == tempLine.startNode)
            {
                TempLineManager.GetInstance().RemoveTempLine();
                return;
            }

            if (CheckDuplicated(tempLine.startNode, node))
            {
                TempLineManager.GetInstance().RemoveTempLine();
                return;
            }
            
            Line newLine = new Line();
            newLine.lineId = SetLineId();
            newLine.nodeList.Add(tempLine.startNode);
            newLine.nodeList.Add(node);
            GameObject lineGameObject = Instantiate(linePrefab,lineLayer.transform);
            lineGameObject.name = newLine.lineId;
            newLine.lineGameObject = lineGameObject;
            newLine.lineRenderer = lineGameObject.GetComponent<LineRenderer>();
            newLine.lineRenderer.SetPosition(0,newLine.nodeList[0].gameObject.transform.position);
            newLine.lineRenderer.SetPosition(1,newLine.nodeList[1].gameObject.transform.position);
            LineManipulator.GetInstance().UpdateMeshCollider(newLine);
            _currentLineList.Add(newLine);
            
            node.lineList.Add(newLine);
            tempLine.startNode.lineList.Add(newLine);
            
            ViewInfoModeManager.GetInstance().UpdateInfo();
            
            TempLineManager.GetInstance().RemoveTempLine();
        }
        
        public void UpdateNodeLinePosition(Node node)
        {
            foreach (var line in node.lineList)
            {
                line.lineRenderer.SetPosition(0,line.nodeList[0].gameObject.transform.position);
                line.lineRenderer.SetPosition(1,line.nodeList[1].gameObject.transform.position);
                LineManipulator.GetInstance().UpdateMeshCollider(line);
            }
        }

        private bool CheckDuplicated(Node node01, Node node02)
        {
            foreach (var line01 in node01.lineList)
            {
                foreach (var line02 in node02.lineList)
                {
                    if (line01.lineId == line02.lineId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<Line> GetAllLine()
        {
            return _currentLineList;
        }

        public Line MakeLine(string lineId, List<string> nodeList)
        {
            Line newLine = new Line();
            GameObject newlineObject = Instantiate(linePrefab,lineLayer.transform);
            newLine.lineId = lineId;
            newLine.lineGameObject = newlineObject;
            newLine.lineRenderer = newlineObject.GetComponent<LineRenderer>();
            newLine.nodeList.Add(NodeManager.GetInstance().GetNode(nodeList[0]));
            newLine.nodeList.Add(NodeManager.GetInstance().GetNode(nodeList[1]));
            newLine.lineRenderer.SetPosition(0,newLine.nodeList[0].gameObject.transform.position);
            newLine.lineRenderer.SetPosition(1,newLine.nodeList[1].gameObject.transform.position);
            LineManipulator.GetInstance().UpdateMeshCollider(newLine);
            _currentLineList.Add(newLine);
            return newLine;
        }

        public void RemoveLine(Line line)
        {
            foreach (var node in line.nodeList)
            {
                node.lineList.Remove(line);
            }
            Destroy(line.lineGameObject);
            _currentLineList.Remove(line);
            
            ViewInfoModeManager.GetInstance().UpdateInfo();
        }

        public List<Line> GetAllLineConnectWithNode(Node node)
        {
            List<Line> result = new List<Line>();
            foreach (var line in _currentLineList)
            {
                if (line.nodeList.Contains(node))
                {
                    result.Add(line);
                }
            }

            return result;
        }

        public void ClearAllLine()
        {
            foreach (var line in _currentLineList)
            {
                Destroy(line.lineGameObject);
            }

            _currentLineList = new List<Line>();
        }

        public Line GetLine(GameObject lineGameObject)
        {
            foreach (var line in _currentLineList)
            {
                if (line.lineGameObject == lineGameObject)
                {
                    return line;
                }
            }

            return null;
        }
    }
}


