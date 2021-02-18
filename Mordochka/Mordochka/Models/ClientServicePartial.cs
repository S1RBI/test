using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mordochka.Models
{
    partial class ClientService
    {
        public int FileCount
        {
            get
            {
                return db.Files.Where(c => c.id_client_service == id).ToList().Count();
            }
        }
        static mordochkaEntities db = new mordochkaEntities();
    }
}
