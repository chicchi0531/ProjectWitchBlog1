using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class MenuController : MonoBehaviour
    {
        [Header("各メニューへの参照")]
        //各メニューへの参照
        [SerializeField]
        private TopMenu mTopMenu = null;
        public TopMenu TopMenu { get { return mTopMenu; } private set { } }

        [Header("Animator")]
        //各アニメーターへの参照
        [SerializeField]
        private Animator mAnimTop = null;

        [SerializeField]
        private Animator mAnimCommon = null;

        //操作できるか
        public bool InputEnable { get; set; }
     

        public void Start()
        {
            InputEnable = true;

            //トップメニューを開く
            mTopMenu.Open();
        }
    }
}