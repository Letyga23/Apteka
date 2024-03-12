using Android.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Apteka
{
    public class Medicines
    {
        public string NameMedicine { get; set; }
        public string Warehouse { get; set; }
        public int Value { get; set; }
    }
    internal class APIReader
    {
        private static readonly string url = "https://192.168.0.103:7060";
        private static readonly HttpClient client = SettingHttpClient();

        public static HttpClient SettingHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            HttpClient client = new HttpClient(handler);
            return client;
        }

        public static async Task<Medicines> getMedicine(int id)
        {
            if (!await canConnectToAPI())
                return null;

            Medicines medicine = null;
            HttpResponseMessage response = await client.GetAsync(url + $"/Apteka/{id}"); //Нет такого метода в моей API
            if (response.IsSuccessStatusCode)
            {
                medicine = await response.Content.ReadFromJsonAsync<Medicines>();
            }
            return medicine;
        }

        public static async Task<List<Medicines>> getMedicines()
        {
            if (!await canConnectToAPI())
                return null;

            List<Medicines> medicines = null;
            HttpResponseMessage response = await client.GetAsync(url + "/Apteka");
            await Console.Out.WriteLineAsync(url + "/Apteka");
            if (response.IsSuccessStatusCode)
            {
                medicines = await response.Content.ReadFromJsonAsync<List<Medicines>>();
            }
            return medicines;
        }

        public static async Task<bool> canConnectToAPI()
        {
            try
            {
                await client.GetAsync(url);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
