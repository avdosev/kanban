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
            viewPager.Adapter = new ColumnPagerAdapter(SupportFragmentManager);

            tabbar.SetupWithViewPager(viewPager);
        }
    }

    public class ColumnPageFragment : Android.Support.V4.App.Fragment {
        private const string ARG_PAGE = "ARG_PAGE";

        private int mPage;

        public static ColumnPageFragment newInstance(int page) {
            Bundle args = new Bundle();
            args.PutInt(ARG_PAGE, page);
            ColumnPageFragment fragment = new ColumnPageFragment();
            fragment.Arguments = args;
            return fragment;
        }

        override public void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            if (Arguments != null) {
                mPage = Arguments.GetInt(ARG_PAGE);
            }
        }

        override public View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            View view = inflater.Inflate(Resource.Layout.kanban_content, container, false);
            // Making recicler view
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

        public ColumnPagerAdapter(Android.Support.V4.App.FragmentManager fm) : base(fm) {
            
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position) {
            return new Java.Lang.String(tabTitles[position]);
        }

        public override Android.Support.V4.App.Fragment GetItem(int position) {
            return ColumnPageFragment.newInstance(position);
        }
    }
}