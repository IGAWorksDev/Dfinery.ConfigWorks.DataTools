using System;
using DFN.Report.Apps;
using ConfigWorks.sdk;
using DFN.Report.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DFN.Report
{
    public class ConfingWorksResult
    {
        public string config_name { get; set; }
        public byte[] data { get; set; }
        public string search1 { get; set; }
        public string search2 { get; set; }
        public string search3 { get; set; }

        public DateTime last_update { get; set; }
    }

    public class internal_configworks_configs
    {
        public string config_name { get; set; }
        public int version { get; set; }
        public string hash { get; set; }
        public string search1 { get; set; }
        public ulong is_buildable { get; set; }
        public string search2 { get; set; }
        public string search3 { get; set; }
        public ulong is_build { get; set; }
        public string domain { get; set; }

        public DateTime last_update { get; set; }
        public byte[] data { get; set; }
    }

    public class Program
    {
        static string PROD_END_POINT = "http://configworksweb-prod.ap-northeast-1.elasticbeanstalk.com/";
        static string PROD_BACKUP_END_POINT = "http://configworksweb-prod.ap-northeast-1.elasticbeanstalk.com/";
        static string DEV_END_POINT = "http://internal-adbrix-configworks-app-elb-1468948642.ap-northeast-1.elb.amazonaws.com/";

        static string PROD_BACKUP_DB_CONNECTION_STRING = "Server=abx-rm-config-prod-cluster-1.cluster-cplt90o0ehi3.ap-northeast-1.rds.amazonaws.com;Uid=igaworks_adbrix;Pwd=adbrix_Lab2018;database=configworks_live;Charset=utf8";
        static string PROD_DB_CONNECTION_STRING = "Server=abx-rm-config-prod-new-cluster.cluster-cplt90o0ehi3.ap-northeast-1.rds.amazonaws.com;Uid=igaworks_adbrix;Pwd=adbrix_Lab2018;database=configworks_live;Charset=utf8";

        static string DEV_DB_CONNECTION_STRING = "Server=igaw-common-auth.cplt90o0ehi3.ap-northeast-1.rds.amazonaws.com;Uid=administrator;Pwd=WkdrkxornjsV1;database=configworks_dev;Charset=utf8";


        public enum configworksEnv
        {
            live,
            dev,
            live_backup
        }


        static void Main(string[] args)
        {
            configworksEnv configworks = configworksEnv.live;

            string api_endpoint;
            string db_connection_str;

            switch (configworks)
            {
                case configworksEnv.live:
                    api_endpoint = PROD_END_POINT;
                    db_connection_str = PROD_DB_CONNECTION_STRING;
                    break;

                case configworksEnv.dev:
                    api_endpoint = DEV_END_POINT;
                    db_connection_str = DEV_DB_CONNECTION_STRING;
                    break;
                default:
                    api_endpoint = "";
                    db_connection_str = "";
                    break;
            }

            CloudConfig.Setup(api_endpoint, 0, true, false);
            
            // 서비스 추가
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(new MySqlManager(db_connection_str));
            services.AddSingleton<AppCompetitionReportService>();
            services.AddSingleton<DataService>();
            
            // 실행코드
            var serviceProvider = services.BuildServiceProvider();
            var appCompetitionReportService = serviceProvider.GetService<AppCompetitionReportService>();
            
            appCompetitionReportService.ParserConsoleAudienceInsight();
        }
    }
}
