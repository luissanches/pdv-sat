using System;
using System.Text;

namespace Syslaps.Pdv.Entity.SAT
{
  public class SendResponse : SatResponse
  {
    public string Xml { get; set; }

    public string Xml64 { get; set; }

    public string TimeStamp { get; set; }

    public string InvoiceKey { get; set; }

    public string Total { get; set; }

    public string CpfCnpj { get; set; }

    public string QrCodeSignature { get; set; }

    public SendResponse Create(string data)
    {
      try
      {
        SendResponse sendResponse = new SendResponse();
        sendResponse.RawResponse = data;
        string[] strArray = data.Split('|');
        if (strArray.Length < 6)
        {
          sendResponse.ErrorMessage = data;
          return sendResponse;
        }
        int result;
        int.TryParse(strArray[0], out result);
        sendResponse.SessionCode = result;
        sendResponse.ErrorCode = strArray[1].Trim();
        sendResponse.ErrorCode2 = strArray[2].Trim();
        sendResponse.ErrorMessage = strArray[3].Trim();
        sendResponse.SefazCode = strArray[4].Trim();
        sendResponse.SefazMessage = strArray[5].Trim();
        if (strArray.Length >= 7)
          sendResponse.Xml64 = strArray[6];
        if (strArray.Length >= 8)
          sendResponse.TimeStamp = strArray[7];
        if (strArray.Length >= 9)
          sendResponse.InvoiceKey = strArray[8].ToLower().Replace("cfe", "");
        if (strArray.Length >= 10)
          sendResponse.Total = strArray[9].Trim();
        if (strArray.Length >= 11)
          sendResponse.CpfCnpj = strArray[10].Trim();
        if (strArray.Length >= 12)
          sendResponse.QrCodeSignature = strArray[11];
        if (!string.IsNullOrEmpty(sendResponse.Xml64))
          sendResponse.Xml = Encoding.UTF8.GetString(Convert.FromBase64String(sendResponse.Xml64));
        return sendResponse;
      }
      catch (Exception ex)
      {
        SendResponse sendResponse = new SendResponse();
        sendResponse.ErrorMessage = string.Format("{0}:{1}", (object) ex.Message, (object) data);
        return sendResponse;
      }
    }
  }
}
