using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mordochka.Models
{
    partial class Client
    {
        public string LastDate
        {
            get
            {
                var log = db.EnterClient.Where(c => c.id_client == id_client).ToList();
                if(log !=null && log.Count > 0)
                {
                    return log[log.Count - 1].date_enter.ToString();
                }
                else
                {
                    return null;
                }
            }
        }
        public int CountEnter
        {
            get
            {
                return db.EnterClient.Where(c => c.id_client == id_client).ToList().Count;
            }
        }
        public List<Tag> TagList
        {
            get
            {
                return db.TagClient.Where(c => c.id_client == id_client).Select(c => c.Tag).ToList();
            }
        }
        static mordochkaEntities db = new mordochkaEntities();
    }
}
