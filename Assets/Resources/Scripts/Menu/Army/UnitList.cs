using UnityEngine;
using System.Linq;

namespace ProjectWitch.Menu
{
    public class UnitList : MonoBehaviour
    {

        [SerializeField]
        private GameObject mContentParent = null;

        [SerializeField]
        private GameObject mUnitPrefab = null;

        [SerializeField]
        private StatusWindow mStatusWindow = null;

        // Use this for initialization
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //リストにコンテンツをセットする
        public void Init()
        {
            //もとからあるオブジェクトを削除
            var children = mContentParent.GetComponentsInChildren<Unit>();
            foreach (var child in children)
                Destroy(child.gameObject);

            var game = Game.GetInstance();

            var territory = game.GameData.Territory[0];
            var group = game.GameData.Group[territory.GroupList[0]];
            foreach(var unitid in group.UnitList)
            {
                //コンテンツを追加
                var inst = Instantiate(mUnitPrefab);
                var cUnit = inst.GetComponent<Unit>();
                cUnit.UnitID = unitid;
                cUnit.StatusWindow = mStatusWindow;
                inst.transform.SetParent(mContentParent.transform, false);
            }
        }

        //ユニットリストを取得
        public Unit[] GetUnitList()
        {
            var unitTranses = mContentParent.GetComponentsInChildren<Transform>();
            var objects =  unitTranses.Select(p => p.GetComponent<Unit>()).ToArray();
            return objects.Where(p => p != null).ToArray();
        }
    }
}