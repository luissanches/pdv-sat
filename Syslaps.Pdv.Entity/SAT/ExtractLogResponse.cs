using System;

namespace Syslaps.Pdv.Entity.SAT
{
  public class ExtractLogResponse : SatResponse
  {
    public string LogFile { get; set; }

    public ExtractLogResponse Create(string data)
    {
      try
      {
        ExtractLogResponse extractLogResponse = new ExtractLogResponse();
        extractLogResponse.RawResponse = data;
        string[] strArray = data.Split('|');
        if (strArray.Length < 4)
        {
          extractLogResponse.ErrorMessage = data;
          return extractLogResponse;
        }
        int result;
        int.TryParse(strArray[0], out result);
        extractLogResponse.SessionCode = result;
        extractLogResponse.ErrorCode = strArray[1].Trim();
        extractLogResponse.ErrorMessage = strArray[2].Trim();
        extractLogResponse.SefazCode = strArray[3].Trim();
        if (strArray.Length >= 5)
          extractLogResponse.SefazMessage = strArray[4].Trim();
        if (strArray.Length >= 6)
          extractLogResponse.LogFile = strArray[5].Trim();
        return extractLogResponse;
      }
      catch (Exception ex)
      {
        ExtractLogResponse extractLogResponse = new ExtractLogResponse();
        extractLogResponse.ErrorMessage = string.Format("{0}:{1}", (object) ex.Message, (object) data);
        return extractLogResponse;
      }
    }
  }
}
