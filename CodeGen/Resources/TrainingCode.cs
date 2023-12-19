﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Resources
{
    public class TrainingCode
    {
        public static readonly string ExampleCode = @"

Using Microsoft.AspNetCore.Mvc;
Using Newtonsoft.Json;
Using Swashbuckle.AspNetCore.Annotations;

Namespace Commerce.Adapters.<#= ControllerMetaData.CompanyName #>Adapter.Controllers
{
    [Route(""v0/Adapter"")]
    [ApiController]
    Public Class <#= ControllerMetaData.CompanyName #>AdapterController :   ControllerBase
    {
        Private ReadOnly IMapper mapper;
        Private ReadOnly ILogger<<#= ControllerMetaData.CompanyName #>AdapterController> logger;
        Private ReadOnly IAvailabilityService availabilityService;
        Private ReadOnly IOrderService orderService;
        Private ReadOnly IPaymentService paymentService;

        Public <#= ControllerMetaData.CompanyName #>AdapterController(IAvailabilityService availabilityService, IOrderService orderService, ILogger<BokunAdapterController> logger, IMapper mapper, IPaymentService paymentService)
        {
            this.logger = logger ?? throw New ArgumentNullException(NameOf(logger));
            this.availabilityService = availabilityService ?? throw New ArgumentNullException(NameOf(availabilityService));
            this.orderService = orderService ?? throw New ArgumentNullException(NameOf(orderService));
            this.mapper = mapper ?? throw New ArgumentNullException(NameOf(mapper)); ;
            this.paymentService = paymentService ?? throw New ArgumentNullException(NameOf(paymentService)); ;
        }


  /// <summary>
        /// Availability search
        /// </summary>
        /// <param name=""request""></param>
        [HttpPost(""Availability/Search"")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, ""The request is invalid"")]
        public async Task<ActionResult<AdapterResultMessage>> AvailabilitySearch([FromBody] AdapterMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var ariRequest = mapper.Map<ServiceMessage>(request);
                using (logger.BeginScope(new Dictionary<string, object>
                {
                    [""RequestBody""] = ariRequest,
                }))
                {
                    logger.LogInformation(""Availability search request"", ariRequest);
                }

                var ariResponse = await availabilityService.AvailabilitySearch(ariRequest, cancellationToken);

                if (ariResponse.ProblemDetails != null || ariResponse.Error)
                {
                    var errorMessage = ariResponse.ProblemDetails?.Detail
                        ?? ariResponse.ErrorMessage
                        ?? string.Empty;

                    logger.LogError(errorMessage);

                    var errorResponse = new AdapterResultMessage
                    {
                        ErrorMessage = errorMessage,
                        ErrorOccured = true,
                        Result = JsonConvert.SerializeObject(ariResponse)
                    };

                    var statusCode = ariResponse.ProblemDetails?.Status
                        ?? StatusCodes.Status500InternalServerError;

                    return StatusCode(statusCode, errorResponse);
                }

                var response = new AdapterResultMessage
                {
                    ErrorOccured = false,
                    ErrorMessage = string.Empty,
                    Result = JsonConvert.SerializeObject(ariResponse)
                };

                return Ok(response);
            }
            catch (JsonException ex)
            {
                using (logger.BeginScope(new Dictionary<string, object>
                {
                    [""RequestPayload""] = request.Payload,
                }))
                {
                    logger.LogWarning(ex, ""Attempting to parse availability request payload"");
                }
                return BadRequest();
            }
        }


        /// <summary>
        /// Create order
        /// </summary>
        /// <param name=""request""></param>
        [HttpPost(""Order/Create"")]
        public async Task<ActionResult<AdapterResultMessage>> CreateOrder([FromBody] AdapterMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var createOrderRequest = mapper.Map<ServiceMessage>(request);
                using (logger.BeginScope(new Dictionary<string, object>
                {
                    [""RequestBody""] = createOrderRequest,
                }))
                {
                    logger.LogInformation(""Create order request"", createOrderRequest);
                }

                var createOrderResponse = await orderService.CreateOrder(createOrderRequest, cancellationToken);

                if (createOrderResponse.ProblemDetails != null || createOrderResponse.Error)
                {
                    var errorMessage = createOrderResponse.ProblemDetails?.Detail
                        ?? createOrderResponse.ErrorMessage
                        ?? string.Empty;

                    logger.LogError(errorMessage);

                    var errorResponse = new AdapterResultMessage
                    {
                        ErrorMessage = errorMessage,
                        ErrorOccured = true,
                        Result = JsonConvert.SerializeObject(createOrderResponse)
                    };

                    var statusCode = createOrderResponse.ProblemDetails?.Status
                        ?? StatusCodes.Status500InternalServerError;

                    return StatusCode(statusCode, errorResponse);
                }

                var response = new AdapterResultMessage
                {
                    ErrorOccured = false,
                    ErrorMessage = string.Empty,
                    Result = JsonConvert.SerializeObject(createOrderResponse)
                };

                return Ok(response);
            }
            catch (JsonException ex)
            {
                using (logger.BeginScope(new Dictionary<string, object>
                {
                    [""RequestPayload""] = request.Payload,
                }))
                {
                    logger.LogWarning(ex, ""Attempting to parse create order request payload"");
                }
                return BadRequest();
            }
        }


        /// <summary>
        /// Cancel order
        /// </summary>
        /// <param name=""adapterMessageRequest""></param>
        [HttpPost(""Order/Cancel"")]
        public async Task<ActionResult<AdapterResultMessage>> CancelOrder([FromBody] AdapterMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var cancelOrderRequest = mapper.Map<ServiceMessage>(request);
                using (logger.BeginScope(new Dictionary<string, object>
                {
                    [""RequestBody""] = cancelOrderRequest,
                }))
                {
                    logger.LogInformation(""Availability search request"", cancelOrderRequest);
                }

                var cancelOrderResponse = await orderService.CancelOrder(cancelOrderRequest, cancellationToken);

                if (cancelOrderResponse.ProblemDetails != null || cancelOrderResponse.Error)
                {
                    var errorMessage = cancelOrderResponse.ProblemDetails?.Detail
                        ?? cancelOrderResponse.ErrorMessage;

                    logger.LogError(errorMessage);

                    var errorResponse = new AdapterResultMessage
                    {
                        ErrorMessage = errorMessage,
                        ErrorOccured = true,
                        Result = JsonConvert.SerializeObject(cancelOrderResponse)
                    };

                    var statusCode = cancelOrderResponse.ProblemDetails?.Status
                        ?? StatusCodes.Status500InternalServerError;

                    return StatusCode(statusCode, errorResponse);
                }

                var response = new AdapterResultMessage
                {
                    ErrorOccured = false,
                    ErrorMessage = string.Empty,
                    Result = JsonConvert.SerializeObject(cancelOrderResponse)
                };

                return Ok(response);
            }
            catch (JsonException ex)
            {
                using (logger.BeginScope(new Dictionary<string, object>
                {
                    [""RequestPayload""] = request.Payload,
                }))
                {
                    logger.LogWarning(ex, ""Attempting to parse cancel order request payload"");
                }
                return BadRequest();
            }
        }

        /// <summary>
        /// Register payment
        /// </summary>
        /// <param name=""adapterMessageRequest""></param>
        [HttpPost(""Order/Payment"")]
        public async Task<ActionResult<AdapterResultMessage>> RegisterPayment([FromBody] AdapterMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}

";
    }
}