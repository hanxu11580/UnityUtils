// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: dsbattle_expedition.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::ILRuntime.Protobuf;
using pbc = global::ILRuntime.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Datap {

  #region Messages
  /// <summary>
  /// ================= message define =================
  /// </summary>
  public sealed class RoleExpeditionData : pb::IMessage {
    private static readonly pb::MessageParser<RoleExpeditionData> _parser = new pb::MessageParser<RoleExpeditionData>(() => new RoleExpeditionData());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RoleExpeditionData> Parser { get { return _parser; } }

    /// <summary>Field number for the "max_finished_chapter_id" field.</summary>
    public const int MaxFinishedChapterIdFieldNumber = 1;
    private int maxFinishedChapterId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int MaxFinishedChapterId {
      get { return maxFinishedChapterId_; }
      set {
        maxFinishedChapterId_ = value;
      }
    }

    /// <summary>Field number for the "is_playing" field.</summary>
    public const int IsPlayingFieldNumber = 2;
    private bool isPlaying_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool IsPlaying {
      get { return isPlaying_; }
      set {
        isPlaying_ = value;
      }
    }

    /// <summary>Field number for the "begin_time" field.</summary>
    public const int BeginTimeFieldNumber = 3;
    private long beginTime_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long BeginTime {
      get { return beginTime_; }
      set {
        beginTime_ = value;
      }
    }

    /// <summary>Field number for the "curr_chapter_id" field.</summary>
    public const int CurrChapterIdFieldNumber = 4;
    private int currChapterId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CurrChapterId {
      get { return currChapterId_; }
      set {
        currChapterId_ = value;
      }
    }

    /// <summary>Field number for the "left_tickets" field.</summary>
    public const int LeftTicketsFieldNumber = 5;
    private int leftTickets_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int LeftTickets {
      get { return leftTickets_; }
      set {
        leftTickets_ = value;
      }
    }

    /// <summary>Field number for the "last_reset_tickets_time" field.</summary>
    public const int LastResetTicketsTimeFieldNumber = 6;
    private long lastResetTicketsTime_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long LastResetTicketsTime {
      get { return lastResetTicketsTime_; }
      set {
        lastResetTicketsTime_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (MaxFinishedChapterId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(MaxFinishedChapterId);
      }
      if (IsPlaying != false) {
        output.WriteRawTag(16);
        output.WriteBool(IsPlaying);
      }
      if (BeginTime != 0L) {
        output.WriteRawTag(24);
        output.WriteInt64(BeginTime);
      }
      if (CurrChapterId != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(CurrChapterId);
      }
      if (LeftTickets != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(LeftTickets);
      }
      if (LastResetTicketsTime != 0L) {
        output.WriteRawTag(48);
        output.WriteInt64(LastResetTicketsTime);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (MaxFinishedChapterId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(MaxFinishedChapterId);
      }
      if (IsPlaying != false) {
        size += 1 + 1;
      }
      if (BeginTime != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(BeginTime);
      }
      if (CurrChapterId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(CurrChapterId);
      }
      if (LeftTickets != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(LeftTickets);
      }
      if (LastResetTicketsTime != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(LastResetTicketsTime);
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
            MaxFinishedChapterId = input.ReadInt32();
            break;
          }
          case 16: {
            IsPlaying = input.ReadBool();
            break;
          }
          case 24: {
            BeginTime = input.ReadInt64();
            break;
          }
          case 32: {
            CurrChapterId = input.ReadInt32();
            break;
          }
          case 40: {
            LeftTickets = input.ReadInt32();
            break;
          }
          case 48: {
            LastResetTicketsTime = input.ReadInt64();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
