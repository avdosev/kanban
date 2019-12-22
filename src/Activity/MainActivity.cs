using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Content;

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

    // Implement the ViewHolder pattern: each ViewHolder holds references
    // to the UI components within the CardView 
    // that is displayed in a row of the RecyclerView:
    public class KanbanViewHolder : RecyclerView.ViewHolder {
        public Android.Widget.TextView Caption { get; private set; }
        public Guid KanbanId { get; set; }

        // Get references to the views defined in the CardView layout.
        public KanbanViewHolder(View itemView, Action<int> listener)
            : base(itemView) {
            // Locate and cache view references:
            Caption = itemView.FindViewById<Android.Widget.TextView>(Resource.Id.title_textview);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }

    // ADAPTER
    // Adapter to connect the data set to the RecyclerView: 
    public class KanbanReposAdapter : RecyclerView.Adapter {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Load the adapter with the data set (photo album) at construction time:
        public KanbanReposAdapter() {

        }

        // Create a new CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.kanban_cardview, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            KanbanViewHolder vh = new KanbanViewHolder(itemView, OnClick);
            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            KanbanViewHolder vh = holder as KanbanViewHolder;
            // Set the ImageView and TextView in this ViewHolder's CardView 
            // from this position in the photo album:
            var item = DataBase.db.Table<Kanbans>().ElementAt(position);
            vh.KanbanId = item.id;
            vh.Caption.Text = item.Title;
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount {
            get => DataBase.db.Table<Kanbans>().Count();
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position) {
            ItemClick?.Invoke(this, position);
        }
    }
}

