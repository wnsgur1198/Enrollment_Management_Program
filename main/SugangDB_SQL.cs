using System;
using System.Text.RegularExpressions;
using System.Windows;
using 의료IT공학과.데이터베이스;

namespace main
{
    class SugangDB_SQL
    {

        //-----------------------------------------------
        public int insertStudent(iLocalDB db, string hakbun, string name, string pass, string pass_Confirm, string dept, int status, string addr, string email, string phone)
        {
            // 학번 유효성검사
            if (IsValid_IDENTIFIER(db, hakbun) == false) return 0;
            if (IsNewHakbun(db, hakbun) == false) return 0;

            // 이름 유효성검사
            if (IsValid_PEOPLE_NAME(name) == false) return 0;

            // 비밀번호 유효성검사
            if (IsValid_PASSWORD(pass, pass_Confirm) == false) return 0;

            // 학과 유효성검사
            if (IsValid_CLASS(dept) == false) return 0;

            // 상태 유효성검사
            if (IsValid_STATUS(status) == false) return 0;

            // 주소 유효성검사
            if (IsValid_HOUSE_ADDRESS(addr) == false) return 0;

            // 이메일 유효성검사
            if (IsValid_EMAIL_ADDRESS(email) == false) return 0;

            // 전화번호 유효성검사
            if (IsValid_PHONE_NUMBER(phone) == false) return 0;

            string str = string.Format("'{0}', '{1}', '{2}', '{3}', {4}, '{5}', '{6}', '{7}'",
                                        hakbun, name, pass, dept, status, addr, email, phone);

            return insertRow(db, "xSTUDENTS", str);
        }

        //------------------------------------------------------
        private int insertRow(iLocalDB db, string tableName, string dataStr)
        {

            string queryStr = string.Format("INSERT INTO {0} VALUES({1})", tableName, dataStr);

            if (!DB_Query(db, queryStr)) return 0;

            //MessageBox.Show("완료되었습니다.");

            return 1;  //OK
        }

        //-----------------------------------------------
        private bool IsValid_IDENTIFIER(iLocalDB db, string id)
        {
            if (string.IsNullOrEmpty(id)) return Error("학번이 null이거나 빈문자열입니다.");            
            if (id.Length != 8) return Error("학번의 길이는 8문자이어야 합니다");
            if (IsZero(id.Substring(5, 3)) == false) return Error("학번의 일련번호 000은 허용되지 않습니다.");
            if (IsNumericString(id.Substring(0, 2)) == false) return Error("학번의 처음 두 문자는 년도를 나타내야 합니다.(예: 19)");
            if (IsValidDeptCode(db, id.Substring(2, 3)) == false) return Error("학번의 3,4,5번째 문자에 일치하는 학과코드가 없습니다.");
            if (IsNumericString(id.Substring(5, 3)) == false) return Error("학번의 마지막 세 문자는 일련번호여야 합니다.(예: 001)");
            
            return true;
        }

        //-----------------------------------------------
        private bool IsNumericString(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {               
                if (!"0123456789".Contains(str.Substring(i, 1))) return false;
            }
            return true;
        }

        //-----------------------------------------------
        private bool IsZero(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if ("000".Equals(str)) return false;
            }
            return true;
        }

        //-----------------------------------------------
        private bool IsValidDeptCode(iLocalDB db, string code)
        {
            string query = string.Format("SELECT * FROM xDEPARTMENT WHERE xDept_code='{0}'", code);
            string res = db.Query(query);

            if (res != null)
            {
                MessageBox.Show(res);
                return false;
            }

            if (db.HasRows) return true;

            return false;
        }

        //-----------------------------------------------
        private bool IsNewHakbun(iLocalDB db, string hakbun)
        {
            string query = string.Format("select xHakbun from xSTUDENTS where xHakbun='{0}'", hakbun);
            string res = db.Query(query);

            if (res != null)    return false;

            if (db.HasRows)     return Error("같은 학번이 이미 존재합니다." + hakbun);

            return true;
        }

        //-----------------------------------------------
        private bool IsValid_PEOPLE_NAME(string name)
        {
            if (string.IsNullOrEmpty(name)) return Error("사람 이름 문자열이 null이거나 비어있습니다.");
            if (name.Length > 20) return Error("사람 이름은 20문자를 초과할 수 없습니다.");
            if (name.Contains(" ")) return Error("사람 이름은 공백이 허용되지 않습니다.");
            if (stringContains_Oneof(name, " \t\r\n")) return Error("사람 이름은 공백이 허용되지 않습니다.");
            if (name.Contains("\t")) return Error("사람 이름은 탭키가 허용되지 않습니다.");
            if (stringContains_Oneof(name, "!@#$%^&*")) return Error("사람 이름은 특수문자가 허용되지 않습니다.");
            if (stringContains_Oneof(name, "0123456789")) return Error("사람 이름은 숫자가 허용되지 않습니다.");

            return true;
        }

        //-----------------------------------------------
        private bool IsValid_PASSWORD(string pass, string passConfirm)
        {
            if (string.IsNullOrEmpty(pass)) return Error("비밀번호가 null이거나 비어있습니다.");
            if (pass.Length > 20 || pass.Length < 10) return Error("비밀번호 길이가 적절하지 않습니다(10문자 이상, 20문자 이하).");
            if (stringContains_Oneof(pass, " \t\r\n")) return Error("비밀번호는 공백이 허용되지 않습니다.");
            if (!stringContains_Oneof(pass, "!@#$%^&*")) return Error("비밀번호는 특수문자가 하나 이상 있어야 합니다.");
            if (!stringContains_Oneof(pass, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")) return Error("비밀번호는 영대문자가 하나 이상 있어야 합니다.");
            if (!stringContains_Oneof(pass, "abcdefghijklmnopqrstuvwxyz")) return Error("비밀번호는 영소문자가 하나 이상 있어야 합니다.");
            if (!stringContains_Oneof(pass, "0123456789")) return Error("비밀번호는 숫자가 하나 이상 있어야 합니다.");
            if(!pass.Equals(passConfirm)) return Error("비밀번호 확인란과 일치하지 않습니다.");

            return true;
        }

        //-----------------------------------------------
        private bool IsValid_CLASS(string dept)
        {
            if (Int32.Parse(dept) < 0) return Error("학과가 선택되지 않았습니다.");

            return true;
        }

        //-----------------------------------------------
        private bool IsValid_STATUS(int status)
        {
            if (status < 0) return Error("상태가 선택되지 않았습니다.");

            return true;
        }

        //-----------------------------------------------
        private bool IsValid_HOUSE_ADDRESS(string addr)
        {
            if (string.IsNullOrEmpty(addr)) return Error("집주소가 null이거나 비어있습니다.");
            if (addr.Length > 50) return Error("집주소의 길이가 적절하지 않습니다(50문자 이하).");
            if (stringContains_Oneof(addr, "!@#$%^&*")) return Error("집주소는 특수문자가 허용되지 않습니다.");

            return true;
        }

        //-----------------------------------------------
        private bool IsValid_EMAIL_ADDRESS(string email)
        {
            if (string.IsNullOrEmpty(email)) return Error("이메일이 null이거나 비어있습니다.");
            if (email.Length > 30) return Error("이메일의 길이가 적절하지 않습니다(50문자 이하).");
            if (stringContains_Oneof(email, " \t\r\n")) return Error("이메일은 공백이 허용되지 않습니다.");

            string[] str = email.Split('@');

            if(str.Length != 2) return Error("이메일의 형식이 일치하지 않습니다(xxx@xxx.xxx).");
            if (stringContains_Oneof(str[0], "!@#$%^&*")) return Error("이메일은 특수문자를 허용하지 않습니다.");

            int count = 0;

            for (int i = 0; i < str[1].Length; i++)
            {
                if (".".Contains(str[1].Substring(i, 1)))
                {
                    count++;
                }                
            }

            if (count > 0)
                return true;
            else
                return Error("이메일의 형식이 일치하지 않습니다(xxx@xxx.xxx).");

        }

        //-----------------------------------------------
        private bool IsValid_PHONE_NUMBER(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return Error("전화번호가 null이거나 비어있습니다.");
            if (phone.Length > 20) return Error("전화번호의 길이가 적절하지 않습니다(50문자 이하).");
            if (stringContains_Oneof(phone, " \t\r\n")) return Error("전화번호는 공백이 허용되지 않습니다.");
            if (stringContains_Oneof(phone, "!@#$%^&*")) return Error("전화번호는 특수문자를 허용하지 않습니다.");
            if (stringContains_Oneof(phone, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")) return Error("전화번호는 문자를 허용하지 않습니다.");

            string[] str = phone.Split('-');
            if (str.Length != 3) return Error("전화번호의 형식이 일치하지 않습니다(999-999-999).");

            return true;
        }

        //-----------------------------------------------
        private bool Error(string msg)
        {
            MessageBox.Show("*** Error: " + msg + " ***");
            return false;
        }

        //------------------------------------------------
        private bool stringContains_Oneof(string str, string oneof)
        {
            for (int i = 0; i < oneof.Length; i++)
            {
                if (str.Contains(oneof.Substring(i, 1))) return true;
            }

            return false;
        }

        //------------------------------------------------
        private bool DB_Query(iLocalDB db, string query)
        {
            string err_msg = db.Query(query);

            if (err_msg != null)
            {
                MessageBox.Show("Error\n" + err_msg + "\n" + query);
                return false;
            }
            return true;
        }
                
    }
}