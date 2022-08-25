using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    public class SaveLoadUILayer : MonoBehaviour
    {
        #region singleTon

        private static SaveLoadUILayer _instance;

        public static SaveLoadUILayer GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<SaveLoadUILayer>();
                if (obj == null)
                {
                    Debug.LogError("Error! SaveUILayer is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion
        
        [SerializeField] private GameObject saveUI;
        [SerializeField] private GameObject loadUI;
        
        public void OpenSaveUI()
        {
            Instantiate(saveUI, this.transform);
        }

        public void OpenLoadUI()
        {
            Instantiate(loadUI, this.transform);
        }
    }
}
