using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Content;


using src.KanbanRecyclerView;
using DataBase;

namespace src
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        RecyclerView.LayoutManager mLayoutManager;
        RecyclerView KanbansView;
        KanbanReposAdapter kanbanReposAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            KanbansView = FindViewById<RecyclerView>(Resource.Id.ProjectsView);
            mLayoutManager = new LinearLayoutManager(this);
            KanbansView.SetLayoutManager(mLayoutManager);

            kanbanReposAdapter = new KanbanReposAdapter();

            // Register the item click handler (below) with the adapter:
            kanbanReposAdapter.ItemClick += OnKanbanClick;

            // Plug the adapter into the RecyclerView:
            KanbansView.SetAdapter(kanbanReposAdapter);

        }

        void OnKanbanClick(object sender, int position) {
            // Display a toast that briefly shows the enumeration of the selected photo:
            // var KanbanHolder = sender as KanbanViewHolder;
            // Console.WriteLine(KanbanHolder.KanbanId);
            var item = DataBase.db.Table<Kanbans>().ElementAt(position);
            if (item == null) {
                return;
            }

            var id = item.id;
            this.ToKanbanActivity(id);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs) {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetView(Resource.Layout.create_kanban);
            
            builder
                .SetTitle("Creating kanban")
                .SetPositiveButton(Resource.String.create, (send, args) => {
                    var view = send as Dialog;
                    var titleTextView = view.FindViewById<Android.Widget.TextView>(Resource.Id.kanban_title);
                    Console.WriteLine("OK");
                    Console.WriteLine(titleTextView.Text);
                    var kanban = new Kanbans {
                        Title = titleTextView.Text
                    };
                    DataBase.db.Insert(kanban);
                }).SetNegativeButton(Resource.String.cancel, (send, args) => {
                    Console.WriteLine("CANCEL");
                });

            builder.Create().Show();
        }

        public void ToKanbanActivity(Guid kanbanId) {
            var intent = new Intent(this, typeof(KanbanActivity));
            intent.PutExtra("kanbanId", kanbanId.ToString());

            StartActivity(intent);
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}

    
}

