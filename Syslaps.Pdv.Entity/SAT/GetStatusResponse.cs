using System;

namespace Syslaps.Pdv.Entity.SAT
{
  public class GetStatusResponse : SatResponse
  {
    public string SerialNumber { get; set; }

    public string LanType { get; set; }

    public string LanIP { get; set; }

    public string LanMac { get; set; }

    public string LanMask { get; set; }

    public string LanGateway { get; set; }

    public string LanDns1 { get; set; }

    public string LanDns2 { get; set; }

    public string LanStatus { get; set; }

    public string BatteryLevel { get; set; }

    public string MemoryTotal { get; set; }

    public string MemoryUsed { get; set; }

    public string DateTime { get; set; }

    public string SoftwareVersion { get; set; }

    public string LayoutVersion { get; set; }

    public string LastCfeSent { get; set; }

    public string FirstCfeQueue { get; set; }

    public string LastCfeQueue { get; set; }

    public string LastTransmissionDate { get; set; }

    public string LastComunicationDate { get; set; }

    public string CertificateIssueDate { get; set; }

    public string CertificateExpirationDate { get; set; }

    public string OperationState { get; set; }

    public GetStatusResponse Create(string data)
    {
      try
      {
        GetStatusResponse getStatusResponse = new GetStatusResponse();
        getStatusResponse.RawResponse = data;
        string[] strArray = data.Split('|');
        if (strArray.Length < 3)
        {
          getStatusResponse.ErrorMessage = data;
          return getStatusResponse;
        }
        int result;
        int.TryParse(strArray[0], out result);
        getStatusResponse.SessionCode = result;
        getStatusResponse.ErrorCode = strArray[1].Trim();
        getStatusResponse.ErrorMessage = strArray[2].Trim();
        getStatusResponse.SefazCode = strArray[3].Trim();
        getStatusResponse.SefazMessage = strArray[4].Trim();
        if (strArray.Length < 6)
          return getStatusResponse;
        getStatusResponse.SerialNumber = strArray[5];
        getStatusResponse.LanType = strArray[6];
        getStatusResponse.LanIP = strArray[7];
        getStatusResponse.LanMac = strArray[8];
        getStatusResponse.LanMask = strArray[9];
        getStatusResponse.LanGateway = strArray[10];
        getStatusResponse.LanDns1 = strArray[11];
        getStatusResponse.LanDns2 = strArray[12];
        getStatusResponse.LanStatus = strArray[13];
        getStatusResponse.BatteryLevel = strArray[14];
        getStatusResponse.MemoryTotal = strArray[15];
        getStatusResponse.MemoryUsed = strArray[16];
        getStatusResponse.DateTime = strArray[17];
        getStatusResponse.SoftwareVersion = strArray[18];
        getStatusResponse.LayoutVersion = strArray[19];
        getStatusResponse.LastCfeSent = strArray[20];
        getStatusResponse.FirstCfeQueue = strArray[21];
        getStatusResponse.LastCfeQueue = strArray[22];
        getStatusResponse.LastTransmissionDate = strArray[23];
        getStatusResponse.LastComunicationDate = strArray[24];
        getStatusResponse.CertificateIssueDate = strArray[25];
        getStatusResponse.CertificateExpirationDate = strArray[26];
        getStatusResponse.OperationState = strArray[27];
        return getStatusResponse;
      }
      catch (Exception ex)
      {
        GetStatusResponse getStatusResponse = new GetStatusResponse();
        getStatusResponse.ErrorMessage = string.Format("{0}:{1}", (object) ex.Message, (object) data);
        return getStatusResponse;
      }
    }
  }
}
