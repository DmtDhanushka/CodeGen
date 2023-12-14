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

    }
}
