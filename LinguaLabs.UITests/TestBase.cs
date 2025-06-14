
using TUnit.Assertions;

namespace LinguaLabs.UITests;

public class TestBase
{
    private IApp? _app;

    static TestBase()
    {
        AppInitializer.TestEnvironment.AndroidAppName = Constants.AndroidAppName;
        AppInitializer.TestEnvironment.WebAssemblyDefaultUri = Constants.WebAssemblyDefaultUri;
        AppInitializer.TestEnvironment.iOSAppName = Constants.iOSAppName;
        AppInitializer.TestEnvironment.AndroidAppName = Constants.AndroidAppName;
        AppInitializer.TestEnvironment.iOSDeviceNameOrId = Constants.iOSDeviceNameOrId;
        AppInitializer.TestEnvironment.CurrentPlatform = Constants.CurrentPlatform;
        AppInitializer.TestEnvironment.WebAssemblyBrowser = Constants.WebAssemblyBrowser;

#if DEBUG
        AppInitializer.TestEnvironment.WebAssemblyHeadless = false;
#endif

        // Start the app only once, so the tests runs don't restart it
        // and gain some time for the tests.
        AppInitializer.ColdStartApp();
    }

    protected IApp App
    {
        get => _app!;
        private set
        {
            _app = value;
            Helpers.App = value;
        }
    }

    [Before(Test)]
    public void SetUpTest()
    {

        App = AppInitializer.AttachToApp();

    }

    [After(Test)]
    public void TearDownTest()
    {
        TakeScreenshot("teardown");
    }
    
    public FileInfo? TakeScreenshot(string stepName)
    {
        if (_app == null)
        {
            Console.WriteLine($"Cannot take screenshot '{stepName}': App is not initialized");
            return null;
        }

        var title = $"{TestContext.Current?.TestDetails.TestName ?? "UnknownTest"}_{stepName}"
            .Replace(" ", "_")
            .Replace(".", "_");

        var fileInfo = App.Screenshot(title);

        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.Name);
        if (fileNameWithoutExt != title && fileInfo.DirectoryName != null)
        {
            var destFileName = Path
                .Combine(fileInfo.DirectoryName, title + Path.GetExtension(fileInfo.Name));

            if (File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }

            File.Move(fileInfo.FullName, destFileName);

            // TUnit doesn't have AddTestAttachment, but we can still save the screenshot
            Console.WriteLine($"Screenshot saved: {destFileName}");

            fileInfo = new FileInfo(destFileName);
        }
        else
        {
            // TUnit doesn't have AddTestAttachment, but we can still save the screenshot
            Console.WriteLine($"Screenshot saved: {fileInfo.FullName}");
        }

        return fileInfo;
    }

}
