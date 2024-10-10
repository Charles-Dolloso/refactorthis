using Microsoft.AspNetCore.Mvc;
using RefactorThis.Application.Dto.Request;
using RefactorThis.Domain.Contants;
using RefactorThis.Domain.Entities;
using RefactorThis.Domain.Interfaces;

namespace RefactorThis.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly ILogger<InvoiceController> _logger;
        private readonly IInvoiceService _invoiceService;

        private string controllerName = nameof(InvoiceController);
        private string actionMethodName = "";

        public InvoiceController(ILogger<InvoiceController> logger, IInvoiceService invoiceService)
        {
            _logger = logger;
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// ProcessPayment
        /// </summary>
        /// <param name="model"></param>
        /// <returns>message</returns>
        /// <response code="200">Successfuly process payments</response>
        /// <response code="422">Failed Custom Validation</response>
        /// <response code="500">Error occured during the process</response>
        [HttpPost("ProcessPayment")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ProcessPaymentAsync([FromBody] PaymentRequest request)
        {
            actionMethodName = nameof(ProcessPaymentAsync);
            var fullActionName = GetFullActionMethodName(controllerName, actionMethodName);

            try
            {
                _logger.LogInformation("--- Start in ProcessPaymentAsync API");
                #region get customer result

                Payment payment = new Payment { Reference = request.Reference, Amount = request.Amount };

                var response = await _invoiceService.ProcessPaymentAsync(payment);

                #endregion
                _logger.LogInformation("--- End in ProcessPaymentAsync API");

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, LoggerConstants.LogError, ex.Message, fullActionName, controllerName, actionMethodName, Environment.StackTrace);
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new { error_message = $"Customer API - {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerConstants.LogError, ex.Message, fullActionName, controllerName, actionMethodName, Environment.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error_message = $"Customer API - {ex.Message}" });
            }
        }

        private string GetFullActionMethodName(string controllerName, string actionMethodName)
        {
            return $"{MainConstants.Common.MS_NAME}.{controllerName}.{actionMethodName}";
        }
    }
}