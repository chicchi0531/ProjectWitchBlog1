using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop.Tool
{
    [RequireComponent(typeof(Animator))]
    public class ToolBaseItemInfo : BaseItemInfo
    {

        //各コンポーネントへの参照
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mHP = null;
        [SerializeField]
        private Text mLPAtk = null;
        [SerializeField]
        private Text mLPDef = null;
        [SerializeField]
        private Text mLMAtk = null;
        [SerializeField]
        private Text mLMDef = null;
        [SerializeField]
        private Text mGPAtk = null;
        [SerializeField]
        private Text mGPDef = null;
        [SerializeField]
        private Text mGMAtk = null;
        [SerializeField]
        private Text mGMDef = null;
        [SerializeField]
        private Text mSpeed = null;
        [SerializeField]
        private Text mLeader = null;
        [SerializeField]
        private Text mCur = null;

        //商品選択時に表示するメッセージ
        [SerializeField]
        protected string mMesNameA = "";
        [SerializeField, Multiline]
        protected string mMessageA = "";

        //buy : 商品選択時にマナが足りない場合のメッセージ
        //sell : 商品売却時のメッセージ
        [SerializeField]
        protected string mMesNameB = "";
        [SerializeField, Multiline]
        protected string mMessageB = "";

        //buy : 商品購入時のメッセージ
        //sell : 商品売却時に誰かが装備していた場合の最終警告メッセージ
        [SerializeField]
        protected string mMesNameC = "";
        [SerializeField, Multiline]
        protected string mMessageC = "";



        public override void Start()
        {
            base.Start();
        }

        public override void Reset()
        {
            base.Reset();

            if (ItemID != -1)
            {
                var game = Game.GetInstance();
                var item = game.GameData.Equipment[ItemID];

                mName.text = item.Name;
                mHP.text = item.MaxHP.ToString();
                mLPAtk.text = item.LeaderPAtk.ToString();
                mLPDef.text = item.LeaderPDef.ToString();
                mLMAtk.text = item.LeaderMAtk.ToString();
                mLMDef.text = item.LeaderMDef.ToString();
                mGPAtk.text = item.GroupPAtk.ToString();
                mGPDef.text = item.GroupPDef.ToString();
                mGMAtk.text = item.GroupMAtk.ToString();
                mGMDef.text = item.GroupMDef.ToString();
                mSpeed.text = item.Agility.ToString();
                mLeader.text = item.Leadership.ToString();
                mCur.text = item.Curative.ToString();

                mAnimator.SetBool("IsShow", true);

            }
            else
                mAnimator.SetBool("IsShow", false);
        }
    }
}
