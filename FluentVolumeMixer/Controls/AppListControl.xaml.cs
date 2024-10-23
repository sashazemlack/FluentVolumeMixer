using System.Collections.ObjectModel;
using System.Windows.Controls;


namespace FluentVolumeMixer.Controls
{
    public partial class AppListControl : UserControl
    {
        public ObservableCollection<ProcessModel> ResultsSource { get; set; } = new ObservableCollection<ProcessModel>();

        public AppListControl()
        {
            InitializeComponent();
            this.DataContext = this;
            GetRunningProcess();
        }

        private void GetRunningProcess()
        {
            System.Collections.Generic.List<ProcessModel> list = ProcessModel.GetAudioSessions();
            ResultsSource = new ObservableCollection<ProcessModel>(list);
        }

    }
}
