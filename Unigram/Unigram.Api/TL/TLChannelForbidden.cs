// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLChannelForbidden : TLChatBase 
	{
		[Flags]
		public enum Flag : Int32
		{
			Broadcast = (1 << 5),
			MegaGroup = (1 << 8),
		}

		public bool IsBroadcast { get { return Flags.HasFlag(Flag.Broadcast); } set { Flags = value ? (Flags | Flag.Broadcast) : (Flags & ~Flag.Broadcast); } }
		public bool IsMegaGroup { get { return Flags.HasFlag(Flag.MegaGroup); } set { Flags = value ? (Flags | Flag.MegaGroup) : (Flags & ~Flag.MegaGroup); } }

		public Flag Flags { get; set; }
		public Int64 AccessHash { get; set; }
		public String Title { get; set; }

		public TLChannelForbidden() { }
		public TLChannelForbidden(TLBinaryReader from)
		{
			Read(from);
		}

		public override TLType TypeId { get { return TLType.ChannelForbidden; } }

		public override void Read(TLBinaryReader from)
		{
			Flags = (Flag)from.ReadInt32();
			Id = from.ReadInt32();
			AccessHash = from.ReadInt64();
			Title = from.ReadString();
		}

		public override void Write(TLBinaryWriter to)
		{
			to.Write(0x8537784F);
			to.Write((Int32)Flags);
			to.Write(Id);
			to.Write(AccessHash);
			to.Write(Title);
		}
	}
}