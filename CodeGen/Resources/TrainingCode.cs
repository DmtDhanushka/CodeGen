using System;
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

        public static readonly string ExampleProductEndpoints = @"

        /// <summary>
        /// <para>Product lookup.</para>
        /// <para>
        /// One entry in HttpEndpoints should be set up:
        /// Product search: {{ApiBase}}/api/activity
        /// </para>
        /// </summary>
        /// <param name=""request"">The product lookup request object. <see cref=""AdapterMessage""/></param>
        /// <returns><see cref=""AdapterResultMessage""/></returns>
        [HttpPost(""Products"")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, ""The request is invalid"")]
        public async Task<ActionResult<AdapterResultMessage>> GetProducts([FromBody] AdapterMessage request, CancellationToken cancellationToken)
        {
            AdapterConfig config;
            ServiceMessage serviceMessage;
            try
            {
                serviceMessage = mapper.Map<ServiceMessage>(request);
                using (logger.BeginScope(new Dictionary<string, object>
                {
                    [""RequestBody""] = serviceMessage,
                }))
                {
                    logger.LogDebug(""GetProducts request"", serviceMessage);
                }

                config = AdapterConfigHelper.GetAdapterConfigFromRequest(serviceMessage, logger, ""GetProducts"");

                if (string.IsNullOrEmpty(config.AuthorizationConfig))
                {
                    ProductResponse configError = ErrorHelpers.HandleConfigError<ProductResponse>(""GetProducts"", logger);

                    return new AdapterResultMessage
                    {
                        ErrorOccured = true,
                        ErrorMessage = configError.ErrorMessage ?? string.Empty,
                        Result = JsonConvert.SerializeObject(configError)
                    };
                }

                if (config.HttpEndpoints == null || !config.HttpEndpoints.Any())
                    throw new Exception(""API endpoint addresses are not specified."");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException == null ?
                    ex.Message : $""{ex.Message}: {ex.InnerException.Message}"";

                ProductResponse errorResponse = new ProductResponse
                {
                    ProblemDetails = new StatusCodeProblemDetails(StatusCodes.Status400BadRequest, ErrorCode.ConfigurationError, message),
                    Error = true
                };

                return new AdapterResultMessage
                {
                    ErrorOccured = true,
                    ErrorMessage = message,
                    Result = JsonConvert.SerializeObject(errorResponse)
                };
            }

            AuthorizationConfig? authorizationConfig;
            try
            {
                authorizationConfig = AdapterConfigHelper.GetAuthorizationConfiguration(config.AuthorizationConfig);

                if (authorizationConfig == null)
                    throw new Exception(""Unable to get authorization configuration."");
            }
            catch (Exception ex)
            {
                ProductResponse authorizationError = new ProductResponse
                {
                    ProblemDetails = new StatusCodeProblemDetails(StatusCodes.Status400BadRequest, ErrorCode.ConfigurationError, ex.Message),
                    Error = true
                };

                return new AdapterResultMessage
                {
                    ErrorOccured = true,
                    ErrorMessage = authorizationError.ErrorMessage ?? string.Empty,
                    Result = JsonConvert.SerializeObject(authorizationError)
                };
            }

            ProductServiceMessage productServiceMessage = CreateProductServiceMessage(serviceMessage, config, authorizationConfig);

            // Allows overriding request timeout.
            Request.Headers.TryGetValue(Constants.RequestTimeout, out var timeoutValues);
            if (timeoutValues.Any())
            {
                bool parseSuccess = TimeSpan.TryParse(timeoutValues.First(), out var timeout);

                if (parseSuccess)
                    productServiceMessage.RequestTimeout = timeout;
            }

            var products = await productService.GetProducts(productServiceMessage, cancellationToken);

            if (products.ProblemDetails != null || !string.IsNullOrEmpty(products.ErrorMessage))
            {
                var errorMessage = products.ProblemDetails?.Detail
                    ?? products.ErrorMessage;

                logger.LogError(errorMessage);

                var statusCode = products.ProblemDetails?.Status
                    ?? StatusCodes.Status500InternalServerError;

                products.Products = null;

                return StatusCode(statusCode, products);
            }

            AdapterResultMessage response = new AdapterResultMessage
            {
                ErrorOccured = false,
                ErrorMessage = string.Empty,
                Result = JsonConvert.SerializeObject(products)
            };

            return Ok(response);
        }

";

        public static readonly string ExampleSpecs = @"
    ""endpoints"": [
        {
            ""name"": ""Products"",
            ""functionName"": ""GetProducts"",
            ""httpMethod"": ""POST"",
            ""route"": ""products"",
        }
    ]
";

        public static readonly string ExampleProductServiceMethods = @"
public async Task<ProductResponse> GetProducts(ProductServiceMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.ServiceConfig == null)
                throw new ArgumentNullException(nameof(request.ServiceConfig));
            if (request.AuthorizationConfig == null)
                throw new ArgumentNullException(nameof(request.AuthorizationConfig));

            ulong page;
            if (string.IsNullOrEmpty(request.Page))
                page = 1;
            else
            {
                bool parseSuccess = ulong.TryParse(request.Page, out page);

                if (!parseSuccess)
                    throw new ArgumentException(""Invalid page number."", nameof(request.Page));
            }

            if (page == 0)
                throw new ArgumentException(""Page number cannot be lower than 1."", nameof(request.Page));
            if (page > long.MaxValue)
                throw new ArgumentException(string.Format(""Page number cannot exceed {maxValue}."", long.MaxValue), nameof(request.Page));

            int? pointOfSalesId = EntityAttributeHelper.GetNumberAttribute(request.SearchAttributes, ""POINTOFSALESID"");
            string language = EntityAttributeHelper.GetAttributeValueAsList(request.SearchAttributes, ""LANGUAGE"").FirstOrDefault();

            if (pointOfSalesId == null)
                throw new ArgumentException(""PointOfSalesId must have a value."", nameof(pointOfSalesId));
            if (language == null)
                throw new ArgumentException(""Language must have a value."", nameof(language));

            int? pageSize = request.PageSize;
            if (pageSize == null)
                pageSize = 50; // Current max.

            if (pageSize > 50)
                throw new ArgumentException(""Page size cannot exceed 50."", nameof(request.PageSize));

            var url = request.ServiceConfig.HttpEndpoints.Single().Url;
            var uri = new Uri(string.Format(url, pointOfSalesId));

            ProductSearchRequest searchRequest = new ProductSearchRequest
            {
                PointOfSalesId = (long)pointOfSalesId,
                PageSize = pageSize.Value,
                Page = (long)page - 1, // Page number in GalaxyApi is 0-based while in our API it's 1-based.
                ProductTypes = new List<string> { ""Activity"", ""Transport"" } // We're using GalaxyAPI only for activities.
            };

            ProductSearchResponse result = await CallProductSearch(searchRequest, uri, language, request.AuthorizationConfig, request.RequestTimeout, cancellationToken);

            bool includeAttributes = request.IncludeAttributes;
            return CreateProductResponse(result, request, language, page, pageSize.Value, includeAttributes);
        }

";

        public static readonly string ExampleProductServiceSpecs = @"
    {
        ""name"": ""Products"",
        ""functionName"": ""GetProduct"",
        ""productTypes"":  ""Activity"", ""Transport"",
    }
";
    }
}
