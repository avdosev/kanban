using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;

namespace src {

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    class KanbanActivity : AppCompatActivity {
        protected Guid kanbanId;
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            kanbanId = Guid.Parse(Intent.GetStringExtra("kanbanId"));

            SetContentView(Resource.Layout.kanban_activity);

            var tabbar = FindViewById<TabLayout>(Resource.Id.tab_layout);

            
            
        }
    }
}