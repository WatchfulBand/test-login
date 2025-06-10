using System;

namespace WPFLauncher.Manager.Log;

[Serializable]
internal class PublicSaData
{
	public string os_name { get; set; }

	public string os_ver { get; set; }

	public string mac_addr { get; set; }

	public string udid { get; set; }

	public string app_ver { get; set; }

	public string sdk_ver { get; set; }

	public string network { get; set; }

	public string disk { get; set; }

	public string is64bit { get; set; }

	public string video_card1 { get; set; }

	public string video_card2 { get; set; }

	public string video_card3 { get; set; }

	public string video_card4 { get; set; }

	public string launcher_type { get; set; }

	public string pay_channel { get; set; }

	public string dotnet_ver { get; set; }

	public string cpu_type { get; set; }

	public string ram_size { get; set; }

	public string disk_size { get; set; }

	public string device_height { get; set; }

	public string os_detail { get; set; }
}
