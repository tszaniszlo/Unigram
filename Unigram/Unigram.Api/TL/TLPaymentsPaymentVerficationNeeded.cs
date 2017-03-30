// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLPaymentsPaymentVerficationNeeded : TLPaymentsPaymentResultBase 
	{
		public String Url { get; set; }

		public TLPaymentsPaymentVerficationNeeded() { }
		public TLPaymentsPaymentVerficationNeeded(TLBinaryReader from)
		{
			Read(from);
		}

		public override TLType TypeId { get { return TLType.PaymentsPaymentVerficationNeeded; } }

		public override void Read(TLBinaryReader from)
		{
			Url = from.ReadString();
		}

		public override void Write(TLBinaryWriter to)
		{
			to.Write(0x6B56B921);
			to.Write(Url);
		}
	}
}