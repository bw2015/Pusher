using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.Models
{
    /// <summary>
    /// 消息日志
    /// </summary>
    [Table("push_Message")]
    public partial class PushMessage
    {

        #region  ========  構造函數  ========
        public PushMessage() { }

        public PushMessage(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "LogID":
                        this.LogID = (Guid)reader[i];
                        break;
                    case "Channel":
                        this.Channel = (string)reader[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)reader[i];
                        break;
                    case "Content":
                        this.Content = (string)reader[i];
                        break;
                    case "Count":
                        this.Count = (int)reader[i];
                        break;
                }
            }
        }


        public PushMessage(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "LogID":
                        this.LogID = (Guid)dr[i];
                        break;
                    case "Channel":
                        this.Channel = (string)dr[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)dr[i];
                        break;
                    case "Content":
                        this.Content = (string)dr[i];
                        break;
                    case "Count":
                        this.Count = (int)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        /// <summary>
        /// 消息编号
        /// </summary>
        [Column("LogID"), Key]
        public Guid LogID { get; set; }


        /// <summary>
        /// 所属频道
        /// </summary>
        [Column("Channel")]
        public string Channel { get; set; }


        [Column("CreateAt")]
        public long CreateAt { get; set; }


        /// <summary>
        /// 消息内容
        /// </summary>
        [Column("Content")]
        public string Content { get; set; }


        /// <summary>
        /// 消息数量
        /// </summary>
        [Column("Count")]
        public int Count { get; set; }

        #endregion


        #region  ========  扩展方法  ========

        public static implicit operator PushMessage(MessageLog log)
        {
            return new PushMessage
            {
                LogID = Guid.NewGuid(),
                Channel = log.Channel,
                Content = log.Content,
                Count = log.Count,
                CreateAt = log.CreateAt
            };
        }
        #endregion

    }

}
