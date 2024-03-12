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
            spec.SetIndicator("Tab 1");
            spec.SetContent(Resource.Id.tab1);
            tabhost.AddTab(spec);

            spec = tabhost.NewTabSpec("2");
            spec.SetIndicator("Tab 2");
            spec.SetContent(Resource.Id.tab2);
            tabhost.AddTab(spec);

            spec = tabhost.NewTabSpec("3");
            spec.SetIndicator("Tab 3");
            spec.SetContent(Resource.Id.tab3);
            tabhost.AddTab(spec);
        }

        private void Connects()
        {
            FindViewById<Button>(Resource.Id.button1).Click += LoadingDataProduct;
        }

        private async void LoadingDataProduct(object sender, EventArgs e)
        {
            if (await APIReader.canConnectToAPI())
            {
                Toast.MakeText(Application.Context, "Началась загрузка", ToastLength.Short).Show();

                //Использование бибиотеки Newtonsoft.Json(надо скачивать)
                //List<Product> products = JsonConvert.DeserializeObject<List<Product>>(json);

                //Способ без библиотеки
                List<Medicines> medicines = await APIReader.getMedicines();

                Product_FillingInData(medicines, FindViewById<TableLayout>(Resource.Id.tableLayout1));
            }
            else
                Toast.MakeText(Application.Context, "Данные не могут быть получены", ToastLength.Short).Show();
        }
      
        //Заполнение таблицы данными
        private void Product_FillingInData(List<Medicines> medicines, TableLayout tableLayout)
        {
            tableLayout.RemoveViews(0, tableLayout.ChildCount);

            foreach (var medicine in medicines)
            {
                TableRow row = new TableRow(this);

                TextView idTextView = new TextView(this);
                Console.WriteLine(medicine.NameMedicine);
                idTextView.Text = medicine.NameMedicine;
                SetStyleCellDataTable(idTextView);

                TextView nameTextView = new TextView(this);
                nameTextView.Text = medicine.Warehouse;
                SetStyleCellDataTable(nameTextView);

                TextView valueTextView = new TextView(this);
                valueTextView.Text = medicine.Value.ToString();
                SetStyleCellDataTable(valueTextView);

                row.AddView(idTextView);
                row.AddView(nameTextView);
                row.AddView(valueTextView);

                tableLayout.AddView(row);
            }

            Toast.MakeText(Application.Context, "Данные успешно загружены", ToastLength.Short).Show();
        }

        //Установка стиля для ячейки данных
        private void SetStyleCellDataTable(TextView textView)
        {
            textView.SetPadding(0, 0, 0, 6);
            textView.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
            //textView.Gravity = GravityFlags.Center; 
            textView.SetMinWidth(30);
            textView.SetMinHeight(30);
        }
    }
}