using CodeGen;
using CodeGen.Prompts;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using CodeGen.Templates;
using CodeGen.Resources;
using CodeGen.Models;


namespace CodeGen;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var kernel = Init();


        //await WriteControllerToFile(kernel);

        await WriteProductController(kernel);

        await WritreProductService(kernel);
    }

    public static async Task WritreProductService(Kernel kernel)
    {
        string generatedCode = await GenerateProductService(kernel);

        var newControllerTemplate = new ProductService()
        {
            GeneratedCode = generatedCode,

        };
        string content = newControllerTemplate.TransformText();
        Console.WriteLine($"\n----------- ProductService -----------\n{content.Length}");

        DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
        File.WriteAllText($"{now.ToString("yyyyMMddHHmmssfff")}ProductService.cs", content);
    }

    public static async Task WriteProductController(Kernel kernel)
    {
        string generatedCode = await GenerateProductController(kernel);

        var newControllerTemplate = new CBProductController()
        {
            EndpointsCode = generatedCode

        };
        string controllerContent = newControllerTemplate.TransformText();
        Console.WriteLine($"\n----------- ProductController -----------\n{controllerContent.Length}");

        DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
        File.WriteAllText($"{now.ToString("yyyyMMddHHmmssfff")}ProductController.cs", controllerContent);
    }

    public static async Task<string> GenerateProductService(Kernel kernel)
    {
        KernelFunction genFunc = kernel.CreateFunctionFromPrompt(
            DefaultPrompts.ProductServiceGenTemplate,
            new OpenAIPromptExecutionSettings() { Temperature = 0.1, TopP = 1 });

        FunctionResult result = await kernel.InvokeAsync(
            genFunc,
            new()
            {
                ["specification"] = EndpointSpecification.ProudctServiceSpecs,
                ["sampleSpecs"] = TrainingCode.ExampleProductServiceSpecs,
                ["sampleCode"] = TrainingCode.ExampleProductServiceMethods
            });
        //Console.WriteLine("\nCODE:\n " + result.GetValue<string>());

        return result.GetValue<string>() ?? "// Empty code";
    }


    public static async Task<string> GenerateProductController(Kernel kernel)
    {
        KernelFunction genFunc = kernel.CreateFunctionFromPrompt(
            DefaultPrompts.ProductControllerGenTemplate,
            new OpenAIPromptExecutionSettings() { Temperature = 0.1, TopP = 1 });

        FunctionResult result = await kernel.InvokeAsync(
            genFunc,
            new()
            {
                ["specification"] = EndpointSpecification.ProductEndpointSpecs,
                ["sampleSpecs"] = TrainingCode.ExampleSpecs,
                ["sampleCode"] = TrainingCode.ExampleProductEndpoints
            });
        //Console.WriteLine("\nCODE:\n " + result.GetValue<string>());

        return result.GetValue<string>() ?? "// Empty code";
    }

    public static async Task WriteControllerToFile(Kernel kernel)
    {
        string generatedCode = await GenerateCode(kernel);

        var newControllerTemplate = new AdapterController()
        {
            ControllerMetaData = new ControllerModel
            {
                // ----- ControllerMetaData Not in use atm
                EndpointName = "find",
                Route = "/v1/find",
                CompanyName = "CreativeTravel"
            },
            EndpointsCode = generatedCode

        };
        string controllerContent = newControllerTemplate.TransformText();
        Console.WriteLine($"\n----------- Content -----------\n{controllerContent}");

        DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
        File.WriteAllText($"{now.ToString("yyyyMMddHHmmssfff")}FakeAdapterController.cs", controllerContent);
    }

    public static async Task<string> GenerateCode(Kernel kernel)
    {
        KernelFunction jokeFunction = kernel.CreateFunctionFromPrompt(
            DefaultPrompts.EndpointGeneratorTemplate,
            new OpenAIPromptExecutionSettings() { Temperature = 0.1, TopP = 1 });

        FunctionResult result = await kernel.InvokeAsync(
            jokeFunction,
            new()
            {
                ["specification"] = EndpointSpecification.Specification,
                ["sampleCode"] = TrainingCode.ExampleCode
            });
        //Console.WriteLine("\nCODE:\n " + result.GetValue<string>());

        return result.GetValue<string>() ?? "// Empty code";
    }

    public static async Task RunAsync(Kernel kernel)
    {
        /*
            * Example: normally you would place prompt templates in a folder to separate
            *          C# code from natural language code, but you can also define a semantic
            *          function inline if you like.
            */

        // Function defined using few-shot design pattern
        string promptTemplate = @"
Generate a creative reason or excuse for the given event.
Be creative and be funny. Let your imagination run wild.

Event: I am running late.
Excuse: I was being held ransom by giraffe gangsters.

Event: I haven't been to the gym for a year
Excuse: I've been too busy training my pet dragon.

Event: {{$input}}
";

        var excuseFunction = kernel.CreateFunctionFromPrompt(promptTemplate, new OpenAIPromptExecutionSettings() { MaxTokens = 100, Temperature = 0.4, TopP = 1 });

        var result = await kernel.InvokeAsync(excuseFunction, new() { ["input"] = "I missed the F1 final race" });
        Console.WriteLine(result.GetValue<string>());

        result = await kernel.InvokeAsync(excuseFunction, new() { ["input"] = "sorry I forgot your birthday" });
        Console.WriteLine(result.GetValue<string>());

        var fixedFunction = kernel.CreateFunctionFromPrompt($"Translate this date {DateTimeOffset.Now:f} to French format", new OpenAIPromptExecutionSettings() { MaxTokens = 100 });

        result = await kernel.InvokeAsync(fixedFunction);
        Console.WriteLine(result.GetValue<string>());

    }

    public static Kernel Init()
    {
        Console.WriteLine("=========== CodeGen v0 =============");
        IConfigurationRoot configuration = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json")
         .Build();

        string openAIModelId = configuration["OpenAIModelID"];
        string openAIDeploymentId = configuration["OpenAIDeploymentName"];
        string openAIEndpoint = configuration["OpenAIEndpoint"];
        string openAIKey = configuration["OpenAIKey"];

        if (openAIModelId == null || openAIKey == null)
        {
            Console.WriteLine("OpenAI credentials not found. Skipping example.");
            return null;
        }

        var builder = new KernelBuilder();

        builder.AddAzureOpenAIChatCompletion(
            openAIDeploymentId!,
            openAIModelId,
            openAIEndpoint!,
            openAIKey
            );

        return builder.Build();
    }



}