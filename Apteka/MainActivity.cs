using Org.Apache.Http.Client;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Android.Widget;
using Android.Views;
using Android.Util;
using static Android.Renderscripts.ScriptGroup;
using static Android.Renderscripts.Sampler;

namespace Apteka
{
    public class Product
    {
        public string NameMedicine { get; set; }
        public string Warehouse { get; set; }
        public int Value { get; set; }
    }


    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            CreateTab();
            Connects();
        }

        private void CreateTab()
        {
            TabHost tabhost = FindViewById<TabHost>(Resource.Id.tabHost1);
            tabhost.Setup();

            TabHost.TabSpec spec = tabhost.NewTabSpec("1");
            spec.SetContent(Resource.Id.tab1);
            spec.SetIndicator("1");
            tabhost.AddTab(spec);

            spec = tabhost.NewTabSpec("2");
            spec.SetContent(Resource.Id.tab2);
            spec.SetIndicator("2");
            tabhost.AddTab(spec);

            spec = tabhost.NewTabSpec("3");
            spec.SetContent(Resource.Id.tab3);
            spec.SetIndicator("3");
            tabhost.AddTab(spec);
        }

        private void Connects()
        {
            FindViewById<Button>(Resource.Id.button1).Click += LoadingDataProduct;
        }

        private async void LoadingDataProduct(object sender, EventArgs e)
        {
            string json = await LoadData();

            //������������� ��������� Newtonsoft.Json(���� ���������)
            //List<Product> products = JsonConvert.DeserializeObject<List<Product>>(json);

            //������ ��� ����������
            List<string> parseJson = ParseJson(json);
            List<Product> products = StringToProduct(parseJson);

            Product_FillingInData(products, FindViewById<TableLayout>(Resource.Id.tableLayout1));
        }

        private async Task<string> LoadData()
        {
            string json = "";
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                HttpClient client = new HttpClient(handler);
                client.BaseAddress = new Uri("https://192.168.0.103:7060/");

                HttpResponseMessage response = await client.GetAsync("Apteka");

                json = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                await Console.Out.WriteLineAsync($"������ ��� ���������� �������: {ex.Message}");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"������: {ex.Message}");
            }

            return json;
        }

        //���� Json ������ � ����� ������
        private List<string> ParseJson(string json)
        {
            List<string> setData = new List<string>();

            // ������� ��������� � �������� ������ �� JSON-������
            json = json.Trim('[', ']');

            // ��������� ������ JSON �� ��������� �������� �������
            string[] items = json.Split(new string[] { "},{" }, StringSplitOptions.None);

            foreach (var item in items)
            {
                // ������� �������� ������� �� �������� �������
                string cleanItem = item.Trim('{', '}');

                // ��������� ������� ������� �� ��������� ���� ����-��������
                string[] keyValuePairs = cleanItem.Split(',');

                string result = "";
                foreach (var pair in keyValuePairs)
                {
                    // ��������� ���� ����-�������� �� ���� � ��������
                    string[] keyValue = pair.Split(':');

                    // ������� ������� �� ����� � ��������
                    string key = keyValue[0].Trim('"');
                    string value = keyValue[1].Trim('"');

                    result += value + ", ";
                }
                setData.Add(result);
            }

            return setData;
        }

        //���������� ������� �������
        private void Product_FillingInData(List<Product> products, TableLayout tableLayout)
        {
            tableLayout.RemoveViews(1, tableLayout.ChildCount - 1);

            foreach (var product in products)
            {
                TableRow row = new TableRow(this);

                TextView idTextView = new TextView(this);
                idTextView.Text = product.NameMedicine;
                SetStyleCellDataTable(idTextView);

                TextView nameTextView = new TextView(this);
                nameTextView.Text = product.Warehouse;
                SetStyleCellDataTable(nameTextView);

                TextView valueTextView = new TextView(this);
                valueTextView.Text = product.Value.ToString();
                SetStyleCellDataTable(valueTextView);

                row.AddView(idTextView);
                row.AddView(nameTextView);
                row.AddView(valueTextView);

                tableLayout.AddView(row);
            }
        }

        //��������� ����� ��� ������ ������
        private void SetStyleCellDataTable(TextView textView)
        {
            textView.SetPadding(0, 0, 0, 6);
            textView.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
            textView.Gravity = GravityFlags.Center; 
            textView.SetMinWidth(30);
            textView.SetMinHeight(30);
        }

        //������� ������ � ������� � ������ ������
        private List<Product> StringToProduct(List<string> dataSet)
        {
            List<Product> products = new List<Product>();

            foreach (var data in dataSet)
            {
                string[] productData = data.Split(", ");

                Product product = new Product();

                foreach (var pair in data)
                {
                    product.NameMedicine = productData[0];
                    product.Warehouse = productData[1];
                    product.Value = int.Parse(productData[2]);
                }

                products.Add(product);
            }

            return products;
        }
    }
}