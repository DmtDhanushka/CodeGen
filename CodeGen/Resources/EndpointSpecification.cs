namespace CodeGen.Resources
{
    public class EndpointSpecification
    {
        public static readonly string Specification = @"
{
    ""endpoints"": [
        {
            ""name"": ""Availability Search"",
            ""description"": ""Endpoint to search for availability"",
            ""httpMethod"": ""POST"",
            ""route"": ""v0/Adapter/Availability/Search"",
            ""requestBody"": {
                ""structure"": {
                    ""field1"": ""description"",
                    ""field2"": ""description"",
                    ""...""
                }
            },
            ""responseStructure"": {
                ""field1"": ""description"",
                ""field2"": ""description"",
                ""...""
            }
        },
        {
            ""name"": ""Create Order"",
            ""description"": ""Endpoint to create an order"",
            ""httpMethod"": ""POST"",
            ""route"": ""v0/Adapter/Order/Create"",
            ""requestBody"": {
                ""structure"": {
                    ""field1"": ""description"",
                    ""field2"": ""description"",
                    ""...""
                }
            },
            ""responseStructure"": {
                ""field1"": ""description"",
                ""field2"": ""description"",
                ""...""
            }
        },
    ]
}

";
    }
}
