using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using adbrix.v2.backend.library.models.models.meta.accounts.audience;
using adbrix.v2.backend.library.models.models.meta.apps.info;
using adbrix.v2.backend.library.models.models.meta.users;
using CsvHelper;
using CsvHelper.Configuration;
using DFN.Report.Common;
using DFN.Report.csv.DynamicPath;
using Newtonsoft.Json;

namespace DFN.Report.Apps
{
    public class AppCompetitionReportService
    {
        private readonly MySqlManager _db;
        private readonly DataService _dataService;

        public AppCompetitionReportService(
            MySqlManager db,
            DataService dataService)
        {
            _db = db;
            _dataService = dataService;
        }

        public Tuple<List<string>, List<string>, List<string>, List<AppCompetition>> GetAppCompetitionUsage()
        {
            var appkeys = new List<string>();
            var userIds = new List<string>();
            var userIps = new List<string>();
            var appCompetitions = new List<AppCompetition>();

            List<ConfingWorksResult> configList = _db.ExecuteDynamicQuery<ConfingWorksResult>(
                $"select config_name, data, search1, search2, search3 from configs where config_name like 'apps:%:competition'"
            );

            foreach (ConfingWorksResult config in configList)
            {
                var result = JsonCompressUtil.Decompress(config.data);
                try
                {
                    var stringList = config.config_name.Split(':');
                    var appkey = stringList[1];

                    var appCompetition = JsonConvert.DeserializeObject<AppCompetition>(result);

                    appCompetitions.Add(appCompetition);
                    if (appCompetition.created_user_id != null)
                        userIds.Add(appCompetition.created_user_id);
                    if (appCompetition.created_user_ip != null)
                        userIps.Add(appCompetition.created_user_ip);
                    appkeys.Add(appkey);
                }
                catch (Exception e)
                {
                    // Console.WriteLine(e.Message);
                }
            }

            return new Tuple<List<string>, List<string>, List<string>, List<AppCompetition>>(
                appkeys, userIds, userIps, appCompetitions
            );
        }

        public void GetAppCompetitionUsageReport()
        {
            var result = GetAppCompetitionUsage();

            var appkeys = result.Item1;
            var userIds = result.Item2;
            var userIps = result.Item3;
            var appCompetitions = result.Item4;


            var appInfoAndAccountIds = _dataService.AppInfoAndAccountIds(appkeys.Distinct().ToList());
            var appExtendAccountInfos = appInfoAndAccountIds.Item3;
            var accountIds = appInfoAndAccountIds.Item2;
            var accountInfos = _dataService.AccountInfos(accountIds.Distinct().ToList());
            var userInfos = _dataService.GetUserInfos(userIds.Distinct().ToList());
            var userIpList = userIps.Distinct().ToList();

            // 최종 아웃풋
            Console.WriteLine("account_id,account_name,appkey,app_name,is_demo,경쟁앱1,경쟁앱2,경쟁앱3,경쟁앱4,경쟁앱5,user_id,user_email,ip,생성일,업데이트일");

            appCompetitions.ForEach(competition =>
            {
                var stringList = competition._key.Split(':');
                var appkey = stringList[1];
                var appInfo = appExtendAccountInfos.FirstOrDefault(x => x.appkey == appkey);
                var accountInfo = accountInfos.FirstOrDefault(x => x.account_id == appInfo?.account_id);
                var userInfo = userInfos.FirstOrDefault(x => x.user_id == competition.created_user_id);

                Console.Write($"{accountInfo.account_id},{accountInfo.name},{appkey},\"{appInfo.name}\",{appInfo.is_demo}");
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        Console.Write($",\"{competition.app_competition_list[i].app_name}\"");
                    }
                    catch (Exception e)
                    {
                        Console.Write(",");
                    }
                }

                Console.Write($",{userInfo?.user_id},{userInfo?.email},{competition?.created_user_ip},{competition.created_datetime},{competition.last_updated_datetime}");
                Console.WriteLine();
            });
        }

        public Dictionary<string, string> GetAudienceIdWithReadCSV()
        {
            var audiIds = new Dictionary<string, string>();
            using (var reader = new StreamReader("../../../csv/DynamicPath/path.csv"))
            using (var csv = new CsvReader(reader, new Configuration()
                   {
                       Delimiter = ",",
                       QuoteAllFields = true,
                       Quote = '"',
                       HasHeaderRecord = true
                   }))
            {
                var records = csv.GetRecords<DynamicPathCSV>();
                foreach (var dynamicPathCsv in records)
                {
                    var dynamicPaths = dynamicPathCsv.dynamic_path.Split("/");
                    var audienceId = dynamicPaths[5];
                    var appkey = dynamicPaths[3];
                    if (!audiIds.ContainsKey(audienceId))
                        audiIds.Add(audienceId, appkey);
                }
            }

            return audiIds;
        }

        public Tuple<List<string>, List<string>, List<string>, List<AppAudienceMeta>> AppAudienceMetas(Dictionary<string, string> dics)
        {
            var appkeys = new List<string>();
            var userIds = new List<string>();
            var userIps = new List<string>();
            var appAudienceMetas = new List<AppAudienceMeta>();
            var where = new List<string>();
            foreach (var keyValuePair in dics)
            {
                where.Add($"'apps:{keyValuePair.Value}:audience:{keyValuePair.Key}'");
            }

            var baseSql = $"select config_name, data, search1, search2, search3 from configs where config_name";
            var sql = $"{baseSql} in ({string.Join(",", where)})";

            var configList = _db.ExecuteDynamicQuery<ConfingWorksResult>(sql);
            foreach (ConfingWorksResult config in configList)
            {
                var result = JsonCompressUtil.Decompress(config.data);
                try
                {
                    var appAudienceMeta = JsonConvert.DeserializeObject<AppAudienceMeta>(result);
                    appAudienceMetas.Add(appAudienceMeta);

                    if (appAudienceMeta.created_user_id != null)
                        userIds.Add(appAudienceMeta.created_user_id);
                    if (appAudienceMeta.created_user_ip != null)
                        userIps.Add(appAudienceMeta.created_user_ip);
                    appkeys.Add(appAudienceMeta.appkey);
                }
                catch (Exception e)
                {
                    // Console.WriteLine(e.Message);
                }
            }

            return new Tuple<List<string>, List<string>, List<string>, List<AppAudienceMeta>>(
                appkeys, userIds, userIps, appAudienceMetas
            );
        }

        public void ParserConsoleDynamicPathRouter()
        {
            var audiIds = GetAudienceIdWithReadCSV();

            var result = AppAudienceMetas(audiIds);

            var appkeys = result.Item1;
            var userIds = result.Item2;
            var userIps = result.Item3;
            var audienceMetas = result.Item4;

            var appInfoAndAccountIds = _dataService.AppInfoAndAccountIds(appkeys.Distinct().ToList());
            var appExtendAccountInfos = appInfoAndAccountIds.Item3;
            var accountIds = appInfoAndAccountIds.Item2;
            var accountInfos = _dataService.AccountInfos(accountIds.Distinct().ToList());
            var userInfos = _dataService.GetUserInfos(userIds.Distinct().ToList());
            var userIpList = userIps.Distinct().ToList();

            // 최종 아웃풋
            Console.WriteLine("appkey,app_name,audience_id,audience_name,스켸쥴타입,is_preset,user_id,user_email,ip,생성일,업데이트일");

            audienceMetas.ForEach(audienceMeta =>
            {
                var appkey = audienceMeta.appkey;
                var appInfo = appExtendAccountInfos.FirstOrDefault(x => x.appkey == appkey);
                var accountInfo = accountInfos.FirstOrDefault(x => x.account_id == appInfo?.account_id);
                var userInfo = userInfos.FirstOrDefault(x => x.user_id == audienceMeta.created_user_id);

                Console.WriteLine($"{audienceMeta.appkey},{appInfo.name}, {audienceMeta.audience_id},\"{audienceMeta.name}\", {audienceMeta.scheduler_type}, {audienceMeta.audience_created_type},{userInfo?.user_id},{userInfo?.email},{audienceMeta?.created_user_ip},{audienceMeta.created_datetime},{audienceMeta.last_updated_datetime}");
            });
        }

        public Tuple<Dictionary<string, string>, List<DynamicPathCSV>> GetAudienceReportViewWithReadCSV()
        {
            var dynamicPathList = new List<DynamicPathCSV>();
            var audiIds = new Dictionary<string, string>();
            using (var reader = new StreamReader("../../../csv/DynamicPath/audi_insights_view.csv"))
            using (var csv = new CsvReader(reader, new Configuration()
                   {
                       Delimiter = ",",
                       QuoteAllFields = true,
                       Quote = '"',
                       HasHeaderRecord = true
                   }))
            {
                var records = csv.GetRecords<DynamicPathCSV>();
                foreach (var dynamicPathCsv in records)
                {
                    var dynamicPaths = dynamicPathCsv.dynamic_path.Split("/");
                    var audienceId = dynamicPaths[5];
                    var appkey = dynamicPaths[3];
                    if (!audiIds.ContainsKey(audienceId))
                        audiIds.Add(audienceId, appkey);
                    dynamicPathList.Add(dynamicPathCsv);
                }
            }

            return new Tuple<Dictionary<string, string>, List<DynamicPathCSV>>(audiIds, dynamicPathList);
        }

        public void ParserConsoleAudienceInsight()
        {
            var result1 = GetAudienceReportViewWithReadCSV();

            var audiIds = result1.Item1;

            var result = AppAudienceMetas(audiIds);

            var appkeys = result.Item1;
            var userIds = result.Item2;
            var userIps = result.Item3;
            var audienceMetas = result.Item4;

            var appInfoAndAccountIds = _dataService.AppInfoAndAccountIds(appkeys.Distinct().ToList());
            var appExtendAccountInfos = appInfoAndAccountIds.Item3;
            var accountIds = appInfoAndAccountIds.Item2;
            var accountInfos = _dataService.AccountInfos(accountIds.Distinct().ToList());
            var userInfos = _dataService.GetUserInfos(userIds.Distinct().ToList());
            var userIpList = userIps.Distinct().ToList();

            // 최종 아웃풋
            Console.WriteLine("daily,appkey,dynamic_page_name,dynamic_path,unique_event_count,total_event_count,page_name,app_name,audience_id,audience_name");
            
            result1.Item2.ForEach(item =>
            {
                var dynamicPaths = item.dynamic_path.Split("/");
                var audienceId = dynamicPaths[5];
                var appInfo = appExtendAccountInfos.FirstOrDefault(x => x.appkey == item.dynamic_path_appkey);
                var audienceMeta = audienceMetas.FirstOrDefault(x => x.audience_id == audienceId);

                Console.WriteLine($"{item.daily}, {item.dynamic_path_appkey}, {item.dynamic_path_page_name}, {item.dynamic_path}, {item.unique_event_count},{item.total_event_count},{item.page_name}, {appInfo.name},{audienceMeta.audience_id},{audienceMeta.name}");
            });
        }
    }
}