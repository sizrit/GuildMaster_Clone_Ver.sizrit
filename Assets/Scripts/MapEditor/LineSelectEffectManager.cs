using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    public class LineSelectEffectManager : MonoBehaviour
    {
        #region singleTon

        private static LineSelectEffectManager _instance;

        public static LineSelectEffectManager GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<LineSelectEffectManager>();
                if (obj == null)
                {
                    Debug.LogError("Error! LineSelectEffectManager is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion
        
        [SerializeField] private GameObject lineSelectEffectPrefab;
        [SerializeField] private GameObject lineSelectEffectGameObject =null;

        public void LineEffectOn(Line line)
        {
            LineEffectOff();
            lineSelectEffectGameObject = Instantiate(lineSelectEffectPrefab,this.transform);
            LineRenderer lineRenderer = lineSelectEffectGameObject.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0,line.lineRenderer.GetPosition(0));
            lineRenderer.SetPosition(1,line.lineRenderer.GetPosition(1));
        }

        public void UpdateLineSelectEffect(Line line)
        {
            if (lineSelectEffectGameObject != null)
            {
                LineRenderer lineRenderer = lineSelectEffectPrefab.GetComponent<LineRenderer>();
                lineRenderer.SetPosition(0, line.lineRenderer.GetPosition(0));
                lineRenderer.SetPosition(1, line.lineRenderer.GetPosition(1));
            }
        }

        public void LineEffectOff()
        {
            if (lineSelectEffectGameObject == null) return;
            Destroy(lineSelectEffectGameObject);
            lineSelectEffectGameObject = null;
        }
    }
}
