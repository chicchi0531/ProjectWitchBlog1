using UnityEngine;


namespace ProjectWitch.Shop
{
    [RequireComponent(typeof(Animator))]
    public class TopMenu : MonoBehaviour
    {   
        //各パーツへの参照
        [SerializeField]
        private Animator mBuyMenu = null;

        [SerializeField]
        private Animator mSellMenu = null;

        [SerializeField]
        private TalkMenu mTalkMenu = null;

        //コントローラ
        [SerializeField]
        private ShopController mController = null;

        //内部コンポーネント
        private Animator mcAnimator = null;

        // Use this for initialization
        void Start()
        {
            mcAnimator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Click_Buy()
        {
            mBuyMenu.SetBool("IsShow", true);
            mcAnimator.SetBool("IsShow", false);
        }

        public void Click_Sell()
        {
            mSellMenu.SetBool("IsShow", true);
            mcAnimator.SetBool("IsShow", false);
        }

        public void Click_Talk()
        {
            mTalkMenu.Begin();
            mcAnimator.SetBool("IsShow", false);
        }
    }
}
