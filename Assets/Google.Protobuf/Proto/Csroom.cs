// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: csroom.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::ILRuntime.Protobuf;
using pbc = global::ILRuntime.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Csp {

  #region Enums
  /// <summary>
  ///========== enum define ==========
  /// </summary>
  public enum CSRoomPlayerStatus {
    KCsroomPlayerStatusNone = 0,
    KCsroomPlayerStatusJoin = 1,
    KCsroomPlayerStatusReady = 2,
    KCsroomPlayerStatusLeave = 3,
    KCsroomPlayerStatusKick = 4,
    /// <summary>
    /// Max
    /// </summary>
    KCsroomPlayerStatusMax = 5,
  }

  #endregion

  #region Messages
  /// <summary>
  ///========== struct define ==========
  /// </summary>
  public sealed class CSRoomInfo : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomInfo> _parser = new pb::MessageParser<CSRoomInfo>(() => new CSRoomInfo());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomInfo> Parser { get { return _parser; } }

    /// <summary>Field number for the "room_id" field.</summary>
    public const int RoomIdFieldNumber = 1;
    private int roomId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int RoomId {
      get { return roomId_; }
      set {
        roomId_ = value;
      }
    }

    /// <summary>Field number for the "chapter_id" field.</summary>
    public const int ChapterIdFieldNumber = 2;
    private int chapterId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int ChapterId {
      get { return chapterId_; }
      set {
        chapterId_ = value;
      }
    }

    /// <summary>Field number for the "owner" field.</summary>
    public const int OwnerFieldNumber = 3;
    private long owner_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long Owner {
      get { return owner_; }
      set {
        owner_ = value;
      }
    }

    /// <summary>Field number for the "members" field.</summary>
    public const int MembersFieldNumber = 4;
    private static readonly pb::FieldCodec<long> _repeated_members_codec
        = pb::FieldCodec.ForInt64(34);
    private readonly pbc::RepeatedField<long> members_ = new pbc::RepeatedField<long>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<long> Members {
      get { return members_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (RoomId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(RoomId);
      }
      if (ChapterId != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(ChapterId);
      }
      if (Owner != 0L) {
        output.WriteRawTag(24);
        output.WriteInt64(Owner);
      }
      members_.WriteTo(output, _repeated_members_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (RoomId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(RoomId);
      }
      if (ChapterId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ChapterId);
      }
      if (Owner != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(Owner);
      }
      size += members_.CalculateSize(_repeated_members_codec);
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
            RoomId = input.ReadInt32();
            break;
          }
          case 16: {
            ChapterId = input.ReadInt32();
            break;
          }
          case 24: {
            Owner = input.ReadInt64();
            break;
          }
          case 34:
          case 32: {
            members_.AddEntriesFrom(input, _repeated_members_codec);
            break;
          }
        }
      }
    }

  }

  /// <summary>
  ///========== proto define ==========
  /// </summary>
  public sealed class CSRoomInfoNotify : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomInfoNotify> _parser = new pb::MessageParser<CSRoomInfoNotify>(() => new CSRoomInfoNotify());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomInfoNotify> Parser { get { return _parser; } }

    /// <summary>Field number for the "info" field.</summary>
    public const int InfoFieldNumber = 1;
    private global::Csp.CSRoomInfo info_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Csp.CSRoomInfo Info {
      get { return info_; }
      set {
        info_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (info_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Info);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (info_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Info);
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
            if (info_ == null) {
              info_ = new global::Csp.CSRoomInfo();
            }
            input.ReadMessage(info_);
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomGetListReq : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomGetListReq> _parser = new pb::MessageParser<CSRoomGetListReq>(() => new CSRoomGetListReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomGetListReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "sub_dungeon_id" field.</summary>
    public const int SubDungeonIdFieldNumber = 1;
    private int subDungeonId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int SubDungeonId {
      get { return subDungeonId_; }
      set {
        subDungeonId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (SubDungeonId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(SubDungeonId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (SubDungeonId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(SubDungeonId);
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
            SubDungeonId = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomGetListResp : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomGetListResp> _parser = new pb::MessageParser<CSRoomGetListResp>(() => new CSRoomGetListResp());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomGetListResp> Parser { get { return _parser; } }

    /// <summary>Field number for the "info" field.</summary>
    public const int InfoFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Csp.CSRoomInfo> _repeated_info_codec
        = pb::FieldCodec.ForMessage(10, global::Csp.CSRoomInfo.Parser);
    private readonly pbc::RepeatedField<global::Csp.CSRoomInfo> info_ = new pbc::RepeatedField<global::Csp.CSRoomInfo>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Csp.CSRoomInfo> Info {
      get { return info_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      info_.WriteTo(output, _repeated_info_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += info_.CalculateSize(_repeated_info_codec);
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
            info_.AddEntriesFrom(input, _repeated_info_codec);
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomCreateReq : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomCreateReq> _parser = new pb::MessageParser<CSRoomCreateReq>(() => new CSRoomCreateReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomCreateReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "chapter_id" field.</summary>
    public const int ChapterIdFieldNumber = 1;
    private int chapterId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int ChapterId {
      get { return chapterId_; }
      set {
        chapterId_ = value;
      }
    }

    /// <summary>Field number for the "level_limit" field.</summary>
    public const int LevelLimitFieldNumber = 2;
    private int levelLimit_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int LevelLimit {
      get { return levelLimit_; }
      set {
        levelLimit_ = value;
      }
    }

    /// <summary>Field number for the "public" field.</summary>
    public const int PublicFieldNumber = 3;
    private bool public_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Public {
      get { return public_; }
      set {
        public_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ChapterId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(ChapterId);
      }
      if (LevelLimit != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(LevelLimit);
      }
      if (Public != false) {
        output.WriteRawTag(24);
        output.WriteBool(Public);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ChapterId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ChapterId);
      }
      if (LevelLimit != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(LevelLimit);
      }
      if (Public != false) {
        size += 1 + 1;
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
            ChapterId = input.ReadInt32();
            break;
          }
          case 16: {
            LevelLimit = input.ReadInt32();
            break;
          }
          case 24: {
            Public = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomCreateResp : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomCreateResp> _parser = new pb::MessageParser<CSRoomCreateResp>(() => new CSRoomCreateResp());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomCreateResp> Parser { get { return _parser; } }

    /// <summary>Field number for the "room_token" field.</summary>
    public const int RoomTokenFieldNumber = 1;
    private string roomToken_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string RoomToken {
      get { return roomToken_; }
      set {
        roomToken_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "room_url" field.</summary>
    public const int RoomUrlFieldNumber = 2;
    private string roomUrl_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string RoomUrl {
      get { return roomUrl_; }
      set {
        roomUrl_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "room_id" field.</summary>
    public const int RoomIdFieldNumber = 3;
    private int roomId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int RoomId {
      get { return roomId_; }
      set {
        roomId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (RoomToken.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(RoomToken);
      }
      if (RoomUrl.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(RoomUrl);
      }
      if (RoomId != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(RoomId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (RoomToken.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(RoomToken);
      }
      if (RoomUrl.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(RoomUrl);
      }
      if (RoomId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(RoomId);
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
            RoomToken = input.ReadString();
            break;
          }
          case 18: {
            RoomUrl = input.ReadString();
            break;
          }
          case 24: {
            RoomId = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomJoinReq : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomJoinReq> _parser = new pb::MessageParser<CSRoomJoinReq>(() => new CSRoomJoinReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomJoinReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "room_id" field.</summary>
    public const int RoomIdFieldNumber = 1;
    private int roomId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int RoomId {
      get { return roomId_; }
      set {
        roomId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (RoomId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(RoomId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (RoomId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(RoomId);
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
            RoomId = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomJoinResp : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomJoinResp> _parser = new pb::MessageParser<CSRoomJoinResp>(() => new CSRoomJoinResp());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomJoinResp> Parser { get { return _parser; } }

    /// <summary>Field number for the "room_token" field.</summary>
    public const int RoomTokenFieldNumber = 1;
    private string roomToken_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string RoomToken {
      get { return roomToken_; }
      set {
        roomToken_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "room_url" field.</summary>
    public const int RoomUrlFieldNumber = 2;
    private string roomUrl_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string RoomUrl {
      get { return roomUrl_; }
      set {
        roomUrl_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (RoomToken.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(RoomToken);
      }
      if (RoomUrl.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(RoomUrl);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (RoomToken.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(RoomToken);
      }
      if (RoomUrl.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(RoomUrl);
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
            RoomToken = input.ReadString();
            break;
          }
          case 18: {
            RoomUrl = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomLeaveReq : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomLeaveReq> _parser = new pb::MessageParser<CSRoomLeaveReq>(() => new CSRoomLeaveReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomLeaveReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "room_id" field.</summary>
    public const int RoomIdFieldNumber = 1;
    private int roomId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int RoomId {
      get { return roomId_; }
      set {
        roomId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (RoomId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(RoomId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (RoomId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(RoomId);
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
            RoomId = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomLeaveResp : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomLeaveResp> _parser = new pb::MessageParser<CSRoomLeaveResp>(() => new CSRoomLeaveResp());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomLeaveResp> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
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
        }
      }
    }

  }

  public sealed class CSRoomPlayerStatusReq : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomPlayerStatusReq> _parser = new pb::MessageParser<CSRoomPlayerStatusReq>(() => new CSRoomPlayerStatusReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomPlayerStatusReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "status" field.</summary>
    public const int StatusFieldNumber = 1;
    private global::Csp.CSRoomPlayerStatus status_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Csp.CSRoomPlayerStatus Status {
      get { return status_; }
      set {
        status_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Status != 0) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Status);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Status != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Status);
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
            status_ = (global::Csp.CSRoomPlayerStatus) input.ReadEnum();
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomPlayerStatusResp : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomPlayerStatusResp> _parser = new pb::MessageParser<CSRoomPlayerStatusResp>(() => new CSRoomPlayerStatusResp());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomPlayerStatusResp> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
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
        }
      }
    }

  }

  public sealed class CSRoomPlayerStatusNotify : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomPlayerStatusNotify> _parser = new pb::MessageParser<CSRoomPlayerStatusNotify>(() => new CSRoomPlayerStatusNotify());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomPlayerStatusNotify> Parser { get { return _parser; } }

    /// <summary>Field number for the "uid" field.</summary>
    public const int UidFieldNumber = 1;
    private long uid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long Uid {
      get { return uid_; }
      set {
        uid_ = value;
      }
    }

    /// <summary>Field number for the "status" field.</summary>
    public const int StatusFieldNumber = 2;
    private global::Csp.CSRoomPlayerStatus status_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Csp.CSRoomPlayerStatus Status {
      get { return status_; }
      set {
        status_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Uid != 0L) {
        output.WriteRawTag(8);
        output.WriteInt64(Uid);
      }
      if (Status != 0) {
        output.WriteRawTag(16);
        output.WriteEnum((int) Status);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Uid != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(Uid);
      }
      if (Status != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Status);
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
            Uid = input.ReadInt64();
            break;
          }
          case 16: {
            status_ = (global::Csp.CSRoomPlayerStatus) input.ReadEnum();
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomReconnectReq : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomReconnectReq> _parser = new pb::MessageParser<CSRoomReconnectReq>(() => new CSRoomReconnectReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomReconnectReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "room_id" field.</summary>
    public const int RoomIdFieldNumber = 1;
    private int roomId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int RoomId {
      get { return roomId_; }
      set {
        roomId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (RoomId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(RoomId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (RoomId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(RoomId);
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
            RoomId = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomReconnectResp : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomReconnectResp> _parser = new pb::MessageParser<CSRoomReconnectResp>(() => new CSRoomReconnectResp());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomReconnectResp> Parser { get { return _parser; } }

    /// <summary>Field number for the "room_token" field.</summary>
    public const int RoomTokenFieldNumber = 1;
    private string roomToken_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string RoomToken {
      get { return roomToken_; }
      set {
        roomToken_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (RoomToken.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(RoomToken);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (RoomToken.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(RoomToken);
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
            RoomToken = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomInviteReq : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomInviteReq> _parser = new pb::MessageParser<CSRoomInviteReq>(() => new CSRoomInviteReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomInviteReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "uid" field.</summary>
    public const int UidFieldNumber = 1;
    private static readonly pb::FieldCodec<long> _repeated_uid_codec
        = pb::FieldCodec.ForInt64(10);
    private readonly pbc::RepeatedField<long> uid_ = new pbc::RepeatedField<long>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<long> Uid {
      get { return uid_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      uid_.WriteTo(output, _repeated_uid_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += uid_.CalculateSize(_repeated_uid_codec);
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
          case 10:
          case 8: {
            uid_.AddEntriesFrom(input, _repeated_uid_codec);
            break;
          }
        }
      }
    }

  }

  public sealed class CSRoomInviteResp : pb::IMessage {
    private static readonly pb::MessageParser<CSRoomInviteResp> _parser = new pb::MessageParser<CSRoomInviteResp>(() => new CSRoomInviteResp());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSRoomInviteResp> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
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
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
