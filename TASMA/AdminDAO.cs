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
            
            private string currentId = null;
             
            public string CurrentId
            {
                get { return currentId; }
            }

            private bool loginState = false;
            
            public bool LoginState
            {
                get { return loginState; }
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
                if (new FileInfo(id + ".sqlite").Exists)
                {
                    MessageBox.Show("ID already exists");
                    return;
                }

                var connStr = @"Data Source=" + id + ".sqlite";

                SQLiteConnection conn = new SQLiteConnection(connStr);

                conn.Open();
                conn.ChangePassword(password);
                conn.Close();
            
                connStr = @"Data Source=" + id + ".sqlite;Password=" + password + ";";
                conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmd = new SQLiteCommand(conn);
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS STUDENTS("
                                + "GRADE STRING NOT NULL,"
                                + "CLASS STRING NOT NULL,"
                                + "SNUM INTEGER NOT NULL,"
                                + "NAME STRING NOT NULL, "
                                + "PRIMARY KEY(GRADE, CLASS, SNUM)"
                                + ");";

                try
                {
                    cmd.ExecuteNonQuery();
                }catch(SQLiteException se)
                {
                    //비밀번호 틀렸을 시의 SQLite 에러코드 알아볼 것
                    se.ErrorCode.ToString();
                }

                conn.Close();
            }

            /// <summary>
            /// 선생님 ID로 로그인 합니다.
            /// </summary>
            /// <param name="id">등록된 ID</param>
            /// <param name="password">비밀번호</param>
            public void LoginAs(string id, string password)
            {
                if (!new FileInfo(id + ".sqlite").Exists)
                {
                    MessageBox.Show("ID is not registered");
                    return;
                }

                var connStr = @"Data Source=" + id + ".db;Password=" + password + ";";

                var conn = new SQLiteConnection(connStr);
                conn.Open();

                var cmd = new SQLiteCommand(conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException se)
                {
                    MessageBox.Show(se.ErrorCode.ToString());
                    return;
                }

                conn.Close();

                currentId = id;
                loginState = true;
            }
        }
    }
}
