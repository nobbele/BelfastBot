using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YohaneBot.Services.Communiciation;
using YohaneWebClient.Models;

namespace YohaneWebClient.Controllers
{
    public class DiscordEmulatorController : Controller
    {
        private WebCommunicationService _communication;

        public DiscordEmulatorController(ICommunicationService communication)
        {
            _communication = communication as WebCommunicationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Index(string input)
        {
            await _communication.SendMessageAsync(null, "hello world");
            DiscordEmulatorModel model = new DiscordEmulatorModel();
            model.Messages = _communication.Messages.ToArray();
            return View(model);
        }
    }
}