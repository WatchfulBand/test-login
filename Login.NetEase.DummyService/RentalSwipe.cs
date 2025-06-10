using System;
using System.Text.RegularExpressions;
using System.Threading;
using ConsoleAppLogin.NetEase;
using Login.loginauth;
using Login.NetEase.RPCServer;
using Newtonsoft.Json;
using Noya.LocalServer.Common.Utilities;
using text;

namespace Login.NetEase.DummyService;

internal class RentalSwipe
{
	private int _processID;

	private RPCPort rPCPort;

	private AuthenticationResponse authenticationResponse;

	public static int Sleep;

	private void RecvPid(byte[] array)
	{
		_processID = BitConverter.ToInt32(array, 0);
		Console.WriteLine("已获取pid" + _processID);
	}

	public RentalSwipe(string cookiePath, string netease_sid, string ip, string port, string spfile)
	{
		RentalSwipe rentalSwipe = this;
		new Thread((ThreadStart)delegate
		{
			try
			{
				string sauth = app.ReadJson(cookiePath);
				Function.ClientLog("SwipeSleep:" + Sleep);
				string value = Http.PE_GetLoginResponse(sauth);
				rentalSwipe.authenticationResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(value);
				Function.ClientLog("PE-AUTH RESPONSE:" + Regex.Unescape(rentalSwipe.authenticationResponse.message));
				new Thread((ThreadStart)delegate
				{
					try
					{
						Thread.Sleep(60000);
						Http.UpdateAuthotp("https://g79obtcore.minecraft.cn:8443", rentalSwipe.authenticationResponse.entity.token);
					}
					catch (Exception)
					{
					}
				}).Start();
				string commonName = Function.GetCommonName(rentalSwipe.authenticationResponse.entity.entity_id, rentalSwipe.authenticationResponse.entity.token);
				rentalSwipe.rPCPort = new RPCPort();
				rentalSwipe.rPCPort.RegisterReceiveCallBack(0, rentalSwipe.RecvPid);
				while (app.start_stop)
				{
					app.InvokeCmd("start FastBuilder.exe --uid " + rentalSwipe.authenticationResponse.entity.entity_id + " --sid " + netease_sid + ":RentalGame --name " + commonName + " --version " + app.Temp.Version + " --engineVersion " + app.Temp.engineVersion + " --patchVersion " + app.Temp.patchVersion + " --url " + LockServer.AuthServerUrl + " --authurl https://g79authobt.minecraft.cn --ip " + ip + " --port " + port + " --token " + Convert.ToBase64String(HashHelper.SafeCompleteMD5(rentalSwipe.authenticationResponse.entity.token)) + " --g79 --typ sp --spfile " + spfile + " --sleep " + Sleep + " --LauncherPort " + rentalSwipe.rPCPort.LauncherControlPort);
				}
				DummySrv.strings.Add(cookiePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}).Start();
	}
}
