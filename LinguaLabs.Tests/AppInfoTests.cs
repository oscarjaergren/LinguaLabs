namespace LinguaLabs.Tests;

public class AppInfoTests
{
    [Before(Test)]
    public async Task Setup()
    {
        // Setup code here
        await Task.CompletedTask;
    }

    [Test]
    public async Task AppInfoCreation()
    {
        var appInfo = new AppConfig { Environment = "Test" };

        await Assert.That(appInfo).IsNotNull();
        await Assert.That(appInfo.Environment).IsEqualTo("Test");
    }
}
