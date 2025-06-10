using System;
using System.Collections.Generic;
using System.IO;
using ConsoleAppLogin.NetEase;
using FeverToSauth;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using text;

namespace Login.NetEase.LoginotpList;

internal class LoginInfo
{
	public static string CookleChoose()
	{
		try
		{
			string[] filePath = GetFilePath(Directory.GetCurrentDirectory() + "\\user");
			if (filePath == null || filePath.Length == 0)
			{
				Console.WriteLine("请添加一个账号！");
				string text = Interaction.InputBox("输入Sauth(Cookie)或者SDK4399", "Login", null);
				Console.Write("你要保存为？名称：");
				string text2 = Console.ReadLine();
				File.WriteAllText("user\\" + text2 + ".json", text);
				return text;
			}
			int num = 0;
			string[] array = filePath;
			foreach (string text3 in array)
			{
				Console.WriteLine("序号[" + num + "]--" + text3);
				num++;
			}
			Console.WriteLine("输入add添加账号");
			Console.Write("输入你要登录的账号（序号）：");
			string text4 = Console.ReadLine();
			if (text4 == "add")
			{
				string text5 = adduser();
				if (text5 != null)
				{
					return text5;
				}
				return null;
			}
			if (text4 == "of")
			{
				return "o";
			}
			int num2 = text4.IndexOf("pe");
			if (num2 != -1)
			{
				int num3 = int.Parse(text4.Substring(0, num2));
				string text6 = app.ReadJson(filePath[num3]);
				app.PE_Login = true;
				if (text6.StartsWith("{"))
				{
					return text6;
				}
				if (text6.StartsWith("ey"))
				{
					return FeverAuth.FeverToSauth(text6);
				}
				if (text6.StartsWith("SDK4399"))
				{
					return text6;
				}
				Console.WriteLine("cookie无效，请重新输入！");
				return null;
			}
			int num4 = int.Parse(text4);
			string text7 = app.ReadJson(filePath[num4]);
			if (text7.StartsWith("{"))
			{
				return text7;
			}
			if (text7.StartsWith("ey"))
			{
				return FeverAuth.FeverToSauth(text7);
			}
			if (text7.StartsWith("SDK4399"))
			{
				return text7;
			}
			Console.WriteLine("cookie无效，请重新输入！");
			return null;
		}
		catch
		{
			Console.WriteLine("输入不在范围内");
			Console.Clear();
			return null;
		}
	}

	private static bool CheckCookie(string loginotp)
	{
		_ = string.Empty;
		try
		{
			if (int.Parse(JsonConvert.DeserializeObject<JObject>(Http.x19post("https://x19apigatewayobt.nie.netease.com", app.Temp.url + "/login-otp", loginotp))["code"].ToString()) != 0)
			{
				return false;
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static string[] GetFilePath(string path)
	{
		try
		{
			string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
			List<string> list = new List<string>();
			for (int i = 0; i < files.Length; i++)
			{
				list.Add(files[i]);
			}
			return list.ToArray();
		}
		catch
		{
			Console.WriteLine("未找到路径");
			return null;
		}
	}

	public static string adduser()
	{
		Console.WriteLine("请添加一个账号！");
		string text = Interaction.InputBox("输入Sauth(Cookie)或者SDK4399", "Login", null);
		Console.Write("你要保存为？名称：");
		string text2 = Console.ReadLine();
		File.WriteAllText("user\\" + text2 + ".json", text);
		return text;
	}
}
