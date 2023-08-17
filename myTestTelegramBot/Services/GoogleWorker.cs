using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using myTestTelegramBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace myTestTelegramBot.Services
{
    public class GoogleWorker
    {
        public static async Task Add(TransactionModel transaction)
        {
            try
            {
                string[] Scopes = { SheetsService.Scope.Spreadsheets };
                var clientSecrets = new ClientSecrets
                {
                    ClientId = "252432177845-q906ditfr1idohcv9lpfkotejapu93vn.apps.googleusercontent.com",
                    ClientSecret = "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"
                };
                var res = GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, Scopes, "user", CancellationToken.None, new FileDataStore("MyAppsToken"));
                var baseClientService = new BaseClientService.Initializer()
                {
                    HttpClientInitializer = res.Result,
                    ApplicationName = "Google Sheets API .NET Quickstart"
                };
                var service = new SheetsService(baseClientService);


                SpreadsheetsResource.ValuesResource.GetRequest getRequest = service.Spreadsheets.Values.Get("1MMJnUAx7IFlQB4Yg-bbAypuhlAlIFlRc1eT3cwi4-Jk", "tr!A:E");
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
                ValueRange getResponse = getRequest.Execute();
                IList<IList<object>> values = getResponse.Values;
                var range = $"{"tr"}!A" + (values.Count + 1) + ":E" + (values.Count + 1);


                var valueRange = new ValueRange();
                valueRange.Values = new List<IList<object>> { new List<object>() { transaction.Date, transaction.Text } };
                var updateRequest = service.Spreadsheets.Values.Update(valueRange, "1MMJnUAx7IFlQB4Yg-bbAypuhlAlIFlRc1eT3cwi4-Jk", range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                var updateResponse = updateRequest.Execute();
            }
            catch
            {
                throw new Exception("Addition error!");
            }
        }
    }
}
