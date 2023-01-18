// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: cstask.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::ILRuntime.Protobuf;
using pbc = global::ILRuntime.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace Csp {

  #region Messages
  /// <summary>
  ///========== proto define ==========
  /// </summary>
  public sealed class CSTaskAwardReq : pb::IMessage {
    private static readonly pb::MessageParser<CSTaskAwardReq> _parser = new pb::MessageParser<CSTaskAwardReq>(() => new CSTaskAwardReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSTaskAwardReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "task_id" field.</summary>
    public const int TaskIdFieldNumber = 1;
    private int taskId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int TaskId {
      get { return taskId_; }
      set {
        taskId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (TaskId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(TaskId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (TaskId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(TaskId);
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
            TaskId = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CSTaskAwardResp : pb::IMessage {
    private static readonly pb::MessageParser<CSTaskAwardResp> _parser = new pb::MessageParser<CSTaskAwardResp>(() => new CSTaskAwardResp());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSTaskAwardResp> Parser { get { return _parser; } }

    /// <summary>Field number for the "task_id" field.</summary>
    public const int TaskIdFieldNumber = 1;
    private int taskId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int TaskId {
      get { return taskId_; }
      set {
        taskId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (TaskId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(TaskId);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (TaskId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(TaskId);
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
            TaskId = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed class CSTaskBatchAwardReq : pb::IMessage {
    private static readonly pb::MessageParser<CSTaskBatchAwardReq> _parser = new pb::MessageParser<CSTaskBatchAwardReq>(() => new CSTaskBatchAwardReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSTaskBatchAwardReq> Parser { get { return _parser; } }

    /// <summary>Field number for the "tasks" field.</summary>
    public const int TasksFieldNumber = 1;
    private static readonly pb::FieldCodec<int> _repeated_tasks_codec
        = pb::FieldCodec.ForInt32(10);
    private readonly pbc::RepeatedField<int> tasks_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> Tasks {
      get { return tasks_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      tasks_.WriteTo(output, _repeated_tasks_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += tasks_.CalculateSize(_repeated_tasks_codec);
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
            tasks_.AddEntriesFrom(input, _repeated_tasks_codec);
            break;
          }
        }
      }
    }

  }

  public sealed class CSTaskBatchAwardResp : pb::IMessage {
    private static readonly pb::MessageParser<CSTaskBatchAwardResp> _parser = new pb::MessageParser<CSTaskBatchAwardResp>(() => new CSTaskBatchAwardResp());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CSTaskBatchAwardResp> Parser { get { return _parser; } }

    /// <summary>Field number for the "tasks" field.</summary>
    public const int TasksFieldNumber = 1;
    private static readonly pbc::MapField<int, global::Datap.TaskProcessStatus>.Codec _map_tasks_codec
        = new pbc::MapField<int, global::Datap.TaskProcessStatus>.Codec(pb::FieldCodec.ForInt32(8), pb::FieldCodec.ForEnum(16, x => (int) x, x => (global::Datap.TaskProcessStatus) x), 10);
    private readonly pbc::MapField<int, global::Datap.TaskProcessStatus> tasks_ = new pbc::MapField<int, global::Datap.TaskProcessStatus>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::MapField<int, global::Datap.TaskProcessStatus> Tasks {
      get { return tasks_; }
    }

    /// <summary>Field number for the "success" field.</summary>
    public const int SuccessFieldNumber = 2;
    private static readonly pb::FieldCodec<int> _repeated_success_codec
        = pb::FieldCodec.ForInt32(18);
    private readonly pbc::RepeatedField<int> success_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> Success {
      get { return success_; }
    }

    /// <summary>Field number for the "failed" field.</summary>
    public const int FailedFieldNumber = 3;
    private static readonly pb::FieldCodec<int> _repeated_failed_codec
        = pb::FieldCodec.ForInt32(26);
    private readonly pbc::RepeatedField<int> failed_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> Failed {
      get { return failed_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      tasks_.WriteTo(output, _map_tasks_codec);
      success_.WriteTo(output, _repeated_success_codec);
      failed_.WriteTo(output, _repeated_failed_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += tasks_.CalculateSize(_map_tasks_codec);
      size += success_.CalculateSize(_repeated_success_codec);
      size += failed_.CalculateSize(_repeated_failed_codec);
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
            tasks_.AddEntriesFrom(input, _map_tasks_codec);
            break;
          }
          case 18:
          case 16: {
            success_.AddEntriesFrom(input, _repeated_success_codec);
            break;
          }
          case 26:
          case 24: {
            failed_.AddEntriesFrom(input, _repeated_failed_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
