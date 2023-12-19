using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Prompts
{
    public static class DefaultPrompts
    {
        public static readonly string JokePromptTemplate = @"
Write a funny joke based on the following person/event.
Use your imagination go wild.
Joke should have less than 50 words.
{{$input}}
";

        public static readonly string EndpointGeneratorTemplate = @"
You are an experienced .NET C# developer. I am giving you few C# endpoints.

++++++++++++++++++++++++ Example code ++++++++++++++++++++++++
      
{{$sampleCode}}
   
++++++++++++++++++++++++ Example ends ++++++++++++++++++++++++

Following the same code style I want you to write an endpoints which has following specification.

++++++++++++++++++++++++ Specification start ++++++++++++++++++++++++

    {{$specification}}

++++++++++++++++++++++++ Specification ends ++++++++++++++++++++++++

Your response should be in C# code. Dont include any other description. Just mention at top these are GPT generated code along with the gpt version/model.

";

    }
}
