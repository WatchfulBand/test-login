using ConsoleAppLogin.NetEase;
using FastBuilder.Utils;
using Login;
using Login.JavaHelper;
using Login.JavaHelper.JavaConfigHelper;
using Login.Native;
using Login.NetEase;
using Login.NetEase.ChatService;
using Login.NetEase.DummyService;
using Login.NetEase.DummyService.Entity;
using Login.NetEase.Entity;
using Login.NetEase.LinkService;
using Login.NetEase.LoginotpList;
using Login.NetEase.Request;
using Login.NetEase.RPCServer;
using Login.NetEase.Utils;
using Mark;
using MCStudio.Model.EnumType;
using MCStudio.Network;
using MCStudio.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Noya.LocalServer.Common.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using text.loginauth;
using WPFLauncher.Manager.Configuration.CppConfigure;
using WPFLauncher.Network.Launcher;
using WPFLauncher.Util;

namespace text;

internal class app
{
	public static ChatConnection chatConnection;

	public static LinkConnection linkConnection;

	public static temp Temp { get; set; }

	public static RPCPort GameRpcPort { get; set; }

	public static string PostHost { get; set; } = "https://x19apigatewayobt.nie.netease.com";

	public static string player_info { get; set; }

	private static string Login_otp { get; set; }

	public static bool isroom { get; set; }

	public static bool start_stop { get; set; }

	public static bool kick_stop { get; set; }

	private static string SavePermissions { get; set; } = "false";

	private static int JoinPermissions { get; set; } = 0;

	private static bool sajoin { get; set; } = true;

	public static bool PE_Login { get; set; } = false;

	public static string displayName { get; set; }

	public static void AppRun()
	{
		do
		{
			Login_otp = LoginInfo.CookleChoose();
		}
		while (Login_otp == null);
		Console.Clear();
		if (Login_otp == "o")
		{
			Command();
			return;
		}
		if (Login_otp == null)
		{
			Console.WriteLine("登录失败");
		}
		Console.WriteLine("正在登录中，请稍等...");
		if (Temp.is_g79 || PE_Login)
		{
			Http.PE_Login(Login_otp);
		}
		else
		{
			Http.Login(Login_otp);
		}
		PE_Login = false;
		if (Temp.chat_connect || ChatConnection.Is_Connected)
		{
			chatConnection = new ChatConnection();
		}
		if (Temp.InitializeName)
		{
			Name();
		}
		Function.GetName(player_info);
		Console.WriteLine("你好！冒险家：" + displayName);
		Console.WriteLine("输入help可查看指令");
		Command();
	}

	public static void AppLoad(string Login_otp)
	{
		Console.WriteLine("正在登录中，请稍等...");
		Http.Login(Login_otp);
		if (Temp.InitializeName)
		{
			Name();
		}
		Function.GetName(player_info);
		Console.WriteLine("你好！冒险家：" + displayName);
		Console.WriteLine("输入help可查看指令");
		Command();
	}

	public static void AppRun_(string response)
	{
		try
		{
			Console.WriteLine("正在登录中，请稍等...");
			Http.LoadToken(ReadJson(response));
			if (Temp.InitializeName)
			{
				Name();
			}
			Console.WriteLine("你好！冒险家：" + Function.GetName(player_info));
			Console.WriteLine("输入help可查看指令");
			Command();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public static void ui()
	{
		Console.WriteLine("=============================");
		Console.WriteLine("获取CPPtoken ---- geth5token");
		Console.WriteLine("更新token ------ updatetoken");
		Console.WriteLine("重新登录 -------------- anew");
		Console.WriteLine("起名 --------------- setname");
		Console.WriteLine("获得名字 ----------- getname");
		Console.WriteLine("退出登录 ------------ delete");
		Console.WriteLine("改名 ------------ nameupdate");
		Console.WriteLine("设置游戏路径 ------- setpath");
		Console.WriteLine("获取游戏 ----------- getgame");
		Console.WriteLine("启动游戏 ------------- start");
		Console.WriteLine("联机大厅开房间 ------ startr");
		Console.WriteLine("允许房客保存 --- visitorsave");
		Console.WriteLine("保存地图 -------------- save");
		Console.WriteLine("载入地图 -------------- load");
		Console.WriteLine("踢人 ------------------ kick");
		Console.WriteLine("房间可见性 ------ visibility");
		Console.WriteLine("转交房主 ------------- setop");
		Console.WriteLine("购买组件 ---------- purchase");
		Console.WriteLine("=============================");
	}

	public static void Help()
	{
		Console.WriteLine("显示该列表 --------------------------- help");
		Console.WriteLine("查看版本号 --------------------------- version");
		Console.WriteLine("获取CPPtoken ------------------------- geth5token");
		Console.WriteLine("获取token ---------------------------- gettoken");
		Console.WriteLine("获取DToken --------------------------- getdtoken");
		Console.WriteLine("更新token ---------------------------- updatetoken");
		Console.WriteLine("重新登录 ----------------------------- anew");
		Console.WriteLine("起名 --------------------------------- setname");
		Console.WriteLine("获得名字 ----------------------------- getname");
		Console.WriteLine("退出登录 ----------------------------- delete");
		Console.WriteLine("改名 --------------------------------- nameupdate");
		Console.WriteLine("设置游戏路径 ------------------------- setpath");
		Console.WriteLine("获取游戏 ----------------------------- getgame");
		Console.WriteLine("启动游戏：自定义ip端口 --------------- ipstart");
		Console.WriteLine("启动游戏 ----------------------------- start");
		Console.WriteLine("启动游戏（启动器端口） --------------- portstart");
		Console.WriteLine("启动游戏设置模式---------------------- gametype");
		Console.WriteLine("认证启动游戏 ------------------------- authstart");
		Console.WriteLine("联机大厅开房间 ----------------------- startr");
		Console.WriteLine("手机版联机大厅开房间 ----------------- startrPE");
		Console.WriteLine("允许房客保存 ------------------------- visitorsave");
		Console.WriteLine("保存地图 ----------------------------- save");
		Console.WriteLine("重置地图 ----------------------------- reset");
		Console.WriteLine("载入地图 ----------------------------- load");
		Console.WriteLine("载入手机版地图 ----------------------- loadPE");
		Console.WriteLine("踢人 --------------------------------- kick");
		Console.WriteLine("一键踢所有人 ------------------------- kickall");
		Console.WriteLine("房间密码热更新 ----------------------- setpassword");
		Console.WriteLine("房间可见性 --------------------------- visibility");
		Console.WriteLine("转交房主 ----------------------------- setop");
		Console.WriteLine("购买组件 ----------------------------- purchase");
		Console.WriteLine("购买手机版组件 ----------------------- purchasePE");
		Console.WriteLine("启动游戏添加Itemid ------------------- additem");
		Console.WriteLine("删除Itemid --------------------------- ritem");
		Console.WriteLine("搜索房间号或联机大厅组件 ------------- search");
		Console.WriteLine("搜索手机版房间号或联机大厅组件 ------- searchPE");
		Console.WriteLine("通过房主名称搜索房间 ----------------- searchroom");
		Console.WriteLine("通过房主名称搜索房间 ----------------- searchroomv2");
		Console.WriteLine("通过uid查找用户 ---------------------- searchuid");
		Console.WriteLine("通过用户名查找uid -------------------- searchuser");
		Console.WriteLine("进入房间 ----------------------------- join");
		Console.WriteLine("进入房间（不掉） --------------------- ujoin");
		Console.WriteLine("进密码房 ----------------------------- passwordjoin");
		Console.WriteLine("进入手机版房间 ----------------------- joinPE");
		Console.WriteLine("退出房间 ----------------------------- left");
		Console.WriteLine("指定房间id退出房间 ------------------- wleft");
		Console.WriteLine("获取房间ip和端口 --------------------- getip");
		Console.WriteLine("搜索物品id --------------------------- searchitem");
		Console.WriteLine("开发功能：自定义接口调用 ------------- post");
		Console.WriteLine("开发功能：自定义接口调用 ------------- postfile");
		Console.WriteLine("开发功能：自定义接口调用（加密） ----- Epost");
		Console.WriteLine("开发功能：自定义接口调用（加密） ----- Epostfile");
		Console.WriteLine("修改post服务器 ----------------------- setposthost");
		Console.WriteLine("自定义接口调用（修改用户信息） ------- spost");
		Console.WriteLine("自定义接口调用（修改用户信息） ------- spostfile");
		Console.WriteLine("测试认证服务器 ----------------------- auth");
		Console.WriteLine("测试认证服务器v2 --------------------- authv2");
		Console.WriteLine("测试认证服务器 ----------------------- peauth");
		Console.WriteLine("测试认证服务器v2 --------------------- peauthv2");
		Console.WriteLine("测试认证服务器 ----------------------- authpath");
		Console.WriteLine("测试认证服务器v2 --------------------- authpathv2");
		Console.WriteLine("测试认证服务器 ----------------------- peauthpath");
		Console.WriteLine("测试认证服务器v2 --------------------- peauthpathv2");
		Console.WriteLine("循环进入房间 ------------------------- ajoin");
		Console.WriteLine("使用entityid循环进房间 --------------- ajoinrid");
		Console.WriteLine("停止进入房间 ------------------------- STOP");
		Console.WriteLine("获取RTX版 ---------------------------- getrtxgame");
		Console.WriteLine("获取校验地址 ------------------------- getallgame");
		Console.WriteLine("获取64位版 --------------------------- test");
		Console.WriteLine("搜索名字 ----------------------------- searchuser");
		Console.WriteLine("封禁玩家 ----------------------------- ban");
		Console.WriteLine("启动封禁 ----------------------------- startban");
		Console.WriteLine("清除所有ban -------------------------- clearbanid");
		Console.WriteLine("解ban -------------------------------- unban");
		Console.WriteLine("自定义进入游戏 ----------------------- gametype");
		Console.WriteLine("进入手机版多人主城 ------------------- joinlobby");
		Console.WriteLine("开启端口连接启动游戏 ----------------- portstart");
		Console.WriteLine("向游戏发送数据 ----------------------- senddata");
		Console.WriteLine("启动控制连接 ------------------------- startconnect");
		Console.WriteLine("关闭控制连接 ------------------------- closeconnect");
		Console.WriteLine("开启房间自动更新 --------------------- startroomupdate");
		Console.WriteLine("关闭房间自动更新 --------------------- closeroomupdate");
		Console.WriteLine("获取uid ------------------------------ getuid");
		Console.WriteLine("获取peuid ---------------------------- getpeuid");
		Console.WriteLine("获取SRCToken ------------------------- gettoken");
		Console.WriteLine("获取DToken --------------------------- getdtoken");
		Console.WriteLine("初始化解包 --------------------------- getiteminfo");
		Console.WriteLine("获取PEuid ---------------------------- getpeuid");
		Console.WriteLine("获取模组下载地址 --------------------- getmod");
		Console.WriteLine("获取模组下载地址 --------------------- getpemod");
		Console.WriteLine("获取模组下载地址 --------------------- searchmod");
		Console.WriteLine("获取模组下载地址 --------------------- searchpemod");
		Console.WriteLine("解包mczip文件 ------------------------ unzip");
		Console.WriteLine("解密模组的所有文件 ------------------- decryptpath");
		Console.WriteLine("获取模组密钥 ------------------------- getitemkey");
		Console.WriteLine("获取手机版模组密钥 ------------------- getpeitemkey");
		Console.WriteLine("获取房间列表 ------------------------- roomlist");
		Console.WriteLine("获取手机版房间列表 ------------------- roomlistPE");
		Console.WriteLine("手机版房间快速匹配 ------------------- fast");
		Console.WriteLine("通过房间id启动自动更新 --------------- wstartroomupdate");
		Console.WriteLine("获取房间成员列表 --------------------- playerlist");
		Console.WriteLine("通过房间id获取房间成员列表 ----------- wplayerlist");
		Console.WriteLine("启动本地authv2认证 ------------------- startserver");
		Console.WriteLine("启动本地FastBuilder认证 -------------- startserverv2");
		Console.WriteLine("设置服务器id（本地认证前置） --------- serverid");
		Console.WriteLine("设置服务器ip（FB认证前置） ----------- serverip");
		Console.WriteLine("本地联机服务器列表 ------------------- allserver");
		Console.WriteLine("本地联机踢人 ------------------------- mkick");
		Console.WriteLine("追踪锁服 ----------------------------- traceUID");
		Console.WriteLine("追踪崩服 ----------------------------- lockUID");
		Console.WriteLine("启动聊天服务器连接 ------------------- startchatconnect");
		Console.WriteLine("启动Link服务器连接 ------------------- startlinkconnect");
		Console.WriteLine("关闭聊天服务器连接 ------------------- closechatconnect");
		Console.WriteLine("关闭Link服务器连接 ------------------- closelinkconnect");
		Console.WriteLine("自定义link消息 ----------------------- sendlink");
		Console.WriteLine("自定义link消息 ----------------------- sendlinkfiel");
		Console.WriteLine("sign签名校验 ------------------------- getsign");
		Console.WriteLine("生成用户索引文件 --------------------- userlist");
		Console.WriteLine("发送好友消息 ------------------------- senduid");
		Console.WriteLine("发送好友消息 ------------------------- senduidpath");
		Console.WriteLine("远程卡人 ----------------------------- asenduid");
		Console.WriteLine("远程卡人 ----------------------------- asenduidpath");
		Console.WriteLine("加好友 ------------------------------- addfriend");
		Console.WriteLine("远程卡人（随机小号） ----------------- kickplayer");
		Console.WriteLine("一键布吉岛 --------------------------- fastbjd");
		Console.WriteLine("一键花雨庭 --------------------------- fasthyt");
		Console.WriteLine("搜索pe租赁服 ------------------------- searchserver");
		Console.WriteLine("获取租赁服ip ------------------------- getserverip");
		Console.WriteLine("锁服（随机小号） --------------------- lockserver");
		Console.WriteLine("（FastBuilder）联大崩服 -------------- lockserverv2");
		Console.WriteLine("（FastBuilder）租赁服锁服 ------------ lockserverv3");
		Console.WriteLine("（FastBuilder）联机大厅刷屏 ---------- lockserverv4");
		Console.WriteLine("（FastBuilder）联机大厅字符画 -------- lockserverv5");
		Console.WriteLine("（FastBuilder）租赁服锁服 ------------ lockserverv6");
		Console.WriteLine("（FastBuilder）设置认证服 ------------ setauthserver");
		Console.WriteLine("（FastBuilder）设置认证服v2 ---------- setwebserver");
		Console.WriteLine("（FastBuilder）显示窗口 -------------- displaywindow");
		Console.WriteLine("（FastBuilder）显示实体 -------------- displayentity");
		Console.WriteLine("（FastBuilder）联大循环刷屏 ---------- sproom");
		Console.WriteLine("（FastBuilder）联大循环刷屏 ---------- spserver");
		Console.WriteLine("（ChatBot）刷卡密 -------------------- getkey");
		Console.WriteLine("（ChatBot）初始化 -------------------- initdummy");
		Console.WriteLine("租赁服人机填充 ----------------------- padding");
		Console.WriteLine("设置租赁服填充预留空位 --------------- setnullpadding");
		Console.WriteLine("联大人机填充（锁服） ----------------- paddinglobby");
		Console.WriteLine("设置标题 ----------------------------- title");
		Console.WriteLine("设置颜色 ----------------------------- color");
		Console.WriteLine("清屏 --------------------------------- cls");
		Console.WriteLine("清屏 --------------------------------- clear");
		Console.WriteLine("更换皮肤------------------------------ setskin");
	}

	public static void Command()
	{
		while (true)
		{
			Console.Write(Http.UID + ">> ");
			string text = Console.ReadLine();
			if (text == "version")
			{
                Console.WriteLine("#####################################################");
                Console.WriteLine("#                 挂福器 分支版 v1.4.1              #");
                Console.WriteLine("#####################################################");
                Console.WriteLine("本程序为闭源项目非官方分支版本");
                Console.WriteLine("原作者：NULL");
                Console.WriteLine("分支作者：ChaosJulien、WatchfulBand");
                Console.WriteLine("版本号：1.4.1");
                Console.WriteLine("更新内容：支持更换皮肤、删除部分敏感内容");
                Console.WriteLine("官方交流群：917598137");
                Console.WriteLine("分支作者交流群：816191635");
                Console.WriteLine("本工具完全免费，如果你是付费购买本工具，说明你被骗了！");
				Console.WriteLine("分支开发请向NULL授权使用");
                Console.WriteLine("仅供技术讨论，请勿用于非法用途！");
                Console.WriteLine("#####################################################");
            }
			if (text == "help")
			{
				Help();
			}
			if (text == "geth5token")
			{
				Console.WriteLine(CppGameM.GetLoginToken());
			}
			if (text == "updatetoken")
			{
				try
				{
					Http.LoginDToken = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(Http.UpdateAuthotp(Temp.url))["entity"].ToString())["token"].ToString();
					Http.LoginSRCToken = MclNetClient.GetDecryptToken(Http.LoginDToken);
					Console.WriteLine("token:" + Http.LoginSRCToken);
					Function.UpdateToken();
				}
				catch
				{
					Console.WriteLine("网络断开连接");
				}
			}
			if (text == "delete")
			{
				Http.Close();
				Http.istrue = false;
				isroom = false;
				if (chatConnection != null)
				{
					chatConnection.Close();
				}
				if (linkConnection != null)
				{
					linkConnection.Close();
				}
			}
			if (text == "anew")
			{
				do
				{
					Login_otp = LoginInfo.CookleChoose();
				}
				while (Login_otp == null);
				if (Login_otp == "o")
				{
					Command();
				}
				else
				{
					if (chatConnection != null)
					{
						chatConnection.Close();
					}
					if (linkConnection != null)
					{
						linkConnection.Close();
					}
					Http.Close();
					Console.Clear();
					Console.WriteLine("正在登录中，请稍等...");
					if (Temp.is_g79 || PE_Login)
					{
						Http.PE_Login(Login_otp);
					}
					else
					{
						Http.Login(Login_otp);
					}
					PE_Login = false;
					if (Temp.InitializeName)
					{
						Name();
					}
					Console.WriteLine("你好！冒险家：" + Function.GetName(player_info));
				}
			}
			if (text == "getname")
			{
				Http.GetName();
				Console.WriteLine(Function.GetName(player_info));
			}
			if (text == "setname")
			{
				Console.Write("输入你要设置的昵称：");
				Http.SetName(Console.ReadLine());
				Http.GetName();
				Function.GetName(player_info);
			}
			if (text == "ipstart")
			{
				if (Temp.path == "")
				{
					Console.WriteLine("你还没有设置游戏路径！");
				}
				if (Temp.item_ids.Count == 0)
				{
					Console.WriteLine("你没有添加itemid！");
				}
				else
				{
					try
					{
						string contents = Function.game();
						File.WriteAllText(Temp.path + "\\mc.cfg", contents);
						InvokeCmdAsync("start " + Temp.path + "\\Minecraft.Windows.exe");
					}
					catch
					{
						Console.WriteLine("路径错误，请检查路径是否存在！");
					}
				}
			}
			if (text == "authstart")
			{
				if (Temp.path == "")
				{
					Console.WriteLine("你还没有设置游戏路径！");
				}
				if (Temp.item_ids.Count == 0)
				{
					Console.WriteLine("你还没有添加itemid！");
				}
				if (Http.AuthServerUrl == null)
				{
					Console.WriteLine("你还没有启动认证服务器！");
				}
				if (Http.server_id == null)
				{
					Console.WriteLine("你还没有设置服务器id！");
				}
				else
				{
					try
					{
						string contents2 = Function.AUTHgame();
						File.WriteAllText(Temp.path + "\\mc.cfg", contents2);
						InvokeCmdAsync("start " + Temp.path + "\\Minecraft.Windows.exe");
					}
					catch
					{
						Console.WriteLine("路径错误，请检查路径是否存在！");
					}
				}
			}
			if (text == "start")
			{
				if (Temp.path == "")
				{
					Console.WriteLine("你还没有设置游戏路径！");
				}
				if (Temp.item_ids.Count == 0)
				{
					Console.WriteLine("你没有添加itemid！");
				}
				else
				{
					try
					{
						JObject jObject = JsonConvert.DeserializeObject<JObject>(Http.postAPIPC("", "/online-lobby-game-enter"));
						CppGameConfig cppGameConfig = new CppGameConfig();
						cppGameConfig.world_info.level_id = "114514";
						cppGameConfig.room_info.ip = jObject["entity"]["server_host"].ToString();
						cppGameConfig.room_info.port = (int)jObject["entity"]["server_port"];
						cppGameConfig.room_info.token = CppGameM.GetLoginToken();
						cppGameConfig.player_info.user_id = uint.Parse(Http.UID);
						cppGameConfig.player_info.user_name = Function.GetName(player_info);
						cppGameConfig.player_info.urs = Http.UID;
                        cppGameConfig.skin_info.skin = app.Temp.skinPath;
                        cppGameConfig.skin_info.md5 = app.Temp.skinMd5;
                        cppGameConfig.skin_info.skin_iid = app.Temp.skinIid;
                        cppGameConfig.misc.multiplayer_game_type = 1000;
						cppGameConfig.misc.auth_server_url = "https://g79authobt.nie.netease.com";
						cppGameConfig.web_server_url = "https://x19apigatewayobt.nie.netease.com";
						cppGameConfig.core_server_url = "https://x19obtcore.nie.netease.com:8443";
						cppGameConfig.world_info.name = "114514";
						cppGameConfig.room_info.item_ids = Temp.item_ids;
						string text2 = JsonConvert.SerializeObject(cppGameConfig, Formatting.Indented);
						if (Temp.cppconfig_encryption)
						{
							text2 = text2.Encrypt();
						}
						File.WriteAllText(Temp.path + "\\mc.cfg", text2);
						InvokeCmdAsync("start " + Temp.path + "\\Minecraft.Windows.exe");
					}
					catch
					{
						Console.WriteLine("路径错误，请检查路径是否存在！");
					}
				}
			}
			if (text == "searchroom")
			{
				Console.Write("输入房间信息：");
				string text3 = Console.ReadLine();
				Console.WriteLine(Regex.Unescape(Http.x19post("https://x19apigatewayobt.nie.netease.com", "/online-lobby-room/query/search-by-name-v2", "{\"keyword\":\"" + text3 + "\",\"length\":10,\"offset\":0,\"version\":\"" + Temp.Version + "\"}")));
			}
			if (text == "searchroomv2")
			{
				Console.Write("输入房间信息：");
				string text4 = Console.ReadLine();
				Console.WriteLine(Regex.Unescape(Http.x19post("https://x19mclobt.nie.netease.com", "/online-lobby-room/query/search-by-name-v2", "{\"keyword\":\"" + text4 + "\",\"length\":10,\"offset\":0,\"version\":\"" + Temp.Version + "\"}")));
			}
			if (text == "searchPE")
			{
				Console.Write("输入房间信息：");
				Http.SearchPE(Console.ReadLine());
			}
			if (text == "search")
			{
				Console.Write("输入房间信息：");
				Http.Search(Console.ReadLine());
			}
			if (text == "roomlistPE")
			{
				Console.Write("输入物品id：");
				Http.RoomListPE(Console.ReadLine());
			}
			if (text == "roomlist")
			{
				Console.Write("输入物品id：");
				Http.RoomList(Console.ReadLine());
			}
			if (text == "getroomlist")
			{
				Console.Write("输入物品id：");
				string text5 = Console.ReadLine();
				Http.postAPIPC("{\"res_id\":\"" + text5 + "\",\"version\":\"" + Temp.Version + "\",\"with_friend\":true,\"offset\":0,\"length\":10}", "/online-lobby-room/query/list-room-by-res-id");
			}
			if (text == "quick")
			{
				Console.Write("输入物品id：");
				string text6 = Console.ReadLine();
				JsonConvert.DeserializeObject<ListEntity>(Http.x19post("https://x19apigatewayobt.nie.netease.com", "/online-lobby-room/query/list-room-by-res-id", "{\"res_id\":\"" + text6 + "\",\"version\":\"" + Temp.Version + "\",\"with_friend\":true,\"offset\":0,\"length\":10}"));
			}
			if (text == "joinPE")
			{
				Console.Write("输入房间id：");
				Http.joined(Console.ReadLine());
			}
			if (text == "join")
			{
				Console.Write("输入房间id：");
				Function.JoinLobbyRoom(Console.ReadLine());
			}
			if (text == "ujoin")
			{
				Console.Write("输入房间id：");
				Function.UJoinLobbyRoom(Console.ReadLine());
			}
			if (text == "ivjoin")
			{
				Console.Write("输入房间id：");
				string text7 = Console.ReadLine();
				Http.postAPIPC("{\"room_id\":\"" + text7 + "\",\"password\":\"\",\"check_visibilily\":false}", "/online-lobby-room-enter");
			}
			if (text == "passwordjoin")
			{
				Console.Write("输入房间id：");
				string text8 = Console.ReadLine();
				Console.Write("输入房间密码：");
				string text9 = Console.ReadLine();
				Http.postAPIPC("{\"room_id\":\"" + text8 + "\",\"password\":\"" + text9 + "\",\"check_visibilily\":false}", "/online-lobby-room-enter");
			}
			if (text == "wstartroomupdate")
			{
				Console.Write("输入房间entity_id：");
				updateroom(Console.ReadLine());
			}
			if (text == "setroomid")
			{
				Console.Write("输入房间entity_id：");
				Http.room_id_public = Console.ReadLine();
			}
			if (text == "startroomupdate")
			{
				updateroom(Http.room_id_public);
			}
			if (text == "wleft")
			{
				Console.WriteLine("PublicRoomID:" + Http.room_id_public);
				Console.Write("输入房间id：");
				Http.left(Console.ReadLine());
				isroom = false;
			}
			if (text == "left")
			{
				Console.WriteLine("PublicRoomID:" + Http.room_id_public);
				Http.left(Http.room_id_public);
				isroom = false;
			}
			if (text == "getip")
			{
				Http.GetIP();
			}
			if (text == "purchasePE")
			{
				Console.Write("输入物品id：");
				Http.PurchaseItem(Console.ReadLine());
			}
			if (text == "purchase")
			{
				Console.Write("输入物品id：");
				Http.PurchaseItemPC(Console.ReadLine());
			}
			if (text == "command")
			{
				Console.Write("输入房间id：");
				string rid = Console.ReadLine();
				Console.Write("输入指令：");
				string command = Console.ReadLine();
				Http.aicommand(rid, command);
			}
			if (text == "nameupdate")
			{
				Console.Write("输入新的名字：");
				Http.NameUpdate(Console.ReadLine());
			}
			if (text == "searchitem")
			{
				Console.Write("输入物品id：");
				Http.SearchItem(Console.ReadLine());
			}
			if (text == "setpath")
			{
				Console.Write("输入windowsmc的路径：");
				Temp.path = Console.ReadLine();
				File.WriteAllText("config.json", JsonConvert.SerializeObject(Temp));
				Temp = JsonConvert.DeserializeObject<temp>(ReadJson("config.json"));
				Console.WriteLine("添加成功");
			}
			if (text == "additem")
			{
				Console.WriteLine("输入clear重置列表 置顶ID必须有效（4641943113293411081）");
				Console.Write("输入物品id：");
				string text10 = Console.ReadLine();
				if (text10 == "clear")
				{
					Temp.item_ids.Clear();
					File.WriteAllText("config.json", JsonConvert.SerializeObject(Temp));
					Console.WriteLine("重置成功");
				}
				else if (Temp.item_ids.Contains(text10))
				{
					Console.WriteLine("已添加过：" + text10);
				}
				else
				{
					Temp.item_ids.Add(text10);
					File.WriteAllText("config.json", JsonConvert.SerializeObject(Temp));
					Console.WriteLine("添加成功");
				}
			}
			if (text == "ritem")
			{
				Console.Write("输入物品id：");
				string item = Console.ReadLine();
				bool num = Temp.item_ids.Remove(item);
				File.WriteAllText("config.json", JsonConvert.SerializeObject(Temp));
				if (num)
				{
					Console.WriteLine("删除成功");
				}
			}
			if (text == "getgame")
			{
				Http.GetGame();
			}
			if (text == "unzip")
			{
				try
				{
					Console.Write("JsonPath:");
					UnzipModJson(Console.ReadLine());
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
			if (text == "unzippath")
			{
				Console.Write("Path:");
				Function.UnzipMCZIP(Console.ReadLine());
			}
			if (text == "Epost")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url = Console.ReadLine();
					Console.Write("POST:");
					string requests = Console.ReadLine();
					Http.EncyptPOST(url, requests, PostHost);
				}
				catch (Exception ex2)
				{
					Console.WriteLine(ex2.Message);
				}
			}
			if (text == "Epostfile")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url2 = Console.ReadLine();
					Console.Write("PATH:");
					string filePath = Console.ReadLine();
					Http.EncyptPOST(url2, ReadJson(filePath), PostHost);
				}
				catch (Exception ex3)
				{
					Console.WriteLine(ex3.Message);
				}
			}
			if (text == "testEpost")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url3 = Console.ReadLine();
					Console.Write("POST:");
					string requests2 = Console.ReadLine();
					Console.WriteLine("g79V3: 1");
					Console.WriteLine("x19V2: 2");
					Console.WriteLine("g79VC: 3");
					Console.Write("EncryptType:");
					switch (Console.ReadLine())
					{
					case "1":
						Http.EncyptPOST_g79v3(url3, requests2, PostHost);
						break;
					case "2":
						Http.EncyptPOST(url3, requests2, PostHost);
						break;
					case "3":
						Http.EncyptPOST_g79v12(url3, requests2, PostHost);
						break;
					default:
						Console.WriteLine("Value Error!");
						break;
					}
				}
				catch (Exception ex4)
				{
					Console.WriteLine(ex4.Message);
				}
			}
			if (text == "spost")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url4 = Console.ReadLine();
					Console.Write("POST:");
					string requests3 = Console.ReadLine();
					Console.Write("UID:");
					string uid = Console.ReadLine();
					Console.Write("TOKEN:");
					Http.postAPIHost(token: Console.ReadLine(), requests: requests3, url: url4, host: PostHost, uid: uid);
				}
				catch (Exception ex5)
				{
					Console.WriteLine(ex5.Message);
				}
			}
			if (text == "spostfile")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url5 = Console.ReadLine();
					Console.Write("PATH:");
					string filePath2 = Console.ReadLine();
					Console.Write("UID:");
					string uid2 = Console.ReadLine();
					Console.Write("TOKEN:");
					Http.postAPIHost(token: Console.ReadLine(), requests: ReadJson(filePath2), url: url5, host: PostHost, uid: uid2);
				}
				catch (Exception ex6)
				{
					Console.WriteLine(ex6.Message);
				}
			}
			if (text == "post")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url6 = Console.ReadLine();
					Console.Write("POST:");
					Http.postAPIHost(Console.ReadLine(), url6, PostHost);
				}
				catch (Exception ex7)
				{
					Console.WriteLine(ex7.Message);
				}
			}
			if (text == "postfile")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url7 = Console.ReadLine();
					Console.Write("PATH:");
					Http.postAPIHost(ReadJson(Console.ReadLine()), url7, PostHost);
				}
				catch (Exception ex8)
				{
					Console.WriteLine(ex8.Message);
				}
			}
			string array;
			if (text == "auth")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url8 = Console.ReadLine();
					Console.Write("POST:");
					string param = Console.ReadLine();
					Console.WriteLine(Http.HttpPost(url8, param, PostHost, out array));
				}
				catch (Exception ex9)
				{
					Console.WriteLine(ex9.Message);
				}
			}
			if (text == "authpath")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url9 = Console.ReadLine();
					Console.Write("PATH:");
					string filePath3 = Console.ReadLine();
					Console.WriteLine(Http.HttpPost(url9, ReadJson(filePath3), PostHost, out array));
				}
				catch (Exception ex10)
				{
					Console.WriteLine(ex10.Message);
				}
			}
			if (text == "peauth")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url10 = Console.ReadLine();
					Console.Write("POST:");
					string param2 = Console.ReadLine();
					Console.WriteLine(Http.HttpPost_(url10, param2, PostHost));
				}
				catch (Exception ex11)
				{
					Console.WriteLine(ex11.Message);
				}
			}
			if (text == "peauthpath")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url11 = Console.ReadLine();
					Console.Write("PATH:");
					string filePath4 = Console.ReadLine();
					Console.WriteLine(Http.HttpPost_(url11, ReadJson(filePath4), PostHost));
				}
				catch (Exception ex12)
				{
					Console.WriteLine(ex12.Message);
				}
			}
			if (text == "authv2")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url12 = Console.ReadLine();
					Console.Write("POST:");
					string param3 = Console.ReadLine();
					Console.WriteLine(Http.HttpPost_v2(url12, param3, PostHost, out array));
				}
				catch (Exception ex13)
				{
					Console.WriteLine(ex13.Message);
				}
			}
			if (text == "authpathv2")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url13 = Console.ReadLine();
					Console.Write("PATH:");
					string filePath5 = Console.ReadLine();
					Console.WriteLine(Http.HttpPost_v2(url13, ReadJson(filePath5), PostHost, out array));
				}
				catch (Exception ex14)
				{
					Console.WriteLine(ex14.Message);
				}
			}
			if (text == "peauthv2")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url14 = Console.ReadLine();
					Console.Write("POST:");
					string param4 = Console.ReadLine();
					Console.WriteLine(Http.HttpPost_v2_(url14, param4, PostHost));
				}
				catch (Exception ex15)
				{
					Console.WriteLine(ex15.Message);
				}
			}
			if (text == "peauthpathv2")
			{
				try
				{
					Console.WriteLine("default server:{0} It can be changed via \"setposthost\" command", PostHost);
					Console.Write("URL:");
					string url15 = Console.ReadLine();
					Console.Write("PATH:");
					string filePath6 = Console.ReadLine();
					Console.WriteLine(Http.HttpPost_v2_(url15, ReadJson(filePath6), PostHost));
				}
				catch (Exception ex16)
				{
					Console.WriteLine(ex16.Message);
				}
			}
			if (text == "setposthost")
			{
				Console.Write("PostHost:");
				PostHost = Console.ReadLine();
			}
			if (text == "ajoin")
			{
				if (sajoin)
				{
					Console.WriteLine("免责声明：假人仅供技术讨论，房间引流，请勿用于非法用途，锁服造成的一切后果均为使用者自行承担！使用请接受条款（y/n）");
					Console.Write("是否接受条款：");
					string text11 = Console.ReadLine();
					if (text11 == "Y" || text11 == "y")
					{
						if (!start_stop)
						{
							try
							{
								start_stop = true;
								Console.WriteLine("谨慎使用");
								Console.Write("输入房间号：");
								string text12 = Console.ReadLine();
								Http.istrue = false;
								string value = Http.postAPIPC("{\"room_name\":\"" + text12 + "\",\"res_id\":\"\",\"version\":\"" + Temp.Version + "\",\"offset\":0,\"length\":50}", "/online-lobby-room/query/search-by-name");
								Thread.Sleep(1000);
								JObject jObject2 = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(value)["entities"][0].ToString());
								Http.PurchaseItemPC(jObject2["res_id"].ToString());
								Ajoin(jObject2["entity_id"].ToString());
							}
							catch (Exception ex17)
							{
								Console.WriteLine(ex17.Message);
							}
						}
						else
						{
							Console.WriteLine("有任务正在执行！");
						}
					}
				}
				else
				{
					Console.WriteLine("该功能已被禁用！");
				}
			}
			if (text == "ajoinrid")
			{
				if (sajoin)
				{
					Console.WriteLine("免责声明：假人仅供技术讨论，房间引流，请勿用于非法用途，锁服造成的一切后果均为使用者自行承担！使用请接受条款（y/n）");
					Console.Write("是否接受条款：");
					string text13 = Console.ReadLine();
					if (text13 == "Y" || text13 == "y")
					{
						try
						{
							if (!start_stop)
							{
								start_stop = true;
								Console.WriteLine("谨慎使用");
								Console.Write("输入房间entity_id：");
								Ajoin(Console.ReadLine());
							}
							else
							{
								Console.WriteLine("有任务正在执行！");
							}
						}
						catch (Exception ex18)
						{
							Console.WriteLine(ex18.Message);
						}
					}
				}
				else
				{
					Console.WriteLine("该功能已被禁用！");
				}
			}
			if (text == "STOP")
			{
				start_stop = false;
			}
			if (text == "adduser")
			{
				LoginInfo.adduser();
			}
			if (text == "startr")
			{
				Console.Write("输入物品id：");
				string text14 = Console.ReadLine();
				Console.Write("输入最大人数：");
				string s = Console.ReadLine();
				try
				{
					SavePermissions = "true";
					Http.istrue = true;
					string value2 = Http.postAPIPC("{\"room_name\":\"" + Function.GetName(player_info) + "\",\"max_count\":" + int.Parse(s) + ",\"visibility\":0,\"res_id\":\"" + text14 + "\",\"save_id\":\"\",\"password\":\"\"}", "/online-lobby-room");
					Thread.Sleep(1000);
					Http.room_id_public = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(value2)["entity"].ToString())["entity_id"].ToString();
					updateroom(Http.room_id_public);
					Http.postAPIPC("{\"op\":\"start\"}", "/online-lobby-game-control");
				}
				catch (Exception ex19)
				{
					Console.WriteLine(ex19.Message);
				}
			}
			if (text == "startrPE")
			{
				Console.Write("输入物品id：");
				string text15 = Console.ReadLine();
				Console.Write("输入最大人数：");
				string s2 = Console.ReadLine();
				try
				{
					SavePermissions = "true";
					Http.istrue = true;
					string value3 = Http.postAPI("{\"room_name\":\"" + Function.GetName(player_info) + "\",\"max_count\":" + int.Parse(s2) + ",\"visibility\":0,\"res_id\":\"" + text15 + "\",\"save_id\":\"\",\"password\":\"\"}", "/online-lobby-room");
					Thread.Sleep(1000);
					Http.room_id_public = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(value3)["entity"].ToString())["entity_id"].ToString();
					updateroom(Http.room_id_public);
					Http.postAPIPC("{\"op\":\"start\"}", "/online-lobby-game-control");
				}
				catch (Exception ex20)
				{
					Console.WriteLine(ex20.Message);
				}
			}
			if (text == "visitorsave")
			{
				Console.Write("是否允许保存(y,n)？：");
				string text16 = Console.ReadLine();
				if (text16 == "y")
				{
					Http.postAPIPC("{\"visibility\":" + JoinPermissions + ",\"allow_save\":true,\"owner_id\":" + Http.UID + ",\"room_id\":\"" + Http.room_id_public + "\",\"entity_id\":null}", "/online-lobby-room/update");
					SavePermissions = "true";
				}
				else if (text16 == "n")
				{
					Http.postAPIPC("{\"visibility\":" + JoinPermissions + ",\"allow_save\":false,\"owner_id\":" + Http.UID + ",\"room_id\":\"" + Http.room_id_public + "\",\"entity_id\":null}", "/online-lobby-room/update");
					SavePermissions = "false";
				}
				else
				{
					Console.WriteLine("输入错误");
				}
			}
			if (text == "save")
			{
				Console.Write("输入保存名称：");
				string text17 = Console.ReadLine();
				Http.postAPIPC("{\"name\":\"" + text17 + "\",\"backup_id\":\"1\"}", "/online-lobby-backup/create");
			}
			if (text == "saveadd")
			{
				Console.Write("输入保存名称：");
				string text18 = Console.ReadLine();
				Console.Write("保存num：");
				string text19 = Console.ReadLine();
				Http.postAPIPC("{\"name\":\"" + text18 + "\",\"backup_id\":\"" + text19 + "\"}", "/online-lobby-backup/create");
			}
			if (text == "setpassword")
			{
				Console.Write("输入新的密码（留空为无密码）：");
				string text20 = Console.ReadLine();
				Http.postAPIPC("{\"visibility\":" + JoinPermissions + ",\"allow_save\":false,\"owner_id\":" + Http.UID + ",\"room_id\":\"" + Http.room_id_public + "\",\"entity_id\":null,\"password\":\"" + text20 + "\"}", "/online-lobby-room/update");
			}
			if (text == "kickall")
			{
				try
				{
					RoomUserList roomUserList = new RoomUserList();
					roomUserList = JsonConvert.DeserializeObject<RoomUserList>(Http.x19post("https://x19apigatewayobt.nie.netease.com", "/online-lobby-member/query/list-by-room-id", "{\"room_id\":\"" + Http.room_id_public + "\"}"));
					long[] array2 = new long[roomUserList.entities.Length];
					for (int i = 0; i < roomUserList.entities.Length; i++)
					{
						array2[i] = long.Parse(roomUserList.entities[i].member_id);
					}
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[j] != long.Parse(Http.UID) && array2[j] != long.Parse(Http.PEUID))
						{
							Http.KickAsync(array2[j].ToString());
						}
					}
				}
				catch (Exception ex21)
				{
					Console.WriteLine(ex21.ToString());
				}
			}
			if (text == "load")
			{
				try
				{
					SavePermissions = "true";
					Http.istrue = true;
					JObject jObject3 = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(Http.getAPIPC("/online-lobby-backup/query/list-by-user"))["entities"][0].ToString());
					string text21 = jObject3["res_id"].ToString();
					string text22 = jObject3["save_id"].ToString();
					Console.Write("输入最大人数：");
					string s3 = Console.ReadLine();
					Console.WriteLine("save_id:" + text22);
					Console.WriteLine("res_id:" + text21);
					string text23 = "{\"room_name\":\"" + Function.GetName(player_info) + "\",\"max_count\":" + int.Parse(s3) + ",\"visibility\":0,\"res_id\":\"" + text21 + "\",\"save_id\":\"" + text22 + "\",\"password\":\"\"}";
					Console.WriteLine(text23);
					Http.room_id_public = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(Http.postAPIPC(text23, "/online-lobby-room"))["entity"].ToString())["entity_id"].ToString();
					updateroom(Http.room_id_public);
					Http.postAPIPC("{\"op\":\"start\"}", "/online-lobby-game-control");
				}
				catch (Exception ex22)
				{
					Console.WriteLine(ex22.Message);
				}
			}
			if (text == "loadPE")
			{
				try
				{
					SavePermissions = "true";
					Http.istrue = true;
					JObject jObject4 = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(Http.getAPIPC("/online-lobby-backup/query/list-by-user"))["entities"][0].ToString());
					string text24 = jObject4["res_id"].ToString();
					string text25 = jObject4["save_id"].ToString();
					Console.Write("输入最大人数：");
					string s4 = Console.ReadLine();
					Console.WriteLine("save_id:" + text25);
					Console.WriteLine("res_id:" + text24);
					string text26 = "{\"room_name\":\"" + Function.GetName(player_info) + "\",\"max_count\":" + int.Parse(s4) + ",\"visibility\":0,\"res_id\":\"" + text24 + "\",\"save_id\":\"" + text25 + "\",\"password\":\"\"}";
					Console.WriteLine(text26);
					Http.room_id_public = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(Http.postAPI(text26, "/online-lobby-room"))["entity"].ToString())["entity_id"].ToString();
					updateroom(Http.room_id_public);
					Http.postAPIPC("{\"op\":\"start\"}", "/online-lobby-game-control");
				}
				catch (Exception ex23)
				{
					Console.WriteLine(ex23.Message);
				}
			}
			if (text == "loadadd")
			{
				try
				{
					SavePermissions = "true";
					Http.istrue = true;
					JObject jObject5 = JsonConvert.DeserializeObject<JObject>(Http.getAPIPC("/online-lobby-backup/query/list-by-user"));
					jObject5["entities"].ToString();
					Console.Write("输入序号（从零开始）：");
					int num2 = int.Parse(Console.ReadLine());
					JObject jObject6 = JsonConvert.DeserializeObject<JObject>(jObject5["entities"][num2].ToString());
					string text27 = jObject6["res_id"].ToString();
					string text28 = jObject6["save_id"].ToString();
					Console.Write("输入最大人数：");
					string s5 = Console.ReadLine();
					Console.WriteLine("save_id:" + text27);
					Console.WriteLine("res_id:" + text28);
					string text29 = "{\"room_name\":\"" + Function.GetName(player_info) + "\",\"max_count\":" + int.Parse(s5) + ",\"visibility\":0,\"res_id\":\"" + text27 + "\",\"save_id\":\"" + text28 + "\",\"password\":\"\"}";
					Console.WriteLine(text29);
					string value4 = Http.postAPIPC(text29, "/online-lobby-room");
					Http.istrue = true;
					Http.room_id_public = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(value4)["entity"].ToString())["entity_id"].ToString();
					updateroom(Http.room_id_public);
					Http.postAPIPC("{\"op\":\"start\"}", "/online-lobby-game-control");
				}
				catch (Exception ex24)
				{
					Console.WriteLine(ex24.Message);
				}
			}
			if (text == "kick")
			{
				Console.Write("输入玩家UID：");
				Http.KickAsync(Console.ReadLine());
			}
			if (text == "visibility")
			{
				try
				{
					Console.WriteLine("0为公开");
					Console.WriteLine("1为仅好友可进");
					Console.WriteLine("2为隐藏");
					Console.Write("可见性：");
					JoinPermissions = int.Parse(Console.ReadLine());
					Http.postAPIPC("{\"visibility\":" + JoinPermissions + ",\"allow_save\":" + SavePermissions + ",\"owner_id\":" + Http.UID + ",\"room_id\":\"" + Http.room_id_public + "\",\"entity_id\":null}", "/online-lobby-room/update");
				}
				catch (Exception ex25)
				{
					Console.WriteLine(ex25.Message);
				}
			}
			if (text == "reset")
			{
				Http.postAPIPC("{\"op\":\"restart\"}", "/online-lobby-game-control");
			}
			if (text == "setop")
			{
				Console.Write("输入玩家UID：");
				string text30 = Console.ReadLine();
				Http.postAPIPC("{\"visibility\":" + JoinPermissions + ",\"allow_save\":" + SavePermissions + ",\"owner_id\":" + text30 + ",\"room_id\":\"" + Http.room_id_public + "\",\"entity_id\":null}", "/online-lobby-room/update");
			}
			if (text == "getrtxgame")
			{
				Http.postAPIPC("{\"os\":\"10.0\",\"version\":200000000,\"entity_id\":null}", "/cpp-game-client-info");
			}
			if (text == "getallgame")
			{
				Http.postAPIPC("{\"setting_name\":\"pc_sdkclient_full_patchlist\"}", "/interconn/web/pack-setting/get-for-mcstudio");
			}
			if (text == "test")
			{
				Http.postAPIPC("{\"os\":\"10.0\",\"version\":300000000,\"entity_id\":null}", "/cpp-game-client-info");
			}
			if (text == "searchuser")
			{
				Console.Write("输入用户名：");
				string text31 = Console.ReadLine();
				Http.postAPIPC("{\"name_or_mail\":\"" + text31 + "\",\"mail_flag\":false}", "/user-search-friend");
			}
			if (text == "startban")
			{
				Console.Write("输入房间entityid：");
				start_stop = true;
				AKICK(Console.ReadLine());
				Console.WriteLine("已启动ban");
			}
			if (text == "ban")
			{
				Console.Write("输入玩家uid：");
				string text32 = Console.ReadLine();
				string[] str = Temp.ban_id.ToArray();
				if (Temp.ban_id.ToArray().Count() == 0 || checkplayer(str, text32))
				{
					Temp.ban_id.Add(text32);
					File.WriteAllText("config.json", JsonConvert.SerializeObject(Temp));
					Temp = JsonConvert.DeserializeObject<temp>(ReadJson("config.json"));
					Console.WriteLine("已添加:" + text32);
					Http.postAPIPC("{\"room_id\":\"" + Http.room_id_public + "\",\"user_id\":" + text32 + "}", "/online-lobby-member-kick");
				}
				else
				{
					Console.WriteLine("已添加过垓玩家");
				}
			}
			if (text == "unban")
			{
				Console.Write("输入玩家uid：");
				string item2 = Console.ReadLine();
				if (Temp.ban_id.Remove(item2))
				{
					Console.WriteLine("成功移除");
					File.WriteAllText("config.json", JsonConvert.SerializeObject(Temp));
				}
				else
				{
					Console.WriteLine("未找到该玩家");
				}
			}
			if (text == "playerlist")
			{
				try
				{
					RoomUserList roomUserList2 = new RoomUserList();
					roomUserList2 = JsonConvert.DeserializeObject<RoomUserList>(Http.x19post("https://x19apigatewayobt.nie.netease.com", "/online-lobby-member/query/list-by-room-id", "{\"room_id\":\"" + Http.room_id_public + "\"}"));
					uint[] array3 = new uint[roomUserList2.entities.Length];
					for (int k = 0; k < roomUserList2.entities.Length; k++)
					{
						array3[k] = uint.Parse(roomUserList2.entities[k].member_id);
					}
					RoomUserNameListRequest roomUserNameListRequest = new RoomUserNameListRequest();
					roomUserNameListRequest.entity_ids = array3;
					JObject jObject7 = JsonConvert.DeserializeObject<JObject>(Http.x19post("https://x19apigatewayobt.nie.netease.com", "/user/query/search-by-ids", JsonConvert.SerializeObject(roomUserNameListRequest)));
					for (int l = 0; l < roomUserList2.entities.Length; l++)
					{
						Console.WriteLine(jObject7["entities"][l]["entity_id"]?.ToString() + "：" + jObject7["entities"][l]["name"].ToString());
					}
				}
				catch
				{
				}
			}
			if (text == "wplayerlist")
			{
				Console.Write("输入房间entityid：");
				string text33 = Console.ReadLine();
				try
				{
					RoomUserList roomUserList3 = new RoomUserList();
					roomUserList3 = JsonConvert.DeserializeObject<RoomUserList>(Http.x19post("https://x19apigatewayobt.nie.netease.com", "/online-lobby-member/query/list-by-room-id", "{\"room_id\":\"" + text33 + "\"}"));
					uint[] array4 = new uint[roomUserList3.entities.Length];
					for (int m = 0; m < roomUserList3.entities.Length; m++)
					{
						array4[m] = uint.Parse(roomUserList3.entities[m].member_id);
					}
					RoomUserNameListRequest roomUserNameListRequest2 = new RoomUserNameListRequest();
					roomUserNameListRequest2.entity_ids = array4;
					JObject jObject8 = JsonConvert.DeserializeObject<JObject>(Http.x19post("https://x19apigatewayobt.nie.netease.com", "/user/query/search-by-ids", JsonConvert.SerializeObject(roomUserNameListRequest2)));
					for (int n = 0; n < roomUserList3.entities.Length; n++)
					{
						Console.WriteLine(jObject8["entities"][n]["entity_id"]?.ToString() + "：" + jObject8["entities"][n]["name"].ToString());
					}
				}
				catch
				{
				}
			}
			if (text == "clearbanid")
			{
				Temp.ban_id.Clear();
				File.WriteAllText("config.json", JsonConvert.SerializeObject(Temp));
				Temp = JsonConvert.DeserializeObject<temp>(ReadJson("config.json"));
				Console.WriteLine("已清除");
			}
			if (text == "gametype")
			{
				if (Temp.path == "")
				{
					Console.WriteLine("你还没有设置游戏路径！");
				}
				if (Temp.item_ids.Count == 0)
				{
					Console.WriteLine("你没有添加itemid！");
				}
				else
				{
					try
					{
						Console.Write("multiplayer_game_type:");
						int multiplayer_game_type = int.Parse(Console.ReadLine());
						Console.Write("IP:");
						string ip = Console.ReadLine();
						Console.Write("Port:");
						int port = int.Parse(Console.ReadLine());
						string contents3 = Function.game_(ip, port, multiplayer_game_type);
						File.WriteAllText(Temp.path + "\\mc.cfg", contents3);
						InvokeCmdAsync("start " + Temp.path + "\\Minecraft.Windows.exe");
					}
					catch
					{
						Console.WriteLine("路径错误，请检查路径是否存在！");
					}
				}
			}
			if (text == "joinlobby")
			{
				try
				{
					string value5 = Http.postAPIHost("{\"version\":\"" + Temp.Version + "\"}", "/enter-main-city", "https://g79mclobt.minecraft.cn");
					string item3 = JsonConvert.DeserializeObject<JObject>(value5)["entity"]["res_id"].ToString();
					string ip2 = JsonConvert.DeserializeObject<JObject>(value5)["entity"]["server_host"].ToString();
					string s6 = JsonConvert.DeserializeObject<JObject>(value5)["entity"]["server_port"].ToString();
					CppGameConfig cppGameConfig2 = new CppGameConfig();
					cppGameConfig2.world_info.level_id = "114514";
					cppGameConfig2.room_info.ip = ip2;
					cppGameConfig2.room_info.port = int.Parse(s6);
					cppGameConfig2.room_info.token = CppGameM.GetLoginToken();
					cppGameConfig2.player_info.user_id = uint.Parse(Http.UID);
					cppGameConfig2.player_info.user_name = Function.GetName(player_info);
					cppGameConfig2.player_info.urs = Http.UID;
                    cppGameConfig2.skin_info.skin = app.Temp.skinPath;
                    cppGameConfig2.skin_info.md5 = app.Temp.skinMd5;
                    cppGameConfig2.skin_info.skin_iid = app.Temp.skinIid;
                    cppGameConfig2.misc.multiplayer_game_type = 1000;
					cppGameConfig2.misc.auth_server_url = "https://g79authobt.nie.netease.com";
					cppGameConfig2.web_server_url = "https://x19apigatewayobt.nie.netease.com";
					cppGameConfig2.core_server_url = "https://x19obtcore.nie.netease.com:8443";
					cppGameConfig2.world_info.name = "114514";
					cppGameConfig2.room_info.item_ids.Add("4668698705152194374");
					cppGameConfig2.room_info.item_ids.Add(item3);
					string text34 = JsonConvert.SerializeObject(cppGameConfig2, Formatting.Indented);
					if (Temp.cppconfig_encryption)
					{
						text34 = text34.Encrypt();
					}
					Function.GameLeftLobbyAsync(text34);
				}
				catch
				{
				}
			}
			if (text == "portstart")
			{
				try
				{
					Console.Write("multiplayer_game_type:");
					int multiplayer_game_type2 = int.Parse(Console.ReadLine());
					Console.Write("IP:");
					string ip3 = Console.ReadLine();
					Console.Write("Port:");
					int port2 = int.Parse(Console.ReadLine());
					GameRpcPort = new RPCPort();
					CppGameConfig cppGameConfig3 = new CppGameConfig();
					cppGameConfig3.world_info.level_id = "114514";
					cppGameConfig3.room_info.ip = ip3;
					cppGameConfig3.room_info.port = port2;
					cppGameConfig3.room_info.token = CppGameM.GetLoginToken();
					cppGameConfig3.player_info.user_id = uint.Parse(Http.UID);
					cppGameConfig3.player_info.user_name = Function.GetName(player_info);
					cppGameConfig3.player_info.urs = Http.UID;
                    cppGameConfig3.skin_info.skin = app.Temp.skinPath;
                    cppGameConfig3.skin_info.md5 = app.Temp.skinMd5;
                    cppGameConfig3.skin_info.skin_iid = app.Temp.skinIid;
                    cppGameConfig3.misc.multiplayer_game_type = multiplayer_game_type2;
					cppGameConfig3.misc.auth_server_url = "https://g79authobt.nie.netease.com";
					cppGameConfig3.misc.launcher_port = GameRpcPort.LauncherControlPort;
					cppGameConfig3.web_server_url = "https://x19apigatewayobt.nie.netease.com";
					cppGameConfig3.core_server_url = "https://x19obtcore.nie.netease.com:8443";
					cppGameConfig3.world_info.name = "114514";
					cppGameConfig3.room_info.item_ids = Temp.item_ids;
					string text35 = JsonConvert.SerializeObject(cppGameConfig3, Formatting.Indented);
					if (Temp.cppconfig_encryption)
					{
						text35 = text35.Encrypt();
					}
					Function.GameAsync(text35, GameRpcPort);
				}
				catch
				{
				}
			}
			if (text == "senddata")
			{
				try
				{
					if (GameRpcPort.CheckConnect())
					{
						Console.Write("DATA:");
						GameRpcPort.SendControlDataSRC(Encoding.ASCII.GetBytes(Console.ReadLine()));
					}
					else
					{
						Console.WriteLine("游戏还未启动");
					}
				}
				catch
				{
					Console.WriteLine("游戏还未启动");
				}
			}
			if (text == "startconnect")
			{
				try
				{
					if (GameRpcPort == null)
					{
						Console.Write("Port:");
						GameRpcPort = new RPCPort(int.Parse(Console.ReadLine()));
						Console.WriteLine(GameRpcPort.LauncherControlPort);
					}
				}
				catch (Exception ex26)
				{
					Console.WriteLine(ex26.Message);
				}
			}
			if (text == "javaconnect")
			{
				try
				{
					if (GameRpcPort == null)
					{
						Console.Write("Port:");
						string s7 = Console.ReadLine();
						Console.Write("IP:");
						Function.IP = Console.ReadLine();
						Console.Write("Port:");
						Function.Port = Console.ReadLine();
						Console.Write("Name:");
						Function.Name = Console.ReadLine();
						GameRpcPort = new RPCPort(int.Parse(s7));
						GameRpcPort.RegisterReceiveCallBack(517, Function.StartCheck);
						Console.WriteLine(GameRpcPort.LauncherControlPort);
					}
				}
				catch (Exception ex27)
				{
					Console.WriteLine(ex27.Message);
				}
			}
			if (text == "closeconnect")
			{
				try
				{
					GameRpcPort.NormalExit();
					GameRpcPort = null;
					Console.WriteLine("close");
				}
				catch
				{
					Console.WriteLine("游戏还未启动");
				}
			}
			if (text == "closeroomupdate")
			{
				isroom = false;
				Console.WriteLine("已关闭自动更新");
			}
			if (text == "searchuid")
			{
				Console.Write("输入UID：");
				Http.postAPIPC("{\"user_id\":\"" + Console.ReadLine() + "\"}", "/user/query/search-by-uid");
			}
			if (text == "sendjava")
			{
				try
				{
					if (GameRpcPort.CheckConnect())
					{
						Console.Write("GUID:");
						string gUID = Console.ReadLine();
						Console.Write("WorldName:");
						string worldName = Console.ReadLine();
						JavaGameConfig javaGameConfig = new JavaGameConfig();
						javaGameConfig.GUID = gUID;
						javaGameConfig.WorldName = worldName;
						new JavaMCGameM().StartExistGame(javaGameConfig, isLanGame: false);
					}
					else
					{
						Console.WriteLine("游戏还未启动");
					}
				}
				catch (Exception ex28)
				{
					Console.WriteLine(ex28.Message);
				}
			}
			if (text == "sendjavav2")
			{
				try
				{
					if (GameRpcPort.CheckConnect())
					{
						Console.Write("GUID:");
						string gUID2 = Console.ReadLine();
						Console.Write("WorldName:");
						string worldName2 = Console.ReadLine();
						JavaGameConfig javaGameConfig2 = new JavaGameConfig();
						javaGameConfig2.GUID = gUID2;
						javaGameConfig2.WorldName = worldName2;
						javaGameConfig2.GameMode = GameMode.SURVIVAL;
						javaGameConfig2.AllowCheat = true;
						javaGameConfig2.GenerateBuildings = true;
						javaGameConfig2.BonusChest = false;
						javaGameConfig2.MapType = MapType.DEFAULT;
						javaGameConfig2.Seed = "";
						new JavaMCGameM().StartExistGame(javaGameConfig2, isLanGame: false);
					}
					else
					{
						Console.WriteLine("游戏还未启动");
					}
				}
				catch (Exception ex29)
				{
					Console.WriteLine(ex29.Message);
				}
			}
			if (text == "getdtoken")
			{
				Console.WriteLine(Http.LoginDToken);
			}
			if (text == "chachareset")
			{
				GameRpcPort.startChaChaEnc();
			}
			if (text == "getuid")
			{
				Console.WriteLine(Http.UID);
			}
			if (text == "gettoken")
			{
				Console.WriteLine(Http.LoginSRCToken);
			}
			if (text == "getiteminfo")
			{
				try
				{
					Console.Write("ITEM_ID:");
					sd.j(Console.ReadLine());
				}
				catch (Exception ex30)
				{
					Console.WriteLine(ex30.Message);
				}
			}
			if (text == "searchpemod")
			{
				Console.Write("输入模组名称：");
				Http.postAPIHost("{\"offset\":0,\"length\":20,\"keyword\":\"" + Console.ReadLine() + "\",\"init\":0,\"first_type\":\"0\",\"channel_id\":5}\r\n", "/pe-item/query/search-by-keyword/", "https://g79mclobtgray.nie.netease.com");
			}
			if (text == "searchmod")
			{
				Console.Write("输入模组名称：");
				Http.postAPIPC("{\"item_type\":2,\"keyword\":\"" + Console.ReadLine() + "\",\"master_type_id\":\"0\",\"secondary_type_id\":\"0\",\"sort_type\":1,\"order\":0,\"offset\":0,\"length\":24,\"is_has\":true,\"year\":0,\"is_sync\":0,\"price_type\":0}", "/item/query/search-by-keyword");
			}
			if (text == "getpeuid")
			{
				Console.WriteLine(Http.PEUID);
			}
			if (text == "getpemod")
			{
				Console.Write("ITEM_ID:");
				Http.postAPIHost("{\"item_ids\":[\"" + Console.ReadLine() + "\"]}", "/pe-item/query/search-lobby-by-id-list", "https://g79mclobtgray.nie.netease.com");
			}
			if (text == "getmod")
			{
				Console.Write("ITEM_ID:");
				Http.postAPIPC("{\"item_id\":\"" + Console.ReadLine() + "\",\"length\":0,\"offset\":0}", "/user-item-download-v2");
			}
			if (text == "getitemkey")
			{
				try
				{
					Console.Write("ITEM_ID:");
					string text36 = Console.ReadLine();
					string randomKey = Function.GetRandomKey(32);
					string text37 = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(Http.EncyptPOST("/pe-item/get-encryption-key-list-for-guests", "{\"device_id\":\"" + randomKey + "\",\"item_ids\":[\"" + text36 + "\"]}", "https://x19obtcore.nie.netease.com:8443"))["entities"][0].ToString())["jwt"].ToString().Remove(0, 37);
					string text38 = text37.Remove(text37.IndexOf('.'));
					if (text38.Length % 4 == 3)
					{
						text38 += "=";
					}
					if (text38.Length % 4 == 2)
					{
						text38 += "==";
					}
					string value6 = Encoding.ASCII.GetString(Convert.FromBase64String(text38));
					Console.WriteLine("KEY：" + Function.GetDecryptionKey(randomKey, JsonConvert.DeserializeObject<JObject>(value6)["contentKey"].ToString(), Http.UID));
					Console.WriteLine("UUID：" + JsonConvert.DeserializeObject<JObject>(value6)["contentUuid"].ToString(), Http.UID);
				}
				catch (Exception ex31)
				{
					Console.WriteLine(ex31.Message);
				}
			}
			if (text == "getpeitemkey")
			{
				try
				{
					Console.Write("ITEM_ID:");
					string text39 = Console.ReadLine();
					string randomKey2 = Function.GetRandomKey(32);
					string text40 = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(Http.EncyptPOST("/pe-item/get-encryption-key-list-for-guests", "{\"device_id\":\"" + randomKey2 + "\",\"item_ids\":[\"" + text39 + "\"]}", "https://x19obtcore.nie.netease.com:8443"))["entities"][0].ToString())["jwt"].ToString().Remove(0, 37);
					string text41 = text40.Remove(text40.IndexOf('.'));
					if (text41.Length % 4 == 3)
					{
						text41 += "=";
					}
					if (text41.Length % 4 == 2)
					{
						text41 += "==";
					}
					string value7 = Encoding.ASCII.GetString(Convert.FromBase64String(text41));
					Console.WriteLine("KEY：" + Function.GetDecryptionKey(randomKey2, JsonConvert.DeserializeObject<JObject>(value7)["contentKey"].ToString(), Http.UID));
					Console.WriteLine("UUID：" + JsonConvert.DeserializeObject<JObject>(value7)["contentUuid"].ToString(), Http.UID);
				}
				catch (Exception ex32)
				{
					Console.WriteLine(ex32.Message);
				}
			}
			if (text == "ununzip")
			{
				Console.Write("path:");
				string path = Console.ReadLine();
				Console.Write("key:");
				string key = Console.ReadLine();
				Console.Write("uuid:");
				string uuid = Console.ReadLine();
				Function.UnUnzipJson(key, path, uuid);
			}
			if (text == "ununzippath")
			{
				Console.Write("path:");
				string path2 = Console.ReadLine();
				Console.Write("key:");
				string key2 = Console.ReadLine();
				Console.Write("uuid:");
				string uuid2 = Console.ReadLine();
				Function.UnUnzipJsonPath(path2, key2, uuid2);
			}
			if (text == "unzipmodjsonpath")
			{
				Console.Write("path:");
				string path3 = Console.ReadLine();
				Console.Write("key:");
				string key3 = Console.ReadLine();
				Console.Write("uuid:");
				string uuid3 = Console.ReadLine();
				Function.UnzipModJsonMCZIP(path3, key3, uuid3);
			}
			if (text == "decryptpath")
			{
				Console.Write("path:");
				string path4 = Console.ReadLine();
				Console.Write("key:");
				string key4 = Console.ReadLine();
				Console.Write("uuid:");
				string uuid4 = Console.ReadLine();
				Function.UnzipModJsonPath(path4, key4, uuid4);
			}
			if (text == "startserver")
			{
				try
				{
					Console.Write("输入认证服务器端口：");
					string text42 = Console.ReadLine();
					Http.httpobj = new HttpListener();
					string text43 = "http://127.0.0.1:" + text42 + "/";
					Http.httpobj.Prefixes.Add(text43);
					Http.httpobj.Start();
					new Thread((ThreadStart)delegate
					{
						while (true)
						{
							Http.Result();
						}
					}).Start();
					Http.AuthServerUrl = text43;
					Console.WriteLine("服务端初始化完毕，正在等待客户端请求,时间：" + DateTime.Now.ToString() + "\r\n");
				}
				catch (Exception ex33)
				{
					Console.WriteLine("未经处理的异常：" + ex33.ToString());
				}
			}
			if (text == "closeencrypt")
			{
				Http.EncryptionAuthentication = false;
				Console.WriteLine("已关闭加密");
			}
			if (text == "startencrypt")
			{
				Http.EncryptionAuthentication = true;
				Console.WriteLine("已开启加密");
			}
			if (text == "startserverv2")
			{
				try
				{
					Console.Write("输入认证服务器端口：");
					string text44 = Console.ReadLine();
					Http.httpobj = new HttpListener();
					string text45 = "http://127.0.0.1:" + text44 + "/";
					Http.httpobj.Prefixes.Add(text45);
					Http.httpobj.Start();
					new Thread((ThreadStart)delegate
					{
						while (true)
						{
							Http.Result_v2();
						}
					}).Start();
					Http.AuthServerUrl = text45;
					Console.WriteLine("服务端初始化完毕，正在等待客户端请求,时间：" + DateTime.Now.ToString() + "\r\n");
				}
				catch (Exception ex34)
				{
					Console.WriteLine("未经处理的异常：" + ex34.ToString());
				}
			}
			if (text == "startserverv3")
			{
				try
				{
					Console.Write("输入认证服务器端口：");
					string text46 = Console.ReadLine();
					Http.httpobj = new HttpListener();
					string text47 = "http://127.0.0.1:" + text46 + "/";
					Http.httpobj.Prefixes.Add(text47);
					Http.httpobj.Start();
					new Thread((ThreadStart)delegate
					{
						while (true)
						{
							Http.Result_v3();
						}
					}).Start();
					Http.AuthServerUrl = text47;
					Console.WriteLine("服务端初始化完毕，正在等待客户端请求,时间：" + DateTime.Now.ToString() + "\r\n");
				}
				catch (Exception ex35)
				{
					Console.WriteLine("未经处理的异常：" + ex35.ToString());
				}
			}
			if (text == "serverid")
			{
				Console.Write("id:");
				Http.server_id = Console.ReadLine();
			}
			if (text == "serverip")
			{
				Console.Write("ip:");
				Http.server_ip = Console.ReadLine();
			}
			if (text == "cuccserver")
			{
				Console.WriteLine("默认服务器：https://g79.signal.servertransferser");
				Console.WriteLine(new StreamReader(((HttpWebResponse)((HttpWebRequest)WebRequest.Create("https://impression.update.netease.com/lighten/atlas_g79_hangzhou-cucc.txt")).GetResponse()).GetResponseStream(), Encoding.UTF8).ReadToEnd());
			}
			if (text == "ctccserver")
			{
				Console.WriteLine("默认服务器：https://g79.signal.servertransferser");
				Console.WriteLine(new StreamReader(((HttpWebResponse)((HttpWebRequest)WebRequest.Create("https://impression.update.netease.com/lighten/atlas_g79_hangzhou-ctcc.txt")).GetResponse()).GetResponseStream(), Encoding.UTF8).ReadToEnd());
			}
			if (text == "cmccserver")
			{
				Console.WriteLine("默认服务器：https://g79.signal.servertransferser");
				Console.WriteLine(new StreamReader(((HttpWebResponse)((HttpWebRequest)WebRequest.Create("https://impression.update.netease.com/lighten/atlas_g79_hangzhou-cmcc.txt")).GetResponse()).GetResponseStream(), Encoding.UTF8).ReadToEnd());
			}
			if (text == "allserver")
			{
				Console.WriteLine("默认服务器：https://g79.signal.servertransferser");
				Console.WriteLine(new StreamReader(((HttpWebResponse)((HttpWebRequest)WebRequest.Create("https://g79.update.netease.com/transferserver_obt_new.list")).GetResponse()).GetResponseStream(), Encoding.UTF8).ReadToEnd());
			}
			if (text == "mkick")
			{
				try
				{
					if (GameRpcPort.CheckConnect())
					{
						Console.Write("玩家uid:");
						string s8 = Console.ReadLine();
						GameRpcPort.SendControlData(SimplePack.Pack((ushort)1297, BitConverter.GetBytes(uint.Parse(s8))));
					}
					else
					{
						Console.WriteLine("游戏还未启动");
					}
				}
				catch
				{
					Console.WriteLine("游戏还未启动");
				}
			}
			if (text == "startroom")
			{
				if (Temp.path == "")
				{
					Console.WriteLine("你还没有设置游戏路径！");
				}
				else
				{
					try
					{
						string contents4 = Function.game();
						File.WriteAllText(Temp.path + "\\mc.cfg", contents4);
						InvokeCmdAsync("start " + Temp.path + "\\Minecraft.Windows.exe");
					}
					catch
					{
						Console.WriteLine("路径错误，请检查路径是否存在！");
					}
				}
			}
			if (text == "traceUID")
			{
				try
				{
					if (!start_stop)
					{
						Console.Write("输入uid：");
						Function.TraceUser(Console.ReadLine());
					}
				}
				catch (Exception ex36)
				{
					Console.WriteLine(ex36.ToString());
				}
			}
			if (text == "startlinkconnect" && (!LinkConnection.Is_Connected || linkConnection == null))
			{
				linkConnection = new LinkConnection();
			}
			if (text == "closelinkconnect")
			{
				linkConnection.Close();
				Console.WriteLine("已关闭link");
			}
			if (text == "linkserver")
			{
				Console.WriteLine(new StreamReader(((HttpWebResponse)((HttpWebRequest)WebRequest.Create("https://g79.update.netease.com/linkserver_obt.list")).GetResponse()).GetResponseStream(), Encoding.UTF8).ReadToEnd());
			}
			if (text == "linkserverexpr")
			{
				Console.WriteLine(new StreamReader(((HttpWebResponse)((HttpWebRequest)WebRequest.Create("https://x19.update.netease.com/linkserver_expr.list")).GetResponse()).GetResponseStream(), Encoding.UTF8).ReadToEnd());
			}
			if (text == "newchacha")
			{
				try
				{
					Console.Write("输入key：");
					ChaCha8 chaCha = new ChaCha8(Console.ReadLine().HexToBytes());
					Console.Write("输入data：");
					byte[] array5 = Console.ReadLine().HexToBytes();
					chaCha.Process(array5);
					chaCha.Delete();
					Console.WriteLine(array5.ToHex());
				}
				catch
				{
				}
			}
			if (text == "newchacha_")
			{
				Console.Write("输入key：");
				string hex = Console.ReadLine();
				ChaCha8 chaCha2 = new ChaCha8(hex.HexToBytes());
				try
				{
					Console.WriteLine("输入left退出");
					while (true)
					{
						Console.Write("输入data：");
						hex = Console.ReadLine();
						if (hex == "left")
						{
							break;
						}
						byte[] array6 = hex.HexToBytes();
						chaCha2.Process(array6);
						Console.WriteLine(array6.ToHex());
					}
					chaCha2.Delete();
				}
				catch
				{
					chaCha2.Delete();
				}
			}
			if (text == "newchachafile")
			{
				try
				{
					Console.Write("输入key：");
					ChaCha8 chaCha3 = new ChaCha8(Console.ReadLine().HexToBytes());
					Console.Write("输入path：");
					byte[] array7 = ReadJson(Console.ReadLine()).HexToBytes();
					chaCha3.Process(array7);
					chaCha3.Delete();
					Console.WriteLine(array7.ToHex());
				}
				catch
				{
				}
			}
			if (text == "newchachafile_")
			{
				Console.Write("输入key：");
				string hex2 = Console.ReadLine();
				ChaCha8 chaCha4 = new ChaCha8(hex2.HexToBytes());
				try
				{
					bool flag = true;
					bool flag2 = true;
					bool flag3 = true;
					Console.WriteLine("输入hex切换输出模式");
					Console.WriteLine("输入zlib解密加解压");
					Console.WriteLine("输入left退出");
					while (true)
					{
						Console.Write("输入path：");
						hex2 = Console.ReadLine();
						switch (hex2)
						{
						case "hex":
							flag3 = false;
							flag = !flag;
							Console.WriteLine(flag);
							break;
						case "zlib":
							flag3 = false;
							flag2 = !flag2;
							Console.WriteLine(flag2);
							break;
						case "fast":
						{
							flag3 = false;
							Console.Write("$输入path：");
							hex2 = Console.ReadLine();
							byte[] array8 = ReadJson(hex2).HexToBytes();
							int num4;
							int size;
							for (int num3 = 0; array8.Length >= num3 + 2; num3 += num4 + size)
							{
								num4 = LinkConnection.BytesToVarInt(array8.Skip(num3).ToArray(), out size);
								if (array8.Length < num3 + num4 + size)
								{
									break;
								}
								byte[] array9 = new byte[num4];
								Array.Copy(array8, num3 + size, array9, 0, num4);
								chaCha4.Process(array9);
								if (flag)
								{
									if (!flag2)
									{
										Console.WriteLine(ZlibUtils.MicrosoftDecompress(array9).ToHex());
									}
									else
									{
										Console.WriteLine(array9.ToHex());
									}
								}
								else if (!flag2)
								{
									Console.WriteLine(Encoding.UTF8.GetString(ZlibUtils.MicrosoftZlibDecompress(array9)));
								}
								else
								{
									Console.WriteLine(Encoding.UTF8.GetString(array9));
								}
							}
							break;
						}
						default:
							flag3 = true;
							break;
						}
						if (!flag3)
						{
							continue;
						}
						byte[] array10 = ReadJson(hex2).HexToBytes();
						chaCha4.Process(array10);
						if (flag)
						{
							if (!flag2)
							{
								Console.WriteLine(ZlibUtils.MicrosoftDecompress(array10).ToHex());
							}
							else
							{
								Console.WriteLine(array10.ToHex());
							}
						}
						else if (!flag2)
						{
							Console.WriteLine(Encoding.UTF8.GetString(ZlibUtils.MicrosoftZlibDecompress(array10)));
						}
						else
						{
							Console.WriteLine(Encoding.UTF8.GetString(array10));
						}
					}
				}
				catch (Exception ex37)
				{
					Console.WriteLine(ex37.ToString());
				}
			}
			if (text == "fast")
			{
				try
				{
					Console.Write("输入物品id：");
					string text48 = Console.ReadLine();
					Http.room_id_public = JsonConvert.DeserializeObject<JObject>(Http.x19post("https://g79mclobt.minecraft.cn", "/online-lobby-room-match", "{\"version\": \"" + Temp.Version + "\", \"res_id\": \"" + text48 + "\", \"lobby_manifest_version\": \"\"}"))["entity"]["room_id"].ToString();
					Http.GetIP();
				}
				catch
				{
				}
			}
			if (text == "getsign")
			{
				Console.Write("输入message：");
				Console.WriteLine(Function.Base64Encode(CoreSign.PESignCount(Console.ReadLine())));
			}
			if (text == "startchatconnect" && !ChatConnection.Is_Connected)
			{
				chatConnection = new ChatConnection();
			}
			if (text == "closechatconnect")
			{
				chatConnection.Close();
			}
			if (text == "senduid" && ChatConnection.Is_Connected)
			{
				Console.Write("uid:");
				string text49 = Console.ReadLine();
				Console.Write("message:");
				string text50 = Console.ReadLine();
				string text51 = "[" + text49 + ",\"" + text50 + "\"]";
				Console.WriteLine(text51);
				byte[] bytes = Encoding.UTF8.GetBytes(text51);
				byte[] array11 = new byte[bytes.Length + 2];
				Array.Copy(BitConverter.GetBytes((ushort)1281), 0, array11, 0, 2);
				Array.Copy(bytes, 0, array11, 2, bytes.Length);
				chatConnection.Encryption.Process(array11);
				byte[] array12 = new byte[array11.Length + 2];
				Array.Copy(BitConverter.GetBytes((ushort)array11.Length), 0, array12, 0, 2);
				Array.Copy(array11, 0, array12, 2, array11.Length);
				byte[] array13 = new byte[array12.Length + 2];
				Array.Copy(BitConverter.GetBytes((ushort)array12.Length), 0, array13, 0, 2);
				Array.Copy(array12, 0, array13, 2, array12.Length);
				chatConnection.networkStream.Write(array13, 0, array13.Length);
			}
			if (text == "sendlink")
			{
				try
				{
					Console.Write("message_hex:");
					string hex3 = Console.ReadLine();
					linkConnection.SendMsg(hex3.HexToBytes());
				}
				catch
				{
				}
			}
			if (text == "sendlinkfile")
			{
				try
				{
					Console.Write("message_path:");
					string filePath7 = Console.ReadLine();
					linkConnection.SendMsg(ReadJson(filePath7).HexToBytes());
				}
				catch
				{
				}
			}
			if (text == "userlist")
			{
				try
				{
					Console.Write("输入文件夹路径：");
					string path5 = Console.ReadLine();
					DummyUserList dummyUserList = new DummyUserList();
					dummyUserList.UserList = Function.GetAllFiles(path5, "");
					Function.SaveFile("DummyUserList.json", JsonConvert.SerializeObject(dummyUserList));
					Console.WriteLine("已保存文件：DummyUserList.json");
				}
				catch
				{
				}
			}
			if (text == "asenduid" && (ChatConnection.Is_Connected || start_stop))
			{
				Console.Write("uid:");
				string uid3 = Console.ReadLine();
				Console.Write("message:");
				string message = Console.ReadLine();
				start_stop = true;
				new Thread((ThreadStart)delegate
				{
					while (start_stop)
					{
						byte[] bytes3 = Encoding.UTF8.GetBytes("[" + uid3 + ",\"" + message + "\"]");
						byte[] array19 = new byte[bytes3.Length + 2];
						Array.Copy(BitConverter.GetBytes((ushort)1281), 0, array19, 0, 2);
						Array.Copy(bytes3, 0, array19, 2, bytes3.Length);
						chatConnection.Encryption.Process(array19);
						byte[] array20 = new byte[array19.Length + 2];
						Array.Copy(BitConverter.GetBytes((ushort)array19.Length), 0, array20, 0, 2);
						Array.Copy(array19, 0, array20, 2, array19.Length);
						byte[] array21 = new byte[array20.Length + 2];
						Array.Copy(BitConverter.GetBytes((ushort)array20.Length), 0, array21, 0, 2);
						Array.Copy(array20, 0, array21, 2, array20.Length);
						chatConnection.networkStream.Write(array21, 0, array21.Length);
						Thread.Sleep(2000);
					}
				}).Start();
			}
			if (text == "senduidpath" && ChatConnection.Is_Connected)
			{
				Console.Write("uid:");
				string text52 = Console.ReadLine();
				Console.Write("path:");
				string text53 = ReadJson(Console.ReadLine());
				string text54 = "[" + text52 + ",\"" + text53 + "\"]";
				Console.WriteLine(text54);
				byte[] bytes2 = Encoding.UTF8.GetBytes(text54);
				byte[] array14 = new byte[bytes2.Length + 2];
				Array.Copy(BitConverter.GetBytes((ushort)1281), 0, array14, 0, 2);
				Array.Copy(bytes2, 0, array14, 2, bytes2.Length);
				chatConnection.Encryption.Process(array14);
				byte[] array15 = new byte[array14.Length + 2];
				Array.Copy(BitConverter.GetBytes((ushort)array14.Length), 0, array15, 0, 2);
				Array.Copy(array14, 0, array15, 2, array14.Length);
				byte[] array16 = new byte[array15.Length + 2];
				Array.Copy(BitConverter.GetBytes((ushort)array15.Length), 0, array16, 0, 2);
				Array.Copy(array15, 0, array16, 2, array15.Length);
				chatConnection.networkStream.Write(array16, 0, array16.Length);
			}
			if (text == "asenduidpath" && (ChatConnection.Is_Connected || start_stop))
			{
				Console.Write("uid:");
				string uid4 = Console.ReadLine();
				Console.Write("path:");
				string message2 = ReadJson(Console.ReadLine());
				start_stop = true;
				new Thread((ThreadStart)delegate
				{
					while (start_stop)
					{
						byte[] bytes3 = Encoding.UTF8.GetBytes("[" + uid4 + ",\"" + message2 + "\"]");
						byte[] array19 = new byte[bytes3.Length + 2];
						Array.Copy(BitConverter.GetBytes((ushort)1281), 0, array19, 0, 2);
						Array.Copy(bytes3, 0, array19, 2, bytes3.Length);
						chatConnection.Encryption.Process(array19);
						byte[] array20 = new byte[array19.Length + 2];
						Array.Copy(BitConverter.GetBytes((ushort)array19.Length), 0, array20, 0, 2);
						Array.Copy(array19, 0, array20, 2, array19.Length);
						byte[] array21 = new byte[array20.Length + 2];
						Array.Copy(BitConverter.GetBytes((ushort)array20.Length), 0, array21, 0, 2);
						Array.Copy(array20, 0, array21, 2, array20.Length);
						chatConnection.networkStream.Write(array21, 0, array21.Length);
						Thread.Sleep(2000);
					}
				}).Start();
			}
			if (text == "addfriend")
			{
				Console.Write("uid:");
				string text55 = Console.ReadLine();
				Http.postAPIPC("{\"fid\":" + text55 + ",\"comment\":\"" + displayName + "\",\"message\":\"\"}", "/user-apply-friend");
			}
			if (text == "recvfriend")
			{
				Console.Write("uid:");
				string text56 = Console.ReadLine();
				Http.postAPIPC("{\"fid\":" + text56 + ",\"accept\":true}", "/user-reply-friend");
			}
			if (text == "initdummy")
			{
				DummySrv.Init();
			}
			if (text == "addsrvuser" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				try
				{
					SrvDummyUserListsEntity srvDummyUserListsEntity = new SrvDummyUserListsEntity();
					Console.Write("输入uid：");
					srvDummyUserListsEntity.uid = Console.ReadLine();
					Console.Write("输入到期时间：");
					srvDummyUserListsEntity.overdue_time = int.Parse(Console.ReadLine());
					Console.Write("输入最大任务数量：");
					srvDummyUserListsEntity.max_num = int.Parse(Console.ReadLine());
					if (!DummySrv.dummyUsers.user_lists.Contains(srvDummyUserListsEntity))
					{
						DummySrv.dummyUsers.user_lists.Add(srvDummyUserListsEntity);
						Function.SaveFile("user_srv_dummy_list.json", JsonConvert.SerializeObject(DummySrv.dummyUsers));
						Console.WriteLine("已保存文件：user_srv_dummy_list.json");
					}
					else
					{
						Console.WriteLine("已添加过用户！");
					}
				}
				catch (Exception ex38)
				{
					Console.WriteLine(ex38.ToString());
				}
			}
			if (text == "deleteuser" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				try
				{
					Console.Write("输入uid：");
					string text57 = Console.ReadLine();
					for (int num5 = 0; DummySrv.dummyUsers.user_lists.Count > num5; num5++)
					{
						if (DummySrv.dummyUsers.user_lists[num5].uid == text57)
						{
							DummySrv.dummyUsers.user_lists.RemoveAt(num5);
							Function.SaveFile("user_srv_dummy_list.json", JsonConvert.SerializeObject(DummySrv.dummyUsers));
							Console.WriteLine("已保存文件：user_srv_dummy_list.json");
						}
					}
				}
				catch (Exception ex39)
				{
					Console.WriteLine(ex39.ToString());
				}
			}
			if (text == "setusertime" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				try
				{
					Console.Write("输入uid：");
					string text58 = Console.ReadLine();
					Console.Write("输入新的时间：");
					string s9 = Console.ReadLine();
					for (int num6 = 0; DummySrv.dummyUsers.user_lists.Count > num6; num6++)
					{
						if (DummySrv.dummyUsers.user_lists[num6].uid == text58)
						{
							if (DummySrv.dummyUsers.user_lists[num6].overdue_time + 86400 > int.Parse(Function.GetTimeStamp()))
							{
								DummySrv.dummyUsers.user_lists[num6].overdue_time = int.Parse(s9);
								Function.SaveFile("user_srv_dummy_list.json", JsonConvert.SerializeObject(DummySrv.dummyUsers));
								Console.WriteLine("已保存文件：user_srv_dummy_list.json");
							}
							else
							{
								DummySrv.dummyUsers.user_lists[num6].overdue_time = int.Parse(s9);
								DummySrv.dummyUsers.user_lists[num6].max_num = 1;
								Function.SaveFile("user_srv_dummy_list.json", JsonConvert.SerializeObject(DummySrv.dummyUsers));
								Console.WriteLine("续费时间超过到期时间1天，已自动清除卡槽");
								Console.WriteLine("已保存文件：user_srv_dummy_list.json");
							}
						}
					}
				}
				catch (Exception ex40)
				{
					Console.WriteLine(ex40.ToString());
				}
			}
			if (text == "setusertask" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				try
				{
					Console.Write("输入uid：");
					string text59 = Console.ReadLine();
					Console.Write("输入最大任务数量：");
					string s10 = Console.ReadLine();
					for (int num7 = 0; DummySrv.dummyUsers.user_lists.Count > num7; num7++)
					{
						if (DummySrv.dummyUsers.user_lists[num7].uid == text59)
						{
							DummySrv.dummyUsers.user_lists[num7].max_num = int.Parse(s10);
							Function.SaveFile("user_srv_dummy_list.json", JsonConvert.SerializeObject(DummySrv.dummyUsers));
							Console.WriteLine("已保存文件：user_srv_dummy_list.json");
						}
					}
				}
				catch (Exception ex41)
				{
					Console.WriteLine(ex41.ToString());
				}
			}
			if (text == "uidtimelist" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				try
				{
					for (int num8 = 0; DummySrv.dummyUsers.user_lists.Count > num8; num8++)
					{
						Console.WriteLine(DummySrv.dummyUsers.user_lists[num8].uid + " time:" + DummySrv.dummyUsers.user_lists[num8].overdue_time + " MaxTask:" + DummySrv.dummyUsers.user_lists[num8].max_num);
					}
				}
				catch (Exception ex42)
				{
					Console.WriteLine(ex42.ToString());
				}
			}
			if (text == "kickplayer")
			{
				try
				{
					Console.Write("输入uid：");
					string text60 = Console.ReadLine();
					int dummy_level = DummySrv.BotConfig.dummy_level;
					Random random = new Random();
					DummyUserList dummyUserList2 = new DummyUserList();
					RecvMessageEntity recvMessageEntity = new RecvMessageEntity();
					string[] array17 = new string[dummy_level];
					for (int num9 = 0; num9 < dummy_level; num9++)
					{
						string item4 = (array17[num9] = DummySrv.strings[random.Next(0, DummySrv.strings.Count())]);
						DummySrv.strings.Remove(item4);
					}
					dummyUserList2.UserList = array17;
					DummySrv.dummyClients.Add(DummySrv.StartDummy(text60, recvMessageEntity, dummyUserList2, "发起用户：" + Http.UID + "---目标用户：" + text60 + "---时间：9999分钟", 9999));
				}
				catch
				{
				}
			}
			if (text == "javagetname")
			{
				Console.Write("输入gameid：");
				string text61 = Console.ReadLine();
				Http.postAPIPC("\"offset\":0,\"length\":10,\"user_id\":\"" + Http.UID + "\",\"game_id\":\"" + text61 + "\",\"game_type\":2}", "/game-character/query/user-game-characters");
			}
			if (text == "javaddname")
			{
				Console.Write("输入gameid：");
				string text62 = Console.ReadLine();
				Console.Write("输入游戏昵称：");
				string text63 = Console.ReadLine();
				Http.postAPIPC("{\"entity_id\":\"0\",\"game_id\":\"" + text62 + "\",\"game_type\":2,\"user_id\":\"" + Http.UID + "\",\"name\":\"" + text63 + "\",\"create_time\":555555}", "/game-character");
			}
			if (text == "getuuid")
			{
				Console.Write("输入游戏昵称：");
				Console.WriteLine(JavaLauncher.GenerateUUID(Console.ReadLine()));
			}
			if (text == "reloadconfig")
			{
				try
				{
					Temp = JsonConvert.DeserializeObject<temp>(ReadJson("config.json"));
				}
				catch (Exception ex43)
				{
					Console.WriteLine(ex43.ToString());
				}
			}
			if (text == "lockserver" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				Console.Write("输入房间号：");
				string name = Console.ReadLine();
				Random random2 = new Random();
				string[] texts = new string[10];
				for (int num10 = 0; num10 < 10; num10++)
				{
					string text64 = DummySrv.strings[random2.Next(0, DummySrv.strings.Count())];
					texts[num10] = text64;
					DummySrv.strings.Remove(text64);
				}
				new Thread((ThreadStart)delegate
				{
					new LockServer(texts, name);
				}).Start();
			}
			if (text == "lockserverv2")
			{
				Console.Write("输入ip：");
				string name2 = Console.ReadLine();
				Console.Write("输入端口：");
				string port3 = Console.ReadLine();
				new Thread((ThreadStart)delegate
				{
					new LockServer(name2, port3);
				}).Start();
			}
			if (text == "searchserver")
			{
				Console.Write("服务器号：");
				string text65 = Console.ReadLine();
				Console.WriteLine(Http.x19post("https://g79mclobt.minecraft.cn", "/rental-server/query/search-by-name", "{\"server_name\": \"" + text65 + "\", \"offset\": 0}"));
			}
			if (text == "getserverip")
			{
				Console.Write("服务器号：");
				string text66 = Console.ReadLine();
				Console.Write("服务器密码：");
				string text67 = Console.ReadLine();
				Console.WriteLine(Http.x19post("https://g79mclobt.minecraft.cn", "/rental-server-world-enter/get", "{\"server_id\": \"" + text66 + "\", \"pwd\": \"" + text67 + "\"}"));
			}
			if (text == "lockserverv3" && !start_stop)
			{
				Console.Write("输入ip：");
				string ip4 = Console.ReadLine();
				Console.Write("输入端口：");
				string port4 = Console.ReadLine();
				Console.Write("输入服务器号：");
				string sid = Console.ReadLine();
				new LockServer(ip4, port4, sid);
			}
			if (text == "lockserverv4" && !start_stop)
			{
				Console.Write("输入ip：");
				string ip5 = Console.ReadLine();
				Console.Write("输入端口：");
				string port5 = Console.ReadLine();
				Console.Write("输入刷屏文件：");
				string spfile = Console.ReadLine();
				new LockServer.SPLock(ip5, port5, spfile);
			}
			if (text == "lockserverv6" && !start_stop)
			{
				try
				{
					Console.Write("输入服务器号：");
					string text68 = Console.ReadLine();
					new LockServer(JsonConvert.DeserializeObject<JObject>(Http.x19post("https://g79mclobt.minecraft.cn", "/rental-server/query/search-by-name", "{\"server_name\": \"" + text68 + "\", \"offset\": 0}"))["entities"][0]["entity_id"].ToString());
				}
				catch (Exception ex44)
				{
					Console.WriteLine(ex44.ToString());
				}
			}
			if (text == "lockserverv5" && !start_stop)
			{
				Console.Write("输入ip：");
				string ip6 = Console.ReadLine();
				Console.Write("输入端口：");
				string port6 = Console.ReadLine();
				new LockServer.SPLockLine(ip6, port6);
			}
			if (text == "setauthserver")
			{
				Console.Write("输入认证服：");
				LockServer.AuthServerUrl = Console.ReadLine();
			}
			if (text == "setwebserver")
			{
				Console.Write("输入认证服：");
				LockServer.WebServerUrl = Console.ReadLine();
			}
			if (text == "displaywindow")
			{
				Console.Write("y/n:");
				string text69 = Console.ReadLine();
				if ((text69 == "y") | (text69 == "Y"))
				{
					FLockServer.DisplayWindow = true;
				}
				else if ((text69 == "n") | (text69 == "N"))
				{
					FLockServer.DisplayWindow = false;
				}
				else
				{
					Console.WriteLine("输入错误");
				}
			}
			if (text == "getkey")
			{
				Console.Write("输入文件名：");
				string filename = Console.ReadLine();
				Console.Write("输入数量：");
				string s11 = Console.ReadLine();
				Console.Write("输入时间：");
				string s12 = Console.ReadLine();
				Console.Write("输入初始向量：");
				string s13 = Console.ReadLine();
				DummySrv.Key key5 = new DummySrv.Key();
				int num11 = int.Parse(s11);
				int num12 = int.Parse(s13);
				int.Parse(s12);
				string text70 = string.Empty;
				string empty = string.Empty;
				for (int num13 = 0; num13 < num11; num13++)
				{
					key5.time = uint.Parse(s12);
					key5.id = (uint)(num12 + num13);
					empty = JsonConvert.SerializeObject(key5);
					text70 = text70 + x19Crypt.HttpEncrypt_g79v12(Encoding.UTF8.GetBytes(empty)).ToHex() + "\n";
				}
				Function.SaveFile(filename, text70);
				Console.WriteLine("文件已保存");
			}
			if (text == "fastbjd")
			{
				if (Http.g79)
				{
					if (!Http.IsAuthServer)
					{
						Http.Authserver(11451);
					}
					Http.server_id = "4638866978460665031:NetworkGame";
					string contents5 = Function.FastAUTHgame("play.bjd-mc.com", 19132);
					File.WriteAllText(Temp.path + "\\mc.cfg", contents5);
					InvokeCmdAsync("start " + Temp.path + "\\Minecraft.Windows.exe");
				}
				else
				{
					Console.WriteLine("请pe登录后再使用该功能！");
				}
			}
			if (text == "fasthyt")
			{
				if (Http.g79)
				{
					if (!Http.IsAuthServer)
					{
						Http.Authserver(11451);
					}
					Http.server_id = "4619956946865224702:NetworkGame";
					string contents6 = Function.FastAUTHgame("42.186.61.198", 19132);
					File.WriteAllText(Temp.path + "\\mc.cfg", contents6);
					InvokeCmdAsync("start " + Temp.path + "\\Minecraft.Windows.exe");
				}
				else
				{
					Console.WriteLine("请pe登录后再使用该功能！");
				}
			}
			if (text == "sproom" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				try
				{
					Console.Write("房间号：");
					string rid2 = Console.ReadLine();
					Console.Write("输入刷屏文件：");
					string name3 = Console.ReadLine();
					if (name3 == "")
					{
						name3 = "ChaCha.txt";
					}
					Console.Write("密码：");
					string pwd = Console.ReadLine();
					Random random3 = new Random();
					new Thread((ThreadStart)delegate
					{
						try
						{
							start_stop = true;
							string text80 = DummySrv.strings[random3.Next(0, DummySrv.strings.Count)];
							DummySrv.RoomID.Add(int.Parse(rid2));
							new SpTask(ReadJson(text80), rid2, pwd, name3);
							DummySrv.RoomID.Remove(int.Parse(rid2));
							DummySrv.strings.Add(text80);
						}
						catch
						{
						}
					}).Start();
				}
				catch
				{
				}
			}
			if (text == "spserver" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				start_stop = true;
				try
				{
					Console.Write("输入ip：");
					string name4 = Console.ReadLine();
					Console.Write("输入端口：");
					string port7 = Console.ReadLine();
					Console.Write("输入服务器id：");
					string sid2 = Console.ReadLine();
					Console.Write("输入刷屏文件：");
					string file = Console.ReadLine();
					if (file == "")
					{
						file = "ChaCha.txt";
					}
					Random random4 = new Random();
					new Thread((ThreadStart)delegate
					{
						try
						{
							start_stop = true;
							string text80 = DummySrv.strings[random4.Next(0, DummySrv.strings.Count)];
							new RentalSwipe(ReadJson(text80), sid2, name4, port7, file);
							DummySrv.strings.Add(text80);
						}
						catch
						{
						}
					}).Start();
				}
				catch
				{
				}
			}
			if (text == "asproom")
			{
				try
				{
					Console.Write("输入入房间entityid：");
					Console.ReadLine();
					Console.Write("输入刷屏文件：");
					string file2 = Console.ReadLine();
					if (file2 == "")
					{
						file2 = "ChaCha.txt";
					}
					new Random();
					new Thread((ThreadStart)delegate
					{
						while (true)
						{
							JObject jObject9 = JsonConvert.DeserializeObject<JObject>(Http.postAPIPC("", "/online-lobby-game-enter"));
							if (jObject9["code"].ToString() == "52001")
							{
								break;
							}
							if (jObject9["code"].ToString() == "12031")
							{
								Thread.Sleep(1000);
							}
							else
							{
								string text80 = jObject9["entity"]["server_host"].ToString();
								string text81 = jObject9["entity"]["server_port"].ToString();
								InvokeCmd("start FastBuilder.exe --uid " + Http.UID + " --sid 4654415063569776560:LobbyGame --name " + displayName + " --version " + Temp.Version + " --engineVersion " + Temp.engineVersion + " --url " + LockServer.AuthServerUrl + " --authurl https://g79authobt.nie.netease.com --ip " + text80 + " --port " + text81 + " --token " + CppGameM.GetLoginToken() + " --typ sp --spfile " + file2);
							}
						}
					}).Start();
				}
				catch
				{
				}
			}
			if (text == "setsleep")
			{
				try
				{
					Console.Write("输入间隔：");
					RentalSwipe.Sleep = int.Parse(Console.ReadLine());
				}
				catch
				{
				}
			}
			if (text == "ridlist" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				for (int num14 = 0; num14 < DummySrv.RoomID.Count; num14++)
				{
					if (DummySrv.RoomID.Count == 0)
					{
						break;
					}
					Console.WriteLine(DummySrv.RoomID[num14]);
				}
			}
			if (text == "padding" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				Console.Write("输入ip：");
				string ip7 = Console.ReadLine();
				Console.Write("输入端口：");
				string port8 = Console.ReadLine();
				Console.Write("服务器号：");
				Padding.StartPadding(Console.ReadLine(), ip7, port8);
			}
			if (text == "paddinglobby" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				Console.Write("输入ip：");
				string ip8 = Console.ReadLine();
				Console.Write("输入端口：");
				string port9 = Console.ReadLine();
				PaddingLobbyClient.StartPadding(ip8, port9);
			}
			if (text == "paddinglobbyt" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				Console.Write("输入ip：");
				string ip9 = Console.ReadLine();
				Console.Write("输入端口：");
				string port10 = Console.ReadLine();
				Console.Write("输入任务数量：");
				int num15 = int.Parse(Console.ReadLine());
				for (int num16 = 0; num16 < num15; num16++)
				{
					PaddingLobbyClient.StartPadding(ip9, port10);
					Thread.Sleep(100);
				}
			}
			if (text == "setnullpadding" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				try
				{
					Console.Write("输入预留空位：");
					Padding.NullPadding = int.Parse(Console.ReadLine());
				}
				catch
				{
				}
			}
			if (text == "tasknum")
			{
				Console.WriteLine(Padding.paddingClients.Count);
			}
			if (text == "setlockserverpassword")
			{
				Console.Write("设置密码：");
				LockServer.password = Console.ReadLine();
			}
			if (text == "displayentity")
			{
				Console.Write("y/n:");
				string text71 = Console.ReadLine();
				if ((text71 == "y") | (text71 == "Y"))
				{
					Padding.DisplayEntity = true;
				}
				else if ((text71 == "n") | (text71 == "N"))
				{
					Padding.DisplayEntity = false;
				}
				else
				{
					Console.WriteLine("输入错误");
				}
			}
			if (text == "lockUID" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				try
				{
					if (!start_stop)
					{
						Console.Write("输入uid：");
						TraceUIDLockServer.TraceUser(Console.ReadLine());
					}
				}
				catch (Exception ex45)
				{
					Console.WriteLine(ex45.ToString());
				}
			}
			if (text == "lockpwd" && (DummySrv.IsInit & (DummySrv.dummyUsers != null)))
			{
				try
				{
					Console.Write("输入密码：");
					TraceUIDLockServer.LockPwd = Console.ReadLine();
				}
				catch (Exception ex46)
				{
					Console.WriteLine(ex46.ToString());
				}
			}
			if (text == "title")
			{
				Console.Write("输入标题：");
				Console.Title = Console.ReadLine();
			}
			if (text == "clear")
			{
				Console.Clear();
			}
			if (text == "cls")
			{
				Console.Clear();
			}
			if (text == "color")
			{
				try
				{
					Console.Write("输入色号：");
					switch (int.Parse(Console.ReadLine()))
					{
					case 0:
						Console.ForegroundColor = ConsoleColor.Black;
						break;
					case 1:
						Console.ForegroundColor = ConsoleColor.DarkBlue;
						break;
					case 2:
						Console.ForegroundColor = ConsoleColor.DarkGreen;
						break;
					case 3:
						Console.ForegroundColor = ConsoleColor.DarkCyan;
						break;
					case 4:
						Console.ForegroundColor = ConsoleColor.DarkRed;
						break;
					case 5:
						Console.ForegroundColor = ConsoleColor.DarkMagenta;
						break;
					case 6:
						Console.ForegroundColor = ConsoleColor.DarkYellow;
						break;
					case 7:
						Console.ForegroundColor = ConsoleColor.Gray;
						break;
					case 8:
						Console.ForegroundColor = ConsoleColor.DarkGray;
						break;
					case 9:
						Console.ForegroundColor = ConsoleColor.Blue;
						break;
					case 10:
						Console.ForegroundColor = ConsoleColor.Green;
						break;
					case 11:
						Console.ForegroundColor = ConsoleColor.Cyan;
						break;
					case 12:
						Console.ForegroundColor = ConsoleColor.Red;
						break;
					case 13:
						Console.ForegroundColor = ConsoleColor.Magenta;
						break;
					case 14:
						Console.ForegroundColor = ConsoleColor.Yellow;
						break;
					case 15:
						Console.ForegroundColor = ConsoleColor.White;
						break;
					}
				}
				catch
				{
				}
			}
			if (text == "report")
			{
				try
				{
					Console.Write("输入uid：");
					string text75 = Console.ReadLine();
					Console.Write("举报信息：");
					string text76 = Console.ReadLine();
					string text77 = JsonConvert.DeserializeObject<JObject>(Http.postAPIPC("{\"user_id\":\"" + text75 + "\"}", "/user/query/search-by-uid"))["entity"]["name"].ToString();
					ReportEntity reportEntity2 = new ReportEntity();
					ReportEntities data2 = new ReportEntities
					{
						act_timestamp = Function.GetTimeStampUlong(),
						reach_login_time = Function.GetTimeStampUlong(),
						report_evi = text77 + ":" + text76 + ";",
						report_id = text75,
						report_name = text77
					};
					reportEntity2.type = "social_report_operation";
					reportEntity2.data = data2;
					string text78 = JsonConvert.SerializeObject(reportEntity2);
					Console.WriteLine(text78);
					Http.x19post("https://g79apigatewayobt.minecraft.cn", "/salog", text78);
				}
				catch (Exception ex48)
				{
					Console.WriteLine(ex48.ToString());
				}
			}
			if (text == "reconnection")
			{
				Console.Write("y/n:");
				string text79 = Console.ReadLine();
				if ((text79 == "y") | (text79 == "Y"))
				{
					ChatConnection.ReConnection = true;
				}
				else if ((text79 == "n") | (text79 == "N"))
				{
					ChatConnection.ReConnection = false;
				}
				else
				{
					Console.WriteLine("输入错误");
				}
			}
            if (text == "setskin")
            {
                Console.Write("输入皮肤文件路径：");
                Temp.skinPath = Console.ReadLine();

                try
                {
                    Temp.skinMd5 = GetHash(Temp.skinPath, MD5.Create());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"计算MD5失败: {ex.Message}");
                    continue;
                }

                Console.Write("输入皮肤IID：");
                string inputIid = Console.ReadLine();
                Temp.skinIid = "-" + inputIid.TrimStart('-');

                File.WriteAllText("config.json", JsonConvert.SerializeObject(Temp));
                Temp = JsonConvert.DeserializeObject<temp>(File.ReadAllText("config.json"));

                Console.WriteLine("皮肤信息设置成功");
            }
        }
	}

	public static string GetPlayerDisplayName(string uid)
	{
		try
		{
			return JsonConvert.DeserializeObject<JObject>(Http.postAPIPC("{\"user_id\":\"" + uid + "\"}", "/user/query/search-by-uid", displaylog: false))["entity"]["name"].ToString();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
			return null;
		}
	}

	public static async void AKICK(string rid)
	{
		await Task.Run(delegate
		{
			try
			{
				while (start_stop)
				{
					Thread.Sleep(1000);
					string[] array = Temp.ban_id.ToArray();
					JObject jObject = JsonConvert.DeserializeObject<JObject>(Http.postAPIPC("{\"room_id\":\"" + rid + "\"}", "/online-lobby-member/query/list-by-room-id"));
					int num = jObject["entities"].Count();
					int num2 = 0;
					while (num2 < num)
					{
						for (int i = 0; i < array.Count(); i++)
						{
							JObject jObject2 = JsonConvert.DeserializeObject<JObject>(jObject["entities"][num - 1].ToString());
							if (jObject2["member_id"].ToString() == array[i])
							{
								Http.postAPIPC("{\"room_id\":\"" + rid + "\",\"user_id\":" + jObject2["member_id"].ToString() + "}", "/online-lobby-member-kick");
								Console.WriteLine("KickID:" + jObject2["member_id"].ToString());
								Console.WriteLine("玩家:" + jObject2["member_id"].ToString() + "尝试进入游戏已被踢出");
							}
						}
						num--;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("出错了：" + ex.Message);
			}
		});
	}

	public static async void Ajoin(string rid, string password = "")
	{
		await Task.Run(delegate
		{
			while (start_stop)
			{
				Http.postAPIPC("{\"room_id\":\"" + rid + "\",\"password\":\"" + password + "\",\"check_visibilily\":false}", "/online-lobby-room-enter");
				Http.room_id_public = rid;
				Thread.Sleep(1000);
			}
		});
	}

	public static void StaticAjoin(string rid, string password = "")
	{
		while (start_stop)
		{
			Http.postAPIPC("{\"room_id\":\"" + rid + "\",\"password\":\"" + password + "\",\"check_visibilily\":false}", "/online-lobby-room-enter");
			Thread.Sleep(1000);
		}
	}

	public static string ReadJson(string FilePath)
	{
		try
		{
			Function.ClientLog("[FILE] read path:" + FilePath);
			StreamReader streamReader = new StreamReader(FilePath);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			return result;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
			Function.ClientError(ex.ToString());
			return null;
		}
	}

	public static string SimpleRead(string FilePath)
	{
		try
		{
			StreamReader streamReader = new StreamReader(FilePath);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			return result;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
			Function.ClientError(ex.ToString());
			return null;
		}
	}

	public static string GetAuthenticationOtpJson(string versionvaluas, string aid, string otp_token, string Sauth_json)
	{
		try
		{
			AuthoptSauth authoptSauth = new AuthoptSauth();
			VersionEntity versionEntity = new VersionEntity();
			AuthenticationEntity obj = new AuthenticationEntity
			{
				sa_data = ReadJson("sa_data.json")
			};
			authoptSauth = JsonConvert.DeserializeObject<AuthoptSauth>(Sauth_json);
			obj.sauth_json = authoptSauth.sauth_json;
			versionEntity.version = versionvaluas;
			obj.version = versionEntity;
			obj.aid = aid;
			obj.otp_token = otp_token;
			return JsonConvert.SerializeObject(obj);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return null;
		}
	}

	public static void Name()
	{
		while (Function.GetName(player_info) == "")
		{
			Console.WriteLine("该账号还没有昵称!");
			Console.Write("输入你要设置的昵称：");
			string text = Console.ReadLine();
			if (text == "rj")
			{
				Http.SetName("NO" + Http.UID);
			}
			else
			{
				Http.SetName(text);
			}
			Http.GetName();
			if (Function.GetName(player_info) != "")
			{
				break;
			}
		}
	}

	public static string InvokeCmd(string cmdArgs)
	{
		Process process = new Process();
		process.StartInfo.FileName = "cmd.exe";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.Start();
		process.StandardInput.WriteLine(cmdArgs);
		process.StandardInput.WriteLine("exit");
		string result = process.StandardOutput.ReadToEnd();
		process.Close();
		return result;
	}

	public static int UnzipModJson(string path)
	{
		int num = sd.k(path);
		if (num != 0)
		{
			Console.WriteLine("UnzipModJson Failed! code:" + num);
			return num;
		}
		Console.WriteLine("code:0");
		return num;
	}

	public static async void InvokeCmdAsync(string cmdArgs)
	{
		await Task.Run(delegate
		{
			Process process = new Process();
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			process.StandardInput.WriteLine(cmdArgs);
			process.StandardInput.WriteLine("exit");
			process.StandardOutput.ReadToEnd();
			process.Close();
		});
	}

	public static async void updateroom(string room_id)
	{
		Http.postAPIPC("{\"room_id\":\"" + room_id + "\",\"password\":null,\"check_visibilily\":false}", "/online-lobby-member-tick");
		isroom = true;
		Console.WriteLine("已开启自动更新");
		await Task.Run(delegate
		{
			while (isroom)
			{
				Http.postAPIPC("{\"room_id\":\"" + room_id + "\",\"password\":null,\"check_visibilily\":false}", "/online-lobby-member-tick");
				Thread.Sleep(30000);
			}
		});
	}

	public static async void updateroom_NoCommon(string room_id, string token, string uid)
	{
		Http.postAPIPC_NoCommon("{\"room_id\":\"" + room_id + "\",\"password\":null,\"check_visibilily\":false}", "/online-lobby-member-tick", token, uid);
		isroom = true;
		Console.WriteLine("已开启自动更新");
		await Task.Run(delegate
		{
			while (isroom)
			{
				Http.postAPIPC_NoCommon("{\"room_id\":\"" + room_id + "\",\"password\":null,\"check_visibilily\":false}", "/online-lobby-member-tick", token, uid);
				Thread.Sleep(30000);
			}
		});
	}

	public static bool checkplayer(string[] str, string uid)
	{
		bool flag = true;
		for (int i = 0; i < str.Count(); i++)
		{
			if (str[i] == uid)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			return true;
		}
		return false;
	}
    private static string GetHash(string filePath, HashAlgorithm algorithm)
    {
        using var stream = File.OpenRead(filePath);
        return BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
    }
}
