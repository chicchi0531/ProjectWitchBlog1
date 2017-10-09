using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class Unit : MonoBehaviour
    {
        //子プレハブ
        [SerializeField]
        private Image mRace = null;
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mSoldier = null;
        [SerializeField]
        private Text mHP = null;
        [SerializeField]
        private GameObject mLvUpIcon = null;
        [SerializeField]
        private Text mLv = null;

        [Space(1)]
        [SerializeField]
        private Sprite[] mRaceSprites = new Sprite[(int)UnitDataFormat.UnitJob.Count];

        [Space(1)]
        [SerializeField]
        private Color mSoldierColor_Max = Color.white;
        [SerializeField]
        private Color mSoldierColor_Normal = Color.white;
        [SerializeField]
        private Color mSoldierColor_Dest = Color.white;
        [SerializeField]
        private Color mSoldierColor_Anni = Color.white;

        [Space(1)]
        [SerializeField]
        private Color mHP_Max = Color.white;
        [SerializeField]
        private Color mHP_Normal = Color.white;
        [SerializeField]
        private Color mHP_Dest = Color.white;


        //プロパティ
        public int UnitID { get; set; }
        public StatusWindow StatusWindow { get; set; }

        //内部変数
        private bool mCoIsRunning = false;

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();
            if (game.GameData.Unit[UnitID].CanDoLevelUp()) mLvUpIcon.SetActive(true);
            else mLvUpIcon.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (!mCoIsRunning)
                StartCoroutine(_Update());
        }

        private IEnumerator _Update()
        {
            mCoIsRunning = true;

            var game = Game.GetInstance();
            var unit = game.GameData.Unit[UnitID];

            //テキストをセット
            mName.text = unit.Name;
            SetRace(unit);
            SetSoldierNum(unit);
            SetHP(unit);

            //レベルアップ可能なときはレベルアップアイコンを表示、それ以外のときはレベルを表示
            if (game.GameData.Unit[UnitID].CanDoLevelUp())
            {
                mLvUpIcon.SetActive(true);
                mLv.text = "";
            }
            else
            {
                mLvUpIcon.SetActive(false);
                mLv.text = unit.Level.ToString();
            }

            yield return new WaitForSeconds(0.1f);

            mCoIsRunning = false;
        }

        public void OnClicked()
        {
            StatusWindow.UnitID = UnitID;
            StatusWindow.Init();
        }

        private void SetRace(UnitDataFormat unit)
        {
        
            //リーダーのステータスから種を判断
            mRace.sprite = mRaceSprites[(int)unit.Job];

        }

        private void SetSoldierNum(UnitDataFormat unit)
        {
            mSoldier.text = unit.SoldierNum.ToString();

            var num = unit.SoldierNum;
            var max = unit.MaxSoldierNum;
            if(num == max)
            {
                mSoldier.color = mSoldierColor_Max;
            }
            else if(num == 0)
            {
                mSoldier.color = mSoldierColor_Anni;
            }
            else if((float)num/max < 0.25f)
            {
                mSoldier.color = mSoldierColor_Dest;
            }
            else
            {
                mSoldier.color = mSoldierColor_Normal;
            }
        }

        private void SetHP(UnitDataFormat unit)
        {
            mHP.text = unit.HP.ToString();

            var num = unit.HP;
            var max = unit.MaxHP;
            if (num == max)
            {
                mHP.color = mHP_Max;
            }
            else if ((float)num/max < 0.25f)
            {
                mHP.color = mHP_Dest;
            }
            else
            {
                mHP.color = mHP_Normal;
            }

        }
    }

}