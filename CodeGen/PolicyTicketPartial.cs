using CodeGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen
{
    public partial class PolicyTicket
    {
        public PolicyModel? PolicyDetails { get; set; }

        public string GetFullName()
        {
            return $"{PolicyDetails?.FirstName} {PolicyDetails?.LastName}";
        }

        public JokeModel? Joke { get; set; }
    }
}
