using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace ExcelTranscript
{
	public class SmsService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			return Task.FromResult(0);
		}
	}
}
