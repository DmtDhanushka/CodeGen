using CodeGen;
using CodeGen.Prompts;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using CodeGen.Templates;

namespace HelloWorld;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var kernel = Init();

        //await WriteJokeToHTML(kernel);

        await GenerateController(kernel);


    }

    public static async Task GenerateController(Kernel kernel)
    {
        var newControllerTemplate = new AdapterController()
        {
            ControllerMetaData = new CodeGen.Models.ControllerModel
            {
                EndpointName = "find",
                Route = "/v1/find",
                CompanyName = "CreativeTravel"
            }

        };
        string controllerContent = newControllerTemplate.TransformText();
        Console.WriteLine($"Content {controllerContent}");
        File.WriteAllText("FakeAdapterController.cs", controllerContent);
    }

    public static async Task WriteJokeToHTML(Kernel kernel)
    {
        string res = await GenerateJoke(kernel);

        var newTemplate = new PolicyTicket()
        {
            PolicyDetails = new CodeGen.Models.PolicyModel
            {
                FirstName = "John",
                LastName = "Doe",
                PolicyName = "Name",
                StartDate = DateTime.Now.AddDays(2),
            },
            Joke = new CodeGen.Models.JokeModel
            {
                Description = res,
                Title = "Joke",
            }
        };
        var content = newTemplate.TransformText();
        File.WriteAllText("policy.html", content);
    }

    public static async Task<string> GenerateJoke(Kernel kernel)
    {
        KernelFunction jokeFunction = kernel.CreateFunctionFromPrompt(
            DefaultPrompts.JokePromptTemplate,
            new OpenAIPromptExecutionSettings() { MaxTokens = 100, Temperature = 0.4, TopP = 1 });
        // User input from key board
        Console.WriteLine("Enter your joke event/thing here: ");
        string input = Console.ReadLine();

        FunctionResult result = await kernel.InvokeAsync(jokeFunction, new() { ["input"] = input });
        Console.WriteLine("\nJOKE: " + result.GetValue<string>());

        return result.GetValue<string>() ?? "Empty Joke";
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
        Console.WriteLine("=========== GptOnConsole =============");
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