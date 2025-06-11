using LinguaLabs.Features.Icons;
using System.Text.Json;

namespace LinguaLabs.Tests;

public class IconifyServiceTests
{
    [Test]
    public async Task TestIconifySearchResponseDeserialization()
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

        await Assert.That(response).IsNotNull();
        await Assert.That(response!.Icons.Length).IsEqualTo(3);
        await Assert.That(response.Icons[0]).IsEqualTo("mdi:fish");
        await Assert.That(response.Request.Query).IsEqualTo("fish");
        await Assert.That(response.Collections.ContainsKey("mdi")).IsTrue();
        await Assert.That(response.Collections["mdi"].Name).IsEqualTo("Material Design Icons");
    }
}
