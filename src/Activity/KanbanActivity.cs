using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Content;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V4.App;
using Android.Views;

using DataBase;

namespace src {

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    class KanbanActivity : AppCompatActivity {
        protected Guid kanbanId;
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            kanbanId = Guid.Parse(Intent.GetStringExtra("kanbanId"));

            SetContentView(Resource.Layout.kanban_activity);

            var tabbar = FindViewById<TabLayout>(Resource.Id.tab_layout);

            tabbar.AddTab(tabbar.NewTab().SetText("test1"));
            tabbar.AddTab(tabbar.NewTab().SetText("test2"));
            tabbar.AddTab(tabbar.NewTab().SetText("test3"));

            var viewPager = FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.viewpager);
            viewPager.Adapter = new ColumnPagerAdapter(SupportFragmentManager, this);

            tabbar.SetupWithViewPager(viewPager);
        }
    }

    public class ColumnPageFragment : Android.Support.V4.App.Fragment {
        private Context context;
        private const string ARG_PAGE = "ARG_PAGE";
        public Guid KanbanId {
            get; set;
        }

        private int mPage;

        public static ColumnPageFragment newInstance(int page, Context context) {
            Bundle args = new Bundle();
            args.PutInt(ARG_PAGE, page);
            ColumnPageFragment fragment = new ColumnPageFragment();
            fragment.Arguments = args;
            fragment.context = context;
            return fragment;
        }

        override public void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            if (Arguments != null) {
                mPage = Arguments.GetInt(ARG_PAGE);
                Console.WriteLine(mPage);
            }
        }

        override public View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            Console.WriteLine("creating view");
            View view = inflater.Inflate(Resource.Layout.kanban_content, container, false);

            // Making recicler view
            var ColumnView = view.FindViewById<RecyclerView>(Resource.Id.ColumnView);
            var mLayoutManager = new LinearLayoutManager(context);
            ColumnView.SetLayoutManager(mLayoutManager);

            var columnId = DataBase.db.Table<Columns>().Where((column) => column.KanbanId == KanbanId).ElementAt(mPage).id;
            var ticketReposAdapter = new TicketReposAdapter(columnId, KanbanId);

            // Plug the adapter into the RecyclerView:
            ColumnView.SetAdapter(ticketReposAdapter);

            return view;
        }
    }

   

    public class ColumnPagerAdapter : FragmentPagerAdapter {
        const int PAGE_COUNT = 3;
        private string[] tabTitles = new string[] { "Tab1", "Tab2", "Tab3" };
        Context context;
        public override int Count {
            get { return PAGE_COUNT; }
        }

        public ColumnPagerAdapter(Android.Support.V4.App.FragmentManager fm, Context context) : base(fm) {
            this.context = context;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position) {
            return new Java.Lang.String(tabTitles[position]);
        }

        public override Android.Support.V4.App.Fragment GetItem(int position) {
            return ColumnPageFragment.newInstance(position, context);
        }
    }

    public class TicketViewHolder : RecyclerView.ViewHolder {
        public Android.Widget.TextView textView { get; private set; }
        public Guid TicketId { get; set; }

        // Get references to the views defined in the CardView layout.
        public TicketViewHolder(View itemView /* , Action<int> listener */)
            : base(itemView) {
            // Locate and cache view references:
            textView = itemView.FindViewById<Android.Widget.TextView>(Resource.Id.ticket_text_textview);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            // itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }

    // ADAPTER
    // Adapter to connect the data set to the RecyclerView: 
    public class TicketReposAdapter : RecyclerView.Adapter {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;


        private Guid ColumnId;
        private Guid KanbanId;

        public TicketReposAdapter(Guid ColumnID, Guid KanbanID) {
            ColumnId = ColumnID;
            KanbanId = KanbanID;
        }

        // Create a new CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.ticket_cardview, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            TicketViewHolder vh = new TicketViewHolder(itemView);
            return vh;
        }

        
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            TicketViewHolder vh = holder as TicketViewHolder;
            // Set the ImageView and TextView in this ViewHolder's CardView 
            var item = DataBase.db.Table<Ticket>().Where((column) => column.ColumnKanbanId == KanbanId && column.ColumnId == ColumnId).ElementAt(position);
            vh.textView.Text = item.Text;
            vh.TicketId = item.id;
        }

        
        public override int ItemCount {
            get => DataBase.db.Table<Ticket>().Where((column) => column.ColumnKanbanId == KanbanId && column.ColumnId == ColumnId).Count();
        }

        
    }
}