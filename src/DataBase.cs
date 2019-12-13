using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Environment = System.Environment;

using SQLite;
using DataBase;

namespace src {
    static class DataBase {
        static private string dbName = "database.db";

        static private SQLiteConnection _db = null;
        static public SQLiteConnection db {
            get {
                if (_db == null) {
                    SetConnection();
                    InitDB();
                }
                return db;
            }
        }

        public static string GetDatabasePath() {
            string databasePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string completePath = Path.Combine(databasePath, dbName);
            return completePath;
        }

        public static void SetConnection() {
            string completePath = GetDatabasePath();
            _db = new SQLiteConnection(completePath);
        }

        private static void InitDB() {
            _db.CreateTable<Kanbans>();
            _db.CreateTable<Columns>();
            _db.CreateTable<Ticket>();
        }

    }
}