using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    public class TempLine
    {
        public Node startNode;
        public GameObject lineGameObject;
        public LineRenderer lineRenderer;
    }
    
    public class TempLineManager : MonoBehaviour
    {
        #region singleTon

        private static TempLineManager _instance;

        public static TempLineManager GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<TempLineManager>();
                if (obj == null)
                {
                    Debug.LogError("Error! TempLineManager is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion
        
        [SerializeField] private GameObject lineLayer;
        [SerializeField] private GameObject linePrefab;
        
        private TempLine tempLine;

        public TempLine GetTempLine()
        {
            return tempLine;
        }
        
        public void MakeTempLine(Node node)
        {
            GameObject line = Instantiate(linePrefab, lineLayer.transform);
            tempLine = new TempLine
            {
                startNode = node,
                lineGameObject = line,
                lineRenderer = line.GetComponent<LineRenderer>()
            };
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition.z -= 0;
            tempLine.lineRenderer.SetPosition(0,tempLine.startNode.gameObject.transform.position);
            tempLine.lineRenderer.SetPosition(1,newPosition);
        }

        public void MovePos2OfTempLine(Vector3 pos)
        {
            tempLine.lineRenderer.SetPosition(1,pos);
        }

        public void RemoveTempLine()
        {
            Destroy(tempLine.lineGameObject);
            tempLine = null;
        }
    }
}
