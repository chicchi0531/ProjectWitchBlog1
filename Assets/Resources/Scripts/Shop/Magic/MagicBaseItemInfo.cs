using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop.Magic
{
    [RequireComponent(typeof(Animator))]
    public class MagicBaseItemInfo : BaseItemInfo
    {
        //各コンポーネントへの参照
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mFireCount = null;
        [SerializeField]
        private Text mSkill = null;
        [SerializeField]
        private Text mTiming = null;
        [SerializeField]
        private Text mExplanation = null;

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
        [SerializeField]
        protected string mMesNameC = "";
        [SerializeField, Multiline]
        protected string mMessageC = "";

        public override void Reset()
        {
            base.Reset();

            if (ItemID != -1)
            {
                var game = Game.GetInstance();
                var item = game.GameData.Card[ItemID];

                mName.text = item.Name;
                mFireCount.text = item.Duration.ToString();
                mSkill.text = game.GameData.Skill[item.SkillID].Name;
                mTiming.text = CardDataFormat.CardTimingName[item.Timing];
                mExplanation.text = item.Description;

                mAnimator.SetBool("IsShow", true);

            }
            else
                mAnimator.SetBool("IsShow", false);
        }
    }
}
