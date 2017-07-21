using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

[ServiceContract]
public interface Ischat
{
    [OperationContract]
    Task MandaMsg(string msg, int idusuario);

    [OperationContract]
    Task MsgNuevo(string msg, int idusuario);
}
