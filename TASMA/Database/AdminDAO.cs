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
            private string currentDB = null;
            private string currentPassword = null;

            public string CurrentDB
            {
                get { return currentDB; }
            }

            //선생님 계정 로그인 상태
            private bool dbLoginState = false;

            public bool LoginState
            {
                get { return dbLoginState; }
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
                if (dbLoginState == false)
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
                if (dbLoginState == false)
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
                if (dbLoginState == false)
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
            public void ReturnToInitialState()
            {
                //현재 선택된 학년, 반, 학생 번호 초기화
                currentGrade = null;
                currentClass = null;
                currentSnum = -1;
            }

            /// <summary>
            /// 계정 폴더를 생성하고 비밀번호를 등록합니다.
            /// </summary>
            /// <param name="id"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public bool RegisterAccount(string id, string password)
            {
                try
                {
                    if (Directory.Exists(id))
                    {
                        MessageBox.Show("ID already exists");
                        return false;
                    }

                    Directory.CreateDirectory(id);
                }
                catch(Exception e)
                {
                    return false;
                }

                var connStr = @"Data Source=" + id + "/Authentication.db";
                SQLiteConnection conn = new SQLiteConnection(connStr);

                conn.Open();
                conn.ChangePassword(password);
                conn.Close();

                connStr = @"Data Source=" + id + "/Authentication.db;Password=" + password + ";Foreign Keys=True";
                conn = new SQLiteConnection(connStr);

                conn.Open();
                var cmd = new SQLiteCommand(conn);

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS CHECKPASSWORD("
                               + "CHECKPASSWORD STRING NOT NULL, "
                               + "PRIMARY KEY(CHECKPASSWORD) "
                               + ");";
                try
                {
                    cmd.ExecuteNonQuery();
                }catch(SQLiteException se){
                    return false;
                }

                cmd.Dispose();
                conn.Close();

                return true;
            }

            /// <summary>
            /// 입력한 계정과 비밀번호를 인증합니다.
            /// </summary>
            /// <param name="id">계정</param>
            /// <param name="password">비밀번호</param>
            /// <returns></returns>
            public bool Authenticate(string id, string password)
            {
                if (!Directory.Exists(id))
                {
                    MessageBox.Show("ID doesn't exist");
                    return false;
                }

                var connStr = @"Data Source=" + id + "/Authentication.db;Password=" + password + ";Foreign Keys=True";
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
                    MessageBox.Show("Password doesn't match");
                    return false;
                }

                cmd.Dispose();
                conn.Close();

                currentPassword = password;
                return true;
            }

            /// <summary>
            /// 새로운 데이터베이스를 생성합니다.
            /// </summary>
            /// <param name="path">DB생성 경로</param>
            /// <param name="password">비밀번호</param>
            /// <returns>실행 성공 여부</returns>
            public bool CreateDatabase(string path, string[] metaData)
            {
                if (dbLoginState == true)
                {
                    MessageBox.Show("You should logout first");
                    return false;
                }

                var connStr = @"Data Source=" + path + ".db";

                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();
                conn.ChangePassword(currentPassword);
                conn.Close();

                connStr = @"Data Source=" + path + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmd = new SQLiteCommand(conn);

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS DBINFO("
                                + "SCHOOLNAME STRING, "
                                + "YEAR STRING, "
                                + "REGION STRING, "
                                + "ADDRESS STRING "
                                + ");";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO DBINFO VALUES('" + metaData[0] + "', '" + metaData[1] + "', '" + metaData[2] + "', '" + metaData[3] + "');";

                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE IF NOT EXISTS GRADE("
                                + "GRADE STRING NOT NULL, "
                                + "PRIMARY KEY(GRADE)"
                                + ");";
                
                cmd.ExecuteNonQuery();
                
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
                               
                conn.Close();
                MessageBox.Show("Database is successfully created");
                return true;
            }

            /// <summary>
            /// 데이터베이스의 정보를 가져옵니다.
            /// </summary>
            /// <param name="dbPath">데이터베이스 경로</param>
            /// <returns></returns>
            public string[] GetDBInfo(string dbPath = null)
            {
                if(dbPath == null)
                {
                    dbPath = currentDB;
                }

                var connStr = @"Data Source=" + dbPath + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT * FROM DBINFO";
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();
                var result = new string[4];

                while (reader.Read())
                {
                    result[0] = reader["SCHOOLNAME"].ToString();
                    result[1] = reader["YEAR"].ToString();
                    result[2] = reader["REGION"].ToString();
                    result[3] = reader["ADDRESS"].ToString();
                }

                cmd.Dispose();
                conn.Close();

                return result;
            }

            /// <summary>
            /// 데이터베이스의 정보를 수정합니다.
            /// </summary>
            /// <param name="dbPath"></param>
            /// <param name="metaData">적용할 데이터베이스 정보</param>
            /// <returns></returns>
            public bool ModifyDBInfo(string dbPath, string[] metaData)
            {
                var connStr = @"Data Source=" + dbPath + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmd = new SQLiteCommand(conn);

                try
                {
                    cmd.CommandText = "DELETE FROM DBINFO;";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO DBINFO VALUES('" + metaData[0] + "', '" + metaData[1] + "', '" + metaData[2] + "', '" + metaData[3] + "');";
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException)
                {
                    return false;
                }

                cmd.Dispose();
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
                if(dbLoginState == true)
                {
                    MessageBox.Show("You should logout first");
                    return false;
                }

                if (!new DirectoryInfo(id).Exists)
                {
                    MessageBox.Show("ID doesn't exist");
                    return false;
                }

                var dir = new DirectoryInfo(id);
                var fileList = dir.GetFiles();
                foreach(var fileInfo in fileList)
                {
                    var dbPath = id + "/" + fileInfo.Name;

                    var connStr = @"Data Source=" + dbPath + ";Password = " + oldPassword + ";Foreign Keys=True;";
                    var conn = new SQLiteConnection(connStr);
                    conn.Open();
                    conn.ChangePassword(newPassword);
                    conn.Close();
                    MessageBox.Show("Password is successfully changed");
                }

                return true;
            }

           
            /// <summary>
            /// 데이터베이스로 로그인 합니다.
            /// </summary>
            /// <param name="dbPath">등록된 ID</param>
            /// <param name="password">비밀번호</param>
            public void LoginDatabase(string dbPath)
            {
                var connStr = @"Data Source=" + dbPath + ".db;Password=" + currentPassword + ";Foreign Keys=True;";

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
                    conn.Close();
                    MessageBox.Show(se.ErrorCode.ToString());
                    return;
                }

                cmd.Dispose();
                conn.Close();

                currentDB = dbPath;
                dbLoginState = true;
            }

            /// <summary>
            /// 현재 계정에서 로그아웃 합니다.
            /// </summary>
            public void LogoutAccount()
            {
                if (dbLoginState == true)
                {
                    currentDB = null;
                    currentPassword = null;
                    dbLoginState = false;
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
                    return true;

                } else if (currentSnum == -1 && currentClass != null)
                {
                    currentClass = null;
                    return true;

                } else if (currentSnum == -1 && currentClass == null && currentGrade != null)
                {
                    currentGrade = null;
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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";

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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";

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

                result.Sort();

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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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

                result.Sort();

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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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
            public List<Tuple<int, string>> GetStudentList() {

                if (!CheckClassState())
                    return null;

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT SNUM, SNAME FROM STUDENT WHERE GRADE = '" + currentGrade + "' AND CLASS = '" + currentClass + "';";
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();
                var result = new List<Tuple<int, string>>();

                while (reader.Read())
                {
                    int sNum = (int)((long)reader["SNUM"]);
                    string sName = null;

                    if (reader["SNAME"] != null)
                        sName = (string)reader["SNAME"];
                    
                    result.Add(new Tuple<int, string>(sNum, sName));
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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
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
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + subjectName + "("
                                    + "SEMESTER INTEGER NOT NULL, "
                                    + "GRADE STRING NOT NULL, "
                                    + "CLASS STRING NOT NULL, "
                                    + "SNUM INTEGER NOT NULL, "
                                    + "Midterm REAL, "
                                    + "Final REAL, "
                                    + "PRIMARY KEY(SEMESTER, GRADE, CLASS, SNUM), "
                                    + "FOREIGN KEY(GRADE, CLASS, SNUM) REFERENCES STUDENT(GRADE, CLASS, SNUM) "
                                    + "ON DELETE CASCADE "
                                    + "ON UPDATE CASCADE "
                                    + ");";
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
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
            public bool UpdateSubject(string oldSubjectName, string newSubjectName)
            {
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);

                try
                {
                    cmd.CommandText = "UPDATE SUBJECT SET SUBJECT = '" + newSubjectName + "' WHERE SUBJECT = '" + oldSubjectName + "';";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "ALTER TABLE " + oldSubjectName + " RENAME TO " + newSubjectName + ";";
                    cmd.ExecuteNonQuery();
                }catch(SQLiteException se)
                {
                    MessageBox.Show(se.Message);
                    return false;
                }

                cmd.Dispose();
                conn.Close();

                return true;
            }

            /// <summary>
            /// 과목을 제거합니다.
            /// </summary>
            /// <param name="subjectName">제거할 과목 이름</param>
            /// <returns>실행 성공 여부</returns>
            public bool DeleteSubject(string subjectName)
            {
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);
                               
                try
                {
                    cmd.CommandText = "DELETE FROM SUBJECT WHERE SUBJECT = '" + subjectName + "';";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "DROP TABLE " + subjectName + ";";
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    MessageBox.Show(se.Message);
                    return false;
                }

                cmd.Dispose();
                conn.Close();

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
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);

                try
                {
                    cmd.CommandText = "INSERT INTO EVALUATION VALUES('" + subjectName + "', '" + evaluationName + "');";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "ALTER TABLE " + subjectName + " ADD COLUMN " + evaluationName + " INTEGER;";
                    cmd.ExecuteNonQuery();
                }catch(SQLiteException se)
                {
                    MessageBox.Show(se.Message);
                    return false;
                }

                cmd.Dispose();
                conn.Close();
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
                var evaluationList = GetEvaluationList(subjectName);

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);

                //바뀔 Evaluation name의 인덱스
                int indexChanged = -1;

                try
                {
                    cmd.CommandText = "UPDATE EVALUATION SET EVALUATION = '" + newEvaluationName + "' WHERE SUBJECT = '" + subjectName + "' AND EVALUATION = '" + oldEvaluationName + "';";
                    cmd.ExecuteNonQuery();

                    var cmdStr = "";
                    cmdStr += "BEGIN TRANSACTION; ";
                    cmdStr += "ALTER TABLE " + subjectName + " RENAME TO TEMP; ";
                    cmdStr += "CREATE TABLE " + subjectName + "(";
                    cmdStr += "SEMESTER INTEGER NOT NULL, ";
                    cmdStr += "GRADE STRING NOT NULL, ";
                    cmdStr += "CLASS STRING NOT NULL, ";
                    cmdStr += "SNUM INTEGER NOT NULL, ";
                    for (int i = 0; i < evaluationList.Count; ++i)
                    {
                        if (evaluationList[i] == oldEvaluationName)
                        {
                            indexChanged = i;
                            evaluationList[i] = newEvaluationName;
                            cmdStr += newEvaluationName + " REAL, ";
                        }
                        else
                            cmdStr += evaluationList[i] + " REAL, ";
                    }
                    cmdStr += "PRIMARY KEY(SEMESTER, GRADE, CLASS, SNUM), ";
                    cmdStr += "FOREIGN KEY(GRADE, CLASS, SNUM) REFERENCES STUDENT(GRADE, CLASS, SNUM) ";
                    cmdStr += "ON DELETE CASCADE ";
                    cmdStr += "ON UPDATE CASCADE ";
                    cmdStr += "); ";
                    cmdStr += "INSERT INTO " + subjectName + "(";
                    cmdStr += "SEMESTER, GRADE, CLASS, SNUM, ";
                    for (int i = 0; i < evaluationList.Count; ++i)
                    {
                        if (i != evaluationList.Count - 1)
                            cmdStr += evaluationList[i] + ", ";
                        else
                            cmdStr += evaluationList[i] + ") ";

                        if (i == indexChanged)
                            evaluationList[i] = oldEvaluationName;
                    }
                    cmdStr += "SELECT ";
                    cmdStr += "SEMESTER, GRADE, CLASS, SNUM, ";
                    for (int i = 0; i < evaluationList.Count; ++i)
                    {
                        if (i != evaluationList.Count - 1)
                            cmdStr += evaluationList[i] + ", ";
                        else
                            cmdStr += evaluationList[i] + " ";
                    }
                    cmdStr += "FROM TEMP; ";
                    cmdStr += "DROP TABLE TEMP; ";
                    cmdStr += "COMMIT; ";
                    cmd.CommandText = cmdStr;
                    cmd.ExecuteNonQuery();   
                }catch(SQLiteException se)
                {
                    MessageBox.Show(se.Message);
                    return false;
                }
                cmd.Dispose();
                conn.Close();

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
                var evaluationList = GetEvaluationList(subjectName);
                evaluationList.Remove(evaluationName);            
                
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);

                try
                {
                    cmd.CommandText = "DELETE FROM EVALUATION WHERE SUBJECT = '" + subjectName + "' AND EVALUATION = '" + evaluationName + "';";
                    cmd.ExecuteNonQuery();

                    var cmdStr = "";
                    cmdStr += "BEGIN TRANSACTION; ";
                    cmdStr += "ALTER TABLE " + subjectName + " RENAME TO TEMP; ";
                    cmdStr += "CREATE TABLE " + subjectName + "(";
                    cmdStr += "SEMESTER INTEGER NOT NULL, ";
                    cmdStr += "GRADE STRING NOT NULL, ";
                    cmdStr += "CLASS STRING NOT NULL, ";
                    cmdStr += "SNUM INTEGER NOT NULL, ";
                    for (int i = 0; i < evaluationList.Count; ++i)
                        cmdStr += evaluationList[i] + " REAL, ";    
          
                    cmdStr += "PRIMARY KEY(GRADE, CLASS, SNUM), ";
                    cmdStr += "FOREIGN KEY(GRADE, CLASS, SNUM) REFERENCES STUDENT(GRADE, CLASS, SNUM) ";
                    cmdStr += "ON DELETE CASCADE ";
                    cmdStr += "ON UPDATE CASCADE ";
                    cmdStr += "); ";
                    cmdStr += "INSERT INTO " + subjectName + "(";
                    cmdStr += "SEMESTER, GRADE, CLASS, SNUM, ";
                    for (int i = 0; i < evaluationList.Count; ++i)
                    {
                        if (i != evaluationList.Count - 1)
                            cmdStr += evaluationList[i] + ", ";
                        else
                            cmdStr += evaluationList[i] + ") ";
                    }
                    cmdStr += "SELECT ";
                    cmdStr += "SEMESTER, GRADE, CLASS, SNUM, ";
                    for (int i = 0; i < evaluationList.Count; ++i)
                    {
                        if (i != evaluationList.Count - 1)
                            cmdStr += evaluationList[i] + ", ";
                        else
                            cmdStr += evaluationList[i] + " ";
                    }
                    cmdStr += "FROM TEMP; ";
                    cmdStr += "DROP TABLE TEMP; ";
                    cmdStr += "COMMIT; ";
                    cmd.CommandText = cmdStr;
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    MessageBox.Show(se.Message);
                    return false;
                }
                cmd.Dispose();
                conn.Close();

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
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);

                try
                {
                    cmd.CommandText = "INSERT INTO CLASSSUBJECT(SUBJECT, GRADE, CLASS) VALUES('" + subjectName +"', '" + gradeName + "', '" + className + "');";
                    cmd.ExecuteNonQuery();
                }catch(SQLiteException se)
                {
                    MessageBox.Show(se.Message);
                    return false;
                }

                cmd.Dispose();
                conn.Close();
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
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);

                try
                {
                    cmd.CommandText = "DELETE FROM CLASSSUBJECT WHERE SUBJECT = '" + subjectName + "' AND GRADE = '" + gradeName + "' AND CLASS = '" + className + "';";
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    MessageBox.Show(se.Message);
                    return false;
                }

                cmd.Dispose();
                conn.Close();
                return true;
            }

            /// <summary>
            /// 과목 리스트를 반환합니다.
            /// </summary>
            /// <returns>과목 리스트</returns>
            public List<string> GetSubjectList()
            {
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT SUBJECT FROM SUBJECT;";
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();
                var result = new List<string>();

                while (reader.Read())
                {
                    result.Add(reader["SUBJECT"].ToString());
                }

                result.Sort();

                conn.Close();
                cmd.Dispose();

                return result;
            }

            /// <summary>
            /// 과목의 평가 항목 리스트를 반환합니다.
            /// </summary>
            /// <param name="subjectName">과목</param>
            /// <returns>평가 항목 리스트</returns>
            public List<string> GetEvaluationList(string subjectName)
            {
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT EVALUATION FROM EVALUATION WHERE SUBJECT = '" + subjectName + "';";
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();
                var result = new List<string>();

                while (reader.Read())
                {
                    result.Add(reader["EVALUATION"].ToString());
                }

                for(int i = 0; i < result.Count; ++i)
                {
                    if (result[i] == "Midterm")
                    {
                        var temp = result[0];
                        result[0] = "Midterm";
                        result[i] = temp;
                    }

                    if (result[i] == "Final")
                    {
                        var temp = result[1];
                        result[1] = "Final";
                        result[i] = temp;
                    }
                }

                conn.Close();
                cmd.Dispose();

                return result;
            }

            /// <summary>
            /// 반에 등록된 과목 리스트를 반환합니다.
            /// </summary>
            /// <param name="gradeName">학년</param>
            /// <param name="className">반</param>
            /// <returns>등록된 과목 리스트</returns>
            public List<string> GetClassSubjects(string gradeName, string className)
            {
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT SUBJECT FROM CLASSSUBJECT WHERE GRADE = '" + gradeName + "' AND CLASS = '" + className + "';";
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();
                var result = new List<string>();

                while (reader.Read())
                {
                    result.Add(reader["SUBJECT"].ToString());
                }

                conn.Close();
                cmd.Dispose();

                return result;
            }
            
            /// <summary>
            /// 과목에 등록된 반 데이터 테이블을 반환합니다.
            /// </summary>
            /// <param name="subjectName">과목</param>
            /// <returns>등록된 반 데이터 테이블</returns>
            public DataTable GetSubjectClasses(string subjectName)
            {
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT * FROM CLASSSUBJECT WHERE SUBJECT = '" + subjectName + "';";
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();

                var dataTable = new DataTable();
                dataTable.Load(reader);

                conn.Close();
                cmd.Dispose();

                return dataTable;
            }

            /// <summary>
            /// 해당 과목에 존재하는 학생 번호 리스트를 불러옵니다.
            /// </summary>
            /// <param name="subjectName">과목 이름</param>
            /// <returns>학생 번호 리스트</returns>
            public List<int> GetStudentNumberFromSubject(string subjectName)
            {
                if (!CheckClassState())
                    return null;

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT SNUM FROM " + subjectName + " WHERE SEMESTER = 0 AND GRADE = '" + currentGrade + "' AND CLASS = '" + currentClass + "' ORDER BY SNUM ASC;";
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();
                var result = new List<int>();

                while (reader.Read())
                {
                    var sNum = (int)((long)reader["SNUM"]);
                    result.Add(sNum);
                }

                conn.Close();
                cmd.Dispose();

                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="subjectName"></param>
            /// <returns>성공 여부</returns>
            public bool CreateStudentInSubject(string subjectName, int sNum)
            {
                if (!CheckClassState())
                    return false;

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);

                try
                {
                    cmd.CommandText = "INSERT INTO " + subjectName + "(SEMESTER, GRADE, CLASS, SNUM) VALUES('0', '" + currentGrade + "', '" + currentClass + "', '" + sNum + "');";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO " + subjectName + "(SEMESTER, GRADE, CLASS, SNUM) VALUES('1', '" + currentGrade + "', '" + currentClass + "', '" + sNum + "');";
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    MessageBox.Show(se.Message);
                    return false;
                }

                cmd.Dispose();
                conn.Close();
                return true;
            }

            /// <summary>
            /// 학생의 점수를 입력합니다. 
            /// </summary>
            /// <param name="subjectName">과목</param>
            /// <param name="sNum">학생번호</param>
            /// <param name="evaluationName">항목</param>
            /// <param name="value">입력 값</param>
            /// <returns>실행 성공 여부</returns>
            public bool UpdateScore(string subjectName, int semester, int sNum, string evaluationName, float? value)
            {
                if (!CheckClassState())
                    return false;

                string score = "";

                if (value == null)
                    score = "NULL";
                else
                    score = value.Value.ToString();    

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();
                var cmd = new SQLiteCommand(conn);

                try
                {
                    cmd.CommandText = "UPDATE " + subjectName + " SET " + evaluationName + " = " + score + " WHERE SEMESTER = " + semester + " AND GRADE = '" + currentGrade + "' AND CLASS = '" + currentClass + "' AND SNUM = " + sNum + ";";
                    cmd.ExecuteNonQuery();

                }
                catch (SQLiteException se)
                {
                    MessageBox.Show(se.Message);
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 학생 점수 테이블을 가져옵니다.
            /// </summary>
            /// <param name="subjectName">과목 이름</param>
            /// <returns>학생 점수 테이블</returns>
            public DataTable GetScoreTable(string subjectName, int semester)
            {
                if (!CheckClassState())
                    return null;
                
                var evaluationList = GetEvaluationList(subjectName);
                
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "";
                cmdStr += "SELECT SNUM";

                foreach (var evaluationName in evaluationList)
                    cmdStr += "," + evaluationName;

                cmdStr += " FROM " + subjectName;
                cmdStr += " WHERE SEMESTER = " + semester + " AND GRADE = '" + currentGrade + "' AND CLASS = '" + currentClass + "' ORDER BY SNUM ASC;";
                                
                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();

                var dataTable = new DataTable();
                dataTable.Load(reader);

                cmd.Dispose();
                conn.Close();

                return dataTable;
            }

            /// <summary>
            /// 학생 정보를 검색합니다.
            /// </summary>
            /// <param name="gradeName">학년 이름</param>
            /// <param name="className">반 이름</param>
            /// <param name="studentName">학생 이름</param>
            /// <returns>학생 정보 테이블</returns>
            public DataTable SearchStudent(string gradeName, string className, string studentName)
            {
                string sqlPhase = "";

                if(gradeName != null)
                {
                    sqlPhase += "WHERE GRADE = '" + gradeName + "' ";

                    if(className != null)
                        sqlPhase += "AND CLASS = '" + className + "' ";
                   
                    if(studentName != "")
                        sqlPhase += "AND SNAME LIKE " + "'%" + studentName + "%'"; 

                }else
                {
                    if(studentName != "")
                        sqlPhase += "WHERE SNAME LIKE " + "'%" + studentName + "%'";    
                }
                       
                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT GRADE, CLASS, SNUM, SNAME FROM STUDENT " + sqlPhase;

                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();

                var dataTable = new DataTable();
                dataTable.Load(reader);

                dataTable.Columns.Add("PRINT", typeof(bool));
                for (var i = 0; i < dataTable.Rows.Count; ++i)
                    dataTable.Rows[i]["PRINT"] = true;
            
                cmd.Dispose();
                conn.Close();

                return dataTable;
            }

            public DataTable GetFilteredStudentDataTable(DataTable filter, string gradeName, string className = null)
            {
                string classPhase = ";";

                if(className != null)
                {
                    classPhase = " AND CLASS = '" + className + "';";
                }

                var connStr = @"Data Source=" + currentDB + ".db;Password=" + currentPassword + ";Foreign Keys=True;";
                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmdStr = "SELECT * FROM STUDENT WHERE GRADE = '" + gradeName + "'" + classPhase;

                var cmd = new SQLiteCommand(cmdStr, conn);
                var reader = cmd.ExecuteReader();

                var dataTable = new DataTable();
                dataTable.Load(reader);

                for(int i = 0; i < dataTable.Rows.Count; ++i)
                {
                    var contains = filter.AsEnumerable().Any(
                        p => (string)dataTable.Rows[i]["GRADE"] == p.Field<string>("GRADE") &&
                             (string)dataTable.Rows[i]["CLASS"] == p.Field<string>("CLASS") &&
                             (string)dataTable.Rows[i]["SNAME"] == p.Field<string>("SNAME"));
                    if (!contains)
                    {
                        dataTable.Rows.RemoveAt(i);
                        i = -1;
                    }
                }

                cmd.Dispose();
                conn.Close();

                return dataTable;
            }

        }
    }
}
