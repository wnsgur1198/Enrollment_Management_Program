using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using 의료IT공학과.데이터베이스;

namespace main
{
    public partial class MainWindow : Window
    {
        private string[] classCode = { "-1", "615", "620", "618", "621", "622" };
        private string[] className = { "선택", "의료IT공학과", "의공학부", "의료신소재공학과", "제약생명공학과", "의료공간디자인" };
        private int[] statusCode = { -1, 0, 1, 2, 3, 4, 9 };
        private string[] statusName = { "선택", "재학", "휴학", "자퇴", "제적", "수료", "졸업" };

        static xLocalDB db = new xLocalDB("Provider=Microsoft.ACE.OLEDB.12.0; " +
                                 "Data Source=../../../DBFiles/SUGANG_DB.accdb;  " +
                                 "Persist Security Info=False");

        static SugangDB_SQL sql = new SugangDB_SQL();

        private int classIndex = 0;
        private int statusIndex = 0;
        List<string> list = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            ClassChoice_comboBox();
            Class_comboBox();
            Status_comboBox();
        }
            
        // 입력 창 초기화
        public void clearAll()
        {
            hakbun.Clear();
            name.Clear();
            password.Clear();
            txtPassword.Clear();
            pass_Confirm.Clear();
            xClass.SelectedIndex = 0;
            status.SelectedIndex = 0;
            addr.Clear();
            email.Clear();
            phone.Clear();
            delete.IsEnabled = true;
            confirm.Content = "확인";
            txtPassword.Visibility = Visibility.Hidden;
            password.Visibility = Visibility.Visible;
        }

        // 테이블 새로고침
        private void tableRefresh()
        {
            list.Clear();

            int choice = classChoice.SelectedIndex;           

            string query = string.Format("SELECT * FROM xSTUDENTS WHERE xDept='{0}'", classCode[choice]);              
            
            Do_Query(query);
        }

        // 콤보박스 초기화
        private void ClassChoice_comboBox()
        {
            for (int i = 0; i < className.Length; i++)
            {
                classChoice.Items.Add(className[i]);
            }
            classChoice.SelectedIndex = 0;
        }

        private void Class_comboBox()
        {
            for(int i=0; i<className.Length; i++)
            {
                xClass.Items.Add(className[i]);
            }
            xClass.SelectedIndex = 0;
        }

        private void Status_comboBox()
        {
            for (int i = 0; i < statusName.Length; i++)
            {
                status.Items.Add(statusName[i]);
            }
            status.SelectedIndex = 0;
        }
        //-----------------------------------------------------------

        // 콤보박스 선택 이벤트
        private void classChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            clearAll();
            list.Clear();

            int choice = classChoice.SelectedIndex;

            if(choice == 0) result.Visibility = Visibility.Hidden;
            else result.Visibility = Visibility.Visible;

            string query = string.Format("SELECT * FROM xSTUDENTS WHERE xDept='{0}'", classCode[choice]);

            Do_Query(query);
        }

        // 쿼리 불러오기
        public void Do_Query(string query)
        {
            db.Open();

            try { db.Query(query); }
            catch (Exception ex) { MessageBox.Show(query + "\n\n" + ex.Message, "SQL Error"); }

            int r = 0;  // 행의 개수를 알기 위함
            DataTable dataTable = new DataTable();

            if (db.HasRows)
            {
                dataTable.Columns.Add("상태");
                dataTable.Columns.Add("학번");
                dataTable.Columns.Add("이름");

                while (db.Read())
                {
                    for (int i = 0; i < db.FieldCount; i++)
                    {
                        list.Add(db.GetData(i).ToString());
                    }
                    r++;
                }
            }

            for (int i=0; i<r; i++)
            {
                bool removeFlag = false;

                // 재학 상태 변환 
                int insertPoint = 4 + i * 8;

                for (int j = 0; j < statusCode.Length; j++)
                {                    
                    if (statusCode[j].ToString().Equals(list.ElementAt(insertPoint)))
                    {
                        list.Insert(insertPoint, statusName[j]);
                        removeFlag = true;
                    }
                }
                if (removeFlag)
                {
                    list.RemoveAt(insertPoint + 1);
                }

                string[] need = { list.ElementAt(4 + i * 8), list.ElementAt(0 + i * 8), list.ElementAt(1 + i * 8) };

                dataTable.Rows.Add(need);
            }

            result.ColumnWidth = this.Width / db.FieldCount;  // 그리드의 가로 폭 지정
            result.RowHeight = result.Height / (r + 1);         // 그리드의 세로 길이 지정
            result.ItemsSource = dataTable.DefaultView;  // 그리드와 테이블 연동

            db.Close();
        }

        private void result_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 오른쪽 입력창에 학생정보 띄우기
            clearAll();
            confirm.Content = "수정";

            if (result.SelectedIndex < 0) return;

            int index = result.SelectedIndex * 8;

            hakbun.Text = list.ElementAt(0 + index);
            hakbun.IsEnabled = false;
            name.Text = list.ElementAt(1 + index);

            for(int i=0; i< classCode.Length; i++)
            {
                if (classCode[i].Equals(list.ElementAt(3 + index)))
                {
                    xClass.SelectedIndex = i;
                }
            }

            for (int i = 0; i < statusName.Length; i++)
            {
                if (statusName[i].Equals(list.ElementAt(4 + index)))
                {
                    status.SelectedIndex = i;
                }
            }

            txtPassword.Text = list.ElementAt(2 + index);
            txtPassword.Visibility = Visibility.Visible;
            password.Visibility = Visibility.Hidden;            

            addr.Text = list.ElementAt(5 + index);
            email.Text = list.ElementAt(6 + index);
            phone.Text = list.ElementAt(7 + index);
        }

        private void xClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            classIndex = xClass.SelectedIndex;
        }

        private void status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            statusIndex = status.SelectedIndex;
        }
        //--------------------------------------------------------------------------------------

        // 버튼 클릭 이벤트 
        private void close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            clearAll();
            hakbun.IsEnabled = true;
            delete.IsEnabled = false;
        }

        private void confirm_Click(object sender, RoutedEventArgs e)
        {
            db.Open();

            // 수정
            if (confirm.Content.Equals("수정"))
            {
                int choice = classChoice.SelectedIndex;
                string query = string.Format("SELECT * FROM xSTUDENTS WHERE xDept='{0}'", classCode[choice]);

                try { db.Query(query); }
                catch (Exception ex) { MessageBox.Show(query + "\n\n" + ex.Message, "SQL Error"); }

                List<string> preData = new List<string>();

                if (db.HasRows)
                {
                    while (db.Read())
                    {
                        for (int i = 0; i < db.FieldCount; i++)
                        {
                            preData.Add(db.GetData(i).ToString());
                        }
                    }
                }

                // 삭제
                string delQuery = string.Format("DELETE * FROM xSTUDENTS WHERE xHakbun='{0}'", hakbun.Text);

                try { db.Query(delQuery); }
                catch (Exception ex) { MessageBox.Show(delQuery + "\n\n" + ex.Message, "SQL Error"); }

                // 수정된 내용포함한 삽입
                if (sql.insertStudent(db, hakbun.Text, name.Text, txtPassword.Text, pass_Confirm.Password, classCode[classIndex], statusCode[statusIndex],
                                     addr.Text, email.Text, phone.Text) == 1)
                {
                    db.Close();
                    tableRefresh();
                }  
                // 원래 내용 삽입
                else
                {
                    int index = result.SelectedIndex * 8;

                    hakbun.Text = preData.ElementAt(0 + index);
                    hakbun.IsEnabled = false;
                    name.Text = preData.ElementAt(1 + index);

                    for (int i = 0; i < classCode.Length; i++)
                    {
                        if (classCode[i].Equals(preData.ElementAt(3 + index)))
                        {
                            xClass.SelectedIndex = i;
                        }
                    }

                    for (int i = 0; i < statusName.Length; i++)
                    {
                        if (statusName[i].Equals(preData.ElementAt(4 + index)))
                        {
                            status.SelectedIndex = i;
                        }
                    }

                    txtPassword.Text = preData.ElementAt(2 + index);
                    txtPassword.Visibility = Visibility.Visible;
                    password.Visibility = Visibility.Hidden;
                    pass_Confirm.Password = txtPassword.Text;

                    addr.Text = preData.ElementAt(5 + index);
                    email.Text = preData.ElementAt(6 + index);
                    phone.Text = preData.ElementAt(7 + index);

                    sql.insertStudent(db, hakbun.Text, name.Text, txtPassword.Text, pass_Confirm.Password, classCode[classIndex], statusCode[statusIndex],
                                     addr.Text, email.Text, phone.Text);

                    db.Close();
                    tableRefresh();
                }
            }

            // 추가
            else
            {
                if(sql.insertStudent(db, hakbun.Text, name.Text, password.Password, pass_Confirm.Password, classCode[classIndex], statusCode[statusIndex],
                                     addr.Text, email.Text, phone.Text) == 1)
                {
                    db.Close();
                    tableRefresh();
                    clearAll();
                }
                return;
            }            
            
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult del = MessageBox.Show("정말로 삭제하시겠습니까?", "삭제확인창", MessageBoxButton.OKCancel);
            if(del == MessageBoxResult.OK)
            {
                db.Open();

                string delQuery = string.Format("DELETE * FROM xSTUDENTS WHERE xHakbun='{0}'", hakbun.Text);                

                try { db.Query(delQuery); }
                catch (Exception ex) { MessageBox.Show(delQuery + "\n\n" + ex.Message, "SQL Error"); }

                tableRefresh();

                MessageBox.Show("삭제되었습니다.");
            }            
         
            db.Close();
            clearAll();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            clearAll();
        }        
        //-----------------------------------------------------------------

    }
}
