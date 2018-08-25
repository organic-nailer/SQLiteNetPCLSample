using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using PCLStorage;
using SQLite;

namespace SQLiteNetPCLSample.SQLites
{
    [Table("Items")]
    public class DBFuncItem
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        
        public string FuncName { get; set; }

        public string Functions { get; set; }
    }

    public class FuncItemService
    {
        /// <summary>
        /// SQLiteデータベースへのコネクションを取得する。
        /// 取得したコネクションは取得した側で正しくクローズ処理すること。
        /// from: http://www.nuits.jp/entry/2016/06/27/191636
        /// </summary>
        /// <returns></returns>
        private static async Task<SQLiteConnection> CreateConnection()
        {
            const string DatabaseFileName = "functions.db3";
            // ルートフォルダを取得する
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            // DBファイルの存在チェックを行う
            var result = await rootFolder.CheckExistsAsync(DatabaseFileName);
            if (result == ExistenceCheckResult.NotFound)
            {
                // 存在しなかった場合、新たにDBファイルを作成しテーブルも併せて新規作成する
                IFile file = await rootFolder.CreateFileAsync(DatabaseFileName, CreationCollisionOption.ReplaceExisting);
                var connection = new SQLiteConnection(file.Path);
                connection.CreateTable<DBFuncItem>();
                return connection;
            }
            else
            {
                // 存在した場合、そのままコネクションを作成する
                IFile file = await rootFolder.CreateFileAsync(DatabaseFileName, CreationCollisionOption.OpenIfExists);
                return new SQLiteConnection(file.Path);
            }
        }

        public async static Task<List<DBFuncItem>> GetList()
        {
            using(var connection = await CreateConnection())
            {
                return connection.Table<DBFuncItem>().ToList();
            }
        }

        /// <summary>
        /// IDで取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async static Task<DBFuncItem> GetById(int id)
        {
            using(var connection = await CreateConnection())
            {
                var searched = connection.Table<DBFuncItem>().Where(x => x.Id == id);

                if(searched.Count() == 0)
                {
                    throw new Exception("Idが存在しません。 id:" + id);
                }
                else if(searched.Count() != 1)
                {
                    throw new Exception("Idが複数存在します。 id:" + id);
                }
                else
                {
                    return searched.ToList()[0];
                }
            }
        }

        /// <summary>
        /// FuncNameで取得
        /// </summary>
        /// <param name="funcname"></param>
        /// <returns></returns>
        public async static Task<DBFuncItem> GetByName(string funcname)
        {
            using (var connection = await CreateConnection())
            {
                var searched = connection.Table<DBFuncItem>().Where(x => x.FuncName == funcname);

                if (searched.Count() == 0)
                {
                    throw new Exception("Idが存在しません。 funcname:" + funcname);
                }
                else if (searched.Count() != 1)
                {
                    throw new Exception("Idが複数存在します。 funcname:" + funcname);
                }
                else
                {
                    return searched.ToList()[0];
                }
            }
        }

        
        public async static Task<bool> IsExist(DBFuncItem funcitem)
        {
            using (var connection = await CreateConnection())
            {
                return connection.Table<DBFuncItem>().Any(x => x.Id == funcitem.Id);
            }
        }

        public async static Task<bool> IsExist(int id)
        {
            using (var connection = await CreateConnection())
            {
                return connection.Table<DBFuncItem>().Any(x => x.Id == id);
            }
        }
        
        public async static Task<bool> IsExist(string funcname)
        {
            using (var connection = await CreateConnection())
            {
                return connection.Table<DBFuncItem>().Any(x => x.FuncName == funcname);
            }
        }

        public async static Task Add(DBFuncItem funcitem)
        {
            using (var connection = await CreateConnection())
            {
                connection.Insert(funcitem, typeof(DBFuncItem));
            }
        }

        public async static Task Add(string funcname, string functions)
        {
            using (var connection = await CreateConnection())
            {
                connection.Insert(new DBFuncItem { FuncName = funcname, Functions = functions }, typeof(DBFuncItem));
            }
        }

        public async static Task Delete(DBFuncItem funcitem)
        {
            using (var connection = await CreateConnection())
            {
                connection.Delete<DBFuncItem>(funcitem.Id);
            }
        }

        public async static Task Update(DBFuncItem funcItem)
        {
            using (var connection = await CreateConnection())
            {
                connection.Update(funcItem, typeof(DBFuncItem));
            }
        }
    }
}
