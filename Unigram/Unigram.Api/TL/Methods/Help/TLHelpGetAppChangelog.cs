// <auto-generated/>
using System;

namespace Telegram.Api.TL.Methods.Help
{
	/// <summary>
	/// RCP method help.getAppChangelog.
	/// Returns <see cref="Telegram.Api.TL.TLUpdatesBase"/>
	/// </summary>
	public partial class TLHelpGetAppChangelog : TLObject
	{
		public String PrevAppVersion { get; set; }

		public TLHelpGetAppChangelog() { }
		public TLHelpGetAppChangelog(TLBinaryReader from)
		{
			Read(from);
		}

		public override TLType TypeId { get { return TLType.HelpGetAppChangelog; } }

		public override void Read(TLBinaryReader from)
		{
			PrevAppVersion = from.ReadString();
		}

		public override void Write(TLBinaryWriter to)
		{
			to.Write(0x9010EF6F);
			to.Write(PrevAppVersion);
		}
	}
}