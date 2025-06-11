using LinguaLabs.Features.Icons;
using System.Text.Json;

namespace LinguaLabs.Tests;

[TestClass]
public class IconifyServiceTests
{
    [TestMethod]
    public void TestIconifySearchResponseDeserialization()
    {
        // Sample response from Iconify API
        var json = """
        {
            "icons": ["mdi:fish", "tabler:fish", "mingcute:fish-fill"],
            "collections": {
                "mdi": {
                    "name": "Material Design Icons",
                    "total": 7447,
                    "version": "7.4.47",
                    "author": {
                        "name": "Pictogrammers",
                        "url": "https://github.com/Templarian/MaterialDesign"
                    },
                    "license": {
                        "title": "Apache 2.0",
                        "spdx": "Apache-2.0",
                        "url": "https://github.com/Templarian/MaterialDesign/blob/master/LICENSE"
                    },
                    "height": 24,
                    "category": "Material",
                    "tags": ["Precise Shapes", "Has Padding"],
                    "palette": false
                }
            },
            "request": {
                "query": "fish"
            }
        }
        """;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var response = JsonSerializer.Deserialize<IconifySearchResponse>(json, options);

        Assert.IsNotNull(response);
        Assert.AreEqual(3, response.Icons.Length);
        Assert.AreEqual("mdi:fish", response.Icons[0]);
        Assert.AreEqual("fish", response.Request.Query);
        Assert.IsTrue(response.Collections.ContainsKey("mdi"));
        Assert.AreEqual("Material Design Icons", response.Collections["mdi"].Name);
    }
}
