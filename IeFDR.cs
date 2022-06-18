using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Data;
using System.ServiceModel.Web;
using System.Text;


namespace IN_eFDR
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IeFDR" in both code and config file together.
    [ServiceContract]
    public interface IeFDR
    {
        [OperationContract]
        string VALIDATE_PAN(string IN_URL, string IN_PAN_NUMBER, string IN_CLIENT_IP, string IN_SOURCE);
       
    }
}
