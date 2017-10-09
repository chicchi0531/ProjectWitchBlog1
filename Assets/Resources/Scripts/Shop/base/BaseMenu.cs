using UnityEngine;

namespace ProjectWitch.Shop
{
    public class BaseMenu : MonoBehaviour
    {
        //トップへの参照
        [SerializeField]
        protected Animator mTop = null;

        //メッセージウィンドウ
        [SerializeField]
        protected MessageBox mMesBox = null;
    }
}
