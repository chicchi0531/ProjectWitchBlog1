using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu
{
    public class CardInfo : BaseInfo {

        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mNum = null;
        [SerializeField]
        private Text mSkillName = null;
        [SerializeField]
        private Text mTiming = null;
        [SerializeField]
        private Text mComment = null;

        // Update is called once per frame
        public override void Init()
        {
            base.Init();

            if (ID == -1)
            {
                mName.text = "";
                mNum.text = "";
                mSkillName.text = "";
                mTiming.text = "";
                mComment.text = "";
            }
            else
            {
                var game = Game.GetInstance();
                var data = game.GameData.Card[ID];

                mName.text = data.Name;
                mNum.text = data.Duration.ToString();
                mSkillName.text = game.GameData.Skill[data.SkillID].Name;
                mTiming.text = CardDataFormat.CardTimingName[data.Timing];
                mComment.text = data.Description;
            }
        }
    }
}