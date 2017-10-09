using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu
{
    public class EquipmentInfo : BaseInfo
    {

        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mHP = null;
        [SerializeField]
        private Text mLPAtk = null;
        [SerializeField]
        private Text mLMAtk = null;
        [SerializeField]
        private Text mLPDef = null;
        [SerializeField]
        private Text mLMDef = null;
        [SerializeField]
        private Text mGPAtk = null;
        [SerializeField]
        private Text mGMAtk = null;
        [SerializeField]
        private Text MGPDef = null;
        [SerializeField]
        private Text MGMDef = null;
        [SerializeField]
        private Text mSpeed = null;
        [SerializeField]
        private Text mLeader = null;
        [SerializeField]
        private Text mCur = null;
        [SerializeField]
        private Text mComment = null;

        // Update is called once per frame
        public override void Init()
        {
            base.Init();

            if (ID == -1)
            {
                mName.text = "";
                mHP.text = "";
                mLPAtk.text = "";
                mLMAtk.text = "";
                mLPDef.text = "";
                mLMDef.text = "";
                mGPAtk.text = "";
                mGMAtk.text = "";
                MGPDef.text = "";
                MGMDef.text = "";
                mSpeed.text = "";
                mLeader.text = "";
                mCur.text = "";
                mComment.text = "";
            }
            else
            {
                var game = Game.GetInstance();
                var data = game.GameData.Equipment[ID];

                mName.text = data.Name;
                mHP.text = data.MaxHP.ToString();
                mLPAtk.text = data.LeaderPAtk.ToString();
                mLMAtk.text = data.LeaderMAtk.ToString();
                mLPDef.text = data.LeaderPDef.ToString();
                mLMDef.text = data.LeaderMDef.ToString();
                mGPAtk.text = data.GroupPAtk.ToString();
                mGMAtk.text = data.GroupMAtk.ToString();
                MGPDef.text = data.GroupPDef.ToString();
                MGMDef.text = data.GroupMDef.ToString();
                mSpeed.text = data.Agility.ToString();
                mLeader.text = data.Leadership.ToString();
                mCur.text = data.Curative.ToString();
                mComment.text = data.Description;
            }
        }
    }
}