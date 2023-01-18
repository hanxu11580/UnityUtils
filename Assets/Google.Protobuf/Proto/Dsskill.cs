// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: dsskill.proto
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
  public enum SkillMisc {
    KSkillMisc = 0,
    /// <summary>
    /// 大招数量上限
    /// </summary>
    KMaxUltimateSkill = 50,
    /// <summary>
    /// 大招星级上限
    /// </summary>
    KMaxSkillStarLevel = 5,
  }

  #endregion

  #region Messages
  /// <summary>
  /// ================= message define =================
  /// </summary>
  public sealed class UltimateSkill : pb::IMessage {
    private static readonly pb::MessageParser<UltimateSkill> _parser = new pb::MessageParser<UltimateSkill>(() => new UltimateSkill());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<UltimateSkill> Parser { get { return _parser; } }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private int id_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    /// <summary>Field number for the "star" field.</summary>
    public const int StarFieldNumber = 2;
    private int star_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Star {
      get { return star_; }
      set {
        star_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Id != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Id);
      }
      if (Star != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Star);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Id != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Id);
      }
      if (Star != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Star);
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
          case 8: {
            Id = input.ReadInt32();
            break;
          }
          case 16: {
            Star = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class RoleSkillData : pb::IMessage {
    private static readonly pb::MessageParser<RoleSkillData> _parser = new pb::MessageParser<RoleSkillData>(() => new RoleSkillData());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RoleSkillData> Parser { get { return _parser; } }

    /// <summary>Field number for the "ultimate_skills" field.</summary>
    public const int UltimateSkillsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Datap.UltimateSkill> _repeated_ultimateSkills_codec
        = pb::FieldCodec.ForMessage(10, global::Datap.UltimateSkill.Parser);
    private readonly pbc::RepeatedField<global::Datap.UltimateSkill> ultimateSkills_ = new pbc::RepeatedField<global::Datap.UltimateSkill>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Datap.UltimateSkill> UltimateSkills {
      get { return ultimateSkills_; }
    }

    /// <summary>Field number for the "inuse_ultimate_skill_id" field.</summary>
    public const int InuseUltimateSkillIdFieldNumber = 2;
    private int inuseUltimateSkillId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int InuseUltimateSkillId {
      get { return inuseUltimateSkillId_; }
      set {
        inuseUltimateSkillId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      ultimateSkills_.WriteTo(output, _repeated_ultimateSkills_codec);
      if (InuseUltimateSkillId != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(InuseUltimateSkillId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += ultimateSkills_.CalculateSize(_repeated_ultimateSkills_codec);
      if (InuseUltimateSkillId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(InuseUltimateSkillId);
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
            ultimateSkills_.AddEntriesFrom(input, _repeated_ultimateSkills_codec);
            break;
          }
          case 16: {
            InuseUltimateSkillId = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
