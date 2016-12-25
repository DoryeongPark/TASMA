using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.IO;

namespace TASMA
{
    namespace Database
    {
        class AdminDAO
        {
            private static AdminDAO instance = new AdminDAO();

            //현재 로그인된 선생님 계정과 비밀번호 
            private string currentId = null;
            private string currentPassword = null;

            public string CurrentId
            {
                get { return currentId; }
            }

            //선생님 계정 로그인 상태
            private bool loginState = false;

            public bool LoginState
            {
                get { return loginState; }
            }

            //현재 선택된 학년, 반, 학생 번호
            private string currentGrade = null;
            private string currentClass = null;
            private int currentSnum = -1;

            public string CurrentGrade
            {
                get { return currentGrade; }
            }

            public string CurrentClass
            {
                get { return currentClass; }
            }

            public int CurrentSnum
            {
                get { return currentSnum; }
            }
           

            private AdminDAO() { }

            /// <summary>
            /// Data Access Object를 반환합니다.
            /// </summary>
            /// <returns>Data Access Object</returns>
            public static AdminDAO GetDAO()
            {
                return instance;
            }

            /// <summary>
            /// 선생님의 새로운 계정을 등록합니다.
            /// </summary>
            /// <param name="id">등록할 ID</param>
            /// <param name="password">비밀번호</param>
            public void RegisterAdmin(string id, string password)
            {
                if (new FileInfo(id + ".db").Exists)
                {
                    MessageBox.Show("ID already exists");
                    return;
                }

                var connStr = @"Data Source=" + id + ".db";

                SQLiteConnection conn = new SQLiteConnection(connStr);

                conn.Open();
                conn.ChangePassword(password);
                conn.Close();

                connStr = @"Data Source=" + id + ".db;Password=" + password + ";Foreign Keys=True;";
                conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmd = new SQLiteCommand(conn);

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS GRADE("
                                + "GRADE STRING NOT NULL, "
                                + "PRIMARY KEY(GRADE)"
                                + ");";
                try
                {
                    cmd.ExecuteNonQuery();
                } catch (SQLiteException se)
                {
                    se.ErrorCode.ToString();
                }

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS CLASS("
                                + "GRADE STRING NOT NULL, "
                                + "CLASS STRING NOT NULL, "
                                + "PRIMARY KEY(GRADE, CLASS),"
                                + "FOREIGN KEY(GRADE) REFERENCES GRADE(GRADE) "
                                + "ON DELETE CASCADE "
                                + "ON UPDATE CASCADE "
                                + ");";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS STUDENT("
                                + "GRADE STRING NOT NULL, "
                                + "CLASS STRING NOT NULL, "
                                + "SNUM INTEGER NOT NULL, "
                                + "SNAME STRING NOT NULL, "
                                + "PRIMARY KEY(GRADE, CLASS, SNUM), "
                                + "FOREIGN KEY(GRADE, CLASS) REFERENCES CLASS(GRADE, CLASS) "
                                + "ON DELETE CASCADE "
                                + "ON UPDATE CASCADE "
                                + ");";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS SUBJECT("
                                + "GRADE STRING NOT NULL, "
                                + "CLASS STRING NOT NULL, "
                                + "SUBJECT STRING NOT NULL, "
                                + "PRIMARY KEY(GRADE, CLASS, SUBJECT), "
                                + "FOREIGN KEY(GRADE, CLASS) REFERENCES CLASS(GRADE, CLASS) "
                                + "ON DELETE CASCADE "
                                + "ON UPDATE CASCADE "
                                + ");";
                cmd.ExecuteNonQuery();
                
                conn.Close();
            }

            /// <summary>
            /// 선생님 ID로 로그인 합니다.
            /// </summary>
            /// <param name="id">등록된 ID</param>
            /// <param name="password">비밀번호</param>
            public void LoginAs(string id, string password)
            {
                if (!new FileInfo(id + ".db").Exists)
                {
                    MessageBox.Show("ID is not registered");
                    return;
                }

                var connStr = @"Data Source=" + id + ".db;Password=" + password + ";Foreign Keys=True;";

                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "SELECT SQLITE_VERSION();";
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    //비밀번호 틀렸을 시의 SQLite 에러코드 알아볼 것
                    MessageBox.Show(se.ErrorCode.ToString());
                    return;
                }

                conn.Close();

                currentId = id;
                currentPassword = password;
                loginState = true;
            }

            /// <summary>
            /// 현재 계정에서 로그아웃 합니다.
            /// </summary>
            public void Logout()
            {
                if (loginState == true)
                {
                    currentId = null;
                    currentPassword = null;
                    loginState = false;
                }
            }
            
            /// <summary>
            /// 새로운 학년을 생성합니다.
            /// </summary>
            /// <returns>
            /// 실행 완료 여부
            /// </returns>
            /// <param name="gradeName">학년 이름</param>
            public bool CreateGrade(string gradeName)
            {
                if(loginState == false)
                {
                    MessageBox.Show("You have to login first");
                    return false;
                }

                var connStr = @"Data Source=" + CurrentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";

                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "INSERT INTO GRADE VALUES('" + gradeName + "');";

                try{
                    cmd.ExecuteNonQuery();
                }catch(SQLiteException se)
                {
                    MessageBox.Show(se.ErrorCode.ToString());
                    return false;
                }

                MessageBox.Show("Grade is successfully created");
                return true;
            }

            /// <summary>
            /// 현재 계정에 저장되어 있는 모든 학년 항목을 리스트 형태로 가져옵니다.
            /// </summary>
            /// <returns>
            /// 학년 항목 리스트
            /// </returns>
            public List<string> GetGradeList()
            {
                if (loginState == false)
                {
                    MessageBox.Show("You should login first");
                    return null;
                }

                var connStr = @"Data Source=" + CurrentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";

                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT GRADE FROM GRADE;";
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();
                var result = new List<string>();
                
                while (reader.Read())
                {
                    result.Add(reader["GRADE"].ToString());
                }

                return result;
            }

            /// <summary>
            /// 학년을 선택합니다.
            /// </summary>
            /// <param name="gradeName"></param>
            public void SelectGrade(string gradeName)
            {
                if(loginState == false)
                {
                    MessageBox.Show("You should login first");
                    return;
;                }

                currentGrade = gradeName;
                currentClass = null;
                currentSnum = -1;       
            }

            /// <summary>
            /// 새로운 반을 생성합니다.
            /// </summary>
            /// <param name="className"></param>
            /// <returns></returns>
            public bool CreateClass(string className)
            {
                if(loginState == false)
                {
                    MessageBox.Show("You should login first");
                    return false;
                }

                if(currentGrade == null)
                {
                    MessageBox.Show("You should select grade first");
                    return false;
                }

                var connStr = @"Data Source=" + CurrentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";

                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "INSERT INTO CLASS VALUES('" + currentGrade + "', '" + className + "');";

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    MessageBox.Show(se.ErrorCode.ToString());
                    return false;
                }

                MessageBox.Show("Class is successfully created");
                return true;
            }

        }
    }
}
