using System;
using System.Text;

namespace Syslaps.Pdv.Entity.SAT
{
  public class InquireResponse : SatResponse
  {
    public string Xml { get; set; }

    public string Xml64 { get; set; }

    public string TimeStamp { get; set; }

    public string DocumentNumber { get; set; }

    public string InvoiceKey { get; set; }

    public InquireResponse Create(string data)
    {
      try
      {
        InquireResponse inquireResponse = new InquireResponse();
        inquireResponse.RawResponse = data;
        string[] strArray = data.Split('|');
        if (strArray.Length < 5)
        {
          inquireResponse.ErrorMessage = data;
          return inquireResponse;
        }
        int result;
        int.TryParse(strArray[0], out result);
        inquireResponse.SessionCode = result;
        if (strArray.Length >= 2)
          inquireResponse.ErrorCode = strArray[1].Trim();
        if (strArray.Length >= 3)
          inquireResponse.ErrorMessage = strArray[2].Trim();
        if (strArray.Length >= 4)
          inquireResponse.SefazCode = strArray[3].Trim();
        if (strArray.Length >= 5)
          inquireResponse.SefazMessage = strArray[4].Trim();
        if (strArray.Length >= 6)
          inquireResponse.Xml64 = strArray[5];
        if (strArray.Length >= 7)
          inquireResponse.TimeStamp = strArray[6];
        if (strArray.Length >= 8)
          inquireResponse.DocumentNumber = strArray[7];
        if (strArray.Length >= 9)
          inquireResponse.InvoiceKey = strArray[8];
        inquireResponse.Xml = Encoding.UTF8.GetString(Convert.FromBase64String(inquireResponse.Xml64));
        return inquireResponse;
      }
      catch (Exception ex)
      {
        InquireResponse inquireResponse = new InquireResponse();
        inquireResponse.ErrorMessage = string.Format("{0}:{1}", (object) ex.Message, (object) data);
        return inquireResponse;
      }
    }
  }
}
