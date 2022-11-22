// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.IO;
// using System.Linq;
// using System.Text;
// using adbrix.v2.backend.library.models.models.meta.accounts.users;
// using adbrix.v2.backend.library.models.models.meta.partners.user;
// using adbrix.v2.backend.library.models.models.meta.users;
// using adbrix.v2.backend.library.models.models.meta.users.two_fa;
// using ConfigWorks.sdk;
// using Newtonsoft.Json;
// using ServiceStack;
// using static adbrix.v2.backend.library.models.models.meta.users.UserInfo;
//
// namespace DFN.Report.User
// {
//     public class UserMigration
//     {
//         // 마이그레이션용 모델
//         public class NotSetting2FAUserList
//         {
//             public string user_id { get; set; }
//             public string name { get; set; }
//             public string email { get; set; }
//             public bool is_active { get; set; }
//             public string type { get; set; }
//             public bool is_set_2fa { get; set; }
//         }
//
//         //        public static void UpdateUserState(MySqlManager db)
//         //        {
//         //            List<ConfingWorksResult> config_list = db.ExecuteDynamicQuery<ConfingWorksResult>(
//         //                $"select config_name, data from configs where config_name like 'users:%'"
//         //            );
//         //
//         //            foreach (ConfingWorksResult config in config_list)
//         //            {
//         //                var result = JsonCompressUtil.Decompress(config.data);
//         //                var user = JsonConvert.DeserializeObject<UserInfo>(result);
//         //
//         //                Console.WriteLine(config.config_name);
//         //                CloudConfig.Update<UserInfo>(
//         //                    config.config_name,
//         //                    (data) => { data.state = UserInfo.UserInfoState.email_confirmed; },
//         //                    $"email:{user.email}"
//         //                );
//         //            }
//         //        }
//         //
//         //        public static void SelectUserList(MySqlManager db)
//         //        {
//         //            List<ConfingWorksResult> config_list = db.ExecuteDynamicQuery<ConfingWorksResult>(
//         //                $"select config_name, data from configs where config_name like 'users:%'"
//         //            );
//         //
//         //            foreach (ConfingWorksResult config in config_list)
//         //            {
//         //                var result = JsonCompressUtil.Decompress(config.data);
//         //                var user = JsonConvert.DeserializeObject<UserInfo>(result);
//         //
//         //                Console.WriteLine(user.email);
//         //            }
//         //        }
//
//
//         // public static void UserInfoImpression2FAReset(MySqlManager db)
//         // {
//         //     List<ConfingWorksResult> config_list = db.ExecuteDynamicQuery<ConfingWorksResult>(
//         //         $"select config_name, data from configs where config_name like 'users:%'"
//         //     );
//         //
//         //     foreach (ConfingWorksResult config in config_list)
//         //     {
//         //         var result = JsonCompressUtil.Decompress(config.data);
//         //         try
//         //         {
//         //             var userInfo = JsonConvert.DeserializeObject<UserInfo>(result);
//         //             if (userInfo.is_impression_2fa)
//         //             {
//         //                 CloudConfig.Update<UserInfo>(
//         //                     config.config_name,
//         //                     (data) =>
//         //                     {
//         //                         data.is_impression_2fa = false;
//         //                     },
//         //                     $"email:{userInfo.email}",
//         //                     $"refer:{UserInfo.UserInfoRegistReferrer.console}",
//         //                     $"type:{UserInfo.UserInfoType.user}"
//         //                 );
//         //
//         //                 Console.WriteLine(userInfo.email);
//         //             }
//         //         }
//         //         catch (Exception e)
//         //         {
//         //             // Console.WriteLine(e.Message);
//         //         }
//         //     }
//         //
//         //     Console.WriteLine("done");
//         // }
//
//
//         //public static void UpdateUserSearchKey(MySqlManager db)
//         //{
//         //    var partner_config_list = db.ExecuteDynamicQuery<ConfingWorksResult>(
//         //        $"select config_name, data from configs where domain like 'partners:%:users' and is_build = false"
//         //    );
//
//         //    var partnerUserId = new List<string>();
//
//         //    foreach (var config in partner_config_list)
//         //    {
//         //        var result = JsonCompressUtil.Decompress(config.data);
//         //        var partnerUser = JsonConvert.DeserializeObject<PartnerUser>(result);
//         //        if (!string.IsNullOrEmpty(partnerUser.user_id))
//         //            partnerUserId.Add(partnerUser.user_id);
//         //    }
//
//         //    var user_config_list = db.ExecuteDynamicQuery<ConfingWorksResult>(
//         //        $"select config_name, data from configs where config_name like 'users:%'"
//         //    );
//
//         //    foreach (var config in user_config_list)
//         //    {
//         //        var result = JsonCompressUtil.Decompress(config.data);
//         //        var userInfo = JsonConvert.DeserializeObject<UserInfo>(result);
//         //        if (string.IsNullOrEmpty(userInfo.user_id))
//         //            continue;
//
//         //        if (partnerUserId.Contains(userInfo.user_id))
//         //        {
//         //            CloudConfig.Update<UserInfo>(
//         //                config.config_name,
//         //                (data) =>
//         //                {
//         //                    data.regist_referrer_name = UserInfo.UserInfoRegistReferrer.partner_console;
//         //                    data.user_type = UserInfo.UserInfoType.user;
//         //                },
//         //                $"email:{userInfo.email}",
//         //                $"refer:{UserInfo.UserInfoRegistReferrer.partner_console}",
//         //                $"type:{UserInfo.UserInfoType.user}"
//         //            );
//         //        }
//         //        else
//         //        {
//         //            CloudConfig.Update<UserInfo>(
//         //                config.config_name,
//         //                (data) =>
//         //                {
//         //                    data.regist_referrer_name = UserInfo.UserInfoRegistReferrer.console;
//         //                    data.user_type = UserInfo.UserInfoType.user;
//         //                },
//         //                $"email:{userInfo.email}",
//         //                $"refer:{UserInfo.UserInfoRegistReferrer.console}",
//         //                $"type:{UserInfo.UserInfoType.user}"
//         //            );
//         //        }
//         //    }
//         //}
//
//         public static void UserCountByMonth(MySqlManager db)
//         {
//             StackTrace stackTrace = new StackTrace();
//             StackFrame stackFrame = stackTrace.GetFrame(0);
//             var methodName = stackFrame.GetMethod().Name;
//
//             Console.WriteLine($"{methodName} 추출 시작");
//
//             List<ConfingWorksResult> config_list = db.ExecuteDynamicQuery<ConfingWorksResult>(
//                 $"select config_name, search2, search3, data from configs where config_name like 'users:%' and search2 = 'refer:console' and search3 = 'type:user'"
//             );
//
//             var dCount = new Dictionary<string, int>();
//
//             foreach (ConfingWorksResult config in config_list)
//             {
//                 var result = JsonCompressUtil.Decompress(config.data);
//                 try
//                 {
//                     var user = JsonConvert.DeserializeObject<UserInfo>(result);
//
//                     var key = user.created_datetime.ToLocalTime().ToString("yyyy-MM");
//                     if (dCount.ContainsKey(key))
//                     {
//                         dCount[key] = dCount[key] + 1;
//                     }
//                     else
//                     {
//                         dCount.Add(key, 1);
//                     }
//                 }
//                 catch (Exception e)
//                 {
//                     Console.WriteLine(e.Message);
//                 }
//             }
//
//             //foreach (KeyValuePair<string, int> pair in dCount.OrderBy(i => i.Key))
//             //{
//             //    Console.WriteLine("{0} {1}",
//             //        pair.Key,
//             //        string.Format("{0:n0}", pair.Value));
//             //}
//
//             string currentMethod = string.Concat(methodName.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
//             string pathTemplate = @"c:\temp\" + currentMethod + "_{0}.csv";
//             int i = -1;
//             do
//             {
//                 var path = String.Format(pathTemplate, $"({++i})");
//                 if (!File.Exists(path))
//                 {
//                     File.WriteAllText(path, dCount.OrderBy(i => i.Key).Select(x => new { month = x.Key, count = x.Value }).ToList().ToCsv(), Encoding.UTF8);
//                     break;
//                 }
//             } while (true);
//
//             Console.WriteLine($"{methodName} 추출 완료");
//
//             //Console.ReadLine();
//         }
//
//         public static void GetNotSetting2FAUserByAccount(MySqlManager db)
//         {
//             var accountId = "zE25MUBRwUWQkvjzg0aw8Q";
//             StackTrace stackTrace = new StackTrace();
//             StackFrame stackFrame = stackTrace.GetFrame(0);
//             var methodName = stackFrame.GetMethod().Name;
//
//             var notSetting2FAUserList = new List<NotSetting2FAUserList>();
//
//             Console.WriteLine($"{methodName} 추출 시작");
//
//             var accountUsers = CloudConfig.SearchByDomain<AccountUserBuild>($"accounts:{accountId}:users", true).ToList();
//
//             foreach (var accountUser in accountUsers)
//             {
//                 try
//                 {
//                     var userInfo = CloudConfig.Get<UserInfo>(new UserInfo().GetKeyName(accountUser.user_id));
//                     var twoFA = CloudConfig.Get<TwoFA>(new TwoFA().GetKeyName(accountUser.user_id));
//
//                     notSetting2FAUserList.Add(new NotSetting2FAUserList()
//                     {
//                         user_id = accountUser.user_id,
//                         name = userInfo.name,
//                         email = accountUser.user.email,
//                         is_active = accountUser.is_active,
//                         type = "user",
//                         is_set_2fa = twoFA != null && twoFA.state == TwoFA.TwoFaState.enabled
//                     });
//                 }
//                 catch (Exception e)
//                 {
//                     Console.WriteLine(e.Message);
//                 }
//             }
//
//             var accountPendingUsers = CloudConfig.SearchByDomain<AccountPendingUserBuild>($"accounts:{accountId}:pending_users", true)
//                                                  .Where(x => x.state == AccountState.pending).ToList();
//
//             foreach (var accountPendingUser in accountPendingUsers)
//             {
//                 try
//                 {
//                     if (string.IsNullOrEmpty(accountPendingUser.invite_user_id))
//                     {
//                         notSetting2FAUserList.Add(new NotSetting2FAUserList()
//                         {
//                             user_id = "",
//                             name = "",
//                             email = accountPendingUser.invite_user_email,
//                             is_active = accountPendingUser.is_active,
//                             type = "pending_user",
//                             is_set_2fa = false
//                         });
//                     }
//                     else
//                     {
//                         var userInfo = CloudConfig.Get<UserInfo>(new UserInfo().GetKeyName(accountPendingUser.invite_user_id));
//                         var twoFA = CloudConfig.Get<TwoFA>(new TwoFA().GetKeyName(accountPendingUser.invite_user_id));
//                         notSetting2FAUserList.Add(new NotSetting2FAUserList()
//                         {
//                             user_id = accountPendingUser.invite_user_id,
//                             name = userInfo.name,
//                             email = accountPendingUser.invite_user_email,
//                             is_active = accountPendingUser.is_active,
//                             type = "pending_user",
//                             is_set_2fa = twoFA != null && twoFA.state == TwoFA.TwoFaState.enabled
//                         });
//                     }
//                 }
//                 catch (Exception e)
//                 {
//                     Console.WriteLine(e.Message);
//                 }
//             }
//
//             string currentMethod = string.Concat(methodName.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
//             string pathTemplate = @"c:\temp\" + currentMethod + "_{0}.csv";
//             int i = -1;
//             do
//             {
//                 var path = String.Format(pathTemplate, $"({++i})");
//                 if (!File.Exists(path))
//                 {
//                     File.WriteAllText(path, notSetting2FAUserList.ToCsv(), Encoding.UTF8);
//                     break;
//                 }
//             } while (true);
//
//             Console.WriteLine($"{methodName} 추출 완료");
//
//             //Console.ReadLine();
//         }
//     }
// }