using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AudioSteganographyProject.Classes;
using AudioSteganographyProject.UI.Views;
using AudioSteganographyProject.Interfaces;

namespace AudioSteganographyProject.UI.Models
{
    class MainWindowModel : PageModel
    {
        MainWindow viewModel;

        Dictionary<PageKeys, Page> subPages;
        public PageKeys currentPage;

        public MainWindowModel(MainWindow view)
        {
            this.viewModel = view;
            subPages = new Dictionary<PageKeys, Page>();

            subPages.Add(PageKeys.About, new AboutPage());
            subPages.Add(PageKeys.HideSong, new HideSongPage());
            subPages.Add(PageKeys.ReadHiddenInfo, new ReadHiddenInfoPage());

            currentPage = PageKeys.About;
        }

        public void setCurrentPage(PageKeys page)
        {
            this.currentPage = page;
        }

        public Page getCurrentPage()
        {
            return subPages[currentPage];
        }

        override public void updateModel(object page)
        {
            /* 
             * There is no content directly on this page to update.
             * Only current subpage needs to be updated for now.
             */
            return;
        }
    }
}
