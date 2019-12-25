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
using SQLiteNetExtensions.Extensions;

namespace src {

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    class KanbanActivity : AppCompatActivity {
        protected Guid kanbanId;
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            kanbanId = Guid.Parse(Intent.GetStringExtra("kanbanId"));

            SetContentView(Resource.Layout.kanban_activity);

            var tabbar = FindViewById<TabLayout>(Resource.Id.tab_layout);

            var viewPager = FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.viewpager);
            viewPager.Adapter = new ColumnPagerAdapter(SupportFragmentManager, this, kanbanId);

            tabbar.SetupWithViewPager(viewPager);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick; ;
        }

        private void FabOnClick(object sender, EventArgs e) {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetView(Resource.Layout.create_ticket);

            builder
                .SetTitle("Creating ticket")
                .SetPositiveButton(Resource.String.create, (send, args) => {
                    var view = send as Dialog;
                    var titleTextView = view.FindViewById<Android.Widget.TextView>(Resource.Id.ticket_text_textview);
                    var colorTextView = view.FindViewById<Android.Widget.TextView>(Resource.Id.ticket_color_textview);

                    var ticket = new Ticket {
                        ColumnKanbanId = kanbanId,
                        ColumnId = DataBase.db.Table<Columns>().Where<Columns>((column) => column.KanbanId == kanbanId).ElementAt(0).id,
                        Text = titleTextView.Text,
                        Color = colorTextView.Text
                    };
                    DataBase.db.Insert(ticket);

                    // kanbanReposAdapter.NotifyItemInserted(kanbanReposAdapter.ItemCount);

                }).SetNegativeButton(Resource.String.cancel, (send, args) => {
                    Console.WriteLine("CANCEL");
                });

            builder.Create().Show();
        }
    }

    public class ColumnPageFragment : Android.Support.V4.App.Fragment {
        private Context context;
        private TicketReposAdapter ticketReposAdapter;
        private const string ARG_PAGE = "ARG_PAGE";
        private const string ARG_KANBAN = "ARG_KANBAN";

        private Guid KanbanId;
        private int mPage;

        public delegate void ObjectDelegate(object sender);
        public event ObjectDelegate DataSetChanged;

        public static ColumnPageFragment newInstance(int page, Context context, Guid kanbanId) {
            Bundle args = new Bundle();
            args.PutInt(ARG_PAGE, page);
            args.PutString(ARG_KANBAN, kanbanId.ToString());
            ColumnPageFragment fragment = new ColumnPageFragment();
            fragment.Arguments = args;
            fragment.context = context;
            return fragment;
        }

        override public void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            if (Arguments != null) {
                mPage = Arguments.GetInt(ARG_PAGE);
                KanbanId = Guid.Parse(Arguments.GetString(ARG_KANBAN));
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
            ticketReposAdapter = new TicketReposAdapter(columnId, KanbanId);
            ticketReposAdapter.DataSetChanged += TicketReposAdapter_DataSetChanged;

            // Plug the adapter into the RecyclerView:
            ColumnView.SetAdapter(ticketReposAdapter);

            return view;
        }

        private void TicketReposAdapter_DataSetChanged(object sender) {
            DataSetChanged.Invoke(sender);
        }

        public void OnTicketReposAdapter_DataSetChanged(object sender) {
            if (ticketReposAdapter == sender) return;

            ticketReposAdapter.NotifyDataSetChanged();
        }
    }

   

    public class ColumnPagerAdapter : FragmentPagerAdapter {
        Context context;
        Guid KanbanId;

        private delegate void ObjectDelegate(object sender);
        private event ObjectDelegate DataInColumnChanged;

        public override int Count {
            get => DataBase.db.Table<Columns>().Where<Columns>((column) => column.KanbanId == KanbanId).Count();
        }

        public ColumnPagerAdapter(Android.Support.V4.App.FragmentManager fm, Context context, Guid KanbanId) : base(fm) {
            this.context = context;
            this.KanbanId = KanbanId;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position) {
            return new Java.Lang.String(DataBase.db.Table<Columns>().Where<Columns>((column) => column.KanbanId == KanbanId).ElementAt(position).Name);
        }

        public override Android.Support.V4.App.Fragment GetItem(int position) {
            var column = ColumnPageFragment.newInstance(position, context, KanbanId);
            column.DataSetChanged += Column_DataSetChanged;
            this.DataInColumnChanged += column.OnTicketReposAdapter_DataSetChanged;
            return column;
        }

        private void Column_DataSetChanged(object sender) {
            DataInColumnChanged.Invoke(sender);
        }
    }

    public class TicketViewHolder : RecyclerView.ViewHolder {
        public delegate void OnChangeDelegate(int Position);
        public event OnChangeDelegate Deleted;

        public Android.Widget.TextView textView { get; private set; }
        public Guid TicketId { get; set; }
        public string Color { get; set; }

        private Android.Widget.ImageButton arrowBackBtn, arrowForwardBtn, deleteBtn;  

        // Get references to the views defined in the CardView layout.
        public TicketViewHolder(View itemView /* , Action<int> listener */)
            : base(itemView) {
            // Locate and cache view references:
            textView = itemView.FindViewById<Android.Widget.TextView>(Resource.Id.ticket_text_textview);

            arrowBackBtn = itemView.FindViewById<Android.Widget.ImageButton>(Resource.Id.ticket_arrow_back);
            arrowForwardBtn = itemView.FindViewById<Android.Widget.ImageButton>(Resource.Id.ticket_arrow_forward);
            deleteBtn = itemView.FindViewById<Android.Widget.ImageButton>(Resource.Id.ticket_delete);
            
            itemView.Click += ItemView_Click;

            arrowBackBtn.Click += ArrowBackBtn_Click;
            arrowForwardBtn.Click += ArrowForwardBtn_Click;
            deleteBtn.Click += DeleteBtn_Click;
        }

        private void DeleteBtn_Click(object sender, EventArgs e) {
            DataBase.db.Delete<Ticket>(TicketId);
            Deleted(LayoutPosition);
        }

        private void ArrowForwardBtn_Click(object sender, EventArgs e) {
            var ticket = DataBase.db.Get<Ticket>(TicketId);
            var columns = DataBase.db.Table<Columns>().Where<Columns>((column) => column.KanbanId == ticket.ColumnKanbanId).ToList();
            var columnsAfterCurrent = columns.SkipWhile((column) => column.id != ticket.ColumnId).ToList();
            if (columnsAfterCurrent.Count >= 2) {
                ticket.ColumnId = columnsAfterCurrent[1].id;
                DataBase.db.Update(ticket);
                Deleted(LayoutPosition);
            }
        }

        private void ArrowBackBtn_Click(object sender, EventArgs e) {
            var ticket = DataBase.db.Get<Ticket>(TicketId);
            var columns = DataBase.db.Table<Columns>().Where<Columns>((column) => column.KanbanId == ticket.ColumnKanbanId).ToList();
            var columnsBeforeCurrent = columns.TakeWhile((column) => column.id != ticket.ColumnId).ToList();
            if (columnsBeforeCurrent.Count >= 1) {
                ticket.ColumnId = columnsBeforeCurrent.Last().id;
                DataBase.db.Update(ticket);
                Deleted(LayoutPosition);
            }
        }

        private void ItemView_Click(object sender, EventArgs e) {
            var view = sender as View;
            var buttonLayout = view.FindViewById<Android.Widget.LinearLayout>(Resource.Id.ticket_button_layout);
            buttonLayout.Visibility = buttonLayout.Visibility == ViewStates.Gone ? ViewStates.Visible : ViewStates.Gone;
        }
    }

    // ADAPTER
    // Adapter to connect the data set to the RecyclerView: 
    public class TicketReposAdapter : RecyclerView.Adapter {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        public delegate void ObjectDelegate(object sender);
        public event ObjectDelegate DataSetChanged;


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
            vh.Deleted += NotifyItemRemoved;
            vh.Deleted += Vh_Deleted;
            return vh;
        }

        private void Vh_Deleted(int Position) {
            DataSetChanged.Invoke(this);
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