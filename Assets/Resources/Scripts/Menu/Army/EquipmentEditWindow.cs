using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu
{
    public class EquipmentEditWindow : MonoBehaviour
    {
        //装備品ID
        public int ItemID { get; set; }

        //装備者のID
        public int UnitID { get; set; }

        //ステータスウィンドウ
        public StatusWindow StatusWindow { get; set; }

        [SerializeField]
        private GameObject mPanel = null;

        [SerializeField]
        private ArmyMenu mArmyMenu = null;

        //リストへの参照
        [SerializeField]
        private EquipmentEditList mList = null;
        

        [Header("現在の値への参照")]
        [SerializeField]
        private Text mEquipmentName = null;
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mHP = null;
        [SerializeField]
        private Text mPAtk = null;
        [SerializeField]
        private Text mPDef = null;
        [SerializeField]
        private Text mMAtk = null;
        [SerializeField]
        private Text mMDef = null;
        [SerializeField]
        private Text mGPAtk = null;
        [SerializeField]
        private Text mGPDef = null;
        [SerializeField]
        private Text mGMAtk = null;
        [SerializeField]
        private Text mGMDef = null;
        [SerializeField]
        private Text mLeader = null;
        [SerializeField]
        private Text mAgility = null;
        [SerializeField]
        private Text mCur = null;

        [Header("変更後の値への参照")]
        [SerializeField]
        private Text mAf_HP = null;
        [SerializeField]
        private Text mAf_PAtk = null;
        [SerializeField]
        private Text mAf_PDef = null;
        [SerializeField]
        private Text mAf_MAtk = null;
        [SerializeField]
        private Text mAf_MDef = null;
        [SerializeField]
        private Text mAf_GPAtk = null;
        [SerializeField]
        private Text mAf_GPDef = null;
        [SerializeField]
        private Text mAf_GMAtk = null;
        [SerializeField]
        private Text mAf_GMDef = null;
        [SerializeField]
        private Text mAf_Leader = null;
        [SerializeField]
        private Text mAf_Agility = null;
        [SerializeField]
        private Text mAf_Cur = null;
        
        [Header("テキストの色")]
        [SerializeField]
        private Color mPlusColor = Color.red;
        [SerializeField]
        private Color mMinusColor = Color.blue;
        private Color mDefaultColor = Color.black;

        // Use this for initialization
        void Start()
        {
            ItemID = -1;
            mDefaultColor = mEquipmentName.color;
        }

        //ウィンドウを表示
        public void Show(int unitID)
        {
            UnitID = unitID;
            ItemID = Game.GetInstance().GameData.Unit[UnitID].Equipment;
            mArmyMenu.Closable = false;
            mPanel.SetActive(true);
            mList.Reset();
            Reset();
        }

        //ウィンドウを閉じる
        public void Close()
        {
            mArmyMenu.Closable = true;
            mPanel.SetActive(false);
        }

        //ステータスのみのリセット
        public void Reset()
        {
            var game = Game.GetInstance();
            var unit = game.GameData.Unit[UnitID];

            mName.text = unit.Name;
            mHP.text = unit.MaxHP.ToString();
            mPAtk.text = unit.LeaderPAtk.ToString();
            mPDef.text = unit.LeaderPDef.ToString();
            mMAtk.text = unit.LeaderMAtk.ToString();
            mMDef.text = unit.LeaderMDef.ToString();
            mGPAtk.text = unit.GroupPAtk.ToString();
            mGPDef.text = unit.GroupPDef.ToString();
            mGMAtk.text = unit.GroupMAtk.ToString();
            mGMDef.text = unit.GroupMDef.ToString();
            mLeader.text = unit.Leadership.ToString();
            mAgility.text = unit.Agility.ToString();
            mCur.text = unit.Curative.ToString();


            var item = (ItemID == -1) ? EquipmentDataFormat.Zero : game.GameData.Equipment[ItemID];

            //変更後の数値
            var af_hp = unit.BaseMaxHP + item.MaxHP;
            var af_patk = unit.BaseLPAtk + item.LeaderPAtk;
            var af_pdef = unit.BaseLPDef + item.LeaderPDef;
            var af_matk = unit.BaseLMAtk + item.LeaderMAtk;
            var af_mdef = unit.BaseLMDef + item.LeaderMDef;
            var af_gpatk = unit.BaseGPAtk + item.GroupPAtk;
            var af_gpdef = unit.BaseGPDef + item.GroupPDef;
            var af_gmatk = unit.BaseGMAtk + item.GroupMAtk;
            var af_gmdef = unit.BaseGMDef + item.GroupMDef;
            var af_leader = unit.BaseLeader + item.Leadership;
            var af_agi = unit.BaseAgi + item.Agility;
            var af_cur = unit.BaseCur + item.Curative;

            mEquipmentName.text = item.Name;
            mAf_HP.text = af_hp.ToString();
            mAf_PAtk.text = af_patk.ToString();
            mAf_PDef.text = af_pdef.ToString();
            mAf_MAtk.text = af_matk.ToString();
            mAf_MDef.text = af_mdef.ToString();
            mAf_GPAtk.text = af_gpatk.ToString();
            mAf_GPDef.text = af_gpdef.ToString();
            mAf_GMAtk.text = af_gmatk.ToString();
            mAf_GMDef.text = af_gmdef.ToString();
            mAf_Leader.text = af_leader.ToString();
            mAf_Agility.text = af_agi.ToString();
            mAf_Cur.text = af_cur.ToString();

            //色変更
            mAf_HP.color = (unit.MaxHP == af_hp) ? mDefaultColor : ((unit.MaxHP < af_hp) ? mPlusColor : mMinusColor);
            mAf_PAtk.color = (unit.LeaderPAtk == af_patk) ? mDefaultColor : ((unit.LeaderPAtk < af_patk) ? mPlusColor : mMinusColor);
            mAf_PDef.color = (unit.LeaderPDef == af_pdef) ? mDefaultColor : ((unit.LeaderPDef < af_pdef) ? mPlusColor : mMinusColor);
            mAf_MAtk.color = (unit.LeaderMAtk == af_matk) ? mDefaultColor : ((unit.LeaderMAtk < af_matk) ? mPlusColor : mMinusColor);
            mAf_MDef.color = (unit.LeaderMDef == af_mdef) ? mDefaultColor : ((unit.LeaderMDef < af_mdef) ? mPlusColor : mMinusColor);
            mAf_GPAtk.color = (unit.GroupPAtk == af_gpatk) ? mDefaultColor : ((unit.GroupPAtk < af_gpatk) ? mPlusColor : mMinusColor);
            mAf_GPDef.color = (unit.GroupPDef == af_gpdef) ? mDefaultColor : ((unit.GroupPDef < af_gpdef) ? mPlusColor : mMinusColor);
            mAf_GMAtk.color = (unit.GroupMAtk == af_gmatk) ? mDefaultColor : ((unit.GroupMAtk < af_gmatk) ? mPlusColor : mMinusColor);
            mAf_GMDef.color = (unit.GroupMDef == af_gmdef) ? mDefaultColor : ((unit.GroupMDef < af_gmdef) ? mPlusColor : mMinusColor);
            mAf_Leader.color = (unit.Leadership == af_leader) ? mDefaultColor : ((unit.Leadership < af_leader) ? mPlusColor : mMinusColor);
            mAf_Agility.color = (unit.Agility == af_agi) ? mDefaultColor : ((unit.Agility < af_agi) ? mPlusColor : mMinusColor);
            mAf_Cur.color = (unit.Curative == af_cur) ? mDefaultColor : ((unit.Curative < af_cur) ? mPlusColor : mMinusColor);


        }

        public void Click_Cancel()
        {
            Close();
        }

        public void Click_OK()
        {
            var game = Game.GetInstance();
            var unit = game.GameData.Unit[UnitID];

            //装備を変更
            unit.ChangeEquipment(ItemID);

            StatusWindow.Init();
            mList.ItemUpdate();

            Close();
        }
    }
}