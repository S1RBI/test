using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mordochka.Models
{
    partial class Tag
    {
        public bool IsChecked
        {
            get
            {
                if (Properties.Settings.Default.editClientId != 0)
                {
                    var ClientTag = db.TagClient.FirstOrDefault(c => c.id_tag == id_tag && c.id_client == Properties.Settings.Default.editClientId);
                    if(ClientTag == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }

            }
            
        }
        static mordochkaEntities db = new mordochkaEntities();
    }
}
