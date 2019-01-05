using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace WasteReducer
{

    /// <summary>
    /// This represents a placeholder for the real database.
    /// The text file should be under res\products.db by default.
    /// Each line should be of the following format:
    /// img_id;product_name;category;is_diary;limit;price;facing
    /// Empty lines and lines starting with # are ignored
    /// If an invalid format is found, a Database Exception is thrown
    /// </summary>
    class DatabaseHandler
    {
        private const string database_name = @"res\products.db";
        private List<ProductBase> database;
        public List<ProductBase> Database { get => database; } 
        public readonly string PATH = "";

        /// <summary>
        /// Connects to the database
        /// </summary>
        private void Connect() 
        {
            ///Placeholder for actual database;
            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(database_name);
            }
            catch
            {
                throw new DatabaseException("Could not open File");
            }
            database = new List<ProductBase>();

            try
            {
                foreach (string line in lines)
                {
                    ///# is considered a comment.
                    if (line.Length<4 || line[0].Equals('#'))
                        continue;

                    string[] attributes = line.Split(';');
                    this.database.Add(
                        new ProductBase(long.Parse(attributes[0]), PATH + attributes[0] + ".jpg", attributes[1],attributes[2],
                        int.Parse(attributes[3])==1,int.Parse(attributes[4]), double.Parse(attributes[5]),int.Parse(attributes[6]))
                        );
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Database corrupt: " + ex.Source + ex.Message);
            }

        }

        /// <summary>
        /// Returns the first product with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProductBase GetProduct(long id)
        {
            var target = database.Find(p => p.Id == id);
            if (target == null)
                return null;
            else
            {
                return new ProductBase(target);
            }
        }

        /// <summary>
        /// Initializes connection to database. If connection failes an exception is thrown and null is returned
        /// </summary>
        public DatabaseHandler(string path)
        {
            PATH = path;
            Connect();

        }
    }
}
