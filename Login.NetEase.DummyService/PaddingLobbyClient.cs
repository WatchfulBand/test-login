using System;
using System.Text.RegularExpressions;
using System.Threading;
using ConsoleAppLogin.NetEase;
using Login.loginauth;
using Newtonsoft.Json;
using Noya.LocalServer.Common.Utilities;
using text;

namespace Login.NetEase.DummyService;

internal class PaddingLobbyClient
{
	private AuthenticationResponse authenticationResponse;

	public static void StartPadding(string ip, string port)
	{
		Random random = new Random();
		new PaddingLobbyClient(DummySrv.strings[random.Next(0, DummySrv.strings.Count)], ip, port);
	}

	public PaddingLobbyClient(string cookiePath, string ip, string port)
	{
		PaddingLobbyClient paddingLobbyClient = this;
		DummySrv.strings.Remove(cookiePath);
		new Thread((ThreadStart)delegate
		{
			try
			{
				string loginToken = Http.GetLoginToken(app.ReadJson(cookiePath));
				paddingLobbyClient.authenticationResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(loginToken);
				Function.ClientLog("AUTH RESPONSE:" + Regex.Unescape(paddingLobbyClient.authenticationResponse.message));
				string name = Function.GetName(paddingLobbyClient.authenticationResponse.entity.entity_id, paddingLobbyClient.authenticationResponse.entity.token);
				if (Padding.DisplayEntity)
				{
					app.InvokeCmd("start FastBuilder.exe --uid " + paddingLobbyClient.authenticationResponse.entity.entity_id + " --sid 4654415063569776560:LobbyGame --name " + name + " --version " + app.Temp.Version + " --engineVersion " + app.Temp.engineVersion + " --patchVersion " + app.Temp.patchVersion + " --url " + LockServer.AuthServerUrl + " --authurl https://g79authobt.minecraft.cn --ip " + ip + " --port " + port + " --token " + Convert.ToBase64String(HashHelper.SafeCompleteMD5(paddingLobbyClient.authenticationResponse.entity.token)) + " --typ join");
				}
				else
				{
					app.InvokeCmd("start FastBuilder.exe --uid " + paddingLobbyClient.authenticationResponse.entity.entity_id + " --sid 4654415063569776560:LobbyGame --name " + name + " --version " + app.Temp.Version + " --engineVersion " + app.Temp.engineVersion + " --patchVersion " + app.Temp.patchVersion + " --url " + LockServer.AuthServerUrl + " --authurl https://g79authobt.minecraft.cn --ip " + ip + " --port " + port + " --token " + Convert.ToBase64String(HashHelper.SafeCompleteMD5(paddingLobbyClient.authenticationResponse.entity.token)));
				}
				DummySrv.strings.Add(cookiePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				DummySrv.strings.Add(cookiePath);
			}
		}).Start();
	}
}
