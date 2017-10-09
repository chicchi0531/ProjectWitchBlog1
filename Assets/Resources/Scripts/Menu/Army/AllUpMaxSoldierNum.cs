using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu
{
    public class AllUpMaxSoldierNum : AllUpWindowBase
    {
        //上昇する兵士数
        [SerializeField]
        private int mUpSoldierNum = 100;

        [SerializeField]
        private Text mManaText = null;

        //マナが足りない場合に表示するテキスト
        [SerializeField]
        private GameObject mWarningText = null;

        private int mManaCost = 0;
        private List<int> mUnits = null;


        // Use this for initialization
        public override void Reset()
        {
            var game = Game.GetInstance();
            mUnits = game.GameData.Group[game.GameData.Territory[0].GroupList[0]].UnitList;
            

            //総消費マナの計算
            var mana = 0.0f;
            foreach (var unit in mUnits)
            {
                var data = game.GameData.Unit[unit];
                mana += mUpSoldierNum * data.SoldierLimitCost;
            }
            mManaCost = (int)mana;

            mManaText.text = mana.ToString();

            if (game.GameData.PlayerMana < mManaCost)
                mWarningText.SetActive(true);
            else
                mWarningText.SetActive(false);
        }

        public override void Yes()
        {
            var game = Game.GetInstance();

            foreach (var unit in mUnits)
            {
                var data = game.GameData.Unit[unit];

                //マナがある限り兵数増員
                if (game.GameData.PlayerMana >= data.SoldierLimitCost * mUpSoldierNum)
                {
                    data.MaxSoldierNum += mUpSoldierNum;
                    data.SoldierNum += mUpSoldierNum;
                }
                else break;

                //マナ消費
                game.GameData.PlayerMana -= (int)(mUpSoldierNum * data.SoldierLimitCost);
            }

            Close();
        }
    }
}