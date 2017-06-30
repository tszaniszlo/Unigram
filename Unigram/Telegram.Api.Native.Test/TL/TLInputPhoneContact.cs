// <auto-generated/>
using System;
using Telegram.Api.Native.TL;

namespace Telegram.Api.TL
{
	public partial class TLInputPhoneContact : TLInputContactBase 
	{
		public Int64 ClientId { get; set; }
		public String Phone { get; set; }
		public String FirstName { get; set; }
		public String LastName { get; set; }

		public TLInputPhoneContact() { }
		public TLInputPhoneContact(TLBinaryReader from)
		{
			Read(from);
		}

		public override TLType TypeId { get { return TLType.InputPhoneContact; } }

		public override void Read(TLBinaryReader from)
		{
			ClientId = from.ReadInt64();
			Phone = from.ReadString();
			FirstName = from.ReadString();
			LastName = from.ReadString();
		}

		public override void Write(TLBinaryWriter to)
		{
			to.WriteInt64(ClientId);
			to.WriteString(Phone ?? string.Empty);
			to.WriteString(FirstName ?? string.Empty);
			to.WriteString(LastName ?? string.Empty);
		}
	}
}