using System;

namespace Syslaps.Pdv.Entity.SAT
{
  public class SatResponse
  {
    public int SessionCode { get; set; }

    public string ErrorCode { get; set; }

    public string ErrorCode2 { get; set; }

    public string ErrorMessage { get; set; }

    public string SefazCode { get; set; }

    public string SefazMessage { get; set; }

    public string RawResponse { get; set; }

    public SatResponse Create(string data)
    {
      try
      {
        SatResponse satResponse = new SatResponse();
        satResponse.RawResponse = data;
        string[] strArray = data.Split('|');
        if (strArray.Length < 5)
        {
          satResponse.ErrorMessage = data;
          return satResponse;
        }
        int result;
        int.TryParse(strArray[0], out result);
        satResponse.SessionCode = result;
        satResponse.ErrorCode = strArray[1].Trim();
        satResponse.ErrorMessage = strArray[2].Trim();
        satResponse.SefazCode = strArray[3].Trim();
        satResponse.SefazMessage = strArray[4].Trim();
        return satResponse;
      }
      catch (Exception ex)
      {
        return new SatResponse() { ErrorMessage = string.Format("{0}:{1}", (object) ex.Message, (object) data) };
      }
    }
  }
}
