using UnityEngine;
using UnityEngine.Experimental.AI;

namespace Assets.Scripts.MapEditor
{
    public class LineManipulator : MonoBehaviour
    {
        #region singleTon

        private static LineManipulator _instance;

        public static LineManipulator GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<LineManipulator>();
                if (obj == null)
                {
                    Debug.LogError("Error! LineManipulator is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion

        private Line selectedLine;

        public void SetSelectedLine(Line line)
        {
            selectedLine = line;
            LineSelectEffectManager.GetInstance().LineEffectOn(selectedLine);
        }
        
        public void SetSelectedLine(GameObject line)
        {
            selectedLine = LineManager.GetInstance().GetLine(line);
            LineSelectEffectManager.GetInstance().LineEffectOn(selectedLine);
        }

        public void LineLeftClick()
        {
            
        }
        
        public void ClearSelectedLine()
        {
            selectedLine = null;
            LineSelectEffectManager.GetInstance().LineEffectOff();
        }

        public Line GetSelectedLine()
        {
            return selectedLine;
        }
        
        
        public void UpdateMeshCollider(Line line)
        {
            Mesh mesh = new Mesh();
            line.lineRenderer.BakeMesh(mesh);
            line.lineGameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        
    }
}
