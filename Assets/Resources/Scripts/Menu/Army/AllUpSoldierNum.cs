using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu
{
    public class AllUpSoldierNum : AllUpWindowBase
    {
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
            var mana = 0;
            foreach(var unit in mUnits)
            {
                var data = game.GameData.Unit[unit];

                //回復兵数
                var solNum = data.MaxSoldierNum - data.SoldierNum;

                mana += (int)(solNum * data.SoldierCost);
            }
            mManaCost = mana;

            mManaText.text = mana.ToString();

            if (game.GameData.PlayerMana < mManaCost)
                mWarningText.SetActive(true);
            else
                mWarningText.SetActive(false);
        }

        public override void Yes()
        {
            var game = Game.GetInstance();

            foreach(var unit in mUnits)
            {
                var data = game.GameData.Unit[unit];

                //マナがある限り兵数回復
                var solNum = (int)Mathf.Min(data.MaxSoldierNum - data.SoldierNum, game.GameData.PlayerMana / data.SoldierCost);
                data.SoldierNum += solNum;

                //マナ消費
                game.GameData.PlayerMana -= (int)(solNum * data.SoldierCost);
            }

            Close();
        }
    }
}