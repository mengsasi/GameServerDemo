// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: Sync_Character.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace GameProto {

  /// <summary>Holder for reflection information generated from Sync_Character.proto</summary>
  public static partial class SyncCharacterReflection {

    #region Descriptor
    /// <summary>File descriptor for Sync_Character.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static SyncCharacterReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChRTeW5jX0NoYXJhY3Rlci5wcm90bxIJZ2FtZVByb3RvGg9DaGFyYWN0ZXIu",
            "cHJvdG8iOQoOU3luY19DaGFyYWN0ZXISJwoJY2hhcmFjdGVyGAEgASgLMhQu",
            "Z2FtZVByb3RvLkNoYXJhY3RlcmIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::GameProto.CharacterReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::GameProto.Sync_Character), global::GameProto.Sync_Character.Parser, new[]{ "Character" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Sync_Character : pb::IMessage<Sync_Character> {
    private static readonly pb::MessageParser<Sync_Character> _parser = new pb::MessageParser<Sync_Character>(() => new Sync_Character());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Sync_Character> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::GameProto.SyncCharacterReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Sync_Character() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Sync_Character(Sync_Character other) : this() {
      Character = other.character_ != null ? other.Character.Clone() : null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Sync_Character Clone() {
      return new Sync_Character(this);
    }

    /// <summary>Field number for the "character" field.</summary>
    public const int CharacterFieldNumber = 1;
    private global::GameProto.Character character_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::GameProto.Character Character {
      get { return character_; }
      set {
        character_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Sync_Character);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Sync_Character other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Character, other.Character)) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (character_ != null) hash ^= Character.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (character_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Character);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (character_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Character);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Sync_Character other) {
      if (other == null) {
        return;
      }
      if (other.character_ != null) {
        if (character_ == null) {
          character_ = new global::GameProto.Character();
        }
        Character.MergeFrom(other.Character);
      }
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
            if (character_ == null) {
              character_ = new global::GameProto.Character();
            }
            input.ReadMessage(character_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
