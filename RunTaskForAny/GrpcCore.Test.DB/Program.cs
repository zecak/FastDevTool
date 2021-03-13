using GrpcCore.Common.Security;
using GrpcCore.DB;
using GrpcCore.DB.Database;
using GrpcCore.DB.DBCache;
using System;
using System.Linq;

namespace GrpcCore.Test.DB
{
    class Program
    {
        static void Main(string[] args)
        {
            //var redisHelper = new Redis("127.0.0.1:6379,password=123456");
            //bool r1 = redisHelper.Insert("key", "gg");
            //if (r1)
            //{
            //    Console.WriteLine(redisHelper.Get("key"));
            //    redisHelper.Remove("key");
            //}

            //using (var db = new BloggingContext())
            //{

            //    db.Database.EnsureCreated();

            //    // Note: This sample requires the database to be created before running.

            //    // Create
            //    Console.WriteLine("Inserting a new blog");
            //    db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
            //    db.SaveChanges();

            //    // Read
            //    Console.WriteLine("Querying for a blog");
            //    var blog = db.Blogs
            //        .OrderBy(b => b.BlogId)
            //        .First();

            //    //Update
            //    Console.WriteLine("Updating the blog and adding a post");
            //    blog.Url = "https://devblogs.microsoft.com/dotnet";
            //    blog.Posts = new System.Collections.Generic.List<Post>();
            //    blog.Posts.Add(
            //        new Post { Title = "Hello World", Content = "I wrote an app using EF Core!" });
            //    db.SaveChanges();

            //    //// Delete
            //    //Console.WriteLine("Delete the blog");
            //    //db.Remove(blog);
            //    //db.SaveChanges();
            //}


            try
            {
                Console.WriteLine("商家余额:" + Helper.GetForWanjuOfflineMerchantWallet());
                decimal price = 100;
                Helper.UpdateForWanjuOfflineMerchantWallet(price);
                Console.WriteLine("商家充值成功:"+ price);

                Console.WriteLine("商家余额:" + Helper.GetForWanjuOfflineMerchantWallet());
            }
            catch (Exception ex)
            {
                Console.WriteLine("商家充值失败:" + ex.Message);
            }

            try
            {
                
                string uuid = "b7d909afa92d11e995ed1866da1b6893";
                decimal price = 10;

                Console.WriteLine("用户余额:" + Helper.GetForWanjuOfflineEmployeeWallet(uuid));

                Helper.UpdateForWanjuOfflineEmployeeWallet(uuid,price);
                Console.WriteLine("用户充值成功:" + price);

                Console.WriteLine("用户余额:" + Helper.GetForWanjuOfflineEmployeeWallet(uuid));
            }
            catch (Exception ex)
            {
                Console.WriteLine("用户充值失败:" + ex.Message);
            }


        }
    }
}
