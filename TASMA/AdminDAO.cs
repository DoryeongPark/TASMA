using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.IO;
using System.Data;

namespace TASMA
{
    namespace Database
    {
        public class AdminDAO
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
            /// 로그인 상태를 체크합니다.
            /// </summary>
            /// <returns>로그인 여부</returns>
            private bool CheckLoginState()
            {
                if (loginState == false)
                {
                    MessageBox.Show("You should login first");
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 학년을 선택한 상태를 체크합니다.
            /// </summary>
            /// <returns>선택 여부</returns>
            private bool CheckGradeState()
            {
                if (loginState == false)
                {
                    MessageBox.Show("You should login first");
                    return false;
                }

                if (currentGrade == null)
                {
                    MessageBox.Show("You should select grade first");
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 반을 선택한 상태를 체크합니다.
            /// </summary>
            /// <returns>선택 여부</returns>
            private bool CheckClassState()
            {
                if (loginState == false)
                {
                    MessageBox.Show("You should login first");
                    return false;
                }

                if (currentGrade == null)
                {
                    MessageBox.Show("You should select grade first");
                    return false;
                }

                if (currentClass == null)
                {
                    MessageBox.Show("You should select class first");
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Data Access Object를 로그인 초기 상태로 되돌립니다.
            /// </summary>
            public void ReturnToInitialLoginState()
            {
                //현재 선택된 학년, 반, 학생 번호 초기화
                currentGrade = null;
                currentClass = null;
                currentSnum = -1;
            }

            /// <summary>
            /// 선생님의 새로운 계정을 등록합니다.
            /// </summary>
            /// <param name="id">등록할 ID</param>
            /// <param name="password">비밀번호</param>
            /// <returns>실행 성공 여부</returns>
            public bool RegisterAdmin(string id, string password)
            {
                if (loginState == true)
                {
                    MessageBox.Show("You should logout first");
                    return false;
                }

                if (new FileInfo(id + ".db").Exists)
                {
                    MessageBox.Show("ID already exists");
                    return false;
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
                                + "SNAME STRING NOT NULL DEFAULT 'Student', "
                                + "SEX STRING NOT NULL DEFAULT 'M', "
                                + "PNUM STRING, "
                                + "ADDRESS STRING, "
                                + "PRIMARY KEY(GRADE, CLASS, SNUM), "
                                + "FOREIGN KEY(GRADE, CLASS) REFERENCES CLASS(GRADE, CLASS) "
                                + "ON DELETE CASCADE "
                                + "ON UPDATE CASCADE "
                                + ");";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS SUBJECT("
                                + "SUBJECT STRING NOT NULL, "
                                + "PRIMARY KEY(SUBJECT) "
                                + ");";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS EVALUATION("
                                + "SUBJECT STRING NOT NULL, "
                                + "EVALUATION STRING NOT NULL, "
                                + "PRIMARY KEY(SUBJECT, EVALUATION), "
                                + "FOREIGN KEY(SUBJECT) REFERENCES SUBJECT(SUBJECT) "
                                + "ON DELETE CASCADE "
                                + "ON UPDATE CASCADE "
                                + ");";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS CLASSSUBJECT("
                                + "GRADE STRING NOT NULL, "
                                + "CLASS STRING NOT NULL, "
                                + "SUBJECT STRING NOT NULL, "
                                + "PRIMARY KEY(GRADE, CLASS, SUBJECT), "
                                + "FOREIGN KEY(GRADE, CLASS) REFERENCES CLASS(GRADE, CLASS) "
                                + "ON DELETE CASCADE " 
                                + "ON UPDATE CASCADE, " 
                                + "FOREIGN KEY(SUBJECT) REFERENCES SUBJECT(SUBJECT) "
                                + "ON DELETE CASCADE "
                                + "ON UPDATE CASCADE "
                                + ");";
                cmd.ExecuteNonQuery();
                
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS CHECKPASSWORD("
                                + "CHECKPASSWORD STRING NOT NULL, "
                                + "PRIMARY KEY(CHECKPASSWORD) "
                                + ");";
                cmd.ExecuteNonQuery();
                               
                conn.Close();
                MessageBox.Show("ID is successfully created");
                return true;
            }

            /// <summary>
            /// 계정의 비밀번호의 매칭 여부를 검사합니다.
            /// </summary>
            /// <param name="id">선생님 계정</param>
            /// <param name="password">비밀번호</param>
            /// <returns>매칭 여부</returns>
            public bool Authenticate(string id, string password)
            {

                if (!new FileInfo(id + ".db").Exists)
                {
                    MessageBox.Show("ID doesn't exist");
                    return false;
                }

                var connStr = @"Data Source=" + id + ".db;Password=" + password + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "UPDATE CHECKPASSWORD SET CHECKPASSWORD = '1' WHERE CHECKPASSWORD = '0'";
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    //비밀번호 틀렸을 시의 SQLite 에러코드 알아볼 것
                    conn.Close();
                    MessageBox.Show("Error code - " + se.ErrorCode.ToString());
                    return false;
                }

                conn.Close();
                return true;

            }

            /// <summary>
            /// 계정의 비밀번호를 바꿉니다.
            /// </summary>
            /// <param name="id">선생님 계정</param>
            /// <param name="oldPassword">이전 비밀번호</param>
            /// <param name="newPassword">새 비밀번호</param>
            /// <returns>실행 성공 여부</returns>
            public bool ChangePassword(string id, string oldPassword, string newPassword)
            {
                if(loginState == true)
                {
                    MessageBox.Show("You should logout first");
                    return false;
                }

                if (!new FileInfo(id + ".db").Exists)
                {
                    MessageBox.Show("ID doesn't exist");
                    return false;
                }

                var connStr = @"Data Source=" + id + ".db;Password=" + oldPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "UPDATE CHECKPASSWORD SET CHECKPASSWORD = '1' WHERE CHECKPASSWORD = '0'";
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    //Wrong password - Error code(26)
                    conn.Close();
                    MessageBox.Show("Error code - " + se.ErrorCode.ToString());
                    return false;
                }

                conn.ChangePassword(newPassword);
                conn.Close();

                MessageBox.Show("Password is successfully changed");
                return true;
            }

            /// <summary>
            /// 선생님 계정을 지웁니다.
            /// </summary>
            /// <param name="id">선생님 계정</param>
            /// <param name="password">계정 비밀번호</param>
            /// <returns>실행 성공 여부</returns>
            public bool DeleteAdmin(string id, string password)
            {
                if (loginState == true)
                {
                    MessageBox.Show("You should logout first");
                    return false;
                }

                if (!new FileInfo(id + ".db").Exists)
                {
                    MessageBox.Show("ID doesn't exist");
                    return false;
                }

                var connStr = @"Data Source=" + id + ".db;Password=" + password + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "UPDATE CHECKPASSWORD SET CHECKPASSWORD = '1' WHERE CHECKPASSWORD = '0'";
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    //비밀번호 틀렸을 시의 SQLite 에러코드 알아볼 것
                    conn.Close();
                    MessageBox.Show("Error code - " + se.ErrorCode.ToString());
                    return false;
                }

                cmd.Dispose();
                conn.Close();
                
                GC.Collect();

                File.Delete(id + ".db");

                MessageBox.Show("ID is successfully removed");
                return true;
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
                    conn.Close();
                    MessageBox.Show(se.ErrorCode.ToString());
                    return;
                }

                cmd.Dispose();
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
            /// 현재 선택 상태를 한 단계 뒤로 돌립니다.(학생 -> 반, 반 -> 학년, 학년 -> 초기 상태)
            /// </summary>
            /// <returns>실행 성공 여부</returns>
            public bool MovePrevious()
            {
                if (!CheckLoginState())
                    return false;

                if (currentSnum != -1)
                {
                    currentSnum = -1;
                    MessageBox.Show("Return to student selection display");
                    return true;

                } else if (currentSnum == -1 && currentClass != null)
                {
                    currentClass = null;
                    MessageBox.Show("Return to class selection display");
                    return true;

                } else if (currentSnum == -1 && currentClass == null && currentGrade != null)
                {
                    currentGrade = null;
                    MessageBox.Show("Return to grade selection display");
                    return true;
                }

                return false;
            }
            
            /// <summary>
            /// 새로운 학년을 생성합니다.
            /// </summary>
            /// <returns>
            /// 실행 성공 여부
            /// </returns>
            /// <param name="gradeName">학년 이름</param>
            public bool CreateGrade(string gradeName)
            {
                if (!CheckLoginState())
                    return false;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";

                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "INSERT INTO GRADE VALUES('" + gradeName + "');";

                try{
                    cmd.ExecuteNonQuery();
                }catch(SQLiteException se)
                {
                    conn.Close();
                    MessageBox.Show(se.ErrorCode.ToString());
                    return false;
                }

                cmd.Dispose();
                conn.Close();

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
                if (!CheckLoginState())
                    return null;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";

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

                cmd.Dispose();
                conn.Close();

                return result;
            }

            /// <summary>
            /// 학년을 제거합니다.
            /// </summary>
            /// <param name="gradeName">제거할 학년</param>
            /// <returns>실행 성공 여부</returns>
            public bool DeleteGrade(string gradeName)
            {
                if (!CheckLoginState())
                    return false;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "DELETE FROM GRADE WHERE GRADE = '" + gradeName + "';";

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    conn.Close();
                    MessageBox.Show("Error code - " + se.ErrorCode.ToString());
                    return false;
                }

                cmd.Dispose();
                conn.Close();

                MessageBox.Show("Grade is successfully deleted");
                return true;
            }

            /// <summary>
            /// 학년의 이름을 수정합니다.
            /// </summary>
            /// <param name="oldGradeName">수정할 학년</param>
            /// <param name="newGradeName">수정한 학년</param>
            /// <returns>실행 성공 여부</returns>
            public bool UpdateGrade(string oldGradeName, string newGradeName)
            {
                if (!CheckLoginState())
                    return false;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "UPDATE GRADE SET GRADE = '" + newGradeName +"' WHERE GRADE = '" + oldGradeName + "';";

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    conn.Close();
                    MessageBox.Show("Error code - " + se.ErrorCode.ToString());
                    return false;
                }

                cmd.Dispose();
                conn.Close();
                return true;
            }

            /// <summary>
            /// 학년을 선택합니다.
            /// </summary>
            /// <param name="gradeName">선택한 학년 이름</param>
            /// <returns>실행 성공 여부</returns>
            public bool SelectGrade(string gradeName)
            {
                if (!CheckLoginState())
                    return false;

                currentGrade = gradeName;
                currentClass = null;
                currentSnum = -1;

                return true;       
            }

            /// <summary>
            /// 새로운 반을 생성합니다.
            /// </summary>
            /// <param name="className"></param>
            /// <returns>실행 성공 여부</returns>
            public bool CreateClass(string className)
            {
                if (!CheckGradeState())
                    return false;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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
                    conn.Close();
                    MessageBox.Show("Error code - " + se.ErrorCode.ToString());
                    return false;
                }

                cmd.Dispose();
                conn.Close();

                MessageBox.Show("Class is successfully created");
                return true;
            }

            /// <summary>
            /// 현재 학년의 클래스 목록을 불러옵니다.
            /// </summary>
            /// <returns>실행 성공 여부</returns>
            public List<string> GetClassList()
            {
                if (!CheckGradeState())
                    return null;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT CLASS FROM CLASS WHERE GRADE = '" + currentGrade + "';";
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();
                var result = new List<string>();

                while (reader.Read())
                {
                    result.Add(reader["CLASS"].ToString());
                }

                cmd.Dispose();
                conn.Close();

                return result;
            }

            /// <summary>
            /// 반을 제거합니다. 
            /// </summary>
            /// <param name="className">반 이름</param>
            /// <returns>실행 성공 여부</returns>
            public bool DeleteClass(string className)
            {
                if (!CheckGradeState())
                    return false;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "DELETE FROM CLASS WHERE CLASS = '" + className + "';";

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    conn.Close();
                    MessageBox.Show("Error code - " + se.ErrorCode.ToString());
                    return false;
                }

                cmd.Dispose();
                conn.Close();
                return true;
            }

            /// <summary>
            /// 반 이름을 수정합니다.
            /// </summary>
            /// <param name="oldClassName">수정할 반 이름</param>
            /// <param name="newClassName">수정한 반 이름</param>
            /// <returns>실행 성공 여부</returns>
            public bool UpdateClass(string oldClassName, string newClassName)
            {
                if (!CheckGradeState())
                    return false;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "UPDATE CLASS SET CLASS = '" + newClassName + "' WHERE CLASS = '" + oldClassName + "';";
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    conn.Close();
                    MessageBox.Show("Error code - " + se.ErrorCode.ToString());
                    return false;
                }

                cmd.Dispose();
                conn.Close();

                return true;
            }

            /// <summary>
            /// 반을 선택합니다.
            /// </summary>
            /// <param name="className">선택할 반</param>
            /// <returns>실행 성공 여부</returns>
            public bool SelectClass(string className)
            {
                if (!CheckGradeState())
                    return false;

                currentClass = className;
                currentSnum = -1;
                return true;
            }

            /// <summary>
            /// 학생번호를 입력합니다.
            /// </summary>
            /// <param name="snum">입력할 학생번호</param>
            /// <returns>실행 성공 여부</returns>
            public bool CreateStudent(int snum) {

                if (!CheckClassState())
                    return false;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "INSERT INTO STUDENT VALUES('" + currentGrade + "', '" + currentClass + "', '" + snum + "');";

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    conn.Close();
                    MessageBox.Show("Error code - " + se.ErrorCode.ToString());
                    return false;
                }

                cmd.Dispose();
                conn.Close();

                return true;
            }

            /// <summary>
            /// 학생번호 리스트 항목을 불러옵니다.
            /// </summary>
            /// <returns>실행 성공 여부</returns>
            public List<string> GetStudentList() {

                if (!CheckClassState())
                    return null;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT SNUM FROM STUDENT WHERE GRADE = '" + currentGrade + "' AND CLASS = '" + currentClass + "';";
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();
                var result = new List<string>();

                while (reader.Read())
                {
                    result.Add(reader["SNUM"].ToString());
                }

                return result;
            }

            /// <summary>
            /// 학생번호에 해당하는 학생을 삭제합니다.
            /// </summary>
            /// <param name="snum">학생 번호</param>
            /// <returns>실행 성공 여부</returns>
            public bool DeleteStudent(int snum) {

                if (!CheckClassState())
                    return false;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "DELETE FROM STUDENT WHERE GRADE = '" + currentGrade + "' AND CLASS = '" + currentClass + "' AND SNUM = '" + snum + "';";

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    conn.Close();
                    MessageBox.Show("Error code - " + se.ErrorCode.ToString());
                    return false;
                }

                cmd.Dispose();
                conn.Close();

                return true;
            }
            
            /// <summary>
            /// 학생번호를 수정합니다.  
            /// </summary>
            /// <param name="oldSnum">수정할 학생번호</param>
            /// <param name="newSnum">수정한 학생번호</param>
            /// <returns>실행 성공 여부</returns>
            public bool UpdateStudent(int oldSnum, int newSnum) {

                if (!CheckClassState())
                    return false;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "UPDATE STUDENT SET SNUM = '" + newSnum + "' WHERE GRADE = '" + currentGrade + "' AND CLASS = '" + currentClass + "' AND SNUM = '" + oldSnum + "';";
                
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    conn.Close();
                    MessageBox.Show(se.Message);
                    return false;
                }

                cmd.Dispose();
                conn.Close();

                return true;
            }

            /// <summary>
            /// 학생번호에 해당하는 학생을 선택합니다.
            /// </summary>
            /// <param name="snum">선택할 학생의 학생번호</param>
            /// <returns>실행 성공 여부</returns>
            public bool SelectStudent(int snum) {

                if (!CheckClassState())
                    return false;

                currentSnum = snum;
                return true;
            }

            /// <summary>
            /// 현재 선택된 반의 학생 데이터 테이블을 가져옵니다.
            /// </summary>
            /// <returns>학생 데이터 테이블</returns>
            public DataTable GetStudentDataTable(StudentTableOption option)
            {
                if (!CheckClassState())
                    return null;

                var optionStr = "";

                if (option == StudentTableOption.AscByNumber)
                    optionStr = " ORDER BY SNUM ASC;";
                else if (option == StudentTableOption.DescByNumber)
                    optionStr = " ORDER BY SNUM DESC;";
                else if (option == StudentTableOption.AscByName)
                    optionStr = " ORDER BY SNAME ASC;";
                else if (option == StudentTableOption.DescByName)
                    optionStr = " ORDER BY SNAME DESC;";
                else
                    optionStr = ";";

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT * FROM STUDENT WHERE GRADE = '" + currentGrade + "' AND CLASS = '" + currentClass + "'" + optionStr;
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();

                var dataTable = new DataTable();
                dataTable.Load(reader);
                
                conn.Close();

                return dataTable;
            }

            /// <summary>
            /// 현재 수정된 반의 학생 데이터 테이블을 반영합니다.
            /// </summary>
            /// <param name="dataTable">반영되는 학생 데이터 테이블</param>
            public void UpdateStudentDataTable(DataTable dataTable)
            {
                if (!CheckClassState())
                    return;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT * FROM STUDENT WHERE GRADE = '" + currentGrade + "' AND CLASS = '" + currentClass + "';";
                var cmd = new SQLiteCommand(cmdStr, conn);
                var adapter = new SQLiteDataAdapter(cmd);

                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);

                adapter.UpdateCommand = builder.GetUpdateCommand();
                adapter.Update(dataTable);

                conn.Close();
            }

            /// <summary>
            /// 현재 반의 데이터 테이블의 내용을 모두 삭제합니다.
            /// </summary>
            public void ClearStudentDataTable()
            {
                if (!CheckClassState())
                    return;

                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "DELETE FROM STUDENT WHERE GRADE = '" + currentGrade + "' AND CLASS = '" + currentClass + "';";
                cmd.ExecuteNonQuery();

                cmd.Dispose();
                conn.Close();
            }

            /// <summary>
            /// 과목을 생성합니다.
            /// </summary>
            /// <param name="subjectName">과목 이름</param>
            /// <returns>실행 성공 여부</returns>
            public bool CreateSubject(string subjectName)
            {
                var connStr = @"Data Source=" + currentId + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);

                try
                {
                    cmd.CommandText = "INSERT INTO SUBJECT VALUES('" + subjectName + "');";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO EVALUATION VALUES('" + subjectName + "', " + "'Midterm');";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO EVALUATION VALUES('" + subjectName + "', " + "'Final');";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS "+ subjectName +"("
                                    + "GRADE STRING NOT NULL, "
                                    + "CLASS STRING NOT NULL, "
                                    + "SNUM INTEGER NOT NULL, "
                                    + "Midterm INTEGER NOT NULL, "
                                    + "Final INTEGER NOT NULL, "
                                    + "PRIMARY KEY(GRADE, CLASS, SNUM), "
                                    + "FOREIGN KEY(GRADE, CLASS, SNUM) REFERENCES STUDENT(GRADE, CLASS, SNUM) "
                                    + "ON DELETE CASCADE "
                                    + "ON UPDATE CASCADE "
                                    + ");";
                    cmd.ExecuteNonQuery();
                }
                catch(SQLiteException se)
                {
                    MessageBox.Show(se.Message);
                    return false;
                }
               
                cmd.Dispose();
                conn.Close();

                return true;
            }

            /// <summary>
            /// 과목 이름을 수정합니다. 
            /// </summary>
            /// <param name="oldSubjectName">수정할 과목 이름</param>
            /// <param name="newSubjectName">수정한 과목 이름</param>
            /// <returns>실행 성공 여부</returns>
            public bool UpdateSubject(string oldSubjectName,  string newSubjectName)
            {
                return true;
            }

            /// <summary>
            /// 과목을 제거합니다.
            /// </summary>
            /// <param name="subjectName">제거할 과목 이름</param>
            /// <returns>실행 성공 여부</returns>
            public bool DeleteSubject(string subjectName)
            {
                return true;
            }

            /// <summary>
            /// 과목에 평가 항목을 추가합니다.
            /// </summary>
            /// <param name="subjectName">평가 항목을 추가할 과목</param>
            /// <param name="evaluationName">추가할 평가 항목</param>
            /// <returns>실행 성공 여부</returns>
            public bool CreateEvaluation(string subjectName, string evaluationName)
            {
                return true;
            }


            /// <summary>
            /// 과목의 평가 항목 이름을 수정합니다.
            /// </summary>
            /// <param name="subjectName">평가 항목을 수정할 과목</param>
            /// <param name="oldEvaluationName">수정할 평가 항목</param>
            /// <param name="newEvaluationName">수정한 평가 항목</param>
            /// <returns>실행 성공 여부</returns>
            public bool UpdateEvaluation(string subjectName, string oldEvaluationName, string newEvaluationName)
            {
                return true;
            }

            /// <summary>
            /// 과목의 평가 항목을 삭제합니다.
            /// </summary>
            /// <param name="subjectName">평가 항목을 삭제할 과목</param>
            /// <param name="evaluationName">삭제할 항목</param>
            /// <returns>실행 성공 여부</returns>
            public bool DeleteEvaluation(string subjectName, string evaluationName)
            {
                return true;
            }

            
            /// <summary>
            /// 과목에 반을 등록합니다.
            /// </summary>
            /// <param name="subjectName">과목</param>
            /// <param name="gradeName">등록할 반의 학년</param>
            /// <param name="className">등록할 반</param>
            /// <returns>실행 성공 여부</returns>
            public bool RegisterClassOnSubject(string subjectName, string gradeName, string className)
            {
                return true;
            }

            /// <summary>
            /// 과목에 반을 등록 해제합니다.
            /// </summary>
            /// <param name="subjectName">과목</param>
            /// <param name="gradeName">등록 해제할 반의 학년</param>
            /// <param name="className">등록 해제할 반</param>
            /// <returns>실행 성공 여부</returns>
            public bool UnRegisterClassOnSubject(string subjectName, string gradeName, string className)
            {
                return true;
            }
         


        }
    }
}
