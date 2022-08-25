using UnityEngine;

namespace Assets.Scripts.MapEditor.NodeInfoManager
{
    public interface IObjectInfo
    {
        public void Click(RaycastHit2D[] hits);

        public void SetInfo();

        public void CloseInfo();
    }
}
