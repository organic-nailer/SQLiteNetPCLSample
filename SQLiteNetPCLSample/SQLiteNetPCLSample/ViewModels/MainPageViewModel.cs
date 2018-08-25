using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLiteNetPCLSample.SQLites;
using System.Threading.Tasks;

namespace SQLiteNetPCLSample.ViewModels
{
    public class MainPageViewModel : ViewModelBase, INavigationAware
    {
        public DelegateCommand ButtonClickedCommand { get; }

        private List<DBFuncItem> _dBFuncItems;
        public List<DBFuncItem> FuncItems
        {
            get { return _dBFuncItems; }
            set { SetProperty(ref _dBFuncItems, value); }
        }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";

            ButtonClickedCommand = new DelegateCommand(async () => await ButtonClicked());
        }

        private async Task ButtonClicked()
        {
            await FuncItemService.Add(DateTime.Now.ToShortDateString(), "Hello");

            FuncItems = await FuncItemService.GetList();
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public async Task OnNavigatedTo(INavigationParameters parameters)
        {
            FuncItems = await FuncItemService.GetList();
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {

        }
    }
}
