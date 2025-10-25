using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace GestaoSaudeIdosos.Web.Filters
{
    public class TempDataAlertFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Controller is not Controller controller)
            {
                await next();
                return;
            }

            if (context.Result is ViewResult || context.Result is PartialViewResult)
            {
                AplicarMensagem(controller, context, "Sucesso", "X-TempData-Sucesso", "TempDataSucesso");
                AplicarMensagem(controller, context, "Erro", "X-TempData-Erro", "TempDataErro");
            }

            await next();
        }

        private static void AplicarMensagem(Controller controller, ResultExecutingContext context, string tempDataKey, string headerName, string viewDataKey)
        {
            if (!controller.TempData.TryGetValue(tempDataKey, out var valor))
            {
                return;
            }

            if (valor is not string mensagem || string.IsNullOrWhiteSpace(mensagem))
            {
                controller.TempData.Remove(tempDataKey);
                return;
            }

            controller.ViewData[viewDataKey] = mensagem;

            var headerValue = Uri.EscapeDataString(mensagem);
            context.HttpContext.Response.Headers[headerName] = headerValue;
            controller.TempData.Remove(tempDataKey);
        }
    }
}
