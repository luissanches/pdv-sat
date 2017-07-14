using System;
using System.Text;

namespace Syslaps.Pdv.Entity.SAT
{
  public class CancelResponse : SendResponse
  {
    public CancelResponse Create(string data)
    {
      try
      {
        CancelResponse cancelResponse = new CancelResponse();
        cancelResponse.RawResponse = data;
        string[] strArray = data.Split('|');
        if (strArray.Length < 4)
        {
          cancelResponse.ErrorMessage = data;
          return cancelResponse;
        }
        int result;
        int.TryParse(strArray[0], out result);
        cancelResponse.SessionCode = result;
        cancelResponse.ErrorCode = strArray[1];
        cancelResponse.ErrorCode2 = strArray[2];
        cancelResponse.ErrorMessage = strArray[3];
        if (strArray.Length >= 5)
          cancelResponse.SefazCode = strArray[4].Trim();
        if (strArray.Length >= 6)
          cancelResponse.SefazMessage = strArray[5].Trim();
        if (strArray.Length >= 7)
          cancelResponse.Xml64 = strArray[6].Trim();
        if (strArray.Length >= 8)
          cancelResponse.TimeStamp = strArray[7].Trim();
        if (strArray.Length >= 9)
          cancelResponse.InvoiceKey = strArray[8].ToLower().Replace("cfe", "");
        if (strArray.Length >= 10)
          cancelResponse.Total = strArray[9].Trim();
        if (strArray.Length >= 11)
          cancelResponse.CpfCnpj = strArray[10].Trim();
        if (strArray.Length >= 12)
          cancelResponse.QrCodeSignature = strArray[11].Trim();
        cancelResponse.Xml = Encoding.UTF8.GetString(Convert.FromBase64String(cancelResponse.Xml64));
        return cancelResponse;
      }
      catch (Exception ex)
      {
        CancelResponse cancelResponse = new CancelResponse();
        cancelResponse.ErrorMessage = string.Format("{0}:{1}", (object) ex.Message, (object) data);
        return cancelResponse;
      }
    }
  }
}
