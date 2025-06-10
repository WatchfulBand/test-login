using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConsoleAppLogin.NetEase;
using Login.NetEase.DummyService.Entity;
using Login.NetEase.Request;
using Mark;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Noya.LocalServer.Common.Extensions;
using text;

namespace Login.NetEase.DummyService;

internal class DummySrv
{
	public class Key
	{
		public uint time;

		public uint id;
	}

	public class Keys
	{
		public List<uint> keys;

		public static bool IsOverdue(Keys keys, uint key)
		{
			bool result = true;
			for (int i = 0; i < keys.keys.Count; i++)
			{
				if (keys.keys[i] == key)
				{
					result = false;
					break;
				}
			}
			return result;
		}
	}

	public class FriendEntity
	{
		public string comment;

		public string message;

		public uint fid;
	}

	public static List<int> RoomID = new List<int>();

	public static bool IsInit = false;

	public static List<DummyClient> dummyClients = new List<DummyClient>();

	public static List<FLockServer> LockClients = new List<FLockServer>();

	public static SrvDummyUserLists dummyUsers;

	public static BotConfig BotConfig;

	public static Keys keys;

	public static List<string> strings { get; set; }

	public static void Init()
	{
		try
		{
			strings = JsonConvert.DeserializeObject<DummyUserList>(app.ReadJson("DummyUserList.json")).UserList.ToList();
			dummyUsers = JsonConvert.DeserializeObject<SrvDummyUserLists>(app.ReadJson("user_srv_dummy_list.json"));
			keys = JsonConvert.DeserializeObject<Keys>(app.ReadJson("Keys.json"));
			BotConfig = JsonConvert.DeserializeObject<BotConfig>(app.ReadJson("bot_config.json"));
			if (strings.Count() != 0)
			{
				IsInit = true;
			}
			else
			{
				Console.WriteLine("初始化失败！");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}

	public static int Dummy(string message)
	{
		if (IsInit)
		{
			RecvMessageEntity recvMessageEntity = JsonConvert.DeserializeObject<RecvMessageEntity>(message);
			Console.WriteLine("有人发送了消息：" + recvMessageEntity.words + "  UID:" + recvMessageEntity.uid);
			string text = recvMessageEntity.uid.ToString();
			string words;
			if (recvMessageEntity.platform != 0)
			{
				int num = 0;
				words = recvMessageEntity.words;
				for (int i = 0; i < words.Length; i++)
				{
					if (words[i] == ' ')
					{
						num++;
					}
				}
				string[] array = new string[num + 1];
				recvMessageEntity.words += " ";
				if (array.Length == 1)
				{
					array[0] = recvMessageEntity.words;
				}
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				words = recvMessageEntity.words;
				for (int i = 0; i < words.Length; i++)
				{
					if (words[i] != ' ')
					{
						num2++;
						continue;
					}
					array[num4] = recvMessageEntity.words.Substring(num3, num2 - num3);
					num3 = num2 + 1;
					num4++;
					num2++;
				}
				int taskNum = GetTaskNum(text);
				int lockTaskNum = GetLockTaskNum(text);
				int staticTaskNum = GetStaticTaskNum(text);
				Function.ClientLog("user lock task num:" + lockTaskNum);
				Function.ClientLog("user static task num:" + staticTaskNum);
				Function.ClientLog("user kickplayer task num:" + taskNum);
				Function.ClientLog("user_id:" + text);
				if (SrvDummyUserLists.IsUid(dummyUsers, text))
				{
					if (!SrvDummyUserLists.IsTimeOverdue(dummyUsers, text))
					{
						if (SrvDummyUserLists.IsTimeOverdue(dummyUsers, text))
						{
							app.chatConnection.SendUIDMessage("使用到期，请续费！", text);
						}
						else if (array[0] == "/卡人")
						{
							if (array.Length == 2)
							{
								if (taskNum < staticTaskNum)
								{
									try
									{
										Random random = new Random();
										JObject jObject = JsonConvert.DeserializeObject<JObject>(Http.postAPIPC("{\"name_or_mail\":\"" + array[1] + "\",\"mail_flag\":false}", "/user-search-friend"));
										string text2 = string.Empty;
										string uid = string.Empty;
										for (int j = 0; jObject["entities"].Count() > j; j++)
										{
											Console.WriteLine(array[1].Length);
											Console.WriteLine(jObject["entities"][j]["nickname"].ToString().Length);
											if (jObject["entities"][j]["nickname"].ToString() == array[1])
											{
												text2 = jObject["entities"][j]["nickname"].ToString();
												uid = jObject["entities"][j]["uid"].ToString();
											}
										}
										if (text2 == string.Empty)
										{
											throw new Exception("Error: The name is empty");
										}
										DummyUserList dummyUserList = new DummyUserList();
										string[] array2 = new string[BotConfig.dummy_level];
										for (int k = 0; k < BotConfig.dummy_level; k++)
										{
											string item = (array2[k] = strings[random.Next(0, strings.Count())]);
											strings.Remove(item);
										}
										dummyUserList.UserList = array2;
										dummyClients.Add(StartDummy(uid, recvMessageEntity, dummyUserList, "发起用户：" + recvMessageEntity.uid + "---目标用户：" + text2 + "---时间：1分钟"));
										app.chatConnection.SendUIDMessage("任务：《" + text2 + "》已启动！", text);
										Function.ClientLog("[DummyTask] Start the task " + text + " TaskNum:" + GetTaskNum(text), ConsoleColor.Yellow);
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.ToString());
										app.chatConnection.SendUIDMessage("错误：未搜索到用户！", text);
									}
								}
								else
								{
									app.chatConnection.SendUIDMessage("只能开启" + staticTaskNum + "个任务！", text);
								}
							}
							if (array.Length == 3)
							{
								if (array[2].Length <= 8)
								{
									if (taskNum < staticTaskNum)
									{
										try
										{
											Random random2 = new Random();
											int dummy_level = BotConfig.dummy_level;
											JObject jObject2 = JsonConvert.DeserializeObject<JObject>(Http.postAPIPC("{\"name_or_mail\":\"" + array[1] + "\",\"mail_flag\":false}", "/user-search-friend"));
											string text3 = string.Empty;
											string uid2 = string.Empty;
											for (int l = 0; jObject2["entities"].Count() > l; l++)
											{
												if (jObject2["entities"][l]["nickname"].ToString() == array[1])
												{
													text3 = jObject2["entities"][l]["nickname"].ToString();
													uid2 = jObject2["entities"][l]["uid"].ToString();
												}
											}
											if (text3 == string.Empty)
											{
												throw new Exception("Error: The name is empty");
											}
											DummyUserList dummyUserList2 = new DummyUserList();
											string[] array3 = new string[dummy_level];
											for (int m = 0; m < dummy_level; m++)
											{
												string item2 = (array3[m] = strings[random2.Next(0, strings.Count())]);
												strings.Remove(item2);
											}
											dummyUserList2.UserList = array3;
											int time = int.Parse(array[2]);
											dummyClients.Add(StartDummy(uid2, recvMessageEntity, dummyUserList2, "发起用户：" + recvMessageEntity.uid + "---目标用户：" + text3 + "---时间：" + time + "分钟", time));
											app.chatConnection.SendUIDMessage("任务：《" + text3 + "》已启动！时间为：" + array[2] + "分钟！", text);
											Function.ClientLog("[DummyTask] Start the task " + text + " TaskNum:" + GetTaskNum(text), ConsoleColor.Yellow);
										}
										catch (Exception ex2)
										{
											Console.WriteLine(ex2.ToString());
											app.chatConnection.SendUIDMessage("错误：未搜索到用户！", text);
										}
									}
									else
									{
										app.chatConnection.SendUIDMessage("只能开启" + staticTaskNum + "个任务！", text);
									}
								}
								else
								{
									app.chatConnection.SendUIDMessage("最大只能开启99999999分钟！", text);
								}
							}
						}
						else if (array[0] == "/help")
						{
							app.chatConnection.SendUIDMessage("指令教程：\n/锁服 《服务器号》\n/锁服 《服务器号》 《密码》\n输入/stopv2结束锁服任务", text);
						}
						else if (array[0] == "/stop")
						{
							if (staticTaskNum == 1)
							{
								if (array.Length == 1)
								{
									if (taskNum != 0)
									{
										for (int n = 0; n < dummyClients.Count(); n++)
										{
											if (dummyClients[n].UserID == text)
											{
												dummyClients[n].Close(recvMessageEntity);
												Function.ClientLog("[DummyTask] Stop the task " + text + " TaskNum:" + GetTaskNum(text), ConsoleColor.Yellow);
											}
										}
										app.chatConnection.SendUIDMessage("成功停止服务！", text);
									}
									else
									{
										app.chatConnection.SendUIDMessage("你没有开启服务！", text);
									}
								}
								else
								{
									app.chatConnection.SendUIDMessage("你只有一个卡槽！", text);
								}
							}
							else if (array.Length == 2)
							{
								try
								{
									JObject jObject3 = JsonConvert.DeserializeObject<JObject>(Http.postAPIPC("{\"name_or_mail\":\"" + array[1] + "\",\"mail_flag\":false}", "/user-search-friend"));
									string text4 = string.Empty;
									string text5 = string.Empty;
									for (int num5 = 0; jObject3["entities"].Count() > num5; num5++)
									{
										if (jObject3["entities"][num5]["nickname"].ToString() == array[1])
										{
											text4 = jObject3["entities"][num5]["nickname"].ToString();
											text5 = jObject3["entities"][num5]["uid"].ToString();
										}
									}
									if (text4 == string.Empty)
									{
										throw new Exception("Error: The name is empty");
									}
									if (taskNum != 0)
									{
										for (int num6 = 0; num6 < dummyClients.Count(); num6++)
										{
											if (dummyClients[num6].UserID == text && dummyClients[num6].AimUserID == text5)
											{
												dummyClients[num6].Close(recvMessageEntity);
												app.chatConnection.SendUIDMessage("成功停止服务！", text);
												Function.ClientLog("[DummyTask] Stop the task " + text + " TaskNum:" + GetTaskNum(text), ConsoleColor.Yellow);
												break;
											}
										}
									}
									else
									{
										app.chatConnection.SendUIDMessage("没有该用户的记录！", text);
									}
								}
								catch (Exception ex3)
								{
									Console.WriteLine(ex3.ToString());
									app.chatConnection.SendUIDMessage("错误：未搜索到用户！", text);
								}
							}
							else
							{
								app.chatConnection.SendUIDMessage("请指定你要结束的用户！", text);
							}
						}
						else if (array[0] == "/list")
						{
							try
							{
								List<string> list = new List<string>();
								for (int num7 = 0; dummyClients.Count > num7; num7++)
								{
									if (dummyClients[num7].UserID == text)
									{
										list.Add(dummyClients[num7].AimUserID);
									}
								}
								uint[] array4 = new uint[list.Count];
								for (int num8 = 0; num8 < list.Count; num8++)
								{
									array4[num8] = uint.Parse(list[num8]);
								}
								RoomUserNameListRequest roomUserNameListRequest = new RoomUserNameListRequest();
								roomUserNameListRequest.entity_ids = array4;
								JObject jObject4 = JsonConvert.DeserializeObject<JObject>(Http.x19post("https://x19apigatewayobt.nie.netease.com", "/user/query/search-by-ids", JsonConvert.SerializeObject(roomUserNameListRequest)));
								for (int num9 = 0; num9 < list.Count; num9++)
								{
									list[num9] = jObject4["entities"][num9]["name"].ToString();
								}
								string text6 = string.Empty;
								for (int num10 = 0; num10 < list.Count; num10++)
								{
									text6 = text6 + list[num10].ToString() + "\n";
								}
								app.chatConnection.SendUIDMessage(text6, text);
								Function.ClientLog("[DummyTask] Check out the list " + text + " TaskNum:" + taskNum, ConsoleColor.Yellow);
							}
							catch (Exception ex4)
							{
								Console.WriteLine(ex4.ToString());
							}
						}
						else if (array[0] == "/sflist")
						{
							try
							{
								string text7 = "";
								for (int num11 = 0; num11 < LockClients.Count; num11++)
								{
									if (LockClients[num11].uid == text)
									{
										text7 = text7 + LockClients[num11].serverid + "\n";
									}
								}
								app.chatConnection.SendUIDMessage(text7, text);
							}
							catch (Exception ex5)
							{
								Console.WriteLine(ex5.ToString());
							}
						}
						else if (array[0] == "/锁服")
						{
							if (array.Length == 2)
							{
								if (lockTaskNum < staticTaskNum)
								{
									if (WhiteListIsTimeOverdue(array[1]))
									{
										StartLockServer(array[1], null, text);
									}
									else
									{
										app.chatConnection.SendUIDMessage("服务器处于白名单", text);
									}
								}
								else
								{
									app.chatConnection.SendUIDMessage("只能开启" + staticTaskNum + "个任务！", text);
								}
							}
							if (array.Length == 3)
							{
								if (lockTaskNum < staticTaskNum)
								{
									if (WhiteListIsTimeOverdue(array[1]))
									{
										StartLockServer(array[1], array[2], text);
									}
									else
									{
										app.chatConnection.SendUIDMessage("服务器处于白名单", text);
									}
								}
								else
								{
									app.chatConnection.SendUIDMessage("只能开启" + staticTaskNum + "个任务！", text);
								}
							}
						}
						else if (array[0] == "/stopv2")
						{
							Function.ClientLog("stop lock thread");
							if (staticTaskNum == 1)
							{
								if (array.Length == 1)
								{
									if (lockTaskNum != 0)
									{
										Function.ClientLog("LockClientNum:" + LockClients.Count);
										for (int num12 = 0; num12 < LockClients.Count; num12++)
										{
											if (LockClients[num12].uid == text)
											{
												Function.ClientLog("CheckUid:" + LockClients[num12].uid);
												LockClients[num12].Close();
												Function.ClientLog("[DummyTask] Stop the task " + text + " TaskNum:" + GetTaskNum(text), ConsoleColor.Yellow);
											}
										}
										app.chatConnection.SendUIDMessage("成功停止服务！", text);
									}
									else
									{
										app.chatConnection.SendUIDMessage("你没有开启服务！", text);
									}
								}
								else
								{
									app.chatConnection.SendUIDMessage("你只有一个卡槽！", text);
								}
							}
							else if (array.Length == 2)
							{
								if (lockTaskNum != 0)
								{
									for (int num13 = 0; num13 < LockClients.Count(); num13++)
									{
										if (LockClients[num13].uid == text && LockClients[num13].serverid == array[1])
										{
											LockClients[num13].Close();
											app.chatConnection.SendUIDMessage("成功停止服务！", text);
											Function.ClientLog("[DummyTask] Stop the task " + text + " TaskNum:" + GetTaskNum(text), ConsoleColor.Yellow);
											break;
										}
									}
								}
								else
								{
									app.chatConnection.SendUIDMessage("没有该用户的记录！", text);
								}
							}
							else
							{
								app.chatConnection.SendUIDMessage("请指定你要结束的用户！", text);
							}
						}
						else
						{
							app.chatConnection.SendUIDMessage("未知的命令，请输入/help查看教程！", text);
						}
						return 0;
					}
					Handle(text, array[0]);
					return 0;
				}
				Handle(text, array[0]);
				return 0;
			}
			int num14 = 0;
			words = recvMessageEntity.words;
			for (int i = 0; i < words.Length; i++)
			{
				if (words[i] == ' ')
				{
					num14++;
				}
			}
			string[] array5 = new string[num14 + 1];
			recvMessageEntity.words += " ";
			if (array5.Length == 1)
			{
				array5[0] = recvMessageEntity.words;
			}
			int num15 = 0;
			int num16 = 0;
			int num17 = 0;
			words = recvMessageEntity.words;
			for (int i = 0; i < words.Length; i++)
			{
				if (words[i] != ' ')
				{
					num15++;
					continue;
				}
				array5[num17] = recvMessageEntity.words.Substring(num16, num15 - num16);
				num16 = num15 + 1;
				num17++;
				num15++;
			}
			if (SrvDummyUserLists.IsUid(dummyUsers, text))
			{
				if (!SrvDummyUserLists.IsTimeOverdue(dummyUsers, text))
				{
					if ((array5[0] == "/崩服") | (array5[0] == "/bf"))
					{
						if (array5.Length == 2)
						{
							try
							{
								app.chatConnection.SendUIDMessage("正在启动崩服！", text);
								Random random3 = new Random();
								string text8 = strings[random3.Next(0, strings.Count)];
								strings.Remove(text8);
								new FBClient(app.ReadJson(text8), array5[1], text);
							}
							catch (Exception ex6)
							{
								Console.WriteLine("未经处理的异常：" + ex6.ToString());
								app.chatConnection.SendUIDMessage("启动崩服时发生错误，请稍后重试：" + ex6.Message, text);
							}
						}
						if (array5.Length == 3)
						{
							try
							{
								app.chatConnection.SendUIDMessage("正在启动崩服！", text);
								Random random4 = new Random();
								string text9 = strings[random4.Next(0, strings.Count)];
								strings.Remove(text9);
								new FBClient(text9, array5[1], text, array5[2]);
							}
							catch (Exception ex7)
							{
								Console.WriteLine("未经处理的异常：" + ex7.ToString());
								app.chatConnection.SendUIDMessage("启动崩服时发生错误，请稍后重试：" + ex7.Message, text);
							}
						}
					}
					else if (array5[0] == "/help")
					{
						app.chatConnection.SendUIDMessage("________________命令帮助________________\n/崩服 [房间号]\n/崩服 [房间号] [密码]\n同等/bf一样\n_________xiaoyuanzi . shop______________", text);
					}
					else
					{
						app.chatConnection.SendUIDMessage("未知的命令，请输入/help查看教程", text);
					}
				}
				else
				{
					Handle(text, array5[0]);
				}
			}
			else
			{
				Handle(text, array5[0]);
			}
			return 0;
		}
		Console.WriteLine("未初始化！");
		return -1;
	}

	private static void Handle(string struid, string authkey)
	{
		try
		{
			Key key = JsonConvert.DeserializeObject<Key>(x19Crypt.HttpDecrypt_g79v12(authkey.HexToBytes()));
			int num = (int)(uint.Parse(Function.GetTimeStamp()) + key.time);
			if (Keys.IsOverdue(keys, key.id))
			{
				if (IsUser(struid))
				{
					MemoryUpdateUser(struid, num);
					Function.SaveFile("user_srv_dummy_list.json", JsonConvert.SerializeObject(dummyUsers));
					Function.SaveFile("Keys.json", JsonConvert.SerializeObject(keys));
					Console.WriteLine("已保存文件：user_srv_dummy_list.json");
					long num2 = num;
					DateTime dateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(num2);
					app.chatConnection.SendUIDMessage("用户信息已录入，到期时间：" + dateTime.ToString(), struid);
				}
				else
				{
					SrvDummyUserListsEntity srvDummyUserListsEntity = new SrvDummyUserListsEntity();
					srvDummyUserListsEntity.uid = struid;
					srvDummyUserListsEntity.overdue_time = (int)(uint.Parse(Function.GetTimeStamp()) + key.time);
					srvDummyUserListsEntity.max_num = 1;
					keys.keys.Add(key.id);
					dummyUsers.user_lists.Add(srvDummyUserListsEntity);
					Function.SaveFile("user_srv_dummy_list.json", JsonConvert.SerializeObject(dummyUsers));
					Function.SaveFile("Keys.json", JsonConvert.SerializeObject(keys));
					Console.WriteLine("已保存文件：user_srv_dummy_list.json");
					long num3 = srvDummyUserListsEntity.overdue_time;
					DateTime dateTime2 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(num3);
					app.chatConnection.SendUIDMessage("用户信息已录入，到期时间：" + dateTime2.ToString(), struid);
				}
			}
			else
			{
				app.chatConnection.SendUIDMessage("卡密已被使用！", struid);
			}
		}
		catch
		{
			app.chatConnection.SendUIDMessage("卡密错误", struid);
		}
	}

	public static bool CloseDummyTask(string SrcUID, string AimUID)
	{
		bool result = false;
		for (int i = 0; i < dummyClients.Count(); i++)
		{
			if (dummyClients[i].UserID == SrcUID && dummyClients[i].AimUserID == AimUID)
			{
				result = true;
				dummyClients.RemoveAt(i);
			}
		}
		return result;
	}

	public static DummyClient StartDummy(string uid, RecvMessageEntity recvMessageEntity, DummyUserList dummyUserList, string title, int time = 1)
	{
		return new DummyClient(uid, recvMessageEntity, dummyUserList, time, title);
	}

	private static int GetTaskNum(string UserId)
	{
		int num = 0;
		for (int i = 0; i < dummyClients.Count(); i++)
		{
			if (dummyClients[i].UserID == UserId)
			{
				num++;
			}
		}
		return num;
	}

	private static int GetLockTaskNum(string UserId)
	{
		if (LockClients.Count == 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < LockClients.Count(); i++)
		{
			if (LockClients[i].uid == UserId)
			{
				num++;
			}
		}
		return num;
	}

	private static bool IsUser(string UserId)
	{
		for (int i = 0; i < dummyUsers.user_lists.Count; i++)
		{
			if (dummyUsers.user_lists[i].uid == UserId)
			{
				return true;
			}
		}
		return false;
	}

	private static bool MemoryUpdateUser(string UserId, int time)
	{
		for (int i = 0; i < dummyUsers.user_lists.Count; i++)
		{
			if (dummyUsers.user_lists[i].uid == UserId)
			{
				dummyUsers.user_lists[i].overdue_time = time;
				return true;
			}
		}
		return false;
	}

	private static int GetStaticTaskNum(string UserId)
	{
		for (int i = 0; i < dummyUsers.user_lists.Count(); i++)
		{
			if (dummyUsers.user_lists[i].uid == UserId)
			{
				return dummyUsers.user_lists[i].max_num;
			}
		}
		return 0;
	}

	private static bool IsWhiteList(string serverid)
	{
		if (BotConfig.white_list.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < BotConfig.white_list.Count(); i++)
		{
			if (BotConfig.white_list[i].server_name == serverid)
			{
				return true;
			}
		}
		return false;
	}

	public static bool WhiteListIsTimeOverdue(string servername)
	{
		ulong num = ulong.Parse(Function.GetTimeStamp());
		Function.ClientLog("TimeStamp:" + num);
		for (int i = 0; i < BotConfig.white_list.Count; i++)
		{
			if (BotConfig.white_list[i].server_name == servername)
			{
				if (num > BotConfig.white_list[i].time_stamp)
				{
					return true;
				}
				return false;
			}
		}
		return true;
	}

	public static void RecvFriend(string message)
	{
		FriendEntity friendEntity = JsonConvert.DeserializeObject<FriendEntity>(message);
		string uida = friendEntity.fid.ToString();
		Http.postAPIPC("{\"fid\":" + friendEntity.fid + ",\"accept\":true}", "/user-reply-friend");
		Thread.Sleep(2500);
		app.chatConnection.SendUIDMessage("请在一分钟内发送卡密！", uida);
		new Thread((ThreadStart)delegate
		{
			Thread.Sleep(60000);
			if (!SrvDummyUserLists.IsUid(dummyUsers, uida))
			{
				Http.postAPIPC("{\"fid\":" + friendEntity.fid + "}", "/user-del-friend");
			}
		}).Start();
	}

	private static void StartLockServer(string servername, string serverpwd, string uid)
	{
		Random random = new Random();
		string text = strings[random.Next(0, strings.Count())];
		strings.Remove(text);
		FLockServer item = new FLockServer(text, servername, serverpwd, uid);
		LockClients.Add(item);
		app.chatConnection.SendUIDMessage("锁服启动成功！", uid);
	}
}
