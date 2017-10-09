using UnityEngine;
using UnityEngine.UI;
using System;

namespace ProjectWitch.Menu
{
    public class LvUpWindow : MonoBehaviour
    {

        [SerializeField]
        private GameObject mPanel = null;
        [SerializeField]
        private ArmyMenu mArmyMenu = null;

        [Header("テキストフィールド")]
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mLv = null;
        [SerializeField]
        private Text mHP = null;
        [SerializeField]
        private Text mExperience = null;
        [SerializeField]
        private Text mPAtk = null;
        [SerializeField]
        private Text mMAtk = null;
        [SerializeField]
        private Text mPDef = null;
        [SerializeField]
        private Text mMDef = null;
        [SerializeField]
        private Text mLead = null;
        [SerializeField]
        private Text mAgi = null;

        //プロパティ
        public int UnitID { get; set; }
        public StatusWindow StatusWindow { get; set; }

        //内部変数
        private UnitDataFormat mNewUnit = null;

        // Use this for initialization
        void Start()
        {

        }

        public void Reset()
        {
            var game = Game.GetInstance();
            mNewUnit = game.GameData.Unit[UnitID].Clone();
            var unit = game.GameData.Unit[UnitID];

            //上昇するレベルを計算
            int upLv = (int)(unit.Experience / UnitDataFormat.REQUIPRED_EXPERIENCE_TO_LVUP);
            if (unit.MaxLevel != -1)
                upLv = Math.Min(unit.MaxLevel - unit.Level, upLv);

            //新たなステータスを計算
            mNewUnit.Level += upLv;
            mNewUnit.Experience -= upLv * UnitDataFormat.REQUIPRED_EXPERIENCE_TO_LVUP;

            //表示の更新
            Func<int, int, string> LvParamToString =
                (o, n) =>
                {
                    return o.ToString() + " → " + n.ToString();
                };
            mName.text = unit.Name;
            mLv.text = LvParamToString(unit.Level, mNewUnit.Level);
            mHP.text = LvParamToString(unit.MaxHP, mNewUnit.MaxHP);
            mExperience.text = LvParamToString((int)(unit.Experience*100), (int)(mNewUnit.Experience*100));
            mPAtk.text = LvParamToString(unit.LeaderPAtk, mNewUnit.LeaderPAtk);
            mMAtk.text = LvParamToString(unit.LeaderMAtk, mNewUnit.LeaderMAtk);
            mPDef.text = LvParamToString(unit.LeaderPDef, mNewUnit.LeaderPDef);
            mMDef.text = LvParamToString(unit.LeaderMDef, mNewUnit.LeaderMDef);
            mLead.text = LvParamToString(unit.Leadership, mNewUnit.Leadership);
            mAgi.text = LvParamToString(unit.Agility, mNewUnit.Agility);
        }

        public void Yes()
        {
            var game = Game.GetInstance();

            //var unit = game.GameData.Unit[UnitID];
            //unit = mNewUnit;
            game.GameData.Unit[UnitID] = mNewUnit;

            //HP回復
            game.GameData.Unit[UnitID].HP = game.GameData.Unit[UnitID].MaxHP;

            StatusWindow.Init();

            //閉じる
            Close();
        }

        public void No()
        {
            //閉じる
            Close();
        }

        //ウィンドウを表示
        public void Show(int unitID)
        {
            mArmyMenu.Closable = false;
            UnitID = unitID;
            mPanel.SetActive(true);
            Reset();
        }

        //ウィンドウを閉じる
        private void Close()
        {
            mArmyMenu.Closable = true;
            mPanel.SetActive(false);
        }
    }
}