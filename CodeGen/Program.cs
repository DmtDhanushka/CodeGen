using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel;
using System.Threading.Tasks;

namespace HelloWorld;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
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
            return;
        }

        /*
         * Example: normally you would place prompt templates in a folder to separate
         *          C# code from natural language code, but you can also define a semantic
         *          function inline if you like.
         */

        var builder = new KernelBuilder();

        builder.AddAzureOpenAIChatCompletion(
            openAIDeploymentId,
            openAIModelId,
            openAIEndpoint,
            openAIKey
            );

        var kernel = builder.Build();

        await RunAsync( kernel );
       

        
    }

    public static async Task RunAsync(Kernel kernel)
    {
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
}