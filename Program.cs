using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace dotnet_SQLite_creation
{
    class Program
    {
        //See: https://www.developersoapbox.com/connecting-to-a-sqlite-database-using-net-core/

        static void Main(string[] args)
        {

            //Set strings
            string tableName = "favorite_coffee";
            string columnName = "name";
            var entryList = new List<String>();

            entryList.Add("Colombian");
            entryList.Add("Puerto Rican");
            entryList.Add("Hawaiian");
            entryList.Add("Ethiopian");

            //Build connection string
            var ConnectionStringBuilder = new SqliteConnectionStringBuilder();

            ConnectionStringBuilder.DataSource = "./SQLiteDB.db";

            using (var connection = new SqliteConnection(ConnectionStringBuilder.ConnectionString))
            {
                connection.Open();
                //Create new table (drop existing)
                var deleteTableCommand = new SqliteCommand($"DROP TABLE IF EXISTS {tableName}", connection);
                deleteTableCommand.ExecuteNonQuery();

                var createTableCommand = new SqliteCommand($"CREATE TABLE {tableName}({columnName} VARCHAR(50))", connection);
                createTableCommand.ExecuteNonQuery();

                //Add table data
                using (var transaction = connection.BeginTransaction())
                {  
                    foreach (string entry in entryList)
                    {
                        var insertCommand = new SqliteCommand($"INSERT INTO {tableName} VALUES('{entry}')", connection, transaction);
                        insertCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }

                var selectCommand = new SqliteCommand($"SELECT {columnName} from {tableName}", connection);

                using (var reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read() == true) {
                        System.Console.WriteLine($"Table Name: {tableName}");
                        System.Console.WriteLine($"Column Name: {columnName}");
                        
                        while (reader.Read() == true)
                        {
                            var message = reader.GetString(0);
                            System.Console.WriteLine(message);
                        }
                    } else
                    {
                        System.Console.WriteLine("No data!");
                    }

                }
            }

        }
    }
}
