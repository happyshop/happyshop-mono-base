using System.Linq;
using System.Net.Mail;
using System.Text;

namespace HappyShop.ServiceConnector
{
  public class SimpleSmtpClient
  {
    private readonly MailMessage _mailMessage;
    private readonly StringBuilder _bodyBuilder = new StringBuilder();

    public SimpleSmtpClient(string subject, string receipients)
    {
      _mailMessage = new MailMessage { From = new MailAddress((string)Configuration.Static.Merged.SimpleSmtpFromAddress), Subject = subject};
      receipients.Split(';').ToList().ForEach(_mailMessage.To.Add);
    }

    public SimpleSmtpClient AddBody(string body)
    {
      _bodyBuilder.AppendLine(body);
      return this;
    }

    public SimpleSmtpClient AddBody(string format, params object[] args)
    {
      return AddBody(string.Format(format, args));
    }

    public void Send()
    {
      _mailMessage.Body = _bodyBuilder.ToString();

      //System.Net.NetworkCredential nc = new System.Net.NetworkCredential("user@host.com", "secret");
      //var smtpClient = new SmtpClient { Host = "smtp.gmail.com", EnableSsl = true, Credentials = nc };

      var smtpClient = new SmtpClient { Host = (string)Configuration.Static.Merged.SimpleSmtpServerHost, EnableSsl = true };
      smtpClient.Send(_mailMessage);
    }
  }
}