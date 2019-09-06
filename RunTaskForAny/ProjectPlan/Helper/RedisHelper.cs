using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPlan.Helper
{
    public class RedisHelper
    {

        #region 基本用户名密码，使用配置文件
        /// <summary>
        /// 写入redis服务器的ip+port
        /// </summary>
        public static string WriteServerList = ConfigurationManager.AppSettings["WriteServerList"];
        /// <summary>
        /// 读取服务器的ip +port
        /// </summary>
        public static string ReadServerList = ConfigurationManager.AppSettings["ReadServerList"];
        /// <summary>
        /// 服务器的密码
        /// </summary>
        public static string Password = ConfigurationManager.AppSettings["Password"];

        public static int RedisMaxReadPool = int.Parse(ConfigurationManager.AppSettings["redis_max_read_pool"]);
        public static int RedisMaxWritePool = int.Parse(ConfigurationManager.AppSettings["redis_max_write_pool"]);

        #endregion

        #region Resid基础连接设置


        /// <summary>
        /// redis程序池
        /// </summary>
        private static PooledRedisClientManager _redisprcm;

        /// <summary>
        /// 连接
        /// </summary>
        private static void CreateManager()
        {
            try
            {
                string[] writeServerList = redisSplitString(WriteServerList, ",");
                string[] readServerList = redisSplitString(ReadServerList, ",");


                _redisprcm = new PooledRedisClientManager(readServerList, writeServerList,
                                       new RedisClientManagerConfig
                                       {
                                           MaxWritePoolSize = RedisMaxWritePool,
                                           MaxReadPoolSize = RedisMaxReadPool,
                                           AutoStart = true,
                                       });
                //如果服务端有密码则设置
                string pwd = Password;
                if (!string.IsNullOrEmpty(pwd))
                {
                    _redisprcm.GetClient().Password = pwd;
                }

            }
            catch (Exception ex)
            {

                _redisprcm = null;
            }


        }

        private static string[] redisSplitString(string strSource, string split)
        {
            return strSource.Split(split.ToArray());
        }


        /// <summary>
        /// 设置redis操作对象
        /// </summary>
        /// <returns></returns>
        public static IRedisClient GetClient()
        {
            if (_redisprcm == null)
                CreateManager();


            return _redisprcm.GetClient();
        }


        #endregion

        #region Object T类型

        /// <summary>
        /// 写入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="redisClient"></param>
        /// <returns></returns>
        public static bool Set<T>(string key, T value, IRedisClient redisClient)
        {
            bool flag = false;
            try
            {
                redisClient.Set<T>(key, value);
                redisClient.Save();
                flag = true;
            }
            catch (Exception)
            {
                flag = false;

            }
            return flag;
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="redisClient"></param>
        /// <returns></returns>
        public static T Get<T>(string key, IRedisClient redisClient)
        {
            T Y = default(T);
            try
            {
                Y = redisClient.Get<T>(key);
            }
            catch (Exception EX)
            {
                Y = default(T);

            }
            return Y;
        }

        #endregion

        #region 删除缓存

        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Remove(string key, IRedisClient redisClient)
        {
            var rz = redisClient.Remove(key);
            redisClient.Save();
            return rz;
        }
        #endregion

        #region 释放内存
        /// <summary>
        /// 释放资源
        /// </summary>
        public static void Dispose(IRedisClient redisClient)
        {
            if (redisClient != null)
            {
                redisClient.Dispose();
                redisClient = null;
            }
            //强制垃圾回收
            GC.Collect();

        }
        #endregion
    }

}
