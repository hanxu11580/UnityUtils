// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: cscombat.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::ILRuntime.Protobuf;
using pbc = global::ILRuntime.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Csp {

  #region Messages
  /// <summary>
  ///========== struct define ==========
  /// </summary>
  public sealed class CSCombatInput : pb::IMessage {
    private static readonly pb::MessageParser<CSCombatInput> _parser = new pb::MessageParser<CSCombatInput>(() => new CSCombatInput());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSCombatInput> Parser { get { return _parser; } }

    /// <summary>Field number for the "role_level" field.</summary>
    public const int RoleLevelFieldNumber = 1;
    private int roleLevel_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int RoleLevel {
      get { return roleLevel_; }
      set {
        roleLevel_ = value;
      }
    }

    /// <summary>Field number for the "cur_region" field.</summary>
    public const int CurRegionFieldNumber = 2;
    private global::Datap.RegionItem curRegion_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Datap.RegionItem CurRegion {
      get { return curRegion_; }
      set {
        curRegion_ = value;
      }
    }

    /// <summary>Field number for the "talent" field.</summary>
    public const int TalentFieldNumber = 3;
    private global::Datap.RoleTalentData talent_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Datap.RoleTalentData Talent {
      get { return talent_; }
      set {
        talent_ = value;
      }
    }

    /// <summary>Field number for the "cards" field.</summary>
    public const int CardsFieldNumber = 4;
    private static readonly pb::FieldCodec<global::Csp.CSCard> _repeated_cards_codec
        = pb::FieldCodec.ForMessage(34, global::Csp.CSCard.Parser);
    private readonly pbc::RepeatedField<global::Csp.CSCard> cards_ = new pbc::RepeatedField<global::Csp.CSCard>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Csp.CSCard> Cards {
      get { return cards_; }
    }

    /// <summary>Field number for the "cur_skin_id" field.</summary>
    public const int CurSkinIdFieldNumber = 5;
    private int curSkinId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CurSkinId {
      get { return curSkinId_; }
      set {
        curSkinId_ = value;
      }
    }

    /// <summary>Field number for the "cur_skin_attr" field.</summary>
    public const int CurSkinAttrFieldNumber = 6;
    private global::Datap.RoleSkinAttrItem curSkinAttr_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Datap.RoleSkinAttrItem CurSkinAttr {
      get { return curSkinAttr_; }
      set {
        curSkinAttr_ = value;
      }
    }

    /// <summary>Field number for the "ultimate_skill" field.</summary>
    public const int UltimateSkillFieldNumber = 7;
    private global::Csp.CSUltimateSkillAttr ultimateSkill_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Csp.CSUltimateSkillAttr UltimateSkill {
      get { return ultimateSkill_; }
      set {
        ultimateSkill_ = value;
      }
    }

    /// <summary>Field number for the "type" field.</summary>
    public const int TypeFieldNumber = 8;
    private global::Datap.CSBattleType type_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Datap.CSBattleType Type {
      get { return type_; }
      set {
        type_ = value;
      }
    }

    /// <summary>Field number for the "equipment_slots" field.</summary>
    public const int EquipmentSlotsFieldNumber = 9;
    private static readonly pbc::MapField<int, global::Csp.CSSlotsWithEquipmentAttrs>.Codec _map_equipmentSlots_codec
        = new pbc::MapField<int, global::Csp.CSSlotsWithEquipmentAttrs>.Codec(pb::FieldCodec.ForInt32(8), pb::FieldCodec.ForMessage(18, global::Csp.CSSlotsWithEquipmentAttrs.Parser), 74);
    private readonly pbc::MapField<int, global::Csp.CSSlotsWithEquipmentAttrs> equipmentSlots_ = new pbc::MapField<int, global::Csp.CSSlotsWithEquipmentAttrs>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::MapField<int, global::Csp.CSSlotsWithEquipmentAttrs> EquipmentSlots {
      get { return equipmentSlots_; }
    }

    /// <summary>Field number for the "stat_data" field.</summary>
    public const int StatDataFieldNumber = 10;
    private static readonly pbc::MapField<int, global::Datap.RoleStatPeriod>.Codec _map_statData_codec
        = new pbc::MapField<int, global::Datap.RoleStatPeriod>.Codec(pb::FieldCodec.ForInt32(8), pb::FieldCodec.ForMessage(18, global::Datap.RoleStatPeriod.Parser), 82);
    private readonly pbc::MapField<int, global::Datap.RoleStatPeriod> statData_ = new pbc::MapField<int, global::Datap.RoleStatPeriod>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::MapField<int, global::Datap.RoleStatPeriod> StatData {
      get { return statData_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (RoleLevel != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(RoleLevel);
      }
      if (curRegion_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(CurRegion);
      }
      if (talent_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Talent);
      }
      cards_.WriteTo(output, _repeated_cards_codec);
      if (CurSkinId != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(CurSkinId);
      }
      if (curSkinAttr_ != null) {
        output.WriteRawTag(50);
        output.WriteMessage(CurSkinAttr);
      }
      if (ultimateSkill_ != null) {
        output.WriteRawTag(58);
        output.WriteMessage(UltimateSkill);
      }
      if (Type != 0) {
        output.WriteRawTag(64);
        output.WriteEnum((int) Type);
      }
      equipmentSlots_.WriteTo(output, _map_equipmentSlots_codec);
      statData_.WriteTo(output, _map_statData_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (RoleLevel != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(RoleLevel);
      }
      if (curRegion_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(CurRegion);
      }
      if (talent_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Talent);
      }
      size += cards_.CalculateSize(_repeated_cards_codec);
      if (CurSkinId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(CurSkinId);
      }
      if (curSkinAttr_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(CurSkinAttr);
      }
      if (ultimateSkill_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(UltimateSkill);
      }
      if (Type != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Type);
      }
      size += equipmentSlots_.CalculateSize(_map_equipmentSlots_codec);
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
            RoleLevel = input.ReadInt32();
            break;
          }
          case 18: {
            if (curRegion_ == null) {
              curRegion_ = new global::Datap.RegionItem();
            }
            input.ReadMessage(curRegion_);
            break;
          }
          case 26: {
            if (talent_ == null) {
              talent_ = new global::Datap.RoleTalentData();
            }
            input.ReadMessage(talent_);
            break;
          }
          case 34: {
            cards_.AddEntriesFrom(input, _repeated_cards_codec);
            break;
          }
          case 40: {
            CurSkinId = input.ReadInt32();
            break;
          }
          case 50: {
            if (curSkinAttr_ == null) {
              curSkinAttr_ = new global::Datap.RoleSkinAttrItem();
            }
            input.ReadMessage(curSkinAttr_);
            break;
          }
          case 58: {
            if (ultimateSkill_ == null) {
              ultimateSkill_ = new global::Csp.CSUltimateSkillAttr();
            }
            input.ReadMessage(ultimateSkill_);
            break;
          }
          case 64: {
            type_ = (global::Datap.CSBattleType) input.ReadEnum();
            break;
          }
          case 74: {
            equipmentSlots_.AddEntriesFrom(input, _map_equipmentSlots_codec);
            break;
          }
          case 82: {
            statData_.AddEntriesFrom(input, _map_statData_codec);
            break;
          }
        }
      }
    }

  }

  public sealed class CSCombatOutput : pb::IMessage {
    private static readonly pb::MessageParser<CSCombatOutput> _parser = new pb::MessageParser<CSCombatOutput>(() => new CSCombatOutput());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSCombatOutput> Parser { get { return _parser; } }

    /// <summary>Field number for the "power" field.</summary>
    public const int PowerFieldNumber = 1;
    private global::Datap.RolePower power_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Datap.RolePower Power {
      get { return power_; }
      set {
        power_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (power_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Power);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (power_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Power);
      }
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
            if (power_ == null) {
              power_ = new global::Datap.RolePower();
            }
            input.ReadMessage(power_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
