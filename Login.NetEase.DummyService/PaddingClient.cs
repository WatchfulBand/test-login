using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using ConsoleAppLogin.NetEase;
using Login.loginauth;
using Login.NetEase.RPCServer;
using Mark;
using MCStudio.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Noya.LocalServer.Common.Utilities;
using text;

namespace Login.NetEase.DummyService;

internal class PaddingClient
{
	private int _processID;

	private AuthenticationResponse authenticationResponse;

	private RPCPort rPCPort;

	public PaddingClient(string cookiePath, string netease_sid, string ip, string port)
	{
		PaddingClient paddingClient = this;
		new Thread((ThreadStart)delegate
		{
			try
			{
				string text = app.ReadJson(cookiePath);
				string value = Http.PE_GetLoginResponse(text);
				paddingClient.authenticationResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(value);
				Function.ClientLog("PE-AUTH RESPONSE:" + Regex.Unescape(paddingClient.authenticationResponse.message));
				string name = paddingClient.GetName(paddingClient.authenticationResponse.entity.entity_id, paddingClient.authenticationResponse.entity.token);
				paddingClient.rPCPort = new RPCPort();
				paddingClient.rPCPort.RegisterReceiveCallBack(0, paddingClient.RecvPid);
				if (Padding.DisplayEntity)
				{
					app.InvokeCmd("start FastBuilder.exe --uid " + paddingClient.authenticationResponse.entity.entity_id + " --sid " + netease_sid + ":RentalGame --name " + name + " --version " + app.Temp.Version + " --engineVersion " + app.Temp.engineVersion + " --patchVersion " + app.Temp.patchVersion + " --url " + LockServer.AuthServerUrl + " --authurl https://g79authobt.minecraft.cn --ip " + ip + " --port " + port + " --token " + Convert.ToBase64String(HashHelper.SafeCompleteMD5(paddingClient.authenticationResponse.entity.token)) + " --g79 --typ join --LauncherPort " + paddingClient.rPCPort.LauncherControlPort);
				}
				else
				{
					app.InvokeCmd("start FastBuilder.exe --uid " + paddingClient.authenticationResponse.entity.entity_id + " --sid " + netease_sid + ":RentalGame --name " + name + " --version " + app.Temp.Version + " --engineVersion " + app.Temp.engineVersion + " --patchVersion " + app.Temp.patchVersion + " --url " + LockServer.AuthServerUrl + " --authurl https://g79authobt.minecraft.cn --ip " + ip + " --port " + port + " --token " + Convert.ToBase64String(HashHelper.SafeCompleteMD5(paddingClient.authenticationResponse.entity.token)) + " --g79 --LauncherPort " + paddingClient.rPCPort.LauncherControlPort);
				}
				DummySrv.strings.Add(text);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}).Start();
	}

	private void RecvPid(byte[] array)
	{
		_processID = BitConverter.ToInt32(array, 0);
		Console.WriteLine("已获取pid" + _processID);
	}

	public void Close()
	{
		byte[] message = SimplePack.Pack((ushort)1, (byte)21);
		rPCPort.SendControlData(message);
		Thread.Sleep(1000);
		app.InvokeCmdAsync("taskkill /pid " + _processID + " -t -f");
		rPCPort.NormalExit();
	}

	public string GetName(string UID, string token)
	{
		string value = x19Crypt.ComputeDynamicToken("/user-detail", "", token);
		HttpContent httpContent = new StringContent("");
		httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		httpContent.Headers.Add("user-token", value);
		httpContent.Headers.Add("user-id", UID);
		string result = Http.client.PostAsync("https://x19apigatewayobt.nie.netease.com/user-detail", httpContent).Result.Content.ReadAsStringAsync().Result;
		if (Function.GetName(result) == "")
		{
			bool flag = false;
			string text;
			do
			{
				text = (flag ? Function.GetRandomKeyv2(8) : ("NO" + UID));
				flag = true;
			}
			while (!(JsonConvert.DeserializeObject<JObject>(Http.postAPIPC_NoCommon("{\"name\":\"" + text + "\",\"entity_id\":\"" + UID + "\"}", "/nickname-setting", authenticationResponse.entity.token, authenticationResponse.entity.entity_id))["code"].ToString() == "0"));
			return text;
		}
		return Function.GetName(result);
	}
}
