using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Support.V7.App;
using Android.Support.V7.Widget;

using DataBase;

namespace src.KanbanRecyclerView {
    // Implement the ViewHolder pattern: each ViewHolder holds references
    // to the UI components within the CardView 
    // that is displayed in a row of the RecyclerView:
    public class KanbanViewHolder : RecyclerView.ViewHolder {
        public Android.Widget.TextView Caption { get; private set; }
        public Guid KanbanId { get; set; }

        // Get references to the views defined in the CardView layout.
        public KanbanViewHolder(View itemView, Action<int> listener, Action<int> listener2)
            : base(itemView) {
            // Locate and cache view references:
            Caption = itemView.FindViewById<Android.Widget.TextView>(Resource.Id.title_textview);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
            itemView.LongClick += (sender, e) => listener2(base.LayoutPosition);
            
        }
    }

    // ADAPTER
    // Adapter to connect the data set to the RecyclerView: 
    public class KanbanReposAdapter : RecyclerView.Adapter {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> ItemLongClick;

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
            KanbanViewHolder vh = new KanbanViewHolder(itemView, OnClick, OnLongClick);
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

        void OnLongClick(int position) {
            ItemLongClick?.Invoke(this, position);
        }
    }
}