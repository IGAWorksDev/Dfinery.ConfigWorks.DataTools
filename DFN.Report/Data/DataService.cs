using System;
using System.Collections.Generic;
using adbrix.v2.backend.library.models.models.meta.accounts;
using adbrix.v2.backend.library.models.models.meta.accounts.audience;
using adbrix.v2.backend.library.models.models.meta.apps.info;
using adbrix.v2.backend.library.models.models.meta.users;
using Newtonsoft.Json;

namespace DFN.Report.Common
{
    public class AppInfoExtendAccountId : AppInfo
    {
        public string account_id { get; set; }
    }

    public class DataService
    {
        public static readonly List<string> FilterIPs = new List<string>()
        {
            "127.0.0.1", // 로컬
            "106.241.27.81" // 회사
        };

        private readonly MySqlManager _db;

        public DataService(MySqlManager db)
        {
            _db = db;
        }

        public List<AccountInfo> AccountInfos(List<string> accountIds)
        {
            var accountInfos = new List<AccountInfo>();
            var where = new List<string>();
            accountIds.ForEach(accountId => { where.Add($"'accounts:{accountId}'"); });

            var baseSql = $"select config_name, data, search1, search2, search3 from configs where config_name";
            var sql = $"{baseSql} in ({string.Join(",", where)})";

            var configList = _db.ExecuteDynamicQuery<ConfingWorksResult>(sql);
            foreach (ConfingWorksResult config in configList)
            {
                var result = JsonCompressUtil.Decompress(config.data);
                try
                {
                    var appInfo = JsonConvert.DeserializeObject<AccountInfo>(result);
                    accountInfos.Add(appInfo);
                }
                catch (Exception e)
                {
                    // Console.WriteLine(e.Message);
                }
            }

            return accountInfos;
        }

        public Tuple<List<AppInfo>, List<string>, List<AppInfoExtendAccountId>> AppInfoAndAccountIds(List<string> appkeys)
        {
            var accountIds = new List<string>();
            var appInfos = new List<AppInfo>();
            var appInfoExtendAccountIds = new List<AppInfoExtendAccountId>();
            var where = new List<string>();
            appkeys.ForEach(appkey => { where.Add($"'apps:{appkey}:info'"); });

            var baseSql = $"select config_name, data, search1, search2, search3 from configs where config_name";
            var sql = $"{baseSql} in ({string.Join(",", where)})";

            var configList = _db.ExecuteDynamicQuery<ConfingWorksResult>(sql);
            foreach (ConfingWorksResult config in configList)
            {
                var stringList = config.search1.Split(':');
                var accountId = stringList[1];
                var result = JsonCompressUtil.Decompress(config.data);
                try
                {
                    var appInfo = JsonConvert.DeserializeObject<AppInfo>(result);
                    appInfos.Add(appInfo);
                    accountIds.Add(accountId);

                    var appInfoExtendAccountId = JsonConvert.DeserializeObject<AppInfoExtendAccountId>(result);
                    appInfoExtendAccountId.account_id = accountId;

                    appInfoExtendAccountIds.Add(appInfoExtendAccountId);
                }
                catch (Exception e)
                {
                    // Console.WriteLine(e.Message);
                }
            }

            return new Tuple<List<AppInfo>, List<string>, List<AppInfoExtendAccountId>>(appInfos, accountIds, appInfoExtendAccountIds);
        }

        public List<UserInfo> GetUserInfos(List<string> userIds)
        {
            var userInfos = new List<UserInfo>();
            var where = new List<string>();
            userIds.ForEach(userId => { where.Add($"'users:{userId}'"); });

            var baseSql = $"select config_name, data, search1, search2, search3 from configs where config_name";
            var sql = $"{baseSql} in ({string.Join(",", where)})";

            var configList = _db.ExecuteDynamicQuery<ConfingWorksResult>(sql);
            foreach (ConfingWorksResult config in configList)
            {
                var result = JsonCompressUtil.Decompress(config.data);
                try
                {
                    var userInfo = JsonConvert.DeserializeObject<UserInfo>(result);
                    userInfos.Add(userInfo);
                }
                catch (Exception e)
                {
                    // Console.WriteLine(e.Message);
                }
            }

            return userInfos;
        }

        public List<AppAudienceMeta> AppAudienceMetas(Dictionary<string, string> dics)
        {
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
                    var appInfo = JsonConvert.DeserializeObject<AppAudienceMeta>(result);
                    appAudienceMetas.Add(appInfo);
                }
                catch (Exception e)
                {
                    // Console.WriteLine(e.Message);
                }
            }

            return appAudienceMetas;
        }
    }
}