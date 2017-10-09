using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace ProjectWitch.Menu
{
    public class SoldierEditWindow : MonoBehaviour
    {

        [SerializeField]
        private GameObject mPanel = null;
        [SerializeField]
        private ArmyMenu mArmyMenu = null;

        [Header("スライダー")]
        [SerializeField]
        private Slider mSolCur_Slider = null;

        [Header("兵士数表示テキスト")]
        [SerializeField]
        private Text mSolCur_CurrentNum = null;
        [SerializeField]
        private Text mSolCur_MaxNum = null;

        [Header("最大兵士数表示テキスト")]
        [SerializeField]
        private Text mMaxSol_CurrentNum = null;
        [SerializeField]
        private Text mMaxSol_NextNum = null;

        [Header("コスト表示テキスト")]
        [SerializeField]
        private Text mSolCur_Cost = null;

        [Header("消費マナ表示テキスト")]
        [SerializeField]
        private Text mSolCur_Mana = null;
        [SerializeField]
        private Text mSolCurAll_Mana = null;
        [SerializeField]
        private Text mMaxSol_Mana = null;

        [Header("マナ表示テキスト")]
        [SerializeField]
        private Text mCurrentMana = null;

        [Header("ボタン")]
        [SerializeField]
        private Button mSolCur_Button = null;
        [SerializeField]
        private Button mSolCurAll_Button = null;
        [SerializeField]
        private Button mMaxSol_Button = null;

        //内部変数
        private int mUpSoldierNum = 0;
        private int mUpMaxSoldierNum = 0;
        private int mSoldierCurCost = 0;
        private int mSoldierCurAllCost = 0;
        private int mMaxSoldierAddCost = 0;

        //ID
        public int UnitID { get; set; }
        public StatusWindow StatusWindow { get; set; }

        // Use this for initialization
        void Start()
        {
            UnitID = -1;
        }

        // Update is called once per frame
        void Update()
        {
            if (UnitID != -1)
                Reset();
        }

        void Reset()
        {
            var game = Game.GetInstance();
            var unit = game.GameData.Unit[UnitID];
            var currentMana = game.GameData.PlayerMana;

            //スライダーの範囲を設定
            var solMax = unit.MaxSoldierNum - unit.SoldierNum;
            if (solMax == 0) mSolCur_Slider.interactable = false;
            else mSolCur_Slider.interactable = true;
            mSolCur_Slider.maxValue = solMax;

            //上昇する数値の更新
            mUpSoldierNum = (int)mSolCur_Slider.value;
            mUpMaxSoldierNum = 100;

            //総コストの計算
            mSoldierCurCost = (int)(mUpSoldierNum * unit.SoldierCost);
            mSoldierCurAllCost = (int)(solMax * unit.SoldierCost);
            mMaxSoldierAddCost = (int)(mUpMaxSoldierNum * unit.SoldierLimitCost);
            
            //兵数消費コストの表示
            mSolCur_Cost.text = unit.SoldierCost.ToString();

            //消費コストの表示
            mSolCur_Mana.text = mSoldierCurCost.ToString();
            mSolCurAll_Mana.text = mSoldierCurAllCost.ToString();
            mMaxSol_Mana.text = mMaxSoldierAddCost.ToString();

            //マナ表示
            mSolCur_CurrentNum.text = ((int)(mUpSoldierNum + unit.SoldierNum)).ToString();

            //兵数表示更新
            mSolCur_MaxNum.text = ((int)(unit.MaxSoldierNum)).ToString();
            mMaxSol_CurrentNum.text = ((int)(unit.MaxSoldierNum)).ToString();
            mMaxSol_NextNum.text = ((int)(mUpMaxSoldierNum + unit.MaxSoldierNum)).ToString();

            mCurrentMana.text = currentMana.ToString();


            //ボタンの有効無効か
            if (mSoldierCurCost <= currentMana) mSolCur_Button.interactable = true;
            else mSolCur_Button.interactable = false;
            if (mSoldierCurAllCost <= currentMana) mSolCurAll_Button.interactable = true;
            else mSolCurAll_Button.interactable = false;
            if (mMaxSoldierAddCost <= currentMana) mMaxSol_Button.interactable = true;
            else mMaxSol_Button.interactable = false;
        }

        public void Click_CurSoldier()
        {
            var game = Game.GetInstance();
            var unit = game.GameData.Unit[UnitID];

            unit.SoldierNum += mUpSoldierNum;
            game.GameData.PlayerMana -= mSoldierCurCost;
        }

        public void Click_AllCurSoldier()
        {
            var game = Game.GetInstance();
            var unit = game.GameData.Unit[UnitID];

            unit.SoldierNum = unit.MaxSoldierNum;
            game.GameData.PlayerMana -= mSoldierCurAllCost;
        }

        public void Click_AddMaxSoldier()
        {
            var game = Game.GetInstance();
            var unit = game.GameData.Unit[UnitID];

            unit.MaxSoldierNum += mUpMaxSoldierNum;
            unit.SoldierNum += mUpMaxSoldierNum;
            game.GameData.PlayerMana -= mMaxSoldierAddCost;
        }

        public void Show(int unitID)
        {
            UnitID = unitID;
            mArmyMenu.Closable = false;
            mPanel.SetActive(true);
        }

        public void Click_Close()
        {
            StatusWindow.Init();
            mSolCur_Slider.value = 0;

            mArmyMenu.Closable = true;
            mPanel.SetActive(false);
        }
    }
}