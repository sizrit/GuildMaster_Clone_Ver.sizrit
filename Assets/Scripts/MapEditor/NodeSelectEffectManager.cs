using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    public class NodeSelectEffectManager : MonoBehaviour
    {
        #region singleTon

        private static NodeSelectEffectManager _instance;

        public static NodeSelectEffectManager GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<NodeSelectEffectManager>();
                if (obj == null)
                {
                    Debug.LogError("Error! NodeSelectEffectManager is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion

        [SerializeField] private GameObject nodeSelectEffectPrefab;
        [SerializeField] private GameObject nodeSelectEffectGameObject =null;
        
        public void NodeSelectEffectOn(Node node)
        {
            NodeSelectEffectOff();
            nodeSelectEffectGameObject = Instantiate(nodeSelectEffectPrefab, this.transform);
            nodeSelectEffectGameObject.transform.position = node.gameObject.transform.position;
        }

        public void UpdateNodeSelectEffect(Vector3 position)
        {
            if (nodeSelectEffectGameObject != null)
            {
                nodeSelectEffectGameObject.transform.position = position;
            }
        }

        public void NodeSelectEffectOff()
        {
            if (nodeSelectEffectGameObject == null) return;
            Destroy(nodeSelectEffectGameObject);
            nodeSelectEffectGameObject = null;
        }

    }
}
