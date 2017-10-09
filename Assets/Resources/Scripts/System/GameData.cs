using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq; //iOSで動かないかも

using UnityEngine;
using ProjectWitch.Extention;

namespace ProjectWitch
{
	//プレイヤーデータ用のメタデータ
	public class GameMetaData : SaveMetaData
	{
        public GameMetaData()
        {
            Major = 5;
            Minor = 0;
        }

		//タイムスタンプ
		public int Year { get; set; }       //yearのみ255を超えるのでint型
		public byte Month { get; set; }
		public byte Day { get; set; }
		public byte Hour { get; set; }
		public byte Minute { get; set; }

		//占領済み領地数
		public int DominatedTerritory { get; set; }

		//アリスのレベル
		public int Level { get; set; }

		//経過ターン数
		public int Turn { get; set; }

		//クラスサイズの取得
		public override int GetSize()
		{
			var size = base.GetSize();
			size += Marshal.SizeOf(Year);
			size += Marshal.SizeOf(Month);
			size += Marshal.SizeOf(Day);
			size += Marshal.SizeOf(Hour);
			size += Marshal.SizeOf(Minute);
			size += Marshal.SizeOf(DominatedTerritory);
			size += Marshal.SizeOf(Level);
			size += Marshal.SizeOf(Turn);

			return size;
		}

		//セーブファイルに書き込むバイト列を取得
		public override byte[] GetSaveBytes()
		{
			List<byte> outList = new List<byte>();

			//値のアップデート
			Update();

			//version
			outList.Add(Major);
			outList.Add(Minor);

			//time stamp
			outList.AddRange(BitConverter.GetBytes(Year));
			outList.Add(Month);
			outList.Add(Day);
			outList.Add(Hour);
			outList.Add(Minute);

			//ゲームデータ
			outList.AddRange(BitConverter.GetBytes(DominatedTerritory));
			outList.AddRange(BitConverter.GetBytes(Level));
			outList.AddRange(BitConverter.GetBytes(Turn));

			return outList.ToArray();
		}

		//バイト列からデータを復元
		public override void SetFromBytes(byte[] data)
		{
			int offset = 0;

			//version
			Major = data[offset++];
			Minor = data[offset++];

			//time stamp
			Year = BitConverter.ToInt32(data, offset); offset += 4;
			Month = data[offset++];
			Day = data[offset++];
			Hour = data[offset++];
			Minute = data[offset++];

			//ゲームデータ
			DominatedTerritory = BitConverter.ToInt32(data, offset); offset += 4;
			Level = BitConverter.ToInt32(data, offset); offset += 4;
			Turn = BitConverter.ToInt32(data, offset); offset += 4;
		}

		//値のアップ―デート
		public void Update()
		{
			var game = Game.GetInstance();

			//タイムスタンプ
			var timestamp = DateTime.Now;
			Year = timestamp.Year;
			Month = (byte)timestamp.Month;
			Day = (byte)timestamp.Day;
			Hour = (byte)timestamp.Hour;
			Minute = (byte)timestamp.Minute;

			//ゲームデータ
			DominatedTerritory = game.GameData.Territory[0].AreaList.Count;
			Level = game.GameData.Unit[0].Level;

		}
	}

	//システムデータ用のメタデータ
	public class SystemMetaData : SaveMetaData
	{
		//セーブファイルに書き込むバイト列を取得
		public override byte[] GetSaveBytes()
		{
			List<byte> outList = new List<byte>();

			//version
			outList.Add(Major);
			outList.Add(Minor);

			return outList.ToArray();
		}

		//バイト列からデータを復元
		public override void SetFromBytes(byte[] data)
		{
			int offset = 0;

			//version
			Major = data[offset++];
			Minor = data[offset++];
		}

	}

	//プレイヤーデータ
    [Serializable]
	public class GameData : ISaveableData
	{
		//セーブファイルのバージョン
		private GameMetaData metaData = new GameMetaData();
        public GameMetaData Meta { get { return metaData; }}

        //コンストラクタ
        public GameData() { }

		#region data_member

		//ユニットデータ
		public List<UnitDataFormat> Unit { get; set; }
		//スキルデータ
		public List<SkillDataFormat> Skill { get; set; }
		//土地データ
		public List<AreaDataFormat> Area { get; set; }
		//領地データ
		public List<TerritoryDataFormat> Territory { get; set; }
		//グループデータ
		public List<GroupDataFormat> Group { get; set; }
		//AIデータ
		public List<AIDataFormat> AI { get; set; }
		//装備データ
		public List<EquipmentDataFormat> Equipment { get; set; }
		//カードデータ
		public List<CardDataFormat> Card { get; set; }
		//イベントデータ
		public List<EventDataFormat> FieldEvent { get; set; }
		public List<EventDataFormat> TownEvent { get; set; }

		//所持マナ
		public int PlayerMana { get; set; }

		//システム変数
		public VirtualMemory Memory { get; private set; }
		#endregion

		#region method
		//データをリセットする
		public void Reset()
		{
			try
			{
				//データ系の初期化
				Unit = new List<UnitDataFormat>();
				Skill = new List<SkillDataFormat>();
				Area = new List<AreaDataFormat>();
				Territory = new List<TerritoryDataFormat>();
				Group = new List<GroupDataFormat>();
				AI = new List<AIDataFormat>();
				Equipment = new List<EquipmentDataFormat>();
				Card = new List<CardDataFormat>();
				FieldEvent = new List<EventDataFormat>();
				TownEvent = new List<EventDataFormat>();
				PlayerMana = 10000;
                
                Memory = new VirtualMemory(20000);


			}
			catch (InvalidCastException)
			{
				Debug.LogError("キャストミスです");
				return;
			}
			catch (OverflowException)
			{
				Debug.LogError("データがオーバーフローしました");
				return;
			}

			//データロード

			//ユニットデータの読み出し
			Unit = DataLoader.LoadUnitData(GamePath.Data + "unit_data");

			//スキルデータの読み出し
			Skill = DataLoader.LoadSkillData(GamePath.Data + "skill_data");

			//地点データの読み出し
			Area = DataLoader.LoadAreaData(GamePath.Data + "area_data");

			//領地データの読み出し
			Territory = DataLoader.LoadTerritoryData(GamePath.Data + "territory_data");

			//グループデータの読み出し
			Group = DataLoader.LoadGroupData(GamePath.Data + "group_data");

			//AI
			AI = DataLoader.LoadAIData(GamePath.Data + "ai_data");

			//装備
			Equipment = DataLoader.LoadEquipmentData(GamePath.Data + "equipment_data");

			//カード
			Card = DataLoader.LoadCardData(GamePath.Data + "card_data");

			//イベントデータの読み出し
			FieldEvent = DataLoader.LoadEventData(GamePath.Data + "event_data_field");
			TownEvent = DataLoader.LoadEventData(GamePath.Data + "event_data_town");

			//装備データの総量に基づき、Territroyの装備データを確保
			Territory[0].EquipmentList = new List<List<int>>();
			for (int i = 0; i < Equipment.Count; i++)
				Territory[0].EquipmentList.Add(new List<int>());
		}

		//データをセーブファイルに書き出す
		public void Save(int slot)
		{
			FileIO.SaveBinary(GamePath.GameSaveFilePath(slot), metaData, this);
		}

		//データをセーブファイルから読み込む
		public bool Load(int slot)
		{
			var meta = new GameMetaData();

			var inst = new GameData();
			inst.Copy(this);
            if (!FileIO.LoadBinary(GamePath.GameSaveFilePath(slot), ref meta, inst))
                return false;
			this.Copy(inst);
            return true;
		}

		//引数に与えられたオブジェクトをコピーする
		public void Copy(GameData inst)
        {
            Memory = inst.Memory;

            Unit = inst.Unit;
			Skill = inst.Skill;
			Area = inst.Area;
			Territory = inst.Territory;
			Group = inst.Group;
			AI = inst.AI;
			Equipment = inst.Equipment;
			Card = inst.Card;
			FieldEvent = inst.FieldEvent;
			TownEvent = inst.TownEvent;
			PlayerMana = inst.PlayerMana;
		}

		//セーブ用データをByte配列にパックして取得
		public override byte[] GetSaveBytes()
		{
			var outdata = new List<byte>();

			//セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
			outdata.AddRange(BitConverter.GetBytes(Unit.Count));
			outdata.AddRange(Unit.GetBytes());

			outdata.AddRange(BitConverter.GetBytes(Area.Count));
			outdata.AddRange(Area.GetBytes());

			outdata.AddRange(BitConverter.GetBytes(Territory.Count));
			outdata.AddRange(Territory.GetBytes());

			outdata.AddRange(BitConverter.GetBytes(Group.Count));
			outdata.AddRange(Group.GetBytes());

			outdata.AddRange(BitConverter.GetBytes(PlayerMana));
			outdata.AddRange(Memory.GetSaveBytes());

			return outdata.ToArray();
		}

		//byte配列からデータを再現
		public override int SetFromBytes(int _offset, byte[] data)
		{
			int offset = _offset;

			//データ代入
			var unitCount = BitConverter.ToInt32(data, offset); offset += 4;
			if (Unit.Count < unitCount) Unit = new List<UnitDataFormat>(unitCount);
			for (int i = 0; i < unitCount; i++)
			{
				offset = Unit[i].SetFromBytes(offset, data);
			}

			var areaCount = BitConverter.ToInt32(data, offset); offset += 4;
			if (Area.Count < areaCount) Area = new List<AreaDataFormat>(areaCount);
			for (int i = 0; i < areaCount; i++)
			{
				offset = Area[i].SetFromBytes(offset, data);
			}

			var territoryCount = BitConverter.ToInt32(data, offset); offset += 4;
			if (Territory.Count < territoryCount) Territory = new List<TerritoryDataFormat>(territoryCount);
			for (int i = 0; i < territoryCount; i++)
			{
				offset = Territory[i].SetFromBytes(offset, data);
			}

			var groupCount = BitConverter.ToInt32(data, offset); offset += 4;
			if (Group.Count > groupCount) Group = new List<GroupDataFormat>(groupCount);
			for (int i = 0; i < groupCount; i++)
			{
				offset = Group[i].SetFromBytes(offset, data);
			}

			PlayerMana = BitConverter.ToInt32(data, offset); offset += 4;

			offset = Memory.SetFromBytes(offset, data);

			return offset;
		}
		#endregion
	}

	//システムデータ
	public class SystemData : ISaveableData
	{
		//セーブファイルのバージョン
		private SystemMetaData metaData = new SystemMetaData();

		#region data_member

		//コンフィグ
		public ConfigDataFormat Config { get; set; }

		//仮想メモリ(CGギャラリーの開放、周回フラグなどを含める)
		public VirtualMemory Memory { get; set; }
		#endregion

		#region method
		//データを初期化する
		public void Reset()
		{
			Config = new ConfigDataFormat();
			Memory = new VirtualMemory(1024);
		}

		//データをシステムファイルに書き出す
		public void Save()
		{
			FileIO.SaveBinary(GamePath.SystemSaveFilePath(), metaData, this);
		}

		//データをシステムファイルから読み込む
		public bool Load()
		{
			if (global::System.IO.File.Exists(GamePath.SystemSaveFilePath()))
			{
				var inst = new SystemData();
				inst.Copy(this);
                if (!FileIO.LoadBinary(GamePath.SystemSaveFilePath(), ref metaData, inst))
                    return false;

				this.Copy(inst);
			}
			else
			{
				Debug.Log("システムファイルが見つかりません。初回起動モードで実行します。");
			}
            return true;
		}

		//コピーメソッド
		public void Copy(SystemData inst)
		{
			Config = inst.Config;
			Memory = inst.Memory;
		}

		//セーブ用データをByte配列にパックして取得
		public override byte[] GetSaveBytes()
		{
			var outdata = new List<byte>();

			//セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
			outdata.AddRange(Config.GetSaveBytes());
			outdata.AddRange(Memory.GetSaveBytes());

			return outdata.ToArray();
		}

		//byte配列からデータを再現
		public override int SetFromBytes(int _offset, byte[] data)
		{
			int offset = _offset;

			//データ代入
			offset = Config.SetFromBytes(offset, data);
			offset = Memory.SetFromBytes(offset, data);

			return offset;
		}
		#endregion
	}

	//各種データ構造

	#region ゲームデータ系

	//ユニットデータ
	public class UnitDataFormat : ISaveableData
	{
		//コンストラクタ
		public UnitDataFormat()
		{
			Love = 0;
			IsAlive = true;
			Experience = 0.0f;
			SoldierCost = 0.5f;
		}

		#region data_member

		//レベルアップに必要な経験値
		public static readonly int REQUIPRED_EXPERIENCE_TO_LVUP = 1;

		//ID
		public int ID { get; set; }

		//ユニット名
		public string Name { get; set; }

        //ユニットの種別
        public enum UnitJob : int
        {
            MagicAttacker,
            MagicAssistant,
            PhysicsAttacker,
            PhysicsAssistant,
            Dragon,
            Zombi,
            Homunclus,
            Count
        };
        public UnitJob Job { get; set; }

		//レベル
		private int mLevel = 0;
		public int Level { get { return (mLevel > MaxLevel && MaxLevel > 0) ? MaxLevel : mLevel; } set { mLevel = value; } }

		//レベル成長限界
		public int MaxLevel { get; set; }

		//HP
		public int HP { get; set; }
		//最大HP
		public int HP0 { get; set; }
		public int HP100 { get; set; }     //HP成長率

		//経験値
		public float Experience { get; set; }

		//初期値
		public int LPAtk0 { get; set; }     //物理攻撃
		public int LMAtk0 { get; set; }     //魔法攻撃
		public int LPDef0 { get; set; }     //物理防御
		public int LMDef0 { get; set; }     //魔法防御

		public int GPAtk0 { get; set; }     //物理攻撃
		public int GMAtk0 { get; set; }     //魔法攻撃
		public int GPDef0 { get; set; }     //物理防御
		public int GMDef0 { get; set; }     //魔法防御

		public int LPAtk100 { get; set; }   //物理攻撃Lv100時
		public int LMAtk100 { get; set; }   //魔法攻撃Lv100時
		public int LPDef100 { get; set; }   //物理防御Lv100時
		public int LMDef100 { get; set; }   //魔法防御Lv100時

		public int GPAtk100 { get; set; }   //物理攻撃Lv100時
		public int GMAtk100 { get; set; }   //魔法攻撃Lv100時
		public int GPDef100 { get; set; }   //物理防御Lv100時
		public int GMDef100 { get; set; }   //魔法防御Lv100時

		//指揮力
		public int Lead0 { get; set; }    //指揮力初期値
		public int Lead100 { get; set; }    //指揮力Lv100時
											//機動力
		public int Agi0 { get; set; }    //機動力初期値
		public int Agi100 { get; set; }    //機動力Lv100時
										   //回復力
		public int Cur0 { get; set; }   //回復力初期値
		public int Cur100 { get; set; }   //回復力Lv100時

		//兵士数
		public int SoldierNum { get; set; }
		public int MaxSoldierNum { get; set; }

		//撤退するか死ぬか ture:死ぬ　false：撤退する
		public bool Deathable { get; set; }

		//捕獲可能か不可能か
		public bool Catchable { get; set; }

        //解雇可能か
        public bool Unemployable { get; set; }

		//生きているか
		public bool IsAlive { get; set; }

		//好感度
		public int Love { get; set; }

		//リーダースキル
		public int LAtkSkill { get; set; }
		public int LDefSkill { get; set; }

		//部下スキル
		public int GAtkSkill { get; set; }

		//部下の大きさ（小:0 中:1 大:2 特大:3
		public int GUnitSize { get; set; }

		//装備ID
		public int Equipment { get; set; }

		//AI番号
		public int AIID { get; set; }

		//兵士回復コスト
		public float SoldierCost { get; set; }

		//最大兵士数成長コスト
		public float SoldierLimitCost
        {
            get
            {
                if(MaxSoldierNum < 800)
                {
                    return 3.0f;
                }
                else
                {
                    return 3.0f + 3.6f * ((MaxSoldierNum / 100.0f) - 8.0f);
                }
            }
        }

		//立ち絵画像名
		public string StandImagePath { get; set; }
		//顔アイコン画像名
		public string FaceIamgePath { get; set; }
		//戦闘リーダープレハブ名
		public string BattleLeaderPrefabPath { get; set; }
		//戦闘兵士プレハブ名
		public string BattleGroupPrefabPath { get; set; }

		//死亡時セリフ
		public string OnDeadSerif { get; set; }
		//捕獲時セリフ
		public string OnCapturedSerif { get; set; }
		//逃走時セリフ
		public string OnEscapedSerif { get; set; }

		//アリスのコメント
		public string Comment { get; set; }

		//戦闘に出したかのフラグ。ターン開始時にリセットされる
		public bool IsBattled { get; set; }

		#endregion

		#region query

		//装備を含めたステータス
		public int MaxHP { get { return BaseMaxHP + EquipmentData.MaxHP; } private set { } }
		public int LeaderPAtk { get { return BaseLPAtk + EquipmentData.LeaderPAtk; } private set { } } //物理攻撃
		public int LeaderMAtk { get { return BaseLMAtk + EquipmentData.LeaderMAtk; } private set { } } //魔法攻撃
		public int LeaderPDef { get { return BaseLPDef + EquipmentData.LeaderPDef; } private set { } } //物理防御
		public int LeaderMDef { get { return BaseLMDef + EquipmentData.LeaderMDef; } private set { } } //魔法防御
		public int GroupPAtk { get { return BaseGPAtk + EquipmentData.GroupPAtk; } private set { } }  //物理攻撃
		public int GroupMAtk { get { return BaseGMAtk + EquipmentData.GroupMAtk; } private set { } }  //魔法攻撃
		public int GroupPDef { get { return BaseGPDef + EquipmentData.GroupPDef; } private set { } }  //物理防御
		public int GroupMDef { get { return BaseGMDef + EquipmentData.GroupMDef; } private set { } }  //魔法防御
		public int Leadership { get { return BaseLeader + EquipmentData.Leadership; } private set { } }  //指揮力
		public int Curative { get { return BaseCur + EquipmentData.Curative; } private set { } }       //回復力
		public int Agility { get { return BaseAgi + EquipmentData.Agility; } private set { } }        //機動力


		//装備を含めないステータス
		public int BaseMaxHP { get { return (int)(HP0 + (HP100-HP0) / 100.0f * Level); } private set { } }
		public int BaseLPAtk { get { return (int)(LPAtk0 + (LPAtk100-LPAtk0) / 100.0f * Level); } private set { } } //物理攻撃
		public int BaseLMAtk { get { return (int)(LMAtk0 + (LMAtk100-LMAtk0) / 100.0f * Level); } private set { } } //魔法攻撃
		public int BaseLPDef { get { return (int)(LPDef0 + (LPDef100-LPDef0) / 100.0f * Level); } private set { } } //物理防御
		public int BaseLMDef { get { return (int)(LMDef0 + (LMDef100-LMDef0) / 100.0f * Level); } private set { } } //魔法防御
		public int BaseGPAtk { get { return (int)(GPAtk0 + (GPAtk100-GPAtk0) / 100.0f * Level); } private set { } }  //物理攻撃
		public int BaseGMAtk { get { return (int)(GMAtk0 + (GMAtk100-GMAtk0) / 100.0f * Level); } private set { } }  //魔法攻撃
		public int BaseGPDef { get { return (int)(GPDef0 + (GPDef100-GPDef0) / 100.0f * Level); } private set { } }  //物理防御
		public int BaseGMDef { get { return (int)(GMDef0 + (GMDef100-GMDef0) / 100.0f * Level); } private set { } }  //魔法防御
		public int BaseLeader { get { return (int)(Lead0 + (Lead100-Lead0) / 100.0f * Level); } private set { } }  //指揮力
		public int BaseCur { get { return (int)(Cur0 + (Cur100-Cur0)/ 100.0f * Level); } private set { } }       //回復力
		public int BaseAgi { get { return (int)(Agi0 + (Agi100-Agi0) / 100.0f * Level); } private set { } }        //機動力

		//装備品のデータ
		public EquipmentDataFormat EquipmentData
		{
			get
			{
				return (Equipment == -1) ?
					EquipmentDataFormat.Zero :
					Game.GetInstance().GameData.Equipment[Equipment];
			}
			private set { }
		}

		#endregion

		#region method 

        //戦闘後に得られる経験値を計算
        public float CalcBattleExperience(UnitDataFormat[] Enemy, bool IsWin)
        {
            //敵の平均レベルを算出
            float lvAve = (float)Enemy.Select(x => x.Level).Average();

            //自分のレベルとの差を計算
            var subLv = lvAve - this.Level;

            //経験値を計算
            var offset = 10.0f;
            var minval = 0.3f;
            var maxval = 6.0f;
            var divFactor = 10.0f;
            var ex = (subLv+offset) / divFactor;

            //戦闘にまけた場合は経験値を半分にする
            if (!IsWin) ex /= 2.0f;

            //一定の範囲でクリップ
            ex = Math.Max(minval, ex);
            ex = Math.Min(maxval, ex);

            
            return ex;
        }

		//装備を変更する（ユニットが自領地にいるときのみ有効
		public void ChangeEquipment(int ItemID)
		{
			var game = Game.GetInstance();

			//ユニットが自領地にいるかどうか判定
			if (!game.GameData.Group[game.GameData.Territory[0].GroupList[0]].UnitList.Contains(ID))
				return;

			//装備を外す
			var currentItemID = Equipment;
			var list = new List<int>();
			if (currentItemID != -1)
			{
				bool isHaving = false;
				list = game.GameData.Territory[0].EquipmentList[currentItemID];
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] == ID)
					{
						list[i] = -1;
						isHaving = true;
						break;
					}
				}

				//返却時に見つからない場合は新たに追加する
				if (!isHaving) list.Add(-1);
			}


			//ユニットに新しい装備をセット
			Equipment = ItemID;

			//装備品リストを更新
			if (ItemID != -1)
			{
				list = game.GameData.Territory[0].EquipmentList[ItemID];
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] == -1)
					{
						list[i] = ID;
						break;
					}
				}
			}
		}

		//レベルアップ可能化の判定
		public bool CanDoLevelUp()
		{
			//レベル上限∞のキャラはレベル判定をスキップさせる
			var bLevelCheck = true;
			if (MaxLevel > 0) bLevelCheck = Level < MaxLevel;

			return (Experience >= REQUIPRED_EXPERIENCE_TO_LVUP) && bLevelCheck;
		}

		//コピーメソッド
		public UnitDataFormat Clone()
		{
			return (UnitDataFormat)MemberwiseClone();
		}

		//死亡状態にする
		public void Kill()
		{
			SoldierNum = 0;
			HP = 0;
			IsAlive = false;
		}

		//復活させる
		public void Rebirth()
		{
			SoldierNum = MaxSoldierNum;
			HP = MaxHP;
			IsAlive = true;
		}

		//セーブ用データをByte配列にパックして取得
		public override byte[] GetSaveBytes()
		{
			var outdata = new List<byte>();

			//セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
			outdata.AddRange(BitConverter.GetBytes(Level));
			outdata.AddRange(BitConverter.GetBytes(HP));
			outdata.AddRange(BitConverter.GetBytes(Experience));
			outdata.AddRange(BitConverter.GetBytes(SoldierNum));
            outdata.AddRange(BitConverter.GetBytes(MaxSoldierNum));
			outdata.AddRange(BitConverter.GetBytes(Deathable));
			outdata.AddRange(BitConverter.GetBytes(Catchable));
            outdata.AddRange(BitConverter.GetBytes(Unemployable));
			outdata.AddRange(BitConverter.GetBytes(IsAlive));
			outdata.AddRange(BitConverter.GetBytes(Love));
			outdata.AddRange(BitConverter.GetBytes(Equipment));
            outdata.AddRange(BitConverter.GetBytes(LAtkSkill));
			outdata.AddRange(BitConverter.GetBytes(IsBattled));

			return outdata.ToArray();
		}

		//byte配列からデータを再現
		public override int SetFromBytes(int _offset, byte[] data)
		{
			int offset = _offset;
            
			Level = BitConverter.ToInt32(data, offset); offset += 4;
			HP = BitConverter.ToInt32(data, offset); offset += 4;
			Experience = BitConverter.ToSingle(data, offset); offset += 4;
			SoldierNum = BitConverter.ToInt32(data, offset); offset += 4;
            MaxSoldierNum = BitConverter.ToInt32(data, offset); offset += 4;
			Deathable = BitConverter.ToBoolean(data, offset); offset += 1;
			Catchable = BitConverter.ToBoolean(data, offset); offset += 1;
            Unemployable = BitConverter.ToBoolean(data, offset); offset += 1;
			IsAlive = BitConverter.ToBoolean(data, offset); offset += 1;
			Love = BitConverter.ToInt32(data, offset); offset += 4;
			Equipment = BitConverter.ToInt32(data, offset); offset += 4;
            LAtkSkill = BitConverter.ToInt32(data, offset); offset += 4;
			IsBattled = BitConverter.ToBoolean(data, offset); offset += 1;

			return offset;
		}
		#endregion

	}

	//スキルデータ
	public class SkillDataFormat
	{
		public SkillDataFormat()
		{
			Status = Enumerable.Repeat<bool>(false, 7).ToList();
			Attribute = Enumerable.Repeat<bool>(false, 3).ToList();
		}

		//ID
		public int ID { get; set; }

		//名前
		public string Name { get; set; }

		//威力
		public int Power { get; set; }

		//種類
		public enum SkillType
		{
			Damage = 0,       //0:ダメ―ジ
			Heal,           //1:回復
			StatusUp,       //2:ステ上昇
			StatusDown,     //3:ステ下降
			Summon,         //4:召喚
			SoulSteal,      //5:ダメージ還元
			Guard,          //6:ガード
			TurnWait,       //7:順番下げ
			NoDamage,       //8:ダメージ無効
			PutTime,        //9:時間消費
			StatusOff,      //10:ステータス取り消し
			Random          //11:ランダム
		};
		public SkillType Type { get; set; }

		//効果持続時間
		public int Duration { get; set; }

		//ステ種類
		//[0]物功,[1]物防,[2]魔攻,[3]魔防,[4]機動,[5]指揮力,[6]地形補正
		public List<bool> Status { get; set; }

		//攻撃属性
		//[0]毒付与、[1]対ホムンクルス, [2]対ゾンビ
		public List<bool> Attribute { get; set; }

		//召喚用ユニット番号
		public int SummonUnit { get; set; }

		//範囲
		public enum SkillRange
		{
			All = 0,      //0:全員
			Single      //1:単体
		}
		public SkillRange Range { get; set; }

		//対象
		public enum SkillTarget
		{
			Enemy = 0,        //0:敵
			Player,         //1:味方
			Own,            //2:自分
			EnemyLeader,    //3:敵リーダー
			PlayerLeader,   //4:味方リーダー
			EnemyAndPlayer, //5:敵味方両方
		}
		//[0]敵集団、[1]味方集団、[2]自分、[3]敵リーダー、[4]味方リーダー
		public SkillTarget Target { get; set; }

		//視覚エフェクト
		public string EffectPath { get; set; }

		//説明
		public string Description { get; set; }

	}

	//カードデータ
	public class CardDataFormat
	{
        public static readonly int ShopFlagID = 4010;

        //ID
        public int ID { get; set; }

		//名前
		public string Name { get; set; }

		//タイミング
		public enum CardTiming : int
		{
			BattleBegin = 0,      //戦闘開始
			BattleEnd,          //戦闘終了
			UserState_S10,      //カード使用側のどれかのユニットの兵士数10%以下
			UserState_S50,      //兵士数50%以下
			UserState_HP10,     //HP10%以下
			UserState_HP50,     //HP50%以下
			UserState_Poison,   //毒にかかった
			UserState_Death,    //死亡した
			EnemyState_S10,     //カード使用側ではないどれかのユニットの兵士数10%以下
			EnemyState_S50,     //50%以下
			EnemyState_HP10,    //HP10%以下
			EnemyState_HP50,    //HP50%以下
			EnemyState_Poison,  //毒にかかった
			EnemyState_Death,   //死亡した
			Rand80,             //80%で発動
			Rand50,             //50%で発動
			Rand20,             //20%で発動
		}
        public static readonly Dictionary<CardTiming,string> CardTimingName = new Dictionary<CardTiming, string>
        {
            {CardTiming.BattleBegin, "戦闘開始時" },
            {CardTiming.BattleEnd, "戦闘終了時" },
            {CardTiming.UserState_S10, "味方兵士数10%以下" },
            {CardTiming.UserState_S50, "味方兵士数50%以下" },
            {CardTiming.UserState_HP10, "味方HP10%以下" },
            {CardTiming.UserState_HP50, "味方HP50%以下" },
            {CardTiming.UserState_Poison, "味方が毒にかかる" },
            {CardTiming.UserState_Death, "味方死亡" },
            {CardTiming.EnemyState_S10, "敵兵士数10%以下" },
            {CardTiming.EnemyState_S50, "敵兵士数50%以下" },
            {CardTiming.EnemyState_HP10, "敵HP10%以下" },
            {CardTiming.EnemyState_HP50, "敵HP50%以下" },
            {CardTiming.EnemyState_Poison, "敵が毒にかかる" },
            {CardTiming.EnemyState_Death, "敵死亡" },
            {CardTiming.Rand80, "80%の確率で発動" },
            {CardTiming.Rand50, "50%の確率で発動" },
            {CardTiming.Rand20, "20%の確率で発動" },
        };
		public CardTiming Timing { get; set; }

		//持続回数 (-1は無限)
		public int Duration { get; set; }

		//スキルID
		public int SkillID { get; set; }

		//画像表
		public string ImageFront { get; set; }

		//画像裏
		public string ImageBack { get; set; }

		//売値
		public int BuyingPrice { get; set; }
		//買値
		public int SellingPrice { get; set; }
		//並ぶ店のリスト
		public List<int> ShopList { get; set; }

		//効果説明
		public string Description { get; set; }

	}

	//地形補正データ(倍率指定)
	public class AreaBattleFactor
	{
		public float PAtk { get; set; }         //物理攻撃力
		public float MAtk { get; set; }         //魔法攻撃力
		public float PDef { get; set; }         //物理防御力
		public float MDef { get; set; }         //魔法防御力
		public float Leadership { get; set; }   //指揮力
		public float Agility { get; set; }      //機動力
	}

	//地点データ
	public class AreaDataFormat : ISaveableData
	{
		//コンストラクタ
		public AreaDataFormat()
		{
			Position = new Vector2();
			BattleFactor = new AreaBattleFactor();
			NextArea = new List<int>();
		}

		#region data_member
		//地点番号
		public int ID { get; set; }

		//地点名
		public string Name { get; set; }

		//座標
		public Vector2 Position { get; set; }

		//地点所有者 (TerritoryDataFormatのリストのインデックス
		public int Owner { get; set; }
        
		//所有マナ
		public int Mana { get; set; }

		//限界マナ
		public int IncrementalMana { get; set; }

		//戦闘時間
		public int Time { get; set; }

		//地形補正
		public AreaBattleFactor BattleFactor { get; set; }

        //地点の種類
        public enum AreaType :int
        {
            Default=0,
            Town=1,
            EventTown=2
        }
        public AreaType Type { get; set; } 


        //臨時収入時の情報
        public bool HasItem { get; set; }
        public bool ItemIsEquipment { get; set; }
        public int ItemID { get; set; }

        //臨時収入特殊スクリプト
        public string SpecialScriptName { get; set; }

        //特殊スクリプト実行フラグ
        public int SpecialScriptFlag { get; set; }

		//隣接地点
		public List<int> NextArea { get; set; }

		//背景プレハブパス
		public string BackgroundName { get; set; }
		#endregion

		#region method
        public static bool IsPlayerArea(AreaDataFormat area)
        {
            return area.Owner == 0;
        }

        public static bool IsDomiatableArea(AreaDataFormat area)
        {
            var game = Game.GetInstance();
            var nextAreas = area.NextArea;

            //自領地そのものだったら棄却
            if (IsPlayerArea(area)) return false;

            //隣接領地に自領地があるか判定
            bool isPossible = false;
            foreach (var nextArea in nextAreas)
            {
                var data = game.GameData.Area[nextArea];

                if (data.Owner == 0)
                {
                    isPossible = true;
                    break;
                }
            }
            if (!isPossible) return false;

            //当該地域の領主が、自領地と交戦できる状態にあるか
            var territory = game.GameData.Territory[area.Owner];
            if (territory.State == TerritoryDataFormat.TerritoryState.Ready ||
                territory.State == TerritoryDataFormat.TerritoryState.Active ||
                territory.State == TerritoryDataFormat.TerritoryState.Dead)
                return true;

            return false;
            
        }


		public override byte[] GetSaveBytes()
		{
			var outdata = new List<byte>();

			//セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
			outdata.AddRange(BitConverter.GetBytes(ID));
			outdata.AddRange(BitConverter.GetBytes(Owner));
			outdata.AddRange(BitConverter.GetBytes(Mana));
            outdata.AddRange(BitConverter.GetBytes(HasItem));
			outdata.AddRange(BitConverter.GetBytes(NextArea.Count));
			outdata.AddRange(NextArea.GetBytes());

			return outdata.ToArray();
		}

		//byte配列からデータを再現
		public override int SetFromBytes(int _offset, byte[] data)
		{
			int offset = _offset;

			ID = BitConverter.ToInt32(data, offset); offset += 4;
			Owner = BitConverter.ToInt32(data, offset); offset += 4;
			Mana = BitConverter.ToInt32(data, offset); offset += 4;
            HasItem = BitConverter.ToBoolean(data, offset); offset += 1;

			var nextAreaCount = BitConverter.ToInt32(data, offset); offset += 4;
			NextArea = new List<int>();
			for (int i = 0; i < nextAreaCount; i++)
			{
				NextArea.Add(BitConverter.ToInt32(data, offset)); offset += 4;
			}

			return offset;
		}
		#endregion
	}

	//領地データ
	public class TerritoryDataFormat : ISaveableData
	{
		public TerritoryDataFormat()
		{
			EquipmentList = new List<List<int>>();
		}

		#region data_member

		//領主ID
		public int ID { get; set; }

		//領主名
		public string OwnerName { get; set; }

		//領主名（英語）
		public string OwnerNameEng { get; set; }

		//旗画像名
		public string FlagTexName { get; set; }

		//メイン領地
		public int MainArea { get; set; }

		//所有地点リスト

		public List<int> AreaList
		{
			get
			{
				var game = Game.GetInstance();
				var areadata = game.GameData.Area;

				//地域データから自分が所持している地点を探す
				var list = new List<int>();
				foreach (var area in areadata)
				{
					if (area.Owner == ID) list.Add(area.ID);
				}
				return list;
			}
			private set { }
		}

		//所持グループリスト
		public List<int> GroupList { get; set; }

		//占領済みフラグの変数番号
		public int DeadFlagIndex { get; set; }

		//交戦フラグの変数番号
		public int ActiveFlagIndex { get; set; }

		//宣戦布告可能フラグの変数番号
		public int InvationableFlagIndex { get; set; }

		//状態
		public enum TerritoryState : int
		{
			Prepare = 0,    //宣戦布告不可
			Ready,      //宣戦布告可
			Active,     //交戦中
			Dead        //占領済み
		}
		private TerritoryState state = TerritoryState.Prepare;
		public TerritoryState State
		{
			get
			{
				var game = Game.GetInstance();
                state = TerritoryState.Prepare;

                if (InvationableFlagIndex == -1 || !game.GameData.Memory.IsZero(InvationableFlagIndex))
                    state = TerritoryState.Ready;

                if (ActiveFlagIndex == -1 || !game.GameData.Memory.IsZero(ActiveFlagIndex))
                    state = TerritoryState.Active;

                if (DeadFlagIndex == -1 || !game.GameData.Memory.IsZero(DeadFlagIndex))
                    state = TerritoryState.Dead;

				return state;
			}
			private set { }
		}

        //そのターンの最大行動数
        public int MaxActionCount { get; set; }

		//所持している装備品と装備者のリスト
		//Listは装備品IDに対応
		//Dictionaryはkeyの数が所持数、valueが装備者にあたる。-1は装備者なし
		public List<List<int>> EquipmentList { get; set; }

		#endregion

		#region method

		//指定のユニットを全グループから除外
		public void RemoveUnit(int unit)
		{
			var game = Game.GetInstance();
			var groups = game.GameData.Group.GetFromIndex(GroupList);

			//すべてのグループからユニットを除外
			foreach (var group in groups)
			{
				group.UnitList.Remove(unit);
			}

            //ユニットが装備しているアイテムを外す
            var equipmentLists = game.GameData.Territory[0].EquipmentList;
            foreach(var list in equipmentLists)
            {
                var index = list.IndexOf(unit);
                if (index == -1) continue;
                list[index] = -1;
            }
		}

		//特定のユニットを雇う
		public void AddUnit(int unit)
		{
			var game = Game.GetInstance();
			var groupID = GroupList[0];
			var unitList = game.GameData.Group[groupID].UnitList;


			//すでに雇っていた場合は無効にする
			if (unitList.Contains(unit)) return;

			//雇う
			unitList.Add(unit);

			//ユニットリストをID昇順に並び替える
			unitList.Sort();
		}

		//全ユニットをバトルに出せるようにする
		public void ResetIsBattleFlag()
		{
			var game = Game.GetInstance();
			var groups = game.GameData.Group.GetFromIndex(GroupList);

			//すべてのグループのユニットのIsBattledフラグをリセット
			foreach (var group in groups)
			{
				foreach (var unit in group.UnitList)
				{
					game.GameData.Unit[unit].IsBattled = false;
				}
			}
		}

		//セーブするデータをbyte配列にパックして取得
		public override byte[] GetSaveBytes()
		{
			var outdata = new List<byte>();

			//セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
			outdata.AddRange(BitConverter.GetBytes(AreaList.Count));
			outdata.AddRange(AreaList.GetBytes());
			outdata.AddRange(BitConverter.GetBytes((int)State));
			outdata.AddRange(BitConverter.GetBytes(EquipmentList.Count));
			outdata.AddRange(EquipmentList.GetBytes());

			return outdata.ToArray();
		}

		//byte配列からデータを再現
		public override int SetFromBytes(int _offset, byte[] data)
		{
			int offset = _offset;
            
			var areaListCount = BitConverter.ToInt32(data, offset); offset += 4;
			AreaList = new List<int>();
			for (int i = 0; i < areaListCount; i++)
			{
				AreaList.Add(BitConverter.ToInt32(data, offset)); offset += 4;
			}

			State = EnumConverter.ToEnum<TerritoryState>(
						BitConverter.ToInt32(data, offset)); offset += 4;

			var equipmentListCount = BitConverter.ToInt32(data, offset); offset += 4;
			EquipmentList = new List<List<int>>();
			for (int i = 0; i < equipmentListCount; i++)
			{
				EquipmentList.Add(new List<int>());

				var listCount = BitConverter.ToInt32(data, offset); offset += 4;
				for (int j = 0; j < listCount; j++)
				{
					EquipmentList[i].Add(BitConverter.ToInt32(data, offset)); offset += 4;
				}
			}



			return offset;
		}

		#endregion
	}

	//グループデータ
    [Serializable]
	public class GroupDataFormat : ISaveableData
	{
        #region data_member

        //自営団のID
        public static int DefaultID = 37;

		//ID
		public int ID { get; set; }

		//グループ名
		public string Name { get; set; }

		//戦闘タイプ列挙
		public enum BattleType : int
		{
			ToDestroyedAll = 0,         //全ユニットが死亡するまで戦う（撤退しない）
			ToDestroyedOne = 1,         //あるユニットの兵士が全滅するか、リーダーが死亡するまで戦う
			ToDestroyedOneLeader = 2,   //あるユニットのリーダーが死亡するまで戦う
			HarfTimePass = 3,           //総戦闘時間の半分が経過したら撤退する
		}

		//バトル中断条件
		public BattleType StopType_Domination { get; set; }
		public BattleType StopType_Defence { get; set; }

		//防衛優先度（値が小さいほど優先される
		public int DefensePriority { get; set; }

		//リストの選択方法列挙
		public enum UnitChoiseMethodType : int
		{
            Domination3 = 0, //侵攻戦向き max3
            Defence3 = 1,    //防衛戦向き max3
			Domination2 = 2,    //侵攻戦向き max2
			Defence2 = 3,       //防衛戦向き max2
			Domination1or2 = 4,   //侵攻戦向き max1
            Defence1or2 = 5       //防衛戦向き max1
		}
        public enum CardChoiseMethodType : int
        {
            Random3=0,  //ランダムに3枚
            Random2=1,  //ランダムに2枚
            Random1=2,  //ランダムに1枚
            Order3=3,   //順に3枚
            Order2=4,   //順に2枚
            Order1=5    //順に1枚
        }

		//ユニットの選択方法
		public UnitChoiseMethodType UnitChoiseMethodDomination { get; set; }
        public UnitChoiseMethodType UnitChoiseMethodDefence { get; set; }

		//カードの選択方法
		public CardChoiseMethodType CardChoiseMethod { get; set; }

		//侵攻開始フラグ
		public int BeginDominationFlagIndex { get; set; }

		//侵攻ルート
		public List<int> DominationRoute { get; set; }

		//ユニットリスト
		public List<int> UnitList { get; set; }

		//カードリスト
		public List<int> CardList { get; set; }

        //共用隊（他の共用隊グループが既に侵攻していたら、侵攻を中止する）
        public List<int> UnionGroups { get; set; }

		//状態
		public enum GroupState : int
		{
			Ready,  //始動前
			Active, //活動中
			Dead    //死亡
		}
		private GroupState state = GroupState.Ready;

		public GroupState State
		{
			get
			{
				var game = Game.GetInstance();

                //アクティブ化処理
                if (state == GroupState.Ready)
                {
                    if (BeginDominationFlagIndex == -1)
                        state = GroupState.Active;
                    else if (!game.GameData.Memory.IsZero(BeginDominationFlagIndex))
                        state = GroupState.Active;
                }
                //インアクティブ化処理
                else if (state == GroupState.Active && BeginDominationFlagIndex != -1)
                {
                    if (game.GameData.Memory.IsZero(BeginDominationFlagIndex))
                        state = GroupState.Ready;
                }

                //死亡処理
                if (state == GroupState.Active && UnitList.Count == 0)
                    state = GroupState.Dead;

				return state;
			}
			private set { }
		}
        
		#endregion

		#region method

		//戦闘に出すユニットを取得
		public List<int> GetBattleUnits(bool isDomination)
		{
            var game = Game.GetInstance();

			//ユニットリストからメソッドに応じて３体抽出
			var units = new List<int>();

            //ユニットリストが空だったら先頭に自営団をいれて残りに-1で詰めて返す
            if (UnitList.Count == 0)
            {
                units.Add(GetDefaultID());
                while (units.Count < 3) units.Add(-1);
                return units;
            }

            //メソッドの選択と、ユニット数の抽出
            var method = (isDomination) ? UnitChoiseMethodDomination : UnitChoiseMethodDefence;
            bool bMethodTypeIsDomination = false;
            int maxUnit = 1;
			switch (method)
			{
                case UnitChoiseMethodType.Domination3:
                    bMethodTypeIsDomination = true;
                    maxUnit = 3;
                    break;

                case UnitChoiseMethodType.Defence3:
                    bMethodTypeIsDomination = false;
                    maxUnit = 3;
                    break;

				case UnitChoiseMethodType.Domination2:
                    bMethodTypeIsDomination = true;
                    maxUnit = 2;
                    break;

				case UnitChoiseMethodType.Defence2:
                    bMethodTypeIsDomination = false;
                    maxUnit = 2;
                    break;

				case UnitChoiseMethodType.Domination1or2:
                    bMethodTypeIsDomination = true;
                    maxUnit = UnityEngine.Random.Range(1, 2);
                    break;

                case UnitChoiseMethodType.Defence1or2:
                    bMethodTypeIsDomination = false;
                    maxUnit = UnityEngine.Random.Range(1, 2);
                    break;

				default:
					break;
			}

            //ユニットのセット
            if (bMethodTypeIsDomination)
            {
                #region 侵攻戦ユニットセット
                {
                    //1体目：物理アタッカー→ドラゴン→ゾンビ→魔法アタッカー→ガード→ヒーラーの順で探す
                    if (maxUnit >= 1)
                    {
                        var searchJobs = new List<UnitDataFormat.UnitJob> {UnitDataFormat.UnitJob.PhysicsAttacker,
                                                                   UnitDataFormat.UnitJob.Dragon,
                                                                   UnitDataFormat.UnitJob.Zombi,
                                                                   UnitDataFormat.UnitJob.MagicAttacker,
                                                                   UnitDataFormat.UnitJob.PhysicsAssistant,
                                                                   UnitDataFormat.UnitJob.MagicAssistant};
                        var unit = SearchUnitFromJobs(searchJobs, units);

                        //見つからなかったらリストの先頭からユニットを入れておわり
                        if (unit == -1)
                        {
                            units.Add(UnitList[0]);
                            return units;
                        }
                        units.Add(unit);
                    }

                    //2体目：ガードor物理系（物理アタッカー、ドラゴン、ゾンビ）→魔法アタッカー→ヒーラーの順で探す
                    if (maxUnit >= 2)
                    {
                        var isGuard = (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f) ? true : false; //半々で物理かどうか決める

                        //searchJobsリストを更新
                        var searchJobs = new List<UnitDataFormat.UnitJob>();
                        if (isGuard) searchJobs.AddRange(new UnitDataFormat.UnitJob[] { UnitDataFormat.UnitJob.PhysicsAssistant,
                                                                                    UnitDataFormat.UnitJob.PhysicsAttacker,
                                                                                    UnitDataFormat.UnitJob.Dragon,
                                                                                    UnitDataFormat.UnitJob.Zombi });

                        else searchJobs.AddRange(new UnitDataFormat.UnitJob[] {         UnitDataFormat.UnitJob.PhysicsAttacker,
                                                                                    UnitDataFormat.UnitJob.Dragon,
                                                                                    UnitDataFormat.UnitJob.Zombi,
                                                                                    UnitDataFormat.UnitJob.PhysicsAssistant});
                        //残りは魔法アタッカー→ヒーラーの順で詰める
                        searchJobs.AddRange(new UnitDataFormat.UnitJob[] { UnitDataFormat.UnitJob.MagicAttacker,
                                                                       UnitDataFormat.UnitJob.MagicAssistant});
                        var unit = SearchUnitFromJobs(searchJobs, units);
                        if (unit != -1) units.Add(unit);
                    }

                    //3体目：魔法アタッカー→ヒーラー→ガードの順で探す。グループのユニット数が5以下のときは出さない。
                    if (maxUnit >= 3)
                    {
                        var searchJobs = new List<UnitDataFormat.UnitJob> { UnitDataFormat.UnitJob.MagicAttacker,
                                                                    UnitDataFormat.UnitJob.MagicAssistant,
                                                                    UnitDataFormat.UnitJob.PhysicsAssistant};
                        if (UnitList.Count > 5)
                        {
                            var unit = SearchUnitFromJobs(searchJobs, units);
                            if (unit != -1) units.Add(unit);
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region 防衛戦ユニットセット
                {
                    //1体目：物理アタッカー→ドラゴン→ゾンビ→魔法アタッカー→ガード→ヒーラーの順で探す
                    if (maxUnit >= 1)
                    {
                        var searchJobs = new List<UnitDataFormat.UnitJob> {UnitDataFormat.UnitJob.PhysicsAttacker,
                                                                   UnitDataFormat.UnitJob.Dragon,
                                                                   UnitDataFormat.UnitJob.Zombi,
                                                                   UnitDataFormat.UnitJob.MagicAttacker,
                                                                   UnitDataFormat.UnitJob.PhysicsAssistant,
                                                                   UnitDataFormat.UnitJob.MagicAssistant};
                        var unit = SearchUnitFromJobs(searchJobs, units);

                        //見つからなかったらリストの先頭からユニットを入れておわり
                        if (unit == -1)
                        {
                            units.Add(UnitList[0]);
                            return units;
                        }
                        units.Add(unit);
                    }

                    //2体目：ガード→ヒーラー→物理系（物理アタッカー、ドラゴン、ゾンビ）→魔法アタッカーの順で探す
                    if (maxUnit >= 2)
                    {
                        //searchJobsリストを更新
                        var searchJobs = new List<UnitDataFormat.UnitJob>();
                        searchJobs.AddRange(new UnitDataFormat.UnitJob[] { UnitDataFormat.UnitJob.PhysicsAssistant,
                                                                                    UnitDataFormat.UnitJob.MagicAssistant,
                                                                                    UnitDataFormat.UnitJob.PhysicsAttacker,
                                                                                    UnitDataFormat.UnitJob.Dragon,
                                                                                    UnitDataFormat.UnitJob.Zombi,
                                                                                    UnitDataFormat.UnitJob.MagicAttacker});
                        var unit = SearchUnitFromJobs(searchJobs, units);
                        if (unit != -1) units.Add(unit);
                    }

                    //3体目：ヒーラー→ガード→魔法アタッカーの順で探す。グループのユニット数が5以下のときは出さない。
                    if (maxUnit >= 3)
                    {
                        var searchJobs = new List<UnitDataFormat.UnitJob> { UnitDataFormat.UnitJob.MagicAssistant,
                                                                    UnitDataFormat.UnitJob.PhysicsAssistant,
                                                                    UnitDataFormat.UnitJob.MagicAttacker};
                        if (UnitList.Count > 5)
                        {
                            var unit = SearchUnitFromJobs(searchJobs, units);
                            if (unit != -1) units.Add(unit);
                        }
                    }
                }
                #endregion
            }

            //3隊に満たない部分を-1で補充
            while (units.Count < 3)
				units.Add(-1);

			return units;
		}
        //ユニットの探索用
        private int SearchUnitFromJobs(List<UnitDataFormat.UnitJob> searchJobs,List<int> distinctUnits)
        {
            var game = Game.GetInstance();

            var unitList = new List<int>(UnitList);
            foreach (var unit in distinctUnits) unitList.Remove(unit);  //確定済みユニットは間引く

            //ユニットリストが空になったら-1を返す
            if (unitList.Count == 0) return -1;

            //ユニットタイプの配列を指標に、ユニットを見つけて返す
            foreach (var job in searchJobs)
            {
                var list = unitList.Where(x => game.GameData.Unit[x].Job == job).ToList();
                if (list.Count > 0)
                {
                    return list[0];
                }
            }
            return -1;
        }

		//戦闘に出すカードを取得
		public List<int> GetBattleCards()
		{
			//カードリストからメソッドに応じて３体抽出
			var cards = new List<int>();
			switch (CardChoiseMethod)
			{
				case CardChoiseMethodType.Random3:
					cards = CardList.RandomN(3);
					break;
                case CardChoiseMethodType.Random2:
                    cards = CardList.RandomN(2);
                    break;
                case CardChoiseMethodType.Random1:
                    cards = CardList.RandomN(1);
                    break;
                case CardChoiseMethodType.Order3:
                    cards = CardList.GetOrderN(3);
                    break;
                case CardChoiseMethodType.Order2:
                    cards = CardList.GetOrderN(2);
                    break;
				case CardChoiseMethodType.Order1:
					cards = CardList.GetOrderN(1);
					break;
				default:
					break;
			}

			//３つに満たない部分を-1で補充
			while (cards.Count < 3)
				cards.Add(-1);

			return cards;
		}

		//死亡状態に移行する
		public void Kill()
		{
			state = GroupState.Dead;
		}

		//復活させる
		public void Rebirth()
		{
			state = GroupState.Active;
		}

		//自営団のIDを取得
		public static int GetDefaultID()
		{
			return DefaultID;
		}

		//セーブするデータをbyte配列にパックして取得
		public override byte[] GetSaveBytes()
		{
			var outdata = new List<byte>();

			//セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
			outdata.AddRange(BitConverter.GetBytes(UnitList.Count));
			outdata.AddRange(UnitList.GetBytes());
            outdata.AddRange(BitConverter.GetBytes(CardList.Count));
            outdata.AddRange(CardList.GetBytes());
			outdata.AddRange(BitConverter.GetBytes((int)State));

			return outdata.ToArray();
		}

		//byte配列からデータを再現
		public override int SetFromBytes(int _offset, byte[] data)
		{
			int offset = _offset;

			var unitListCount = BitConverter.ToInt32(data, offset); offset += 4;
			UnitList = new List<int>();
			for (int i = 0; i < unitListCount; i++)
			{
				UnitList.Add(BitConverter.ToInt32(data, offset)); offset += 4;
			}

            var cardListCount = BitConverter.ToInt32(data, offset); offset += 4;
            CardList = new List<int>();
            for(int i=0; i<cardListCount;i++)
            {
                CardList.Add(BitConverter.ToInt32(data, offset)); offset += 4;
            }

			State = EnumConverter.ToEnum<GroupState>(
						BitConverter.ToInt32(data, offset)); offset += 4;

			return offset;
		}


		#endregion
	}

	//AIデータ
	public class AIDataFormat
	{
		public float AttackRate { get; set; }
	}

	//装備データ
	public class EquipmentDataFormat
	{
        public static readonly int ShopFlagID = 4010;

		//ID
		public int ID { get; set; }

		//名前
		public string Name { get; set; }

		//最大HP
		public int MaxHP { get; set; }

		//リーダー(Leader)
		public int LeaderPAtk { get; set; } //物理攻撃
		public int LeaderMAtk { get; set; } //魔法攻撃
		public int LeaderPDef { get; set; } //物理防御
		public int LeaderMDef { get; set; } //魔法防御

		//集団(Group)
		public int GroupPAtk { get; set; }  //物理攻撃
		public int GroupMAtk { get; set; }  //魔法攻撃
		public int GroupPDef { get; set; }  //物理防御
		public int GroupMDef { get; set; }  //魔法防御

		//指揮力
		public int Leadership { get; set; }
		//機動力
		public int Agility { get; set; }
		//回復力
		public int Curative { get; set; }

		//買値
		public int BuyingPrice { get; set; }
		//売値
		public int SellingPrice { get; set; }

		//並んでいる商店のリスト
		public List<int> ShopList { get; set; }

		//説明
		public string Description { get; set; }

		//殻のデータ
		static public EquipmentDataFormat Zero
		{
			get
			{
				var outdata = new EquipmentDataFormat();
				outdata.MaxHP = 0;
				outdata.LeaderPAtk = 0;
				outdata.LeaderPDef = 0;
				outdata.LeaderMAtk = 0;
				outdata.LeaderMDef = 0;
				outdata.GroupPAtk = 0;
				outdata.GroupPDef = 0;
				outdata.GroupMAtk = 0;
				outdata.GroupMDef = 0;
				outdata.Leadership = 0;
				outdata.Agility = 0;
				outdata.Curative = 0;

				return outdata;
			}
			private set { }
		}
	}

	//コンフィグ
	public class ConfigDataFormat : ISaveableData
	{
		public ConfigDataFormat()
		{
			//iniで読み込むようにする
			TextSpeed = TextSpeedEnum.Fast;
			Resolution = ResolutionEnum.Native;

			BGMVolume = 0.3f;
			SEVolume = 0.3f;
			VoiceVolume = 1.0f;
			MasterVolume = 0.5f;
		}

		#region type

		public enum ResolutionEnum : int
		{
			Quarter, //480x280
			Harf,   //960x540
			Native, //1280x720
			Full   //FullHD
		}
		public static readonly Dictionary<ResolutionEnum, Vector2> ResolutionValues = new Dictionary<ResolutionEnum, Vector2>
		{
			{ResolutionEnum.Full,       new Vector2(1920,1080) },
			{ResolutionEnum.Native,     new Vector2(1280,720) },
			{ResolutionEnum.Harf,       new Vector2(960,540) },
			{ResolutionEnum.Quarter,    new Vector2(480,270) }
		};

		public enum GraphicQualityEnum : int
		{
			High = 0,
			Middle = 1,
			Low = 2
		};

		public enum TextSpeedEnum : int
		{
			Slow = 0,
			Normal = 1,
			Fast = 2,
			Fastest = 3
		};
		public static readonly Dictionary<TextSpeedEnum, float> TextSpeedValues = new Dictionary<TextSpeedEnum, float>
		{
			{TextSpeedEnum.Slow, 12.5f },
			{TextSpeedEnum.Normal, 25.0f },
			{TextSpeedEnum.Fast, 50.0f },
			{TextSpeedEnum.Fastest, 200.0f }
		};

		#endregion

		#region private_member
		private bool mIsFullScreen = false;
		private ResolutionEnum mResolution = ResolutionEnum.Native;
		private GraphicQualityEnum mGraphicQuality = GraphicQualityEnum.High;
		#endregion

		#region data_member

		//フルスクリーンか否か
		public bool IsFullScreen
		{
			get { return mIsFullScreen; }
			set
			{
				mIsFullScreen = value;
				Screen.fullScreen = value;
			}
		}

		//解像度
		public ResolutionEnum Resolution
		{
			get { return mResolution; }
			set
			{
				Screen.SetResolution(
					(int)ResolutionValues[value].x,
					(int)ResolutionValues[value].y,
					Screen.fullScreen);
				mResolution = value;
			}
		}
		//グラフィックの質
		public GraphicQualityEnum GraphicQuality
		{
			get { return mGraphicQuality; }
			set
			{
				QualitySettings.SetQualityLevel((int)value);
				mGraphicQuality = value;
			}
		}

		//全体の音量
		public float MasterVolume { get; set; }
		//BGMの音量
		public float BGMVolume { get; set; }
		//SEの音量
		public float SEVolume { get; set; }
		//Voiceの音量
		public float VoiceVolume { get; set; }

		//戦闘の速さ
		public int BattleSpeed { get; set; }

		//テキストスピード
		public TextSpeedEnum TextSpeed { get; set; }
		#endregion

		#region method
		//セーブするデータをbyte配列にパックして取得
		public override byte[] GetSaveBytes()
		{
			var outdata = new List<byte>();

			//セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
			outdata.AddRange(BitConverter.GetBytes((int)Resolution));
			outdata.AddRange(BitConverter.GetBytes(IsFullScreen));
			outdata.AddRange(BitConverter.GetBytes((int)GraphicQuality));
			outdata.AddRange(BitConverter.GetBytes(MasterVolume));
			outdata.AddRange(BitConverter.GetBytes(BGMVolume));
			outdata.AddRange(BitConverter.GetBytes(SEVolume));
			outdata.AddRange(BitConverter.GetBytes(BattleSpeed));
			outdata.AddRange(BitConverter.GetBytes((int)TextSpeed));

			return outdata.ToArray();
		}

		//byte配列からデータを再現
		public override int SetFromBytes(int _offset, byte[] data)
		{
			int offset = _offset;

			Resolution = EnumConverter.ToEnum<ResolutionEnum>(
								BitConverter.ToInt32(data, offset)); offset += 4;

			IsFullScreen = BitConverter.ToBoolean(data, offset); offset += 1;
			GraphicQuality = EnumConverter.ToEnum<GraphicQualityEnum>(
								BitConverter.ToInt32(data, offset)); offset += 4;

			MasterVolume = BitConverter.ToSingle(data, offset); offset += 4;
			BGMVolume = BitConverter.ToSingle(data, offset); offset += 4;
			SEVolume = BitConverter.ToSingle(data, offset); offset += 4;
			BattleSpeed = BitConverter.ToInt32(data, offset); offset += 4;
			TextSpeed = EnumConverter.ToEnum<TextSpeedEnum>(
								BitConverter.ToInt32(data, offset)); offset += 4;

			return offset;
		}


		#endregion
	}

	//仮想メモリ
	public class VirtualMemory : ISaveableData
	{
		//シリアライズ出力用のアクセスメンバ
		public List<int> Data { get; set; }

		public int this[int index]
		{
			get { return Data[index]; }
			set { Data[index] = value; }
		}

		public VirtualMemory(int num)
		{
			Data = Enumerable.Repeat<int>(0, num).ToList();
		}
		public VirtualMemory()
		{
			Data = Enumerable.Repeat<int>(0, 1).ToList();
		}

		#region query

		//指定したインデックスの値が、整数の0かどうか
		public bool IsZero(int index)
		{
			if (index < 0 || index >= Count) return false;

			var num = Data[index];
			return (num == 0);
		}

		//配列サイズの取得

		public int Count
		{
			get
			{
				return Data.Count;
			}
			private set { }
		}

		#endregion

		#region method
		//セーブするデータをbyte配列にパックして取得
		public override byte[] GetSaveBytes()
		{
			var outdata = new List<byte>();

			//セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
			outdata.AddRange(BitConverter.GetBytes(Data.Count));
			outdata.AddRange(Data.GetBytes());

			return outdata.ToArray();
		}

		//byte配列からデータを再現
		public override int SetFromBytes(int _offset, byte[] data)
		{
			int offset = _offset;

			var dataCount = BitConverter.ToInt32(data, offset); offset += 4;
			if (dataCount > Data.Count) Data = new List<int>(dataCount);
			for (int i = 0; i < dataCount; i++)
			{
				Data[i] = BitConverter.ToInt32(data, offset); offset += 4;
			}

			return offset;
		}


		#endregion
	}

	//イベントデータ
	public class EventDataFormat
	{
		public EventDataFormat()
		{
			If_Var = new List<int>();
			If_Ope = new List<OperationType>();
			If_Imm = new List<int>();

			NextA = -1;
			NextB = -1;
		}

		//スクリプトファイル名
		public string FileName { get; set; }

		//タイミング
		public enum TimingType : int
		{
			PlayerTurnBegin = 0,
			EnemyTurnBegin,
			PlayerBattle,
			EnemyBattle,
			AfterBattle
		}
		public TimingType Timing { get; set; }

		//場所
		public int Area { get; set; }

		//生きている必要があるユニット
		public List<int> IfAlive { get; set; }

		//条件式に使う変数番号
		public List<int> If_Var { get; set; }

		//条件式に使う演算子
		public enum OperationType : int
		{
			Equal = 0,
			Bigger,
			Smaller,
			BiggerEqual,
			SmallerEqual,
			NotEqual
		}
		public List<OperationType> If_Ope { get; set; }

		//条件式に使う即値
		public List<int> If_Imm { get; set; }

		//次のスクリプトA
		public int NextA { get; set; }

		//次のスクリプトB
		public int NextB { get; set; }

        //以下サブデータ

        //スクリプトのタイトル
        public string Title { get; set; }

        //報酬アイテムが装備かどうかのフラグ true:装備　false:カード
        public bool IsEquipment { get; set; }

        //報酬アイテムのID
        public int ItemID { get; set; }

	}

	//イベントデータ用演算子の拡張メソッド
	public static class OperationTypeExt
	{
		static string[] opeStr =
{
				"=",
				">",
				"<",
				">=",
				"<=",
				"!="
			};
		public static EventDataFormat.OperationType Parse(this EventDataFormat.OperationType ope, string str)
		{
			for (int i = 0; i < opeStr.Length; i++)
			{
				if (str.Equals(opeStr[i]))
				{
					return (EventDataFormat.OperationType)Enum.ToObject(typeof(EventDataFormat.OperationType),
						i);
				}
			}
			return EventDataFormat.OperationType.Equal;
		}
	}

	#endregion


	public class DataLoader
	{
		public static List<UnitDataFormat> LoadUnitData(string filePath)
		{
			var outData = new List<UnitDataFormat>();

			//ファイルからテキストデータを抽出
			var rowData = CSVReader(filePath);

			//ユニットデータに格納（0番目はキャプションなので読み飛ばす
			for (int i = 1; i < rowData.Count; i++)
			{
				if (rowData[i].Count < 47) continue;

				//データの順番
				//[0]ID         [1]名前       [2]種別
                //[3]レベル     [4]レベル成長限界          [5]HP
				//[6]HP成長率
				//[7]LATK       [8]LMAT       [9]LDEF        [10]LMDE       [11]GATK    [12]GMAT
				//[13]GDEF      [14]GMDE      [15]成LATK     [16]成LMAT     [17]成LDEF  [18]成LMDE
				//[19]成GATK    [20]成GMAT    [21]成GDEF     [22]成GMDE     [23]指揮力　[24]機動力
				//[25]成指揮    [26]成機動
				//[27]回復力　  [28]回復力成長率
				//[29]兵士数    [30]死ぬか撤退か [31]捕獲可能か [32]解雇可能か
				//[33]好感度 
				//[34]リーダー攻撃スキル
				//[35]リーダー防御スキル
				//[36]兵士攻撃スキル   [37]兵士サイズ
				//[38]装備             [39]AI番号
				//[40]立ち絵画像名     [41]顔アイコン画像    
				//[42]戦闘リーダープレハブ名
				//[43]戦闘兵士プレハブ名 [44]キャラ説明
				//[45]死亡時セリフ
				//[46]捕獲時セリフ
				//[47]逃走時セリフ
				var unit = new UnitDataFormat();
				var data = rowData[i];

				if (data[0] == "") continue;

				try
				{
					unit.ID = int.Parse(data[0]);
					unit.Name = data[1];
                    unit.Job = EnumConverter.ToEnum<UnitDataFormat.UnitJob>(int.Parse(data[2]));
					unit.Level = int.Parse(data[3]);
					unit.MaxLevel = int.Parse(data[4]);
					unit.HP0 = int.Parse(data[5]);
					unit.HP100 = int.Parse(data[6]);
					unit.HP = unit.BaseMaxHP;
					unit.LPAtk0 = int.Parse(data[7]);
					unit.LMAtk0 = int.Parse(data[8]);
					unit.LPDef0 = int.Parse(data[9]);
					unit.LMDef0 = int.Parse(data[10]);
					unit.GPAtk0 = int.Parse(data[11]);
					unit.GMAtk0 = int.Parse(data[12]);
					unit.GPDef0 = int.Parse(data[13]);
					unit.GMDef0 = int.Parse(data[14]);
					unit.LPAtk100 = int.Parse(data[15]);
					unit.LMAtk100 = int.Parse(data[16]);
					unit.LPDef100 = int.Parse(data[17]);
					unit.LMDef100 = int.Parse(data[18]);
					unit.GPAtk100 = int.Parse(data[19]);
					unit.GMAtk100 = int.Parse(data[20]);
					unit.GPDef100 = int.Parse(data[21]);
					unit.GMDef100 = int.Parse(data[22]);
                    unit.Lead0 = int.Parse(data[23]);
					unit.Agi0 = int.Parse(data[24]);
					unit.Lead100 = int.Parse(data[25]);
					unit.Agi100 = int.Parse(data[26]);
					unit.Cur0 = int.Parse(data[27]);
					unit.Cur100 = int.Parse(data[28]);
					unit.SoldierNum = int.Parse(data[29]);
					unit.MaxSoldierNum = unit.SoldierNum;
					unit.Deathable = (data[30] == "0") ? false : true;
					unit.Catchable = (data[31] == "0") ? false : true;
                    unit.Unemployable = (data[32] == "0") ? false : true;
					unit.Love = int.Parse(data[33]);
					unit.LAtkSkill = int.Parse(data[34]);
					unit.LDefSkill = int.Parse(data[35]);
					unit.GAtkSkill = int.Parse(data[36]);
					unit.GUnitSize = int.Parse(data[37]);
					unit.Equipment = int.Parse(data[38]);
					unit.AIID = int.Parse(data[39]);

					unit.StandImagePath = data[40];
					unit.FaceIamgePath = data[41];
					unit.BattleLeaderPrefabPath = data[42];
					unit.BattleGroupPrefabPath = data[43];
					unit.Comment = data[44];

					unit.OnDeadSerif = data[45];
					unit.OnCapturedSerif = data[46];
					unit.OnEscapedSerif = data[47];

					unit.IsAlive = true;
					unit.IsBattled = false;

				}
				catch (ArgumentNullException e)
				{
					Debug.Log("ユニットデータの読み取りに失敗：データが空です" + ":" + i.ToString() + "行目");
					Debug.Log(e.Message);
				}
				catch (FormatException e)
				{
					Debug.Log("ユニットデータの読み取りに失敗：データの形式が違います" + ":" + i.ToString() +"行目");
					Debug.Log(e.Message);
				}
				catch (OverflowException e)
				{
					Debug.Log("ユニットデータの読み取りに失敗：データがオーバーフローしました" + ":" + i.ToString() + "行目");
					Debug.Log(e.Message);
				}

				outData.Add(unit);
			}


			return outData;
		}

		public static List<AreaDataFormat> LoadAreaData(string filePath)
		{
			var outData = new List<AreaDataFormat>();
			outData.Add(new AreaDataFormat()); //初めから一つ入れておく(デフォルト地点

			//ファイルからテキストデータを抽出
			var rowData = CSVReader(filePath);

			//地点データに格納（0番目はキャプションなので読み飛ばす
			for (int i = 1; i < rowData.Count; i++)
			{
				if (rowData[i].Count < 20) continue;
                

				//データの順番
				//[0]ID         [1]地点名     [2]x     [3]y    [4]所有者
				//[5]レベル       [6]所有マナ  [7]戦闘時間     
				//[8-13]地形補正  
                //[14]地点の種類
                //[15]臨時収入　装備フラグ
                //[16]臨時収入　アイテムID  
                //[17]臨時収入特殊スクリプト [18]臨時収入特殊スクリプトフラグ
                //[19]背景プレハブ名 [20-]隣接地点 
				var area = new AreaDataFormat();
				var data = rowData[i];

                if (data[0] == "") continue;

				try
				{
					area.ID = int.Parse(data[0]);
					area.Name = data[1];
					area.Position = new Vector2(float.Parse(data[2]), float.Parse(data[3]));
					area.Owner = int.Parse(data[4]);
					area.Mana = int.Parse(data[5]);
					area.IncrementalMana = int.Parse(data[6]);
					area.Time = int.Parse(data[7]);

					//地形補正
					area.BattleFactor.PAtk = int.Parse(data[8]);
					area.BattleFactor.MAtk = int.Parse(data[9]);
					area.BattleFactor.PDef = int.Parse(data[10]);
					area.BattleFactor.MDef = int.Parse(data[11]);
					area.BattleFactor.Leadership = int.Parse(data[12]);
					area.BattleFactor.Agility = int.Parse(data[13]);

                    //地点の種類
                    area.Type = EnumConverter.ToEnum<AreaDataFormat.AreaType>(int.Parse(data[14])); 

                    //臨時収入情報
                    area.ItemIsEquipment = (int.Parse(data[15]) == 0) ? false : true;
                    area.ItemID = int.Parse(data[16]);
                    area.HasItem = true;

                    //臨時収入特殊スクリプト
                    area.SpecialScriptName = data[17];
                    area.SpecialScriptFlag = (data[18] != "") ? int.Parse(data[18]) : -1;

					//背景プレハブ名
					area.BackgroundName = data[19];

					//隣接地点
					for (int j = 20; j < data.Count; j++)
					{
						if (data[j] == "") continue;
						area.NextArea.Add(int.Parse(data[j]));
					}
				}
				catch (ArgumentNullException e)
				{
					Debug.Log("エリアデータの読み取りに失敗：データが空です");
					Debug.Log(e.Message);
				}
				catch (FormatException e)
				{
					Debug.Log("エリアデータの読み取りに失敗：データの形式が違います");
					Debug.Log(e.Message);
				}
				catch (OverflowException e)
				{
					Debug.Log("エリアデータの読み取りに失敗：データがオーバーフローしました");
					Debug.Log(e.Message);
				}

				outData.Add(area);
			}



			return outData;
		}

		public static List<EventDataFormat> LoadEventData(string filePath)
		{
			var outData = new List<EventDataFormat>();

			//生データの読み出し
			var rowData = CSVReader(filePath);

			//データの形式
			//[0]id [1]filename [2]タイミング
			//[3]場所 [4]味方登場人物 [5]敵登場人物
			//[6]条件1:変数 [7]条件1:式 [8]条件1:即値
			//[9]条件2:変数 [10]条件2:式 [11]条件2:即値
			//[13]次のスクリプト1 [14]次のスクリプト2

			//データの代入
			for (int i = 1; i < rowData.Count; i++)
			{
				if (rowData[i].Count < 3) continue;

				var data = rowData[i];
				var eventData = new EventDataFormat();

				if (data[0] == "" || data[1] == "") continue;

				try
				{
					//ファイル名
					eventData.FileName = data[1];

					//タイミング
					eventData.Timing = EnumConverter.ToEnum<EventDataFormat.TimingType>(int.Parse(data[2]));

					//地点ＩＤ
					if (data[3] != "")
						eventData.Area = int.Parse(data[3]);
					else
						eventData.Area = -1;

					//IfAlive
					eventData.IfAlive = new List<int>();
					var parts = data[4].Split(' ');
					foreach (string part in parts)
					{
						if (part == "") continue;
						eventData.IfAlive.Add(int.Parse(part));
					}

					//次のスクリプト
					if (data[5] != "") eventData.NextA = int.Parse(data[5]);
					else eventData.NextA = -1;
					if (data[6] != "") eventData.NextB = int.Parse(data[6]);
					else eventData.NextB = -1;


					for (int index = 7, j = 0; j < 3 * 3; j += 3)
					{
						//条件読み出し1
						if (data[index + j] != "")
						{
							var dummy = EventDataFormat.OperationType.Equal;
							eventData.If_Var.Add(int.Parse(data[index + j]));
							eventData.If_Ope.Add(dummy.Parse(data[index + j + 1]));
							eventData.If_Imm.Add(int.Parse(data[index + j + 2]));
						}
						else
						{
							eventData.If_Var.Add(-1);
							eventData.If_Ope.Add(EventDataFormat.OperationType.Equal);
							eventData.If_Imm.Add(0);
						}
					}

                    //サブデータの読み出し
                    if (data.Count >= 19)
                    {
                        eventData.Title = data[16];
                        eventData.IsEquipment = (int.Parse(data[17]) == 0) ? false : true;
                        eventData.ItemID = (data[18] != "") ? int.Parse(data[18]) : -1;
                    }
				}
				catch (ArgumentNullException e)
				{
					Debug.Log("イベントデータの読み取りに失敗：データが空です。: L" + i.ToString());
					Debug.Log(e.Message);
				}
				catch (FormatException e)
				{
					Debug.Log("イベントデータの読み取りに失敗：データの形式が違います。: L" + i.ToString());
					Debug.Log(e.Message);
				}
				catch (OverflowException e)
				{
					Debug.Log("イベントデータの読み取りに失敗：データがオーバーフローしました。: L" + i.ToString());
					Debug.Log(e.Message);
				}

				outData.Add(eventData);
			}


			return outData;
		}

		public static List<TerritoryDataFormat> LoadTerritoryData(string filePath)
		{
			var outData = new List<TerritoryDataFormat>();

			//生データの読み出し
			var rowData = CSVReader(filePath);

			//[0]ID [1]領主名　[2]領主名(英語)
			//[3]旗画像パス [4]メイン領地 [5]グループリスト 
			//[6]占領フラグ [7]交戦フラグ
			//[8]宣戦布告可能フラグ

			//データの代入
			for (int i = 1; i < rowData.Count; i++)
			{
				if (rowData[i].Count != 9) continue;

				var data = rowData[i];
				var terData = new TerritoryDataFormat();

				if (data[0] == "") continue;

				try
				{
					//ID
					terData.ID = int.Parse(data[0]);

					//領主名
					terData.OwnerName = data[1];

					//領主名英語
					terData.OwnerNameEng = data[2];

					//プレハブのロード
					terData.FlagTexName = data[3];

					//メイン領地の読み出し
					terData.MainArea = int.Parse(data[4]);

					//グループリスト
					terData.GroupList = new List<int>();
					var parts = data[5].Split(' ');
					foreach (string part in parts)
					{
						if (part == "") continue;
						terData.GroupList.Add(int.Parse(part));
					}

					//占領フラグ
					if (data[6] == "")
						terData.DeadFlagIndex = -1;
					else
						terData.DeadFlagIndex = int.Parse(data[6]);

					//交戦フラグ
					if (data[7] == "")
						terData.ActiveFlagIndex = -1;
					else
						terData.ActiveFlagIndex = int.Parse(data[7]);

					//宣戦布告可能フラグ
					if (data[8] == "")
						terData.InvationableFlagIndex = -1;
					else
						terData.InvationableFlagIndex = int.Parse(data[8]);
				}
				catch (ArgumentNullException e)
				{
					Debug.Log("領地データの読み取りに失敗：データが空です");
					Debug.Log(e.Message);
				}
				catch (FormatException e)
				{
					Debug.Log("領地データの読み取りに失敗：データの形式が違います");
					Debug.Log(e.Message);
				}
				catch (OverflowException e)
				{
					Debug.Log("領地データの読み取りに失敗：データがオーバーフローしました");
					Debug.Log(e.Message);
				}

				outData.Add(terData);
			}

			return outData;
		}

		public static List<GroupDataFormat> LoadGroupData(string filePath)
		{
			var outData = new List<GroupDataFormat>();

			//生データの読み出し
			var rowData = CSVReader(filePath);

			//[0]ID [1]名前
			//[2]防衛優先度 
            //[3]ユニットの選択方法・侵攻戦
            //[4]ユニットの選択方法・防衛戦
			//[5]カードの選択方法 
            //[6]バトル中断条件　侵攻戦
            //[7]バトル中断条件　防衛戦
            //[8]侵攻開始フラグ [9]侵攻ルート
			//[10]ユニットリスト [11]カードリスト
            //[12]共用隊リスト

			//データの代入
			for (int i = 1; i < rowData.Count; i++)
			{
				if (rowData[i].Count < 10) continue;

				var data = rowData[i];
				var groupData = new GroupDataFormat();

				if (data[0] == "") continue;

				try
				{
					//ID
					groupData.ID = int.Parse(data[0]);

					//領主名
					groupData.Name = data[1];

					//防衛優先度
					groupData.DefensePriority = int.Parse(data[2]);

					//ユニットの選択方法
					groupData.UnitChoiseMethodDomination =
						EnumConverter.ToEnum<GroupDataFormat.UnitChoiseMethodType>(int.Parse(data[3]));
                    groupData.UnitChoiseMethodDefence =
                        EnumConverter.ToEnum<GroupDataFormat.UnitChoiseMethodType>(int.Parse(data[4]));


                    //カードの選択方法
                    groupData.CardChoiseMethod =
						EnumConverter.ToEnum<GroupDataFormat.CardChoiseMethodType>(int.Parse(data[5]));

                    //バトル中断条件
                    groupData.StopType_Domination = 
                        EnumConverter.ToEnum<GroupDataFormat.BattleType>(int.Parse(data[6]));
                    groupData.StopType_Defence =
                        EnumConverter.ToEnum<GroupDataFormat.BattleType>(int.Parse(data[7]));

                    //侵攻開始フラグ
                    if (data[8] == "")
						groupData.BeginDominationFlagIndex = -1;
					else
						groupData.BeginDominationFlagIndex = int.Parse(data[8]);

					//侵攻ルート
					var list = new List<int>();
					var areas = data[9].Split(' ');
					foreach (var area in areas)
					{
						if (area == "") continue;
						list.Add(int.Parse(area));
					}
					groupData.DominationRoute = list;

					//ユニットリスト
					list = new List<int>();
					var units = data[10].Split(' ');
					foreach (var unit in units)
					{
						if (unit == "") continue;
						list.Add(int.Parse(unit));
					}
					groupData.UnitList = list;

					//カードリスト
					list = new List<int>();
					var cards = data[11].Split(' ');
					foreach (var card in cards)
					{
						if (card == "") continue;
						list.Add(int.Parse(card));
					}
					groupData.CardList = list;

                    //共用隊リスト
                    list = new List<int>();
                    var unions = data[12].Split();
                    foreach(var union in unions)
                    {
                        if (union == "") continue;
                        list.Add(int.Parse(union));
                    }
                    groupData.UnionGroups = list;
				}
				catch (ArgumentNullException e)
				{
					Debug.Log("グループデータの読み取りに失敗：データが空です");
					Debug.Log(e.Message);
				}
				catch (FormatException e)
				{
					Debug.Log("グループデータの読み取りに失敗：データの形式が違います");
					Debug.Log(e.Message);
				}
				catch (OverflowException e)
				{
					Debug.Log("グループデータの読み取りに失敗：データがオーバーフローしました");
					Debug.Log(e.Message);
				}

				outData.Add(groupData);
			}

			return outData;
		}

		public static List<EquipmentDataFormat> LoadEquipmentData(string filePath)
		{
			var outData = new List<EquipmentDataFormat>();

			//生データの読み出し
			var rowData = CSVReader(filePath);

			//データの代入
			for (int i = 1; i < rowData.Count; i++)
			{
				if (rowData[i].Count != 18) continue;


				//データの順番
				//[0]ID     [1]名前       [2]HP
				//[3]PAtk   [4]MAtk       [5]PDef
				//[6]MDef   [7]GPAtk      [8]GMAtk
				//[9]GPDef  [10]GMDef     [11]Lead
				//[12]Agi   [13]回復力    [14]売値
				//[15]買値  [16]店に出るかフラグ [17]説明
				var data = rowData[i];
				var equipData = new EquipmentDataFormat();

				//無名アイテムがあったら読み飛ばす
				if (data[1] == "") continue;

				try
				{
					equipData.ID = int.Parse(data[0]);
					equipData.Name = data[1];
					equipData.MaxHP = int.Parse(data[2]);
					equipData.LeaderPAtk = int.Parse(data[3]);
					equipData.LeaderMAtk = int.Parse(data[4]);
					equipData.LeaderPDef = int.Parse(data[5]);
					equipData.LeaderMDef = int.Parse(data[6]);
					equipData.GroupPAtk = int.Parse(data[7]);
					equipData.GroupMAtk = int.Parse(data[8]);
					equipData.GroupPDef = int.Parse(data[9]);
					equipData.GroupMDef = int.Parse(data[10]);
					equipData.Leadership = int.Parse(data[11]);
					equipData.Agility = int.Parse(data[12]);
					equipData.Curative = int.Parse(data[13]);
					equipData.BuyingPrice = int.Parse(data[14]);
					equipData.SellingPrice = int.Parse(data[15]);
                    if (data[16] == "")
                        equipData.ShopList = new List<int>();
                    else
                    {
                        var list = data[16].Split();
                        equipData.ShopList = list.ToList().ConvertAll(x => int.Parse(x));
                    }
                    equipData.Description = data[17];
				}
				catch (ArgumentNullException e)
				{
					Debug.Log("装備データの読み取りに失敗：データが空です");
					Debug.Log(e.Message);
				}
				catch (FormatException e)
				{
					Debug.Log("装備データの読み取りに失敗：データの形式が違います");
					Debug.Log(e.Message);
				}
				catch (OverflowException e)
				{
					Debug.Log("装備データの読み取りに失敗：データがオーバーフローしました");
					Debug.Log(e.Message);
				}

				outData.Add(equipData);
			}

			return outData;
		}

		public static List<AIDataFormat> LoadAIData(string filePath)
		{
			var outData = new List<AIDataFormat>();

			//生データの読み出し
			var rowData = CSVReader(filePath);

			//データの代入
			for (int i = 1; i < rowData.Count; i++)
			{
				if (rowData[i].Count < 2) continue;


				//データの順番
				//[0]ID     [1]攻撃率
				var data = rowData[i];
				var AIData = new AIDataFormat();

				//無名アイテムがあったら読み飛ばす
				if (data[1] == "") continue;

				try
				{
					AIData.AttackRate = float.Parse(data[1]);
				}
				catch (ArgumentNullException e)
				{
					Debug.Log("AIデータの読み取りに失敗：データが空です");
					Debug.Log(e.Message);
				}
				catch (FormatException e)
				{
					Debug.Log("AIデータの読み取りに失敗：データの形式が違います");
					Debug.Log(e.Message);
				}
				catch (OverflowException e)
				{
					Debug.Log("AIデータの読み取りに失敗：データがオーバーフローしました");
					Debug.Log(e.Message);
				}

				outData.Add(AIData);
			}

			return outData;
		}

		public static List<SkillDataFormat> LoadSkillData(string filePath)
		{
			var outData = new List<SkillDataFormat>();

			//生データの読み出し
			var rowData = CSVReader(filePath);

			//データの代入
			for (int i = 1; i < rowData.Count; i++)
			{
				if (rowData[i].Count < 20) continue;


				//データの順番
				//[0]ID [1]名前   [2]威力   [3]スキルタイプ
				//[4]効果時間
				//[5]~[11]ステータスフラグ  
				//[5]物功 [6]物防   [7]魔功   [8]魔防
				//[9]機動 [10]指揮  [11]地形
				//[12]~[14]攻撃属性
				//[12]毒 [13]対ホムンクルス    [14]対ゾンビ
				//[15]召喚するユニットID
				//[16]効果範囲 
				//[17]効果対象
				//[18]エフェクト名
				//[19]説明
				var data = rowData[i];
				var skill = new SkillDataFormat();

				//無名アイテムがあったら読み飛ばす
				if (data[1] == "") continue;

				try
				{
					skill.ID = int.Parse(data[0]);
					skill.Name = data[1];
					skill.Power = int.Parse(data[2]);
					skill.Type = (SkillDataFormat.SkillType)Enum.ToObject(
						typeof(SkillDataFormat.SkillType), int.Parse(data[3]));
					skill.Duration = int.Parse(data[4]);

					//ステータス
					for (int j = 0, index = 5; j < 7; j++, index++)
						skill.Status[j] = (data[index] == "0") ? false : true;

					//特殊ステータス
					for (int j = 0, index = 12; j < 3; j++, index++)
						skill.Attribute[j] = (data[index] == "0") ? false : true;

					skill.SummonUnit = int.Parse(data[15]);
					skill.Range = (SkillDataFormat.SkillRange)Enum.ToObject(
						typeof(SkillDataFormat.SkillRange), int.Parse(data[16]));
					skill.Target = (SkillDataFormat.SkillTarget)Enum.ToObject(
						typeof(SkillDataFormat.SkillTarget), int.Parse(data[17]));
					skill.EffectPath = data[18];
					skill.Description = data[19];
				}
				catch (ArgumentNullException e)
				{
					Debug.Log("スキルデータの読み取りに失敗：データが空です");
					Debug.Log(e.Message);
				}
				catch (FormatException e)
				{
					Debug.Log("スキルデータの読み取りに失敗：データの形式が違います");
					Debug.Log(e.Message);
				}
				catch (OverflowException e)
				{
					Debug.Log("スキルデータの読み取りに失敗：データがオーバーフローしました");
					Debug.Log(e.Message);
				}

				outData.Add(skill);
			}

			return outData;
		}

		public static List<CardDataFormat> LoadCardData(string filePath)
		{
			var outData = new List<CardDataFormat>();

			//生データの読み出し
			var rowData = CSVReader(filePath);

			//データの代入
			for (int i = 1; i < rowData.Count; i++)
			{
				if (rowData[i].Count < 11) continue;


				//データの順番
				var data = rowData[i];
				var card = new CardDataFormat();

				//無名アイテムがあったら読み飛ばす
				if (data[1] == "") continue;

				try
				{
					card.ID = int.Parse(data[0]);
					card.Name = data[1];
					card.Timing = EnumConverter.ToEnum<CardDataFormat.CardTiming>(int.Parse(data[2]));
					card.Duration = int.Parse(data[3]);
					card.SkillID = int.Parse(data[4]);
					card.ImageFront = data[5];
					card.ImageBack = data[6];
					card.BuyingPrice = int.Parse(data[7]);
					card.SellingPrice = int.Parse(data[8]);
                    if (data[9] == "")
                        card.ShopList = new List<int>();
                    else
                    {
                        var list = data[9].Split();
                        card.ShopList = list.ToList().ConvertAll(x => int.Parse(x));
                    }

                    card.Description = data[10];

				}
				catch (ArgumentNullException e)
				{
					Debug.Log("カードデータの読み取りに失敗：データが空です");
					Debug.Log(e.Message);
				}
				catch (FormatException e)
				{
					Debug.Log("カードデータの読み取りに失敗：データの形式が違います");
					Debug.Log(e.Message);
				}
				catch (OverflowException e)
				{
					Debug.Log("カードデータの読み取りに失敗：データがオーバーフローしました");
					Debug.Log(e.Message);
				}

				outData.Add(card);
			}

			return outData;
		}

		private static List<List<string>> CSVReader(string filePath)
		{
			var outData = new List<List<string>>();

			var tAsset = Resources.Load(filePath) as TextAsset;
			if (tAsset == null)
			{
				Debug.Assert(false, "データファイルの読み込みに失敗しました : " + filePath);
			}

			var text = tAsset.text;
			var reader = new StringReader(text);

			while (reader.Peek() > -1)
			{
				var line = reader.ReadLine();
				var factors = line.Split(',');

				var list = new List<string>();
				foreach (var value in factors)
				{
					list.Add(value);
				}
				outData.Add(list);
			}

			return outData;
		}

	}

	public class GamePath
	{
		public static readonly string Data = "Data/";
		public static readonly string SaveFolderPath = Application.dataPath + "/SaveData/";

		public static string GameSaveFilePath(int index) { return SaveFolderPath + "save" + (index == -1 ? "_auto" : index.ToString()) + ".dat"; }
		public static string SystemSaveFilePath() { return SaveFolderPath + "sys_save.dat"; }
	}
}