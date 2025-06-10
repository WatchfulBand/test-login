using System;

namespace WPFLauncher.Network.Launcher;

[Serializable]
public class AuthenticationEntity
{
	public string sa_data { get; set; }

	public string sauth_json { get; set; }

	public VersionEntity version { get; set; }

	public string sdkuid { get; set; }

	public string aid { get; set; }

	public bool hasMessage { get; set; }

	public bool hasGmail { get; set; }

	public string otp_token { get; set; }

	public string otp_pwd { get; set; }

	public int lock_time { get; set; }

	public string env { get; set; }

	public string min_engine_version { get; set; }

	public string min_patch_version { get; set; }

	public int verify_status { get; set; }

	public string unisdk_login_json { get; set; }

	public string token { get; set; }

	public bool is_register { get; set; } = true;

	public string entity_id { get; set; }
}
