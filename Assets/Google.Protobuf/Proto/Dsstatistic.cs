// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: dsstatistic.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::ILRuntime.Protobuf;
using pbc = global::ILRuntime.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Datap {

  #region Enums
  /// <summary>
  /// ================= enum define =================
  /// </summary>
  public enum StatMisc {
    KStatMiscNone = 0,
    /// <summary>
    /// 某一个模块内的统计数据上限
    /// </summary>
    KStatMiscMaxStatOne = 1000,
  }

  public enum StatModel {
    KStatModelNone = 0,
    /// <summary>
    /// 商店模块
    /// </summary>
    KStatShop = 1,
    /// <summary>
    /// 局内模块
    /// </summary>
    KStatGame = 2,
    /// <summary>
    /// 随机事件ID
    /// </summary>
    KStatEventId = 3,
    /// <summary>
    /// 任务状态
    /// </summary>
    KStatTask = 4,
    /// <summary>
    /// 条件计数
    /// </summary>
    KStatCond = 5,
    /// <summary>
    /// 杂项模块
    /// </summary>
    KStatMisc = 6,
    /// <summary>
    /// 服务端模块
    /// </summary>
    KStatSol = 7,
    /// <summary>
    /// 道具获得过的总量
    /// </summary>
    KStatProp = 8,
    /// <summary>
    /// 熟练度商店模块
    /// </summary>
    KStatRegionShop = 9,
    /// <summary>
    /// 战斗次数
    /// </summary>
    KStatBattleTimes = 10,
    /// <summary>
    /// 充值模块
    /// </summary>
    KStatRecharge = 11,
    /// <summary>
    /// 战斗胜利次数
    /// </summary>
    KStatBattleVictoryTimes = 12,
    /// <summary>
    /// 某类副本战斗胜利次数
    /// </summary>
    KStatBattleDungeonTypeVictoryTimes = 13,
    /// <summary>
    /// 战斗特殊奖励次数
    /// </summary>
    KStatBattleSpecAwardsTimes = 14,
    /// <summary>
    /// 某类副本特殊奖励次数
    /// </summary>
    KStatBattleDungeonTypeSpecAwardsTimes = 15,
    /// <summary>
    /// 战斗模块其他统计
    /// </summary>
    KStatBattleMisc = 16,
    /// <summary>
    /// 广告模块
    /// </summary>
    KStatAd = 17,
    KStatPushGift = 18,
    /// <summary>
    /// 随机事件类型
    /// </summary>
    KStatEventType = 19,
    /// <summary>
    /// 技能buff
    /// </summary>
    KStatSkillBuff = 20,
    KStatModelMax = 22,
  }

  public enum StatBattleMisc {
    KStatBattleMiscNone = 0,
    /// <summary>
    /// 元素龙本的高额奖励次数
    /// </summary>
    KStatBattleMiscDragonStoryLargeAwards = 1,
    /// <summary>
    /// 挂机系统道具使用次数
    /// </summary>
    KStatBattleMiscHangupUseProp = 2,
    /// <summary>
    /// 远征胜利次数
    /// </summary>
    KStatBattleMiscExpeditionTimes = 3,
    /// <summary>
    /// 主线多倍体力次数
    /// </summary>
    KStatBattleMiscStageBonusTimes = 4,
    /// <summary>
    /// 日常本多倍体力次数
    /// </summary>
    KStatBattleMiscDungeonBonusTimes = 5,
    /// <summary>
    /// 局内复活次数
    /// </summary>
    KStatBattleMiscRevive = 6,
    KStatBattleMiscMax = 7,
  }

  public enum kStatMiscType {
    None = 0,
    /// <summary>
    /// 送礼次数
    /// </summary>
    SocialSend = 1,
    /// <summary>
    /// 领取次数
    /// </summary>
    SocialRecv = 2,
    /// <summary>
    /// 领取次数
    /// </summary>
    SocialApply = 3,
    /// <summary>
    /// 举报次数
    /// </summary>
    SocialInform = 4,
    /// <summary>
    /// 改名次数
    /// </summary>
    Rename = 5,
    Max = 6,
  }

  public enum kStatSolType {
    None = 0,
    /// <summary>
    /// 在线时长
    /// </summary>
    OnlineTime = 1,
    /// <summary>
    /// 数据差异
    /// </summary>
    Csdiff = 2,
  }

  public enum StatPeriod {
    KStatPeriodNone = 0,
    KStatPeriodForever = 1,
    KStatPeriodDaily = 2,
    KStatPeriodWeekly = 3,
    KStatPeriodMonthly = 4,
    KStatPeriodMax = 5,
  }

  #endregion

  #region Messages
  /// <summary>
  /// ================= message define =================
  /// </summary>
  public sealed class RoleStatOne : pb::IMessage {
    private static readonly pb::MessageParser<RoleStatOne> _parser = new pb::MessageParser<RoleStatOne>(() => new RoleStatOne());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RoleStatOne> Parser { get { return _parser; } }

    /// <summary>Field number for the "stat" field.</summary>
    public const int StatFieldNumber = 1;
    private static readonly pbc::MapField<int, int>.Codec _map_stat_codec
        = new pbc::MapField<int, int>.Codec(pb::FieldCodec.ForInt32(8), pb::FieldCodec.ForInt32(16), 10);
    private readonly pbc::MapField<int, int> stat_ = new pbc::MapField<int, int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::MapField<int, int> Stat {
      get { return stat_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      stat_.WriteTo(output, _map_stat_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += stat_.CalculateSize(_map_stat_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            stat_.AddEntriesFrom(input, _map_stat_codec);
            break;
          }
        }
      }
    }

  }

  public sealed class RoleStatPeriod : pb::IMessage {
    private static readonly pb::MessageParser<RoleStatPeriod> _parser = new pb::MessageParser<RoleStatPeriod>(() => new RoleStatPeriod());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RoleStatPeriod> Parser { get { return _parser; } }

    /// <summary>Field number for the "last_update" field.</summary>
    public const int LastUpdateFieldNumber = 1;
    private long lastUpdate_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long LastUpdate {
      get { return lastUpdate_; }
      set {
        lastUpdate_ = value;
      }
    }

    /// <summary>Field number for the "reset_count" field.</summary>
    public const int ResetCountFieldNumber = 3;
    private int resetCount_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int ResetCount {
      get { return resetCount_; }
      set {
        resetCount_ = value;
      }
    }

    /// <summary>Field number for the "stat_data" field.</summary>
    public const int StatDataFieldNumber = 4;
    private static readonly pbc::MapField<int, global::Datap.RoleStatOne>.Codec _map_statData_codec
        = new pbc::MapField<int, global::Datap.RoleStatOne>.Codec(pb::FieldCodec.ForInt32(8), pb::FieldCodec.ForMessage(18, global::Datap.RoleStatOne.Parser), 34);
    private readonly pbc::MapField<int, global::Datap.RoleStatOne> statData_ = new pbc::MapField<int, global::Datap.RoleStatOne>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::MapField<int, global::Datap.RoleStatOne> StatData {
      get { return statData_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (LastUpdate != 0L) {
        output.WriteRawTag(8);
        output.WriteInt64(LastUpdate);
      }
      if (ResetCount != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(ResetCount);
      }
      statData_.WriteTo(output, _map_statData_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (LastUpdate != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(LastUpdate);
      }
      if (ResetCount != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ResetCount);
      }
      size += statData_.CalculateSize(_map_statData_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            LastUpdate = input.ReadInt64();
            break;
          }
          case 24: {
            ResetCount = input.ReadInt32();
            break;
          }
          case 34: {
            statData_.AddEntriesFrom(input, _map_statData_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
