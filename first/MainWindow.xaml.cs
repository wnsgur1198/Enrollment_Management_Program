using System.Windows;

namespace first
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void StudentManage_Click(object sender, RoutedEventArgs e)
        {
            Window main = new main.MainWindow();
            main.Show();
        }
    

        private void TeacherManage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("개발 중입니다.");
        }

        private void ClassManage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("개발 중입니다.");
        }

        private void CurriManage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("개발 중입니다.");
        }

        private void OpenManage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("개발 중입니다.");
        }

        private void ApplyManage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("개발 중입니다.");
        }
    }
   
}
